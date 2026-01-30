using Microsoft.Recognizers.Text;
using NotepadBasedCalculator.Api;
using NotepadBasedCalculator.Api.Lexer;
using System.Collections.Generic;
using System.Composition;
using System.Threading;

namespace NotepadBasedCalculator.BuiltInPlugins.Data
{
    [Export(typeof(IDataParser))]
    [Culture(SupportedCultures.Any)]
    public sealed class BooleanDataParser : IDataParser
    {
        public IReadOnlyList<IData>? Parse(string culture, TokenizedTextLine tokenizedTextLine, CancellationToken cancellationToken)
        {
            var results = new List<IData>();
            LinkedToken? currentToken = tokenizedTextLine.Tokens;
            while (currentToken is not null)
            {
                if (currentToken.Token.IsOfType(PredefinedTokenAndDataTypeNames.TrueIdentifier))
                {
                    results.Add(
                        new BooleanData(
                            tokenizedTextLine.LineTextIncludingLineBreak,
                            currentToken.Token.StartInLine,
                            currentToken.Token.EndInLine,
                            true));
                }
                else if (currentToken.Token.IsOfType(PredefinedTokenAndDataTypeNames.FalseIdentifier))
                {
                    results.Add(
                        new BooleanData(
                            tokenizedTextLine.LineTextIncludingLineBreak,
                            currentToken.Token.StartInLine,
                            currentToken.Token.EndInLine,
                            false));
                }

                currentToken = currentToken.Next;
            }

            return results;
        }
    }
}
