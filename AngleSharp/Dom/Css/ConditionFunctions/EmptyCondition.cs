namespace AngleSharp.Dom.Css
{
    using System;

    /// <summary>
    /// Condition for empty / no document conjunctions.
    /// </summary>
    sealed class EmptyCondition : CssNode, IConditionFunction
    {
        #region Methods

        public Boolean Check()
        {
            return true;
        }

        public override String ToCss(IStyleFormatter formatter)
        {
            return String.Empty;
        }

        #endregion
    }
}
