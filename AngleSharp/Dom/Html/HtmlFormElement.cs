﻿namespace AngleSharp.Dom.Html
{
    using AngleSharp.Dom.Collections;
    using AngleSharp.Extensions;
    using AngleSharp.Html;
    using AngleSharp.Network;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Text;
    using System.Threading.Tasks;

    /// <summary>
    /// Represents the form element.
    /// </summary>
    sealed class HtmlFormElement : HtmlElement, IHtmlFormElement
    {
        #region Fields

        HtmlFormControlsCollection _elements;

        #endregion

        #region ctor

        /// <summary>
        /// Creates a new HTML form element.
        /// </summary>
        public HtmlFormElement(Document owner, String prefix = null)
            : base(owner, TagNames.Form, prefix, NodeFlags.Special)
        {
        }

        #endregion

        #region Index

        /// <summary>
        /// Gets the form element at the specified index.
        /// </summary>
        /// <param name="index">The index in the elements collection.</param>
        /// <returns>The element or null.</returns>
        public IElement this[Int32 index]
        {
            get { return Elements[index]; }
        }

        /// <summary>
        /// Gets the form element(s) with the specified name.
        /// </summary>
        /// <param name="name">The name or id of the element.</param>
        /// <returns>A collection with elements, an element or null.</returns>
        public IElement this[String name]
        {
            get { return Elements[name]; }
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the value of the name attribute.
        /// </summary>
        public String Name
        {
            get { return this.GetOwnAttribute(AttributeNames.Name); }
            set { this.SetOwnAttribute(AttributeNames.Name, value); }
        }

        /// <summary>
        /// Gets the number of elements in the Elements collection.
        /// </summary>
        public Int32 Length
        {
            get { return Elements.Length; }
        }

        /// <summary>
        /// Gets all the form controls belonging to this form element.
        /// </summary>
        public HtmlFormControlsCollection Elements
        {
            get { return _elements ?? (_elements = new HtmlFormControlsCollection(this)); }
        }

        /// <summary>
        /// Gets all the form controls belonging to this form element.
        /// </summary>
        IHtmlFormControlsCollection IHtmlFormElement.Elements
        {
            get { return Elements; }
        }

        /// <summary>
        /// Gets or sets the character encodings that are to be used for the submission.
        /// </summary>
        public String AcceptCharset
        {
            get { return this.GetOwnAttribute(AttributeNames.AcceptCharset); }
            set { this.SetOwnAttribute(AttributeNames.AcceptCharset, value); }
        }

        /// <summary>
        /// Gets or sets the form's name within the forms collection.
        /// </summary>
        public String Action
        {
            get { return this.GetOwnAttribute(AttributeNames.Action); }
            set { this.SetOwnAttribute(AttributeNames.Action, value); }
        }

        /// <summary>
        /// Gets or sets if autocomplete is turned on or off.
        /// </summary>
        public String Autocomplete
        {
            get { return this.GetOwnAttribute(AttributeNames.AutoComplete); }
            set { this.SetOwnAttribute(AttributeNames.AutoComplete, value); }
        }

        /// <summary>
        /// Gets or sets the encoding to use for sending the form.
        /// </summary>
        public String Enctype
        {
            get { return CheckEncType(this.GetOwnAttribute(AttributeNames.Enctype)); }
            set { this.SetOwnAttribute(AttributeNames.Enctype, CheckEncType(value)); }
        }

        /// <summary>
        /// Gets or sets the encoding to use for sending the form.
        /// </summary>
        public String Encoding
        {
            get { return Enctype; }
            set { Enctype = value; }
        }

        /// <summary>
        /// Gets or sets the method to use for transmitting the form.
        /// </summary>
        public String Method
        {
            get { return this.GetOwnAttribute(AttributeNames.Method) ?? String.Empty; }
            set { this.SetOwnAttribute(AttributeNames.Method, value); }
        }

        /// <summary>
        /// Gets or sets the indicator that the form is not to be validated during submission.
        /// </summary>
        public Boolean NoValidate
        {
            get { return this.HasOwnAttribute(AttributeNames.NoValidate); }
            set { this.SetOwnAttribute(AttributeNames.NoValidate, value ? String.Empty : null); }
        }

        /// <summary>
        /// Gets or sets the target name of the response to the request.
        /// </summary>
        public String Target
        {
            get { return this.GetOwnAttribute(AttributeNames.Target); }
            set { this.SetOwnAttribute(AttributeNames.Target, value); }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Submits the form element from the form element itself.
        /// </summary>
        public Task<IDocument> Submit()
        {
            var request = GetSubmission();
            return this.NavigateTo(request);
        }

        /// <summary>
        /// Submits the form element from another element.
        /// </summary>
        public Task<IDocument> Submit(IHtmlElement sourceElement)
        {
            var request = GetSubmission(sourceElement);
            return this.NavigateTo(request);
        }

        /// <summary>
        /// Gets the document request created from the form submitting itself.
        /// </summary>
        public DocumentRequest GetSubmission()
        {
            return SubmitForm(this, true);
        }

        /// <summary>
        /// Gets the document request created from the form being submitted by another element.
        /// </summary>
        /// <param name="sourceElement">The form's submitter.</param>
        public DocumentRequest GetSubmission(IHtmlElement sourceElement)
        {
            return SubmitForm(sourceElement ?? this, false);
        }

        /// <summary>
        /// Resets the form to the previous (default) state.
        /// </summary>
        public void Reset()
        {
            foreach (var element in Elements)
            {
                element.Reset();
            }
        }

        /// <summary>
        /// Checks if the form is valid, i.e. if all fields fulfill their requirements.
        /// </summary>
        /// <returns>True if the form is valid, otherwise false.</returns>
        public Boolean CheckValidity()
        {
            var controls = GetInvalidControls();
            var result = true;

            foreach (var control in controls)
            {
                if (!control.FireSimpleEvent(EventNames.Invalid, false, true))
                {
                    result = false;
                }
            }

            return result;
        }

        /// <summary>
        /// Evaluates the form controls according to the spec:
        /// https://html.spec.whatwg.org/multipage/forms.html#statically-validate-the-constraints
        /// </summary>
        /// <returns>A list over all invalid controls.</returns>
        IEnumerable<HtmlFormControlElement> GetInvalidControls()
        {
            foreach (var element in Elements)
            {
                if (element.WillValidate && !element.CheckValidity())
                {
                    yield return element;
                }
            }
        }

        public Boolean ReportValidity()
        {
            var controls = GetInvalidControls();
            var result = true;
            var hasfocused = false;

            foreach (var control in controls)
            {
                if (control.FireSimpleEvent(EventNames.Invalid, false, true))
                {
                    continue;
                }

                if (!hasfocused)
                {
                    //TODO Report Problems (interactively, e.g. via UI specific event)
                    control.DoFocus();
                    hasfocused = true;
                }

                result = false;
            }

            return result;
        }

        /// <summary>
        /// Requests the input fields to be automatically filled with previous entries.
        /// </summary>
        public void RequestAutocomplete()
        {
            //TODO see:
            //http://www.whatwg.org/specs/web-apps/current-work/multipage/association-of-controls-and-forms.html#dom-form-requestautocomplete
        }

        #endregion

        #region Helpers

        DocumentRequest SubmitForm(IHtmlElement from, Boolean submittedFromSubmitMethod)
        {
            var owner = Owner;

            if (owner.ActiveSandboxing.HasFlag(Sandboxes.Forms))
            {
                //Do nothing.
            }
            else if (!submittedFromSubmitMethod && !from.HasAttribute(AttributeNames.FormNoValidate) && !NoValidate && !CheckValidity())
            {
                this.FireSimpleEvent(EventNames.Invalid);
            }
            else
            {
                var action = String.IsNullOrEmpty(Action) ? new Url(owner.DocumentUri) : this.HyperReference(Action);
                var createdBrowsingContext = false;
                var targetBrowsingContext = owner.Context;
                var target = Target;

                if (!String.IsNullOrEmpty(target))
                {
                    targetBrowsingContext = owner.GetTarget(target);

                    if (createdBrowsingContext = (targetBrowsingContext == null))
                    {
                        targetBrowsingContext = owner.CreateTarget(target);
                    }
                }

                var replace = createdBrowsingContext || owner.ReadyState != DocumentReadyState.Complete;
                var scheme = action.Scheme;
                var method = Method.ToEnum(HttpMethod.Get);
                return SubmitForm(method, scheme, action, from);
            }

            return null;
        }

        DocumentRequest SubmitForm(HttpMethod method, String scheme, Url action, IHtmlElement submitter)
        {
            if (scheme.IsOneOf(ProtocolNames.Http, ProtocolNames.Https))
            {
                if (method == HttpMethod.Get)
                {
                    return MutateActionUrl(action, submitter);
                }
                else if (method == HttpMethod.Post)
                {
                    return SubmitAsEntityBody(action, submitter);
                }
            }
            else if (scheme.Is(ProtocolNames.Data))
            {
                if (method == HttpMethod.Get)
                {
                    return GetActionUrl(action);
                }
                else if (method == HttpMethod.Post)
                {
                    return PostToData(action, submitter);
                }
            }
            else if (scheme.Is(ProtocolNames.Mailto))
            {
                if (method == HttpMethod.Get)
                {
                    return MailWithHeaders(action, submitter);
                }
                else if (method == HttpMethod.Post)
                {
                    return MailAsBody(action, submitter);
                }
            }
            else if (scheme.IsOneOf(ProtocolNames.Ftp, ProtocolNames.JavaScript))
            {
                return GetActionUrl(action);
            }

            return MutateActionUrl(action, submitter);
        }

        /// <summary>
        /// More information can be found at:
        /// http://www.w3.org/html/wg/drafts/html/master/forms.html#submit-data-post
        /// </summary>
        DocumentRequest PostToData(Url action, IHtmlElement submitter)
        {
            var encoding = String.IsNullOrEmpty(AcceptCharset) ? Owner.CharacterSet : AcceptCharset;
            var formDataSet = ConstructDataSet(submitter);
            var enctype = Enctype;
            var result = String.Empty;
            var stream = CreateBody(enctype, TextEncoding.Resolve(encoding), formDataSet);

            using (var sr = new StreamReader(stream))
            {
                result = sr.ReadToEnd();
            }

            if (action.Href.Contains("%%%%"))
            {
                result = TextEncoding.UsAscii.GetBytes(result).UrlEncode();
                action.Href = action.Href.ReplaceFirst("%%%%", result);
            }
            else if (action.Href.Contains("%%"))
            {
                result = TextEncoding.Utf8.GetBytes(result).UrlEncode();
                action.Href = action.Href.ReplaceFirst("%%", result);
            }

            return GetActionUrl(action);
        }

        /// <summary>
        /// More information can be found at:
        /// http://www.w3.org/html/wg/drafts/html/master/forms.html#submit-mailto-headers
        /// </summary>
        DocumentRequest MailWithHeaders(Url action, IHtmlElement submitter)
        {
            var formDataSet = ConstructDataSet(submitter);
            var result = formDataSet.AsUrlEncoded(TextEncoding.UsAscii);
            var headers = String.Empty;

            using (var sr = new StreamReader(result))
            {
                headers = sr.ReadToEnd();
            }

            action.Query = headers.Replace("+", "%20");
            return GetActionUrl(action);
        }

        /// <summary>
        /// More information can be found at:
        /// http://www.w3.org/html/wg/drafts/html/master/forms.html#submit-mailto-body
        /// </summary>
        DocumentRequest MailAsBody(Url action, IHtmlElement submitter)
        {
            var formDataSet = ConstructDataSet(submitter);
            var enctype = Enctype;
            var encoding = TextEncoding.UsAscii;
            var stream = CreateBody(enctype, encoding, formDataSet);
            var body = String.Empty;

            using (var sr = new StreamReader(stream))
            {
                body = sr.ReadToEnd();
            }

            action.Query = "body=" + encoding.GetBytes(body).UrlEncode();
            return GetActionUrl(action);
        }

        /// <summary>
        /// More information can be found at:
        /// http://www.w3.org/html/wg/drafts/html/master/forms.html#submit-get-action
        /// </summary>
        DocumentRequest GetActionUrl(Url action)
        {
            return DocumentRequest.Get(action, source: this, referer: Owner.DocumentUri);
        }

        /// <summary>
        /// Submits the body of the form.
        /// http://www.w3.org/html/wg/drafts/html/master/forms.html#submit-body
        /// </summary>
        DocumentRequest SubmitAsEntityBody(Url target, IHtmlElement submitter)
        {
            var encoding = String.IsNullOrEmpty(AcceptCharset) ? Owner.CharacterSet : AcceptCharset;
            var formDataSet = ConstructDataSet(submitter);
            var enctype = Enctype;
            var body = CreateBody(enctype, TextEncoding.Resolve(encoding), formDataSet);

            if (enctype.Isi(MimeTypeNames.MultipartForm))
            {
                enctype = String.Concat(MimeTypeNames.MultipartForm, "; boundary=", formDataSet.Boundary);
            }

            return DocumentRequest.Post(target, body, enctype, source: this, referer: Owner.DocumentUri);
        }

        /// <summary>
        /// More information can be found at:
        /// http://www.w3.org/html/wg/drafts/html/master/forms.html#submit-mutate-action
        /// </summary>
        DocumentRequest MutateActionUrl(Url action, IHtmlElement submitter)
        {
            var encoding = String.IsNullOrEmpty(AcceptCharset) ? Owner.CharacterSet : AcceptCharset;
            var formDataSet = ConstructDataSet(submitter);
            var result = formDataSet.AsUrlEncoded(TextEncoding.Resolve(encoding));

            using (var sr = new StreamReader(result))
            {
                action.Query = sr.ReadToEnd();
            }

            return GetActionUrl(action);
        }

        FormDataSet ConstructDataSet(IHtmlElement submitter)
        {
            var formDataSet = new FormDataSet();
            var fields = this.GetElements<HtmlFormControlElement>();

            foreach (var field in fields)
            {
                if (!field.IsDisabled && field.ParentElement is IHtmlDataListElement == false && Object.ReferenceEquals(field.Form, this))
                {
                    field.ConstructDataSet(formDataSet, submitter);
                }
            }

            return formDataSet;
        }

        static Stream CreateBody(String enctype, Encoding encoding, FormDataSet formDataSet)
        {
            if (enctype.Isi(MimeTypeNames.UrlencodedForm))
            {
                return formDataSet.AsUrlEncoded(encoding);
            }
            else if (enctype.Isi(MimeTypeNames.MultipartForm))
            {
                return formDataSet.AsMultipart(encoding);
            }
            else if (enctype.Isi(MimeTypeNames.Plain))
            {
                return formDataSet.AsPlaintext(encoding);
            }
            else if (enctype.Isi(MimeTypeNames.ApplicationJson))
            {
                return formDataSet.AsJson();
            }

            return MemoryStream.Null;
        }

        static String CheckEncType(String encType)
        {
            if (encType.Isi(MimeTypeNames.Plain) || encType.Isi(MimeTypeNames.MultipartForm) || encType.Isi(MimeTypeNames.ApplicationJson))
            {
                return encType;
            }

            return MimeTypeNames.UrlencodedForm;
        }

        #endregion
    }
}
