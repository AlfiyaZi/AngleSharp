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
            get { return GetValue<CssRawString, String>(m => m.CssText); }
            set { SetValue(value, m => new CssRawString(m)); }
        }

        #endregion

        #region Methods

        public override String ToCss(IStyleFormatter formatter)
        {
            return formatter.Rule("@charset", Children) + ";";
        }

        #endregion
    }
}
