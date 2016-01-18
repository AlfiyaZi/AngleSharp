namespace AngleSharp.Dom.Css
{
    using AngleSharp.Css;
    using System;

    /// <summary>
    /// Represents the grid constraint.
    /// </summary>
    sealed class GridMediaFeature : MediaFeature
    {
        #region ctor

        public GridMediaFeature()
            : base(FeatureNames.Grid)
        {
        }

        #endregion

        #region Internal Properties

        internal override IValueConverter Converter
        {
            // Default: Allowed
            get { return Converters.BinaryConverter; }
        }

        #endregion

        #region Methods

        public override Boolean Validate(RenderDevice device)
        {
            var grid = false;
            var expected = grid;
            var available = device.IsGrid;
            return expected == available;
        }

        #endregion
    }
}
