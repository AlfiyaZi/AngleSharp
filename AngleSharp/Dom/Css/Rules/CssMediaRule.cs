namespace AngleSharp.Dom.Css
{
    using AngleSharp.Css;
    using AngleSharp.Dom.Collections;
    using AngleSharp.Parser.Css;
    using System;
    using System.Linq;

    /// <summary>
    /// Represents a CSS @media rule.
    /// </summary>
    sealed class CssMediaRule : CssConditionRule, ICssMediaRule
    {
        #region ctor

        internal CssMediaRule(CssParser parser)
            : base(CssRuleType.Media, parser)
        {
            InsertChild(0, new MediaList(parser));
        }

        #endregion

        #region Properties

        public String ConditionText
        {
            get { return Media.MediaText; }
            set { Media.MediaText = value; }
        }

        public MediaList Media
        {
            get { return Children.OfType<MediaList>().FirstOrDefault(); }
        }

        IMediaList ICssMediaRule.Media
        {
            get { return Media; }
        }

        #endregion

        #region Internal Methods

        internal override Boolean IsValid(RenderDevice device)
        {
            return Media.Validate(device);
        }

        protected override String GetRuleName()
        {
            return "@media";
        }

        #endregion
    }
}
