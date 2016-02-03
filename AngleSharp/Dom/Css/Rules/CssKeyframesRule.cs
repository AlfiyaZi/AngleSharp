namespace AngleSharp.Dom.Css
{
    using AngleSharp.Extensions;
    using AngleSharp.Parser.Css;
    using System;
    using System.Linq;

    /// <summary>
    /// Represents an @keyframes rule.
    /// </summary>
    sealed class CssKeyframesRule : CssRule, ICssKeyframesRule
    {
        #region Fields

        readonly CssRuleList _rules;

        #endregion

        #region ctor

        internal CssKeyframesRule(CssParser parser)
            : base(CssRuleType.Keyframes, parser)
        {
            var block = new CssBlock();
            AppendChild(block);
            _rules = new CssRuleList(block);
        }

        #endregion

        #region Properties

        public String Name
        {
            get { return GetValue<CssRawString, String>(m => m.CssText); }
            set { SetValue(value, m => new CssRawString(m)); }
        }

        public CssRuleList Rules
        {
            get { return _rules; }
        }

        ICssRuleList ICssKeyframesRule.Rules
        {
            get { return Rules; }
        }

        #endregion

        #region Methods

        public void Add(String ruleText)
        {
            var rule = Parser.ParseKeyframeRule(ruleText);
            Rules.Add(rule);
        }

        public void Remove(String key)
        {
            var element = Find(key);
            Rules.Remove(element);
        }

        public CssKeyframeRule Find(String key)
        {
            return Rules.OfType<CssKeyframeRule>().FirstOrDefault(m => key.Isi(m.KeyText));
        }

        ICssKeyframeRule ICssKeyframesRule.Find(String key)
        {
            return Find(key);
        }

        public override String ToCss(IStyleFormatter formatter)
        {
            return formatter.Rule("@keyframes", Children);
        }

        #endregion
    }
}
