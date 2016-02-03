namespace AngleSharp.Dom.Css
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// Condition for "or" document conjunctions.
    /// </summary>
    sealed class OrCondition : CssNode, IConditionFunction
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
            return Conditions.Any(m => m.Check());
        }

        public override String ToCss(IStyleFormatter formatter)
        {
            var conditions = Conditions.Select(m => m.ToCss(formatter));
            return String.Join(" or ", conditions);
        }

        #endregion
    }
}
