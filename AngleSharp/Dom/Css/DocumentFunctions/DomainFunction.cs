namespace AngleSharp.Dom.Css
{
    using AngleSharp.Css;
    using AngleSharp.Extensions;
    using System;

    /// <summary>
    /// Take a domain.
    /// </summary>
    sealed class DomainFunction : DocumentFunction
    {
        #region ctor

        public DomainFunction()
            : base(FunctionNames.Domain)
        {
        }

        #endregion

        #region Methods

        public override Boolean Matches(Url url)
        {
            var data = Data;
            var domain = url.HostName;
            return domain.Isi(data) || domain.EndsWith( "." + data, StringComparison.OrdinalIgnoreCase);
        }

        #endregion
    }
}
