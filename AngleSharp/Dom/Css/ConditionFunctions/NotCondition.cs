namespace AngleSharp.Dom.Css
{
    using System;

    /// <summary>
    /// Condition for "not" document conjunctions.
    /// </summary>
    sealed class NotCondition : CssNode, IConditionFunction
    {
        #region ctor

        public NotCondition()
        {
            AppendChild(new EmptyCondition());
        }

        #endregion

        #region Properties

        public IConditionFunction Content
        {
            get { return GetValue<IConditionFunction, IConditionFunction>(m => m); }
            set { ReplaceChild(Content, value); }
        }

        #endregion

        #region Methods

        public Boolean Check()
        {
            return !Content.Check();
        }

        public override String ToCss(IStyleFormatter formatter)
        {
            return String.Concat("not ", Content.ToCss(formatter));
        }

        #endregion
    }
}
