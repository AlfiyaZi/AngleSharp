﻿namespace AngleSharp.Dom.Css
{
    using AngleSharp.Css;
    using AngleSharp.Extensions;
    using System;

    /// <summary>
    /// Represents a simple selector (either a type selector, universal
    /// selector, attribute, class, id or pseudo-class selector).
    /// </summary>
    sealed class SimpleSelector : CssNode, ISelector
    {
        #region Fields

        readonly Predicate<IElement> _matches;
        readonly Priority _specifity;
        readonly String _code;

        #endregion

        #region ctor

        public SimpleSelector()
            : this(_ => true, Priority.Zero, "*")
        {
        }

        public SimpleSelector(String match)
            : this(el => match.Isi(el.LocalName), Priority.OneTag, match)
        {
        }

        public SimpleSelector(Predicate<IElement> matches, Priority specifify, String code)
        {
            _matches = matches;
            _specifity = specifify;
            _code = code;
        }

        #endregion

        #region Properties

        public static readonly SimpleSelector All = new SimpleSelector();

        public Priority Specifity
        {
            get { return _specifity; }
        }

        public String Text
        {
            get { return _code; }
        }

        #endregion

        #region Static constructors

        public static SimpleSelector PseudoElement(Predicate<IElement> action, String pseudoElement)
        {
            return new SimpleSelector(action, Priority.OneTag, "::" + pseudoElement);
        }

        public static SimpleSelector PseudoClass(Predicate<IElement> action, String pseudoClass)
        {
            return new SimpleSelector(action, Priority.OneClass, ":" + pseudoClass);
        }

        public static SimpleSelector Class(String match)
        {
            return new SimpleSelector(_ => _.ClassList.Contains(match), Priority.OneClass, "." + match);
        }

        public static SimpleSelector Id(String match)
        {
            return new SimpleSelector(_ => _.Id == match, Priority.OneId, "#" + match);
        }

        public static SimpleSelector AttrAvailable(String match, String prefix = null)
        {
            var front = match;

            if (!String.IsNullOrEmpty(prefix))
            {
                front = String.Concat(prefix, "|", match);
                match = Bundle(prefix, match);
            }

            return new SimpleSelector(_ => _.HasAttribute(match), Priority.OneClass, String.Concat("[", front, "]"));
        }

        public static SimpleSelector AttrMatch(String match, String value, String prefix = null)
        {
            var front = match;

            if (!String.IsNullOrEmpty(prefix))
            {
                front = String.Concat(prefix, "|", match);
                match = Bundle(prefix, match);
            }

            var code = String.Format("[{0}={1}]", front, value.CssString());
            return new SimpleSelector(_ => _.GetAttribute(match) == value, Priority.OneClass, code);
        }

        public static SimpleSelector AttrNotMatch(String match, String value, String prefix = null)
        {
            var front = match;

            if (!String.IsNullOrEmpty(prefix))
            {
                front = String.Concat(prefix, "|", match);
                match = Bundle(prefix, match);
            }

            var code = String.Format("[{0}!={1}]", front, value.CssString());
            return new SimpleSelector(_ => _.GetAttribute(match) != value, Priority.OneClass, code);
        }

        public static SimpleSelector AttrList(String match, String value, String prefix = null)
        {
            var front = match;

            if (!String.IsNullOrEmpty(prefix))
            {
                front = String.Concat(prefix, "|", match);
                match = Bundle(prefix, match);
            }

            var code = String.Format("[{0}~={1}]", front, value.CssString());
            var matches = Select(value, _ => (_.GetAttribute(match) ?? String.Empty).SplitSpaces().Contains(value));
            return new SimpleSelector(matches, Priority.OneClass, code);
        }

        public static SimpleSelector AttrBegins(String match, String value, String prefix = null)
        {
            var front = match;

            if (!String.IsNullOrEmpty(prefix))
            {
                front = String.Concat(prefix, "|", match);
                match = Bundle(prefix, match);
            }

            var code = String.Format("[{0}^={1}]", front, value.CssString());
            var matches = Select(value, _ => (_.GetAttribute(match) ?? String.Empty).StartsWith(value));
            return new SimpleSelector(matches, Priority.OneClass, code);
        }

        public static SimpleSelector AttrEnds(String match, String value, String prefix = null)
        {
            var front = match;

            if (!String.IsNullOrEmpty(prefix))
            {
                front = String.Concat(prefix, "|", match);
                match = Bundle(prefix, match);
            }

            var code = String.Format("[{0}$={1}]", front, value.CssString());
            var matches = Select(value, _ => (_.GetAttribute(match) ?? String.Empty).EndsWith(value));
            return new SimpleSelector(matches, Priority.OneClass, code);
        }

        public static SimpleSelector AttrContains(String match, String value, String prefix = null)
        {
            var front = match;

            if (!String.IsNullOrEmpty(prefix))
            {
                front = String.Concat(prefix, "|", match);
                match = Bundle(prefix, match);
            }

            var code = String.Format("[{0}*={1}]", front, value.CssString());
            var matches = Select(value, _ => (_.GetAttribute(match) ?? String.Empty).Contains(value));
            return new SimpleSelector(matches, Priority.OneClass, code);
        }

        public static SimpleSelector AttrHyphen(String match, String value, String prefix = null)
        {
            var front = match;

            if (!String.IsNullOrEmpty(prefix))
            {
                front = String.Concat(prefix, "|", match);
                match = Bundle(prefix, match);
            }

            var code = String.Format("[{0}|={1}]", front, value.CssString());
            var matches = Select(value, _ => (_.GetAttribute(match) ?? String.Empty).HasHyphen(value));
            return new SimpleSelector(matches, Priority.OneClass, code);
        }

        public static SimpleSelector Type(String match)
        {
            return new SimpleSelector(match);
        }

        #endregion

        #region Methods

        public Boolean Match(IElement element)
        {
            return _matches(element);
        }

        public override String ToCss(IStyleFormatter formatter)
        {
            return Text;
        }

        #endregion

        #region Helpers

        static Predicate<IElement> Select(String value, Predicate<IElement> predicate)
        {
            return String.IsNullOrEmpty(value) ? (_ => false) : predicate;
        }

        static String Bundle(String prefix, String match)
        {
            return prefix.Is("*") ? match : String.Concat(prefix, ":", match);
        }

        #endregion
    }
}
