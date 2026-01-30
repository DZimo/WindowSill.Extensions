using CommunityToolkit.Common.Deferred;
using System.Collections.Generic;

namespace NotepadBasedCalculator.Core
{
    internal sealed class ParserAndInterpreterResultUpdatedEventArgs : DeferredCancelEventArgs
    {
        internal IReadOnlyList<ParserAndInterpreterResultLine>? ResultPerLines { get; }

        public ParserAndInterpreterResultUpdatedEventArgs(IReadOnlyList<ParserAndInterpreterResultLine>? resultPerLines)
        {
            ResultPerLines = resultPerLines;
        }
    }
}
