namespace AngleSharp.Dom.Css
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// Condition for "and" document conjunctions.
    /// </summary>
    sealed class AndCondition : CssNode, IConditionFunction
    {
        #region Properties

        public IEnumerable<IConditionFunction> Conditions
        {
            get { return Children.OfType<IConditionFunction>(); }
        }

        #endregion

        #region Methods

        public Boolean Check()
        {
            return !Conditions.Any(m => !m.Check());
        }

        public override String ToCss(IStyleFormatter formatter)
        {
            var conditions = Conditions.Select(m => m.ToCss(formatter));
            return String.Join(" and ", conditions);
        }

        #endregion
    }
}
