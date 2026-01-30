using NotepadBasedCalculator.Api;
using System.Collections.Generic;
using System.Composition;
using System.Composition.Hosting;

namespace NotepadBasedCalculator.Core.Mef
{
    [Export(typeof(IMefProvider))]
    public sealed class MefProvider : IMefProvider
    {
        internal CompositionHost? ExportProvider { get; set; }

        public TExport Import<TExport>()
        {
            return ExportProvider!.GetExport<TExport>()!;
        }

        public IEnumerable<TExport> ImportMany<TExport>()
        {
            return ExportProvider!.GetExports<TExport>();
        }
    }
}
