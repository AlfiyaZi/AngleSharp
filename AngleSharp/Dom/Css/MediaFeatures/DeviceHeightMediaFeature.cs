namespace AngleSharp.Dom.Css
{
    using AngleSharp.Css;
    using AngleSharp.Css.Values;
    using System;

    /// <summary>
    /// Represents the device-height constraint.
    /// </summary>
    sealed class DeviceHeightMediaFeature : MediaFeature
    {
        #region ctor

        public DeviceHeightMediaFeature(String name)
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
            var available = (Single)device.DeviceHeight;
            return Assert(expected, available);
        }

        #endregion
    }
}
