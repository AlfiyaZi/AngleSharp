namespace AngleSharp.Dom.Css
{
    using AngleSharp.Css;
    using System;
    using System.Text.RegularExpressions;

    /// <summary>
    /// Take a regular expression.
    /// </summary>
    sealed class RegexpFunction : DocumentFunction
    {
        #region ctor

        public RegexpFunction()
            : base(FunctionNames.Regexp)
        {
        }

        #endregion

        #region Methods

        public override Boolean Matches(Url url)
        {
            var regex = new Regex(Data, RegexOptions.ECMAScript | RegexOptions.CultureInvariant);
            return regex.IsMatch(url.Href);
        }

        #endregion
    }
}
