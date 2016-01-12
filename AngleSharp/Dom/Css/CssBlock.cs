namespace AngleSharp.Dom.Css
{
    using System;

    sealed class CssBlock : CssNode
    {
        public override String ToCss(IStyleFormatter formatter)
        {
            return formatter.Block(Children);
        }
    }
}
