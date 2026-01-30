using NotepadBasedCalculator.Api;
using NotepadBasedCalculator.BuiltInPlugins.Data;
using System;
using System.Collections.Generic;
using System.Composition.Hosting;
using System.Reflection;

namespace NotepadBasedCalculator.Core.Mef
{
    /// <summary>
    /// Provides a set of methods to initialize and manage MEF.
    /// </summary>
    public sealed class MefComposer : IDisposable
    {
        private readonly Assembly[] _assemblies;
        private readonly object[] _customExports;
        private bool _isExportProviderDisposed = true;

        public IMefProvider Provider { get; }

        public CompositionHost ExportProvider { get; private set; }

        public MefComposer(Assembly[]? assemblies = null, params object[] customExports)
        {
            if (Provider is not null)
            {
                throw new InvalidOperationException("Mef composer already initialized.");
            }

            _assemblies = assemblies ?? Array.Empty<Assembly>();
            _customExports = customExports ?? Array.Empty<object>();
            ExportProvider = InitializeMef();

            Provider = ExportProvider.GetExport<IMefProvider>();
            ((MefProvider)Provider).ExportProvider = ExportProvider;
        }

        public void Dispose()
        {
            if (ExportProvider is not null)
            {
                ((CompositionHost)ExportProvider).Dispose();
            }

            _isExportProviderDisposed = true;
        }

        internal void Reset()
        {
            // For unit tests.
            Dispose();
            InitializeMef();
        }

        private CompositionHost InitializeMef()
        {
            if (!_isExportProviderDisposed)
            {
                return ExportProvider;
            }

            var assemblies = new HashSet<Assembly>(_assemblies)
            {
                Assembly.GetExecutingAssembly(),
                typeof(NumberDataParser).Assembly
            };

            var catalog = new ContainerConfiguration();
            foreach (Assembly assembly in assemblies)
            {
                catalog = catalog.WithAssembly(assembly);
            }

            foreach (var customExport in _customExports)
            {
                catalog = catalog.WithPart(customExport.GetType());
            }

            ExportProvider = catalog.CreateContainer();

            _isExportProviderDisposed = false;

            return ExportProvider;

            //var container = new CompositionContainer(catalog);
            //var batch = new CompositionBatch();
            //batch.AddPart(this);

            //for (int i = 0; i < _customExports.Length; i++)
            //{
            //    batch.AddPart(_customExports[i]);
            //}

            //container.Compose(batch);

            //ExportProvider = container;

            //_isExportProviderDisposed = false;

            //return ExportProvider;
        }
    }
}
