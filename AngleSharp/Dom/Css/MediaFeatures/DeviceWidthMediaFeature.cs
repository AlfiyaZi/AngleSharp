namespace AngleSharp.Dom.Css
{
    using AngleSharp.Css;
    using AngleSharp.Css.Values;
    using System;

    /// <summary>
    /// Represents the device-width constraint.
    /// </summary>
    sealed class DeviceWidthMediaFeature : MediaFeature
    {
        #region ctor

        public DeviceWidthMediaFeature(String name)
            : base(name)
        {
        }

        #endregion

        #region Internal Properties

        internal override IValueConverter Converter
        {
            // Default: Allowed
            get { return Converters.LengthConverter; }
        }

        #endregion

        #region Methods

        public override Boolean Validate(RenderDevice device)
        {
            var length = Length.Zero;
            var expected = length.ToPixel();
            var available = (Single)device.DeviceWidth;
            return Assert(expected, available);
        }

        #endregion
    }
}
