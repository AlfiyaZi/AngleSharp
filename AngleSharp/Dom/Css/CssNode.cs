﻿namespace AngleSharp.Dom.Css
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// Represents a node in the CSS parse tree.
    /// </summary>
    abstract class CssNode : ICssNode
    {
        #region Fields

        readonly List<ICssNode> _children;

        #endregion

        #region ctor

        public CssNode()
        {
            _children = new List<ICssNode>();
        }

        #endregion

        #region Properties

        public IEnumerable<ICssNode> Children
        {
            get { return _children.AsEnumerable(); }
        }

        #endregion

        #region Methods

        public abstract String ToCss(IStyleFormatter formatter);

        public void AppendChild(ICssNode child)
        {
            Setup(child);
            _children.Add(child);
        }

        public void ReplaceChild(ICssNode oldChild, ICssNode newChild)
        {
            if (oldChild != null)
            {
                for (var i = 0; i < _children.Count; i++)
                {
                    if (Object.ReferenceEquals(oldChild, _children[i]))
                    {
                        Teardown(oldChild);

                        if (newChild != null)
                        {
                            Setup(newChild);
                            _children[i] = newChild;
                        }
                        else
                        {
                            _children.RemoveAt(i);
                        }

                        return;
                    }
                }
            }

            AppendChild(newChild);
        }

        public void InsertBefore(ICssNode referenceChild, ICssNode child)
        {
            if (referenceChild != null)
            {
                var index = _children.IndexOf(referenceChild);
                InsertChild(index, child);
            }
            else
            {
                AppendChild(child);
            }
        }

        public void InsertChild(Int32 index, ICssNode child)
        {
            if (child != null)
            {
                Setup(child);
                _children.Insert(index, child);
            }
        }

        public void RemoveChild(ICssNode child)
        {
            Teardown(child);
            _children.Remove(child);
        }

        public void Clear()
        {
            for (int i = _children.Count - 1; i >= 0; i--)
            {
                var child = _children[i];
                RemoveChild(child);
            }
        }

        #endregion

        #region Internal Methods

        protected void ReplaceAll(ICssNode node)
        {
            Clear();

            foreach (var child in node.Children)
            {
                AppendChild(child);
            }
        }

        protected TChild GetValue<TChild>()
            where TChild : ICssNode
        {
            return GetValue<TChild, TChild>(m => m);
        }

        protected TMember GetValue<TChild, TMember>(Func<TChild, TMember> getter)
            where TChild : ICssNode
        {
            return Children.OfType<TChild>().Select(getter).FirstOrDefault();
        }

        protected void SetValue<TChild, TMember>(TMember value, Func<TMember, TChild> creator)
            where TChild : ICssNode
        {
            var existing = Children.OfType<TChild>().FirstOrDefault();
            var novel = value != null ? creator(value) : default(TChild);
            ReplaceChild(existing, novel);
        }

        #endregion

        #region Helper

        void Setup(ICssNode child)
        {
            var rule = child as CssRule;

            if (rule != null)
            {
                rule.Owner = this as ICssStyleSheet;
                rule.Parent = this as ICssRule;
            }
        }

        void Teardown(ICssNode child)
        {
            var rule = child as CssRule;

            if (rule != null)
            {
                rule.Parent = null;
                rule.Owner = null;
            }
        }

        #endregion
    }
}
