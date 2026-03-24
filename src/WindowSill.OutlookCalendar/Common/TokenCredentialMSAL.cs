using Azure.Core;
using Microsoft.Extensions.Logging;
using Microsoft.Identity.Client;
using Microsoft.Identity.Client.Desktop;
using Microsoft.Identity.Client.Extensions.Msal;
using System.Diagnostics;
using Windows.Win32;
using WindowSill.API;

namespace WindowSill.OutlookCalendar.Common
{
    public class TokenCredentialMSAL : TokenCredential
    {
        private string[] scopes = new string[] { "Calendars.Read" };

        private static IPublicClientApplication _clientApp;
        public static IPublicClientApplication PublicClientApp { get { return _clientApp; } }
        private string graphAPIEndpoint = "https://graph.microsoft.com/v1.0/me";
        private const string Tenant = "common";
        private const string Authority = "https://login.microsoftonline.com/" + Tenant;
        private static string MSGraphURL = "https://graph.microsoft.com/v1.0/";
        private static string Instance = "https://login.microsoftonline.com/";
        private const string ClientId = "3c62448e-650a-497a-b43c-35f9db069e4f";
        private ILogger _logger;

        public TokenCredentialMSAL(ILogger logger)
        {
            _logger = logger;
            var brokerOptions = new BrokerOptions(BrokerOptions.OperatingSystems.Windows);

            _clientApp = PublicClientApplicationBuilder.Create(ClientId)
                .WithAuthority($"{Instance}{Tenant}")
                .WithDefaultRedirectUri()
                .WithWindowsDesktopFeatures(brokerOptions)
                .Build();

            var cacheHelper = CreateCacheHelperAsync().GetAwaiter().GetResult();
            cacheHelper.RegisterCache(_clientApp.UserTokenCache);
        }

        public override AccessToken GetToken(TokenRequestContext requestContext, CancellationToken cancellationToken)
        {
            return new AccessToken(LoginNative().Result, DateTimeOffset.MaxValue);
        }

        public async override ValueTask<AccessToken> GetTokenAsync(TokenRequestContext requestContext, CancellationToken cancellationToken)
        {
            return new AccessToken(await LoginNative(), DateTimeOffset.MaxValue);
        }

        private async Task<string> LoginNative()
        {
            AuthenticationResult authResult = null;
            var app = PublicClientApp;
            IAccount firstAccount = null;

            try
            {
                firstAccount = (await app.GetAccountsAsync()).FirstOrDefault();

                if (firstAccount == null)
                {
                    firstAccount = PublicClientApplication.OperatingSystemAccount;
                }
            }
            catch (Exception e)
            {
                _logger.LogWarning("Failed to get the outlook account.");
            }

            try
            {
                authResult = await app.AcquireTokenSilent(scopes, firstAccount)
                    .ExecuteAsync();

                return authResult.AccessToken;

            }
            catch (MsalUiRequiredException ex)
            {
                _logger.LogInformation("Opening native UI on windows to login.");

                try
                {
                    await ThreadHelper.RunOnUIThreadAsync(async () =>
                    {
                        var hWnd = PInvoke.GetActiveWindow();

                        authResult = await app.AcquireTokenInteractive(scopes)
                            .WithAccount(firstAccount)
                            .WithPrompt(Prompt.SelectAccount)
                            .WithParentActivityOrWindow(hWnd)
                            .ExecuteAsync();
                    });
                }
                catch (MsalException msalex)
                {
                    _logger.LogError(msalex, "Failed opening the native UI windows to login.");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "All tries failed to acquire token on LoginNative.");
                return "";
            }

            if (authResult != null)
            {
                await GetHttpContentWithToken(graphAPIEndpoint, authResult.AccessToken);
            }

            return "";
        }
        public async Task<string> GetHttpContentWithToken(string url, string token)
        {
            var httpClient = new HttpClient();
            HttpResponseMessage response;
            try
            {
                var request = new HttpRequestMessage(HttpMethod.Get, url);
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
    }
}
