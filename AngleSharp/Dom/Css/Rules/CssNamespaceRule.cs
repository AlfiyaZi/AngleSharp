namespace AngleSharp.Dom.Css
{
    using AngleSharp.Parser.Css;
    using System;

    /// <summary>
    /// Represents an @namespace rule.
    /// </summary>
    sealed class CssNamespaceRule : CssRule, ICssNamespaceRule
    {
        #region ctor

        internal CssNamespaceRule(CssParser parser)
            : base(CssRuleType.Namespace, parser)
        {
        }

        #endregion

        #region Properties

        public String NamespaceUri
        {
            get { return GetValue<CssRawUrl>(m => m.CssText); }
            set { CheckValidity(); SetValue(value, m => new CssRawUrl(m)); }
        }

        public String Prefix
        {
            get { return GetValue<CssRawString>(m => m.CssText); }
            set { CheckValidity(); SetValue(value, m => new CssRawString(m)); }
        }

        #endregion

        #region String Representation

        public override String ToCss(IStyleFormatter formatter)
        {
            return formatter.SimpleRule("@namespace", Children);
        }

        #endregion

        #region Helpers

        static Boolean IsNotSupported(CssRuleType type)
        {
            return type != CssRuleType.Charset && 
                   type != CssRuleType.Import && 
                   type != CssRuleType.Namespace;
        }

        void CheckValidity()
        {
            var parent = Owner;
            var list = parent != null ? parent.Rules : null;

            if (list != null)
            {
                foreach (var entry in list)
                {
                    if (IsNotSupported(entry.Type))
                    {
                        throw new DomException(DomError.InvalidState);
                    }
                }
            }
        }

        #endregion
    }
}
