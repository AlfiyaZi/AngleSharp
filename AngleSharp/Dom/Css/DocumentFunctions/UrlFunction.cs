namespace AngleSharp.Dom.Css
{
    using AngleSharp.Css;
    using System;

    /// <summary>
    /// Take an url.
    /// </summary>
    sealed class UrlFunction : DocumentFunction
    {
        #region ctor

        public UrlFunction()
            : base(FunctionNames.Url)
        {
        }

        #endregion

        #region Methods

        public override Boolean Matches(Url actual)
        {
            var expected = Url.Create(Data);
            return !expected.IsInvalid && expected.Equals(actual);
        }

        #endregion
    }
}
