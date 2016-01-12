namespace AngleSharp
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Basic interface for CSS node serialization.
    /// </summary>
    public interface IStyleFormatter
    {
        /// <summary>
        /// Concats the given rules to create the stylesheet serialization.
        /// </summary>
        /// <param name="rules">The rules to aggregate.</param>
        /// <returns>The serialization of the sheet.</returns>
        String Rules(IEnumerable<IStyleFormattable> rules);

        /// <summary>
        /// Creates the serialization of a declaration with the given name,
        /// value and important flag.
        /// </summary>
        /// <param name="name">The name of the declaration.</param>
        /// <param name="value">The value of the declaration.</param>
        /// <param name="important">The value of the important flag.</param>
        /// <returns>The serialization of the declaration.</returns>
        String Declaration(String name, String value, Boolean important);

        /// <summary>
        /// Creates the serialization of the declarations with the provided
        /// string representations.
        /// </summary>
        /// <param name="declarations">The declarations to aggregate.</param>
        /// <returns>The serialization of the declarations.</returns>
        String Declarations(IEnumerable<String> declarations);

        /// <summary>
        /// Creates the serialization of the constraint with the provided name
        /// and value, if any.
        /// </summary>
        /// <param name="name">The name of the constraint.</param>
        /// <param name="value">The optional value of the constraint.</param>
        /// <returns>The serialization of the constraint.</returns>
        String Constraint(String name, String value);

        /// <summary>
        /// Creates a serialization of a comment with the provided data.
        /// </summary>
        /// <param name="data">The data of the comment.</param>
        /// <returns>The serialization of the comment.</returns>
        String Comment(String data);

        /// <summary>
        /// Serializes a CSS medium with the provided properties.
        /// </summary>
        /// <param name="exclusive">Is the medium exclusive (only)?</param>
        /// <param name="inverse">Is the medium inverse (not)?</param>
        /// <param name="type">The type of the medium.</param>
        /// <param name="constraints">The constraints to use.</param>
        /// <returns>The serialization of the medium.</returns>
        String Medium(Boolean exclusive, Boolean inverse, String type, IEnumerable<String> constraints);

        /// <summary>
        /// Converts the name and value of the provided rule to a rule.
        /// </summary>
        /// <param name="name">The name of the rule.</param>
        /// <param name="children">The children of the rule.</param>
        /// <returns>The serialization of the rule.</returns>
        String Rule(String name, IEnumerable<IStyleFormattable> children);

        /// <summary>
        /// Concats the given rules to create a block serialization.
        /// </summary>
        /// <param name="children">The rules to aggregate.</param>
        /// <returns>The serialization of the CSS rule block.</returns>
        String Block(IEnumerable<IStyleFormattable> children);

        /// <summary>
        /// Converts the provided selector and declaration to a style rule.
        /// </summary>
        /// <param name="selector">The selector to use.</param>
        /// <param name="declarations">The declarations to consider.</param>
        /// <returns>The serialization of the style rule.</returns>
        String Style(String selector, String declarations);
    }
}
