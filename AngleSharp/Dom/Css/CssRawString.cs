namespace AngleSharp.Dom.Css
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// Represents a raw string taken from the CSS source.
    /// </summary>
    sealed class CssRawString : ICssNode
    {
        readonly String _text;
        TextView _source;

        public CssRawString(String text)
        {
            _text = text;
        }

        public IEnumerable<ICssNode> Children
        {
            get { return Enumerable.Empty<ICssNode>(); }
        }

        public String CssText
        {
            get { return _text; }
        }

        public TextView SourceCode
        {
            get { return _source; }
            set { _source = value; }
        }

        public String ToCss(IStyleFormatter formatter)
        {
            return _text;
        }
    }
}
