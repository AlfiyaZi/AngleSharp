namespace AngleSharp.Dom.Css
{
    using AngleSharp.Extensions;
    using System;

    /// <summary>
    /// Condition for declarative document conjunctions.
    /// </summary>
    sealed class DeclarationCondition : CssNode, IConditionFunction
    {
        #region ctor

        public DeclarationCondition(CssProperty property, CssValue value)
        {
            AppendChild(property);
            AppendChild(value);
        }

        #endregion

        #region Properties

        public CssProperty Property
        {
            get { return GetValue<CssProperty>(); }
            set { ReplaceChild(Property, value); }
        }

        public CssValue Value
        {
            get { return GetValue<CssValue>(); }
            set { ReplaceChild(Value, value); }
        }

        #endregion

        #region Methods

        public Boolean Check()
        {
            var property = Property;
            var value = Value;
            return (property is CssUnknownProperty == false) && property.TrySetValue(value);
        }

        public override String ToCss(IStyleFormatter formatter)
        {
            var property = Property;
            var value = Value;
            var content = formatter.Declaration(property.Name, value.CssText, property.IsImportant);
            return String.Empty.CssFunction(content);
        }

        #endregion
    }
}
