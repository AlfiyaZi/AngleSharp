namespace AngleSharp.Dom.Css
{
    using AngleSharp.Extensions;
    using System;
    using System.Linq;

    /// <summary>
    /// Represents a feature expression within a media query.
    /// </summary>
    abstract class DocumentFunction : CssNode, IDocumentFunction
    {
        #region Fields

        readonly String _name;

        #endregion

        #region ctor

        internal DocumentFunction(String name)
        {
            _name = name;
        }

        #endregion

        #region Properties

        public String Name
        {
            get { return _name; }
        }

        public String Data
        {
            get { return Children.OfType<CssRawString>().Select(m => m.CssText).FirstOrDefault(); }
        }

        #endregion

        #region Methods

        public abstract Boolean Matches(Url url);

        #endregion

        #region String Representation

        public override String ToCss(IStyleFormatter formatter)
        {
            return _name.CssFunction(Data.CssString());
        }

        #endregion
    }
}
