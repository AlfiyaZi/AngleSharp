namespace AngleSharp.Dom.Css
{
    using AngleSharp.Css;
    using System;

    /// <summary>
    /// Represents the device-aspect-ratio constraint.
    /// </summary>
    sealed class DeviceAspectRatioMediaFeature : MediaFeature
    {
        #region ctor

        public DeviceAspectRatioMediaFeature(String name)
            : base(name)
        {
        }

        #endregion

        #region Internal Properties

        internal override IValueConverter Converter
        {
            // Default: NOT Allowed
            get { return Converters.RatioConverter; }
        }

        #endregion

        #region Methods

        public override Boolean Validate(RenderDevice device)
        {
            var ratio = Tuple.Create(1f, 1f);
            var expected = ratio.Item1 / ratio.Item2;
            var available = (Single)device.DeviceWidth / (Single)device.DeviceHeight;
            return Assert(expected, available);
        }

        #endregion
    }
}
