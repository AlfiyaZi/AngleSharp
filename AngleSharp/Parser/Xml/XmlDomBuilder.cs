﻿namespace AngleSharp.Parser.Xml
{
    using AngleSharp.Dom;
    using AngleSharp.Dom.Xml;
    using AngleSharp.Extensions;
    using AngleSharp.Services;
    using AngleSharp.Xml;
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Threading;
    using System.Threading.Tasks;

    /// <summary>
    /// Represents the Tree construction as specified in the official W3C
    /// specification for XML:
    /// http://www.w3.org/TR/REC-xml/
    /// </summary>
    [DebuggerStepThrough]
    sealed class XmlDomBuilder : IDisposable
    {
        #region Fields

        readonly XmlTokenizer _tokenizer;
        readonly Document _document;
        readonly List<Element> _openElements;

        XmlParserOptions _options;
        XmlTreeMode _currentMode;
        Boolean _standalone;

        #endregion

        #region ctor

        /// <summary>
        /// Creates a new instance of the XML parser.
        /// </summary>
        /// <param name="document">The document instance to be filled.</param>
        internal XmlDomBuilder(Document document)
        {
            var resolver = document.Options.GetService<IEntityService>() ?? XmlEntityService.Resolver;
            _tokenizer = new XmlTokenizer(document.Source, document.Options.Events, resolver);
            _document = document;
            _standalone = false;
            _openElements = new List<Element>();
            _currentMode = XmlTreeMode.Initial;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the current node.
        /// </summary>
        public Node CurrentNode
        {
            get
            {
                if (_openElements.Count > 0)
                {
                    return _openElements[_openElements.Count - 1];
                }
                
                return _document;
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Parses the given source asynchronously and creates the document.
        /// </summary>
        /// <param name="options">The options to use for parsing.</param>
        /// <param name="cancelToken">The cancellation token to use.</param>
        public async Task<Document> ParseAsync(XmlParserOptions options, CancellationToken cancelToken)
        {
            var source = _document.Source;
            var token = default(XmlToken);
            _options = options;

            do
            {
                if (source.Length - source.Index < 1024)
                {
                    await source.Prefetch(8192, cancelToken).ConfigureAwait(false);
                }

                token = _tokenizer.Get();
                Consume(token);
            }
            while (token.Type != XmlTokenType.EndOfFile);

            return _document;
        }

        /// <summary>
        /// Parses the given source and creates the document.
        /// </summary>
        /// <param name="options">The options to use for parsing.</param>
        public Document Parse(XmlParserOptions options)
        {
            var token = default(XmlToken);

            do
            {
                token = _tokenizer.Get();
                Consume(token);
            }
            while (token.Type != XmlTokenType.EndOfFile);

            return _document;
        }

        /// <summary>
        /// Releases some parser allocated resources.
        /// </summary>
        public void Dispose()
        {
            _tokenizer.Dispose();
        }

        #endregion

        #region States

        /// <summary>
        /// Consumes a token and processes it.
        /// </summary>
        /// <param name="token">The token to consume.</param>
        void Consume(XmlToken token)
        {
            switch (_currentMode)
            {
                case XmlTreeMode.Initial:
                    Initial(token);
                    break;
                case XmlTreeMode.Prolog:
                    BeforeDoctype(token);
                    break;
                case XmlTreeMode.Misc:
                    InMisc(token);
                    break;
                case XmlTreeMode.Body:
                    InBody(token);
                    break;
                case XmlTreeMode.After:
                    AfterBody(token);
                    break;
            }
        }

        /// <summary>
        /// The initial state. Expects an XML declaration.
        /// </summary>
        /// <param name="token">The consumed token.</param>
        void Initial(XmlToken token)
        {
            if (token.Type == XmlTokenType.Declaration)
            {
                var declarationToken = (XmlDeclarationToken)token;
                _standalone = declarationToken.Standalone;

                if (!declarationToken.IsEncodingMissing)
                {
                    SetEncoding(declarationToken.Encoding);
                }

                if (!CheckVersion(declarationToken.Version) && !_options.IsSuppressingErrors)
                {
                    throw XmlParseError.XmlDeclarationVersionUnsupported.At(token.Position);
                }
            }
            else
            {
                _currentMode = XmlTreeMode.Prolog;
                BeforeDoctype(token);
            }
        }

        /// <summary>
        /// Before any doctype - still in the prolog. No declaration
        /// allowed.
        /// </summary>
        /// <param name="token">The consumed token.</param>
        void BeforeDoctype(XmlToken token)
        {
            switch (token.Type)
            {
                case XmlTokenType.Doctype:
                {
                    var doctypeToken = (XmlDoctypeToken)token;
                    var doctypeNode = new DocumentType(_document, doctypeToken.Name)
                    {
                        SystemIdentifier = doctypeToken.SystemIdentifier,
                        PublicIdentifier = doctypeToken.PublicIdentifier
                    };
                    _document.AppendChild(doctypeNode);
                    _currentMode = XmlTreeMode.Misc;

                    break;
                }
                default:
                {
                    InMisc(token);
                    break;
                }
            }
        }

        /// <summary>
        /// In the body state - no doctypes and declarations allowed.
        /// </summary>
        /// <param name="token">The consumed token.</param>
        void InMisc(XmlToken token)
        {
            switch (token.Type)
            {
                case XmlTokenType.Comment:
                {
                    var commenToken = (XmlCommentToken)token;
                    var commentNode = _document.CreateComment(commenToken.Data);
                    CurrentNode.AppendChild(commentNode);
                    break;
                }
                case XmlTokenType.ProcessingInstruction:
                {
                    var piToken = (XmlPIToken)token;
                    var piNode = _document.CreateProcessingInstruction(piToken.Target, piToken.Content);
                    CurrentNode.AppendChild(piNode);
                    break;
                }
                case XmlTokenType.StartTag:
                {
                    _currentMode = XmlTreeMode.Body;
                    InBody(token);
                    break;
                }
                default:
                {
                    if (!token.IsIgnorable && !_options.IsSuppressingErrors)
                    {
                        throw XmlParseError.XmlMissingRoot.At(token.Position);
                    }

                    break;
                }
            }
        }

        /// <summary>
        /// In the body state - no doctypes and declarations allowed.
        /// </summary>
        /// <param name="token">The consumed token.</param>
        void InBody(XmlToken token)
        {
            switch (token.Type)
            {
                case XmlTokenType.StartTag:
                {
                    var tagToken = (XmlTagToken)token;
                    var element = new XmlElement(_document, tagToken.Name);
                    CurrentNode.AppendChild(element);

                    if (!tagToken.IsSelfClosing)
                    {
                        _openElements.Add(element);
                    }
                    else if (_openElements.Count == 0)
                    {
                        _currentMode = XmlTreeMode.After;
                    }

                    for (var i = 0; i < tagToken.Attributes.Count; i++)
                    {
                        var name = tagToken.Attributes[i].Key;
                        var value = tagToken.Attributes[i].Value.Trim();
                        element.SetAttribute(name, value);
                    }

                    break;
                }
                case XmlTokenType.EndTag:
                {
                    var tagToken = (XmlTagToken)token;

                    if (!CurrentNode.NodeName.Is(tagToken.Name))
                    {
                        if (_options.IsSuppressingErrors)
                        {
                            break;
                        }

                        throw XmlParseError.TagClosingMismatch.At(token.Position);
                    }

                    _openElements.RemoveAt(_openElements.Count - 1);

                    if (_openElements.Count == 0)
                    {
                        _currentMode = XmlTreeMode.After;
                    }

                    break;
                }
                case XmlTokenType.ProcessingInstruction:
                case XmlTokenType.Comment:
                {
                    InMisc(token);
                    break;
                }
                case XmlTokenType.CData:
                {
                    var cdataToken = (XmlCDataToken)token;
                    CurrentNode.AppendText(cdataToken.Data);
                    break;
                }
                case XmlTokenType.Character:
                {
                    var charToken = (XmlCharacterToken)token;
                    CurrentNode.AppendText(charToken.Data);
                    break;
                }
                case XmlTokenType.EndOfFile:
                {
                    if (_options.IsSuppressingErrors)
                    {
                        break;
                    }

                    throw XmlParseError.EOF.At(token.Position);
                }
                case XmlTokenType.Doctype:
                {
                    if (_options.IsSuppressingErrors)
                    {
                        break;
                    }

                    throw XmlParseError.XmlDoctypeAfterContent.At(token.Position);
                }
                case XmlTokenType.Declaration:
                {
                    if (_options.IsSuppressingErrors)
                    {
                        break;
                    }

                    throw XmlParseError.XmlDeclarationMisplaced.At(token.Position);
                }
            }
        }

        /// <summary>
        /// After the body state - nothing except Comment PI S allowed.
        /// </summary>
        /// <param name="token">The consumed token.</param>
        void AfterBody(XmlToken token)
        {
            switch (token.Type)
            {
                case XmlTokenType.ProcessingInstruction:
                case XmlTokenType.Comment:
                {
                    InMisc(token);
                    break;
                }
                case XmlTokenType.EndOfFile:
                {
                    break;
                }
                default:
                {
                    if (!token.IsIgnorable && !_options.IsSuppressingErrors)
                    {
                        throw XmlParseError.XmlMissingRoot.At(token.Position);
                    }

                    break;
                }
            }
        }

        #endregion

        #region Helpers

        /// <summary>
        /// Checks the given version number.
        /// </summary>
        /// <param name="ver"></param>
        /// <returns></returns>
        Boolean CheckVersion(String ver)
        {
            var t = ver.ToDouble(0.0);
            return t >= 1.0 && t < 2.0;
        }

        /// <summary>
        /// Sets the document's encoding to the given one.
        /// </summary>
        /// <param name="charSet">The encoding to use.</param>
        void SetEncoding(String charSet)
        {
            if (TextEncoding.IsSupported(charSet))
            {
                var encoding = TextEncoding.Resolve(charSet);

                if (encoding != null)
                {
                    try
                    {
                        _document.Source.CurrentEncoding = encoding;
                    }
                    catch (NotSupportedException)
                    {
                        _currentMode = XmlTreeMode.Initial;
                        _document.ReplaceAll(null, true);
                        _openElements.Clear();
                    }
                }
            }
        }

        #endregion
    }
}
