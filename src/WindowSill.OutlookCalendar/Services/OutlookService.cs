using Azure.Core;
using Azure.Identity;
using Microsoft.Graph;
using Microsoft.Graph.Models;
using Microsoft.Identity.Client;
using Microsoft.Identity.Client.Desktop;
using Microsoft.Identity.Client.Extensions.Msal;
using System.ComponentModel.Composition;
using System.Diagnostics;
using Windows.Win32;
using WindowSill.OutlookCalendar.Enums;
using WindowSill.OutlookCalendar.Extensions;
using WindowSill.OutlookCalendar.Models;
using Application = Microsoft.Office.Interop.Outlook.Application;
using Outlook = Microsoft.Office.Interop.Outlook;

namespace WindowSill.OutlookCalendar.Services
{
    [Export(typeof(IOutlookService))]

    public class OutlookService : IOutlookService
    {
        public bool IsAppointmentInitiated { get => Appointments.Count > 0; }

        public List<CalendarAppointmentVm> Appointments { get; set; } = new List<CalendarAppointmentVm>();

        public OfficeVersion IsNewerOfficeVersion { get; set; } = OfficeVersion.OfficeGraphql;

        public Outlook.NameSpace? OutlookNameSpace { get; set; }

        private GraphServiceClient? GraphClient { get; set; }

        public bool IsOutlookLogged { get; set; }

        public bool _isLoadingAppointments { get; set; }

        private const string ClientId = "3c62448e-650a-497a-b43c-35f9db069e4f";
        private const string Tenant = "common";
        private const string Authority = "https://login.microsoftonline.com/" + Tenant;

        private static string MSGraphURL = "https://graph.microsoft.com/v1.0/";
        private static AuthenticationResult authResult;
        private static IAccount _currentUserAccount;
        private string[] scopes = new string[] { "Calendars.Read" };

        private static IPublicClientApplication _clientApp;
        public static IPublicClientApplication PublicClientApp { get { return _clientApp; } }
        private string graphAPIEndpoint = "https://graph.microsoft.com/v1.0/me";
        private static string Instance = "https://login.microsoftonline.com/";

        public async Task InitAllAppointments()
        {
            if (_isLoadingAppointments)
                return;

            _isLoadingAppointments = true;

            Appointments.Clear();

            if (IsNewerOfficeVersion == OfficeVersion.OfficeGraphql)
            {
                if (GraphClient is null)
                    return;

                var startDateTime = DateTime.Now.ToString("o");
                var endDateTime = DateTime.Now.AddMonths(1).ToString("o");
                EventCollectionResponse? events = null;
                try
                {
                    events = await GraphClient.Me.CalendarView.GetAsync(config =>
                    {
                        config.QueryParameters.StartDateTime = startDateTime;
                        config.QueryParameters.EndDateTime = endDateTime;
                        config.QueryParameters.Orderby = new[] { "start/dateTime" };
                        config.QueryParameters.Top = 5;
                        config.Headers.Add("Prefer", $"outlook.timezone=\"{TimeZoneInfo.Local.Id}\"");
                    });
                }
                catch (Exception e)
                {

                }

                if (events is null)
                    return;

                if (events.Value != null && events.Value.Any())
                {
                    foreach (var item in events.Value)
                    {
                        if (item.Start is null || item.End is null)
                            continue;

                        var start = item.Start.ToDateTimeExt();
                        var end = item.End.ToDateTimeExt();

                        if (DateTime.Compare(DateTime.Now, item.Start.ToDateTimeExt()) < 0)
                            Appointments.Add(new CalendarAppointmentVm(item.Subject ?? string.Empty, start, end, item.Location.ToString() ?? string.Empty));
                    }
                }
            }
            else
            {
                Outlook.Items? items = null;

                try
                {
                    if (OutlookNameSpace is null)
                        return;

                    Outlook.MAPIFolder? calendar = OutlookNameSpace.GetDefaultFolder(Outlook.OlDefaultFolders.olFolderCalendar);

                    items = calendar.Items;

                    items.IncludeRecurrences = true;

                    items.Sort("[Start]");

                    calendar = null;
                }
                catch
                {
                    return;
                }

                finally
                {

                }

                if (items is null || OutlookNameSpace is null)
                    return;

                //var filter = "[Start] >= '" + DateTime.Now.ToString("g") + "'";
                //Outlook.Items filtereditems = items.Restrict(filter);

                foreach (var item in items)
                {
                    if (item is not Outlook.AppointmentItem appt)
                        continue;

                    if (DateTime.Compare(DateTime.Now, appt.Start) < 0)
                        Appointments.Add(new CalendarAppointmentVm(appt.Subject, appt.Start, appt.End, appt.Location));
                }
                items = null;
            }
        }

        public CalendarAppointmentVm? FirstAppointment()
        {
            if (Appointments.Count > 0)
                return Appointments[0];

            return null;
        }

        public List<CalendarAppointmentVm> GetAllAppointments()
        {
            return Appointments;
        }

        static async Task PreAuthenticateAsync(DeviceCodeCredential credential, SettingsAzure settings)
        {
            if (!string.IsNullOrEmpty(""))
            {
                var tokenContext = new TokenRequestContext(settings.GraphUserScopes ??
                    new[] { "https://graph.microsoft.com/.default" });
                var authRecord = await credential.AuthenticateAsync(tokenContext);

                if (authRecord != null)
                {
                    using var cacheStream = new FileStream(settings.AuthRecordCachePath, FileMode.Create, FileAccess.Write);
                    await authRecord.SerializeAsync(cacheStream);
                }
            }
        }

        public async Task InitLogin(string tenantID)
        {
            if (_isLoadingAppointments)
                return;

            _isLoadingAppointments = true;

            //IsNewerOfficeVersion = OfficeVersion.OfficeGraphql;
            if (IsNewerOfficeVersion == OfficeVersion.OfficeGraphql)
            {

                var brokerOptions = new BrokerOptions(BrokerOptions.OperatingSystems.Windows);

                _clientApp = PublicClientApplicationBuilder.Create(ClientId)
                    .WithAuthority($"{Instance}{Tenant}")
                    .WithDefaultRedirectUri()
                    .WithWindowsDesktopFeatures(brokerOptions)
                    .Build();

                MsalCacheHelper cacheHelper = CreateCacheHelperAsync().GetAwaiter().GetResult();

                cacheHelper.RegisterCache(_clientApp.UserTokenCache);

                LoginNative();

                //if (GraphClient is not null)
                //return;

                //_ = Task.Run(async () =>
                //{
                //    var credential = new InteractiveBrowserCredential(
                //    new InteractiveBrowserCredentialOptions
                //    {
                //        ClientId = ClientId,
                //        TenantId = tenantID,
                //        TokenCachePersistenceOptions = new TokenCachePersistenceOptions
                //        {
                //            Name = "Windowsill.OutlookCalendar"
                //        }
                //    });


                //    var tokenRequestContext = new Azure.Core.TokenRequestContext(new[] { "Calendars.Read" });

                //    try
                //    {
                //        //var token = await credential.GetTokenAsync(tokenRequestContext);
                //        GraphClient = new GraphServiceClient(credential, new[] { "Calendars.Read" });

                //        await GraphClient.Me.GetAsync();

                //        IsOutlookLogged = true;
                //    }
                //    catch (Exception ex)
                //    {
                //        IsOutlookLogged = false;
                //    }
                //});
            }
            else
            {
                var outlookApp = new Application();
                OutlookNameSpace = outlookApp.GetNamespace("MAPI");
                OutlookNameSpace.Logon();
            }
        }

        private static async Task<MsalCacheHelper> CreateCacheHelperAsync()
        {
            var storageProperties = new StorageCreationPropertiesBuilder(
                              System.Reflection.Assembly.GetExecutingAssembly().GetName().Name + ".msalcache.bin",
                              MsalCacheHelper.UserRootDirectory)
                                .Build();

            var cacheHelper = await MsalCacheHelper.CreateAsync(
                        storageProperties,
                        new TraceSource("MSAL.CacheTrace"))
                     .ConfigureAwait(false);

            return cacheHelper;
        }

        private async void LoginNative()
        {
            AuthenticationResult authResult = null;
            var app = PublicClientApp;
            IAccount firstAccount = null;

            try
            {
                firstAccount = (await app.GetAccountsAsync()).FirstOrDefault();

                if (firstAccount == null)
                {
                    firstAccount = Microsoft.Identity.Client.PublicClientApplication.OperatingSystemAccount;
                }
            }
            catch (Exception e)
            {

            }

            try
            {
                authResult = await app.AcquireTokenSilent(scopes, firstAccount)
                    .ExecuteAsync();
            }
            catch (MsalUiRequiredException ex)
            {
                try
                {
                    var hWnd = PInvoke.GetActiveWindow();

                    authResult = await app.AcquireTokenInteractive(scopes)
                        .WithAccount(firstAccount)
                        .WithPrompt(Microsoft.Identity.Client.Prompt.SelectAccount)
                        .WithParentActivityOrWindow(hWnd)
                        .ExecuteAsync();
                }
                catch (MsalException msalex)
                {

                }
            }
            catch (Exception ex)
            {
                return;
            }

            if (authResult != null)
            {
                var res = await GetHttpContentWithToken(graphAPIEndpoint, authResult.AccessToken);
            }
        }

        public async Task<string> GetHttpContentWithToken(string url, string token)
        {
            var httpClient = new System.Net.Http.HttpClient();
            System.Net.Http.HttpResponseMessage response;
            try
            {
                var request = new System.Net.Http.HttpRequestMessage(System.Net.Http.HttpMethod.Get, url);
                //Add the token in Authorization header
                request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
                response = await httpClient.SendAsync(request);
                var content = await response.Content.ReadAsStringAsync();
                return content;
            }
            catch (Exception ex)
            {
                return ex.ToString();
            }
        }
    }
}
