namespace AngleSharp.Dom.Css
{
    using AngleSharp.Css;
    using AngleSharp.Extensions;
    using System;

    /// <summary>
    /// Represents the pointer constraint.
    /// </summary>
    sealed class PointerMediaFeature : MediaFeature
    {
        #region Fields

        static readonly IValueConverter TheConverter = Map.PointerAccuracies.ToConverter();

        #endregion

        #region ctor

        public PointerMediaFeature()
            : base(FeatureNames.Pointer)
        {
        }

        #endregion

        #region Internal Properties

        internal override IValueConverter Converter
        {
            // Default: PointerAccuracy.Fine
            get { return TheConverter; }
        }

        #endregion

        #region Methods

        public override Boolean Validate(RenderDevice device)
        {
            var accuracy = PointerAccuracy.Fine;
            var expected = accuracy;
            //Nothing yet, so we assume we have a headless browser
            return expected == PointerAccuracy.None;
        }

        #endregion
    }
}
