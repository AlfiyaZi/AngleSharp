namespace AngleSharp.Css
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// Represents the an CSS3 markup formatter with inserted intends.
    /// </summary>
    public sealed class PrettyStyleFormatter : IStyleFormatter
    {
        #region Fields

        String _intendString;
        String _newLineString;

        #endregion

        #region ctor

        /// <summary>
        /// Creates a new instance of the pretty style formatter.
        /// </summary>
        public PrettyStyleFormatter()
        {
            _intendString = "\t";
            _newLineString = "\n";
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the indentation string.
        /// </summary>
        public String Indentation
        {
            get { return _intendString; }
            set { _intendString = value; }
        }

        /// <summary>
        /// Gets or sets the newline string.
        /// </summary>
        public String NewLine
        {
            get { return _newLineString; }
            set { _newLineString = value; }
        }

        #endregion

        #region Methods

        String IStyleFormatter.Rules(IEnumerable<IStyleFormattable> rules)
        {
            var lines = new List<String>();

            foreach (var rule in rules)
            {
                lines.Add(rule.ToCss(this));
            }

            return String.Join(_newLineString + _newLineString, lines);
        }

        String IStyleFormatter.Declaration(String name, String value, Boolean important)
        {
            return CssStyleFormatter.Instance.Declaration(name, value, important);
        }

        String IStyleFormatter.Declarations(IEnumerable<String> declarations)
        {
            return String.Join(_newLineString, declarations.Select(m => m + ";"));
        }

        String IStyleFormatter.Constraint(String name, String value)
        {
            return CssStyleFormatter.Instance.Constraint(name, value);
        }

        String IStyleFormatter.Comment(String data)
        {
            return CssStyleFormatter.Instance.Comment(data);
        }

        String IStyleFormatter.Medium(Boolean exclusive, Boolean inverse, String type, IEnumerable<String> constraints)
        {
            return CssStyleFormatter.Instance.Medium(exclusive, inverse, type, constraints);
        }

        String IStyleFormatter.Rule(String name, IEnumerable<IStyleFormattable> children)
        {
            return CssStyleFormatter.Instance.Rule(name, children);
        }

        String IStyleFormatter.Block(IEnumerable<IStyleFormattable> children)
        {
            var sb = Pool.NewStringBuilder().Append('{');

            foreach (var child in children)
            {
                var rule = child.ToCss(this);
                var str = Intend(rule);
                sb.Append(_newLineString).Append(str);
            }

            sb.Append(_newLineString).Append('}');
            return sb.ToPool();
        }

        String IStyleFormatter.Style(String selector, String declarations)
        {
            var sb = Pool.NewStringBuilder().Append(selector);
            sb.Append(" {").Append(_newLineString);
            sb.Append(Intend(declarations));
            sb.Append(_newLineString).Append('}');
            return sb.ToPool();
        }

        #endregion

        #region Helpers

        String Intend(String content)
        {
            return _intendString + content.Replace(_newLineString, _newLineString + _intendString);
        }

        #endregion
    }
}
