namespace AngleSharp.Dom.Css
{
    using AngleSharp.Parser.Css;
    using System;

    /// <summary>
    /// Represents the CSS @charset rule.
    /// </summary>
    sealed class CssCharsetRule : CssRule, ICssCharsetRule
    {
        #region ctor

        internal CssCharsetRule(CssParser parser)
            : base(CssRuleType.Charset, parser)
        {
        }

        #endregion

        #region Properties

        public String CharacterSet
        {
            get { return GetValue<CssRawString>(m => m.CssText); }
            set { SetValue(value, m => new CssRawString(m)); }
        }

        #endregion

        #region String Representation

        public override String ToCss(IStyleFormatter formatter)
        {
            return formatter.SimpleRule("@charset", Children);
        }

        #endregion
    }
}
