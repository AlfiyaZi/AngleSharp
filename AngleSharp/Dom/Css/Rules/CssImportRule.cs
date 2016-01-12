namespace AngleSharp.Dom.Css
{
    using AngleSharp.Dom.Collections;
    using AngleSharp.Extensions;
    using AngleSharp.Parser.Css;
    using System;
    using System.Linq;
    using System.Threading.Tasks;

    /// <summary>
    /// Represents a CSS import rule.
    /// </summary>
    sealed class CssImportRule : CssRule, ICssImportRule
    {
        #region Fields

        CssStyleSheet _styleSheet;

        #endregion

        #region ctor

        internal CssImportRule(CssParser parser)
            : base(CssRuleType.Import, parser)
        {
            AppendChild(new MediaList(parser));
        }

        #endregion

        #region Properties

        public String Href
        {
            get { return GetValue<CssRawUrl>(m => m.CssText); }
            set { SetValue(value, m => new CssRawUrl(m)); }
        }

        public MediaList Media
        {
            get { return Children.OfType<MediaList>().FirstOrDefault(); }
        }

        IMediaList ICssImportRule.Media
        {
            get { return Media; }
        }

        public ICssStyleSheet Sheet
        {
            get { return _styleSheet; }
        }

        #endregion

        #region Internal Methods

        internal async Task LoadStylesheetFrom(Document document)
        {
            if (document != null)
            {
                var loader = document.Loader;
                var baseUrl = Url.Create(Owner.Href ?? document.BaseUri);
                var url = new Url(baseUrl, Href);

                if (!IsRecursion(url) && loader != null)
                {
                    var element = Owner.OwnerNode;
                    var request = element.CreateRequestFor(url);
                    var download = loader.DownloadAsync(request);

                    using (var response = await download.Task.ConfigureAwait(false))
                    {
                        var sheet = new CssStyleSheet(this, response.Address.Href);
                        var source = new TextSource(response.Content);
                        _styleSheet = await Parser.ParseStylesheetAsync(sheet, source).ConfigureAwait(false);
                    }
                }
            }
        }

        protected override void ReplaceWith(ICssRule rule)
        {
            _styleSheet = null;
            base.ReplaceWith(rule);
            //TODO Load New StyleSheet
        }

        #endregion

        #region String Representation

        public override String ToCss(IStyleFormatter formatter)
        {
            return formatter.Rule("@import", Children) + ";";
        }

        #endregion

        #region Helpers

        Boolean IsRecursion(Url url)
        {
            var href = url.Href;
            var owner = Owner;

            while (owner != null && !owner.Href.Is(href))
            {
                owner = owner.Parent;
            }

            return owner != null;
        }

        #endregion
    }
}
