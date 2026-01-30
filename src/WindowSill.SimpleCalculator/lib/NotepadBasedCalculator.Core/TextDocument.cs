using System;

namespace NotepadBasedCalculator.Core
{
    /// <summary>
    /// Represents a text document.
    /// </summary>
    public sealed class TextDocument
    {
        private string _text = string.Empty;

        public string Text
        {
            get => _text;
            set
            {
                _text = value ?? string.Empty;
                TextChanged?.Invoke(this, EventArgs.Empty);
            }
        }

        public event EventHandler? TextChanged;
    }
}
