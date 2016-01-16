namespace AngleSharp.Dom.Css
{
    using AngleSharp.Parser.Css;
    using System;

    /// <summary>
    /// Represents the CSSGroupingRule interface.
    /// </summary>
    abstract class CssGroupingRule : CssRule, ICssGroupingRule
    {
        #region Fields

        readonly CssRuleList _rules;

        #endregion

        #region ctor

        internal CssGroupingRule(CssRuleType type, CssParser parser)
            : base(type, parser)
        {
            var block = new CssBlock();
            AppendChild(block);
            _rules = new CssRuleList(block);
        }

        #endregion

        #region Properties

        public CssRuleList Rules
        {
            get { return _rules; }
        }

        ICssRuleList ICssGroupingRule.Rules
        {
            get { return Rules; }
        }

        #endregion

        #region Methods

        public ICssRule AddNewRule(CssRuleType ruleType)
        {
            var rule = Parser.CreateRule(ruleType);
            Rules.Add(rule);
            return rule;
        }

        public Int32 Insert(String ruleText, Int32 index)
        {
            var rule = Parser.ParseRule(ruleText);
            Rules.Insert(index, rule);
            return index;    
        }

        public void RemoveAt(Int32 index)
        {
            Rules.RemoveAt(index);
        }

        public sealed override String ToCss(IStyleFormatter formatter)
        {
            var name = GetRuleName();
            return formatter.Rule(name, Children);
        }

        #endregion

        #region Internal Methods

        protected abstract String GetRuleName();

        #endregion
    }
}
