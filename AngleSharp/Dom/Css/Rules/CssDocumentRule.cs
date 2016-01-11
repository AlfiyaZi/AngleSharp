﻿namespace AngleSharp.Dom.Css
{
    using AngleSharp.Parser.Css;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// Contains the rules specified by a @document { /* ... */ } rule.
    /// </summary>
    sealed class CssDocumentRule : CssGroupingRule, ICssDocumentRule
    {
        #region ctor

        internal CssDocumentRule(CssParser parser)
            : base(CssRuleType.Document, parser)
        {
        }

        #endregion

        #region Properties

        public String ConditionText
        {
            get
            {
                var entries = Conditions.Select(m => m.ToCss());
                return String.Join(", ", entries); 
            }
            set
            {
                var conditions = Parser.ParseDocumentRules(value);

                if (conditions == null)
                {
                    throw new DomException(DomError.Syntax);
                }

                Clear();

                foreach (var condition in conditions)
                {
                    AppendChild(condition);
                }
            }
        }

        public IEnumerable<IDocumentFunction> Conditions
        {
            get { return Children.OfType<IDocumentFunction>(); }
        }

        #endregion

        #region Internal Methods

        internal Boolean IsValid(Url url)
        {
            return Conditions.Any(m => m.Matches(url));
        }

        protected override String GetRuleName()
        {
            return "@document";
        }

        #endregion
    }
}
