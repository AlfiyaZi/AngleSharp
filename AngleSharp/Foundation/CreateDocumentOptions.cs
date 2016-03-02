namespace AngleSharp
{
    using AngleSharp.Dom;
    using AngleSharp.Dom.Html;
    using AngleSharp.Dom.Svg;
    using AngleSharp.Dom.Xml;
    using AngleSharp.Extensions;
    using AngleSharp.Html;
    using AngleSharp.Network;
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    /// <summary>
    /// Data transport class to abstract common options in document creation.
    /// </summary>
    sealed class CreateDocumentOptions
    {
        #region ctor

        public CreateDocumentOptions(IResponse response, IConfiguration configuration)
        {
            var contentType = response.GetContentType(MimeTypeNames.Html);
            var encoding = configuration.DefaultEncoding();
            var charset = contentType.GetParameter(AttributeNames.Charset);

            if (!String.IsNullOrEmpty(charset) && TextEncoding.IsSupported(charset))
            {
                encoding = TextEncoding.Resolve(charset);
            }

            ContentType = contentType;
            Source = new TextSource(response.Content, encoding);
            Response = response;
        }

        #endregion

        #region Properties

        public IResponse Response
        {
            get;
            private set;
        }

        public TextSource Source
        {
            get;
            private set;
        }

        public MimeType ContentType
        {
            get;
            private set;
        }

        public IDocument ImportAncestor 
        { 
            get; 
            set; 
        }

        #endregion

        #region Methods

        public Func<IBrowsingContext, CreateDocumentOptions, CancellationToken, Task<IDocument>> FindCreator()
        {
            var contentType = ContentType;

            if (contentType.Represents(MimeTypeNames.Xml) || contentType.Represents(MimeTypeNames.ApplicationXml))
            {
                return XmlDocument.LoadAsync;
            }
            else if (contentType.Represents(MimeTypeNames.Svg))
            {
                return SvgDocument.LoadAsync;
            }

            return HtmlDocument.LoadAsync;
        }

        #endregion
    }
}
