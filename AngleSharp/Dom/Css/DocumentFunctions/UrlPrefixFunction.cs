namespace AngleSharp.Dom.Css
{
    using AngleSharp.Css;
    using System;

    /// <summary>
    /// Take an url prefix.
    /// </summary>
    sealed class UrlPrefixFunction : DocumentFunction
    {
        #region ctor

        public UrlPrefixFunction()
            : base(FunctionNames.UrlPrefix)
        {
        }

        #endregion

        #region Methods

        public override Boolean Matches(Url url)
        {
            return url.Href.StartsWith(Data, StringComparison.OrdinalIgnoreCase);
        }

        #endregion
    }
}
