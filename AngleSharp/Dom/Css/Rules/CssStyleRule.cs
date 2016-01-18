﻿namespace AngleSharp.Dom.Css
{
    using AngleSharp.Parser.Css;
    using System;
    using System.Linq;

    /// <summary>
    /// Represents a CSS style rule.
    /// </summary>
	sealed class CssStyleRule : CssRule, ICssStyleRule
    {
        #region ctor

        internal CssStyleRule(CssParser parser)
            : base(CssRuleType.Style, parser)
        {
            AppendChild(SimpleSelector.All);
            AppendChild(new CssStyleDeclaration(this));
        }

        #endregion

        #region Properties

        public ISelector Selector
        {
            get { return Children.OfType<ISelector>().FirstOrDefault(); }
            set { ReplaceChild(Selector, value); }
        }

        public String SelectorText
        {
            get { return Selector.Text; }
            set { Selector = Parser.ParseSelector(value); }
        }

        ICssStyleDeclaration ICssStyleRule.Style
        {
            get { return Style; }
        }

        public CssStyleDeclaration Style
        {
            get { return Children.OfType<CssStyleDeclaration>().FirstOrDefault(); }
        }

        #endregion

        #region Methods

        public override String ToCss(IStyleFormatter formatter)
        {
            var selector = Selector.ToCss(formatter);
            var style = Style.ToCss(formatter);
            return formatter.Style(selector, style);
        }

        #endregion
	}
}
