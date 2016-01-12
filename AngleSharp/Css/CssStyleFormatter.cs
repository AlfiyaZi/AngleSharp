namespace AngleSharp.Css
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Represents the standard CSS3 style formatter.
    /// </summary>
    public sealed class CssStyleFormatter : IStyleFormatter
    {
        #region Instance

        /// <summary>
        /// An instance of the CssStyleFormatter.
        /// </summary>
        public static readonly IStyleFormatter Instance = new CssStyleFormatter();

        #endregion

        #region Methods

        String IStyleFormatter.Rules(IEnumerable<IStyleFormattable> rules)
        {
            var lines = new List<String>();

            foreach (var rule in rules)
            {
                lines.Add(rule.ToCss(this));
            }

            return String.Join(Environment.NewLine, lines);
        }

        String IStyleFormatter.Declaration(String name, String value, Boolean important)
        {
            var rest = String.Concat(value, important ? " !important" : String.Empty);
            return String.Concat(name, ": ", rest);
        }

        String IStyleFormatter.Declarations(IEnumerable<String> declarations)
        {
            return String.Join("; ", declarations);
        }

        String IStyleFormatter.Constraint(String name, String value)
        {
            var ending = value != null ? ": " + value : String.Empty;
            return String.Concat("(", name, ending, ")");
        }

        String IStyleFormatter.Comment(String data)
        {
            return String.Concat("/*", data, "*/");
        }

        String IStyleFormatter.Medium(Boolean exclusive, Boolean inverse, String type, IEnumerable<String> constraints)
        {
            var prefix = exclusive ? "only " : (inverse ? "not " : String.Empty);
            var constraintString = String.Join(" and ", constraints);

            if (!String.IsNullOrEmpty(constraintString))
            {
                var typeString = !String.IsNullOrEmpty(type) ? type + " and " : String.Empty;
                return String.Concat(prefix, typeString, constraintString);
            }

            return String.Concat(prefix, type ?? String.Empty);
        }

        String IStyleFormatter.Rule(String name, IEnumerable<IStyleFormattable> children)
        {
            var sb = Pool.NewStringBuilder().Append(name);

            foreach (var child in children)
            {
                sb.Append(' ').Append(child.ToCss(this));
            }

            return sb.ToPool();
        }

        String IStyleFormatter.Block(IEnumerable<IStyleFormattable> children)
        {
            var sb = Pool.NewStringBuilder().Append('{');

            foreach (var child in children)
            {
                var rule = child.ToCss(this);
                sb.Append(' ').Append(rule);
            }

            return sb.Append(" }").ToPool();
        }

        String IStyleFormatter.Style(String selector, String declarations)
        {
            var sb = Pool.NewStringBuilder().Append(selector);
            sb.Append(" { ");

            if (!String.IsNullOrEmpty(declarations))
            {
                sb.Append(declarations).Append(' ');
            }

            return sb.Append('}').ToPool();
        }

        #endregion
    }
}
