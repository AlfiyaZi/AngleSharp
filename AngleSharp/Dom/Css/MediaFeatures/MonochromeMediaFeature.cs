namespace AngleSharp.Dom.Css
{
    using AngleSharp.Css;
    using AngleSharp.Extensions;
    using System;

    /// <summary>
    /// Represents the monochrome constraint.
    /// </summary>
    sealed class MonochromeMediaFeature : MediaFeature
    {
        #region ctor

        public MonochromeMediaFeature(String name)
            : base(name)
        {
        }

        #endregion

        #region Internal Properties

        internal override IValueConverter Converter
        {
            get
            {
                return IsMinimum || IsMaximum ?
                    Converters.NaturalIntegerConverter :
                    Converters.NaturalIntegerConverter.Option(1);
            }
        }

        #endregion

        #region Methods

        public override Boolean Validate(RenderDevice device)
        {
            var index = 0;
            var expected = index;
            var available = device.MonochromeBits;
            return Assert(expected, available);
        }

        #endregion
    }
}
