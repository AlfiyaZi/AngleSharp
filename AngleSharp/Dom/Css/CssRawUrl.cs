namespace AngleSharp.Dom.Css
{
    using AngleSharp.Extensions;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// Represents a raw string taken from the CSS source.
    /// </summary>
    sealed class CssRawUrl : ICssNode
    {
        readonly String _url;
        TextView _source;

        public CssRawUrl(String url)
        {
            _url = url;
        }

        public IEnumerable<ICssNode> Children
        {
            get { return Enumerable.Empty<ICssNode>(); }
        }

        public String CssText
        {
            get { return _url; }
        }

        public TextView SourceCode
        {
            get { return _source; }
            set { _source = value; }
        }

        public String ToCss(IStyleFormatter formatter)
        {
            return _url.CssUrl();
        }
    }
}
