﻿namespace AngleSharp.Dom.Css
{
    using AngleSharp.Parser.Css;
    using System;
    using System.Linq;

    /// <summary>
    /// Represents a CSS rule.
    /// </summary>
    abstract class CssRule : CssNode, ICssRule
    {
        #region Fields

        readonly CssRuleType _type;
        readonly CssParser _parser;

        ICssStyleSheet _ownerSheet;
        ICssRule _parentRule;

        #endregion

        #region ctor

        internal CssRule(CssRuleType type, CssParser parser)
        {
            _type = type;
            _parser = parser;
        }

        #endregion

        #region Properties

        public String CssText
        {
            get { return this.ToCss(); }
            set
            {
                var rule = _parser.ParseRule(value);

                if (rule == null)
                {
                    throw new DomException(DomError.Syntax);
                }
                else if (rule.Type != _type)
                {
                    throw new DomException(DomError.InvalidModification);
                }

                ReplaceWith(rule);
            }
        }

        public ICssRule Parent
        {
            get { return _parentRule; }
            internal set 
            { 
                _parentRule = value; 

                if (value != null)
                {
                    _ownerSheet = _parentRule.Owner;
                }
            }
        }

        public ICssStyleSheet Owner
        {
            get { return _ownerSheet; }
            internal set { _ownerSheet = value; }
        }

        public CssRuleType Type
        {
            get { return _type; }
        }

        #endregion

        #region Internal Properties

        internal CssParser Parser
        {
            get { return _parser; }
        }

        #endregion

        #region Internal Methods

        protected virtual void ReplaceWith(ICssRule rule)
        {
            ReplaceAll(rule);
        }

        protected String GetValue<T>(Func<T, String> getter)
            where T : ICssNode
        {
            return Children.OfType<T>().Select(getter).FirstOrDefault();
        }

        protected void SetValue<T>(String value, Func<String, T> creator)
            where T : ICssNode
        {
            RemoveChild(Children.OfType<T>().FirstOrDefault());

            if (value != null)
            {
                var child = creator(value);
                AppendChild(child);
            }
        }

        protected void ReplaceSingle(ICssNode oldNode, ICssNode newNode)
        {
            if (oldNode != null)
            {
                if (newNode != null)
                {
                    ReplaceChild(oldNode, newNode);
                }
                else
                {
                    RemoveChild(oldNode);
                }
            }
            else if (newNode != null)
            {
                AppendChild(newNode);
            }
        }

        #endregion
    }
}
