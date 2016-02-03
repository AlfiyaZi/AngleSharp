namespace AngleSharp.Dom.Css
{
    using AngleSharp.Css;
    using System;

    /// <summary>
    /// Represents the orientation constraint.
    /// </summary>
    sealed class OrientationMediaFeature : MediaFeature
    {
        #region Fields

        static readonly IValueConverter TheConverter = Converters.Toggle(Keywords.Portrait, Keywords.Landscape);

        #endregion

        #region ctor

        public OrientationMediaFeature()
            : base(FeatureNames.Orientation)
        {
        }

        #endregion

        #region Internal Properties

        internal override IValueConverter Converter
        {
            // Default: Allowed (value: false => landscape)
            get { return TheConverter; }
        }

        #endregion

        #region Methods

        public override Boolean Validate(RenderDevice device)
        {
            var portrait = false;
            var expected = portrait;
            var available = device.DeviceHeight >= device.DeviceWidth;
            return expected == available;
        }

        #endregion
    }
}
