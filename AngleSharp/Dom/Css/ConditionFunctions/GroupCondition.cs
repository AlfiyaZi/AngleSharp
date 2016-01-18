namespace AngleSharp.Dom.Css
{
    using System;

    /// <summary>
    /// Condition for grouped "()" document conjunctions.
    /// </summary>
    sealed class GroupCondition : CssNode, IConditionFunction
    {
        #region ctor

        public GroupCondition()
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
            return Content.Check();
        }

        public override String ToCss(IStyleFormatter formatter)
        {
            var condition = Content.ToCss(formatter);
            return String.Concat("(", condition, ")");
        }

        #endregion
    }
}
