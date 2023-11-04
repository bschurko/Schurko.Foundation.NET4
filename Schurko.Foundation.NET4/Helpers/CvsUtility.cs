using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Schurko.Foundation.Utilities
{
    /// <summary>
    /// Utilities for comma separated values.
    /// <para>Tab and semicolon separated values are also supported.</para>
    /// </summary>
    public static class CsvUtility
    {
        /// <summary>
        /// Represents the delimiter between data segments in a line.
        /// </summary>
        public enum LineSeparator
        {
            /// <summary>
            /// Unknown separator.
            /// </summary>
            Unknown,
            /// <summary>
            /// Comma (,).
            /// </summary>
            Comma,
            /// <summary>
            /// Tab (\t).
            /// </summary>
            Tab,
            /// <summary>
            /// Semicolon (;).
            /// </summary>
            Semicolon
        }

        /// <summary>
        /// An array containing only <see cref="Environment.NewLine"/>.
        /// </summary>
        public static readonly string[] NEWLINES = { Environment.NewLine };

        private static readonly ReadOnlyDictionary<LineSeparator, Regex> separatorToRegex;

        static CsvUtility()
        {
            // CSV line parsing: From "jgr4" in http://www.kimgentes.com/worshiptech-web-tools-page/2008/10/14/regex-pattern-for-parsing-csv-files-with-embedded-commas-dou.html
            separatorToRegex = new Dictionary<LineSeparator, Regex>
            {
                { LineSeparator.Unknown, null },
                { LineSeparator.Comma, new Regex(@"\s?((?<x>(?=[,]+))|""(?<x>([^""]|"""")+)""|""(?<x>)""|(?<x>[^,]+)),?", RegexOptions.ExplicitCapture | RegexOptions.Compiled) },
                { LineSeparator.Tab, new Regex(@"[\r\n\f\v ]*?((?<x>(?=[\t]+))|""(?<x>([^""]|"""")+)""|""(?<x>)""|(?<x>[^\t]+))\t?", RegexOptions.ExplicitCapture | RegexOptions.Compiled) },
                { LineSeparator.Semicolon, new Regex(@"\s?((?<x>(?=[;]+))|""(?<x>([^""]|"""")+)""|""(?<x>)""|(?<x>[^;]+));?", RegexOptions.ExplicitCapture | RegexOptions.Compiled) }
            }.AsReadOnly();
        }
         
        /// <summary>
        /// Parses the providede lines of comma separated values.
        /// </summary>
        /// <param name="lines">Lines of comma separated values to parse.</param>
        public static IEnumerable<string[]> ParseCommaSeparated(IEnumerable<string> lines)
        {
            return ParseKnown(lines, LineSeparator.Comma);
        }

        /// <summary>
        /// Parses the providede lines of tab separated values.
        /// </summary>
        /// <param name="lines">Lines of tab separated values to parse.</param>
        public static IEnumerable<string[]> ParseTabSeparated(IEnumerable<string> lines)
        {
            return ParseKnown(lines, LineSeparator.Tab);
        }

        /// <summary>
        /// Parses the providede lines of semicolon separated values.
        /// </summary>
        /// <param name="lines">Lines of semicolon separated values to parse.</param>
        public static IEnumerable<string[]> ParseSemicolonSeparated(IEnumerable<string> lines)
        {
            return ParseKnown(lines, LineSeparator.Semicolon);
        }

   

        /// <summary>
        /// Parses the provided line.
        /// <para>The line separator is determined by the max count.</para>
        /// </summary>
        /// <param name="line">The line of text to parse.</param>
        public static string[] ParseLine(string line)
        {
            return ParseLineUnknown(line);
        }

        /// <summary>
        /// Parses the provided line of comma separated values.
        /// </summary>
        /// <param name="line">The line of text to parse.</param>
        public static string[] ParseLineCommaSeparated(string line)
        {
            return ParseLineKnown(line, LineSeparator.Comma);
        }

        /// <summary>
        /// Parses the provided line of tab separated values.
        /// </summary>
        /// <param name="line">The line of text to parse.</param>
        public static string[] ParseLineTabSeparated(string line)
        {
            return ParseLineKnown(line, LineSeparator.Tab);
        }

        /// <summary>
        /// Parses the provided line of semicolon separated values.
        /// </summary>
        /// <param name="line">The line of text to parse.</param>
        public static string[] ParseLineSemicolonSeparated(string line)
        {
            return ParseLineKnown(line, LineSeparator.Semicolon);
        }

        /// <summary>
        /// Parses the provided line separated with the specified line separator.
        /// <para>If the line separator is unknown, it is set to the one with the max count.</para>
        /// </summary>
        /// <param name="line">The line of text to parse.</param>
        /// <param name="lineSeparator">The type of delimiter between data segments in a line.</param>
        public static string[] ParseLine(string line, LineSeparator lineSeparator)
        {
            if (lineSeparator == LineSeparator.Unknown)
            {
                return ParseLineUnknown(line);
            }
            return ParseLineKnown(line, lineSeparator);
        }

        private static IEnumerable<string[]> ParseKnown(IEnumerable<string> lines, LineSeparator lineSeparator)
        {
            Regex regex = separatorToRegex[lineSeparator];
            foreach (string line in lines)
            {
                yield return ParseLine(line, regex);
            }
        }
 

        private static string[] ParseLineKnown(string line, LineSeparator lineSeparator)
        {
            Regex regex = separatorToRegex[lineSeparator];
            return ParseLine(line, regex);
        }

        private static string[] ParseLineUnknown(string line)
        {
            // try to guess by checking the max count of each supported separator
            string[] resultByComma = ParseLine(line, separatorToRegex[LineSeparator.Comma]);
            string[] resultByTab = ParseLine(line, separatorToRegex[LineSeparator.Tab]);
            string[] resultBySemicolon = ParseLine(line, separatorToRegex[LineSeparator.Semicolon]);
            return Util.SelectOneWithMax(r => r.Length, resultByComma, resultByTab, resultBySemicolon);
        }

        private static string[] ParseLine(string line, Regex regex)
        {
            var matches = regex.Matches(line);

            string[] values = (from Match m in matches
                               select m.Groups["x"].Value
                                   .Trim()
                                   .Replace("\"\"", "\"")
                                ).ToArray();

            return values;
        }

        /// <summary>
        /// Returns the provided text data surrounded with double quotes (").
        /// </summary>
        /// <param name="data">The text data.</param>
        /// <param name="onlyIfEmptyOrContainsCommaOrWhitespace">If true, double quotes are added only if data is null/empty or it contains either comma or whitespace.</param>
        public static string SurroundWithDoubleQuotes(string data, bool onlyIfEmptyOrContainsCommaOrWhitespace = false)
        {
            if (!onlyIfEmptyOrContainsCommaOrWhitespace || string.IsNullOrEmpty(data) || (data.Contains(',') || data.ContainsWhitespace()))
            {
                return $"\"{data}\"";
            }
            return data;
        }

        /// <summary>
        /// Returns the provided text data surrounded with double quotes (") if any of the specified conditions are met.
        /// </summary>
        /// <param name="data">The text data.</param>
        /// <param name="ifEmpty">If true, double quotes are added when data is null/empty.</param>
        /// <param name="ifContainsWhitespace">If true, double quotes are added when data contains any whitespace.</param>
        /// <param name="ifContainsAnyOfTheseCharacters">Double quotes are added then data contains any of these characters (case-sensitive).</param>
        public static string SurroundWithDoubleQuotes(string data, bool ifEmpty, bool ifContainsWhitespace, params char[] ifContainsAnyOfTheseCharacters)
        {
            bool surround = false;
            if (string.IsNullOrEmpty(data))
            {
                surround = ifEmpty;
            }
            else if (data.ContainsWhitespace() && ifContainsWhitespace)
            {
                surround = true;
            }
            else
            {
                foreach (char c in data)
                {
                    if (ifContainsAnyOfTheseCharacters.Contains(c))
                    {
                        surround = true;
                        break;
                    }
                }
            }
            if (surround)
            {
                return $"\"{data}\"";
            }
            return data;
        }
    }

    /// <summary>
    /// General utilities.
    /// </summary>
    public static class Util
    {
        
        /// <summary>
        /// Returns the item for which the provided transform function returns the max value of all the provided items. If multiple items have max value, the first is returned.
        /// </summary>
        /// <typeparam name="T">The type of the items.</typeparam>
        /// <param name="valueSelector">A transform function to apply to each item that returns the value to compare items with.</param>
        /// <param name="items">The items.</param>
        public static T SelectOneWithMax<T>(Func<T, double> valueSelector, params T[] items)
        {
            if (items == null)
            {
                throw new ArgumentNullException(nameof(items));
            }
            if (items.Length == 0)
            {
                throw new ArgumentOutOfRangeException(nameof(items), "At least one item must be provided.");
            }
            if (items.Length == 1)
            {
                return items[0];
            }

            T itemWithMax = items[0];
            double max = valueSelector(items[0]);
            for (int i = 1; i < items.Length; ++i)
            {
                T item = items[i];
                double current = valueSelector(item);
                if (current > max)
                {
                    max = current;
                    itemWithMax = item;
                }
            }
            return itemWithMax;
        }

        /// <summary>
        /// Returns all items for which the provided transform function returns the max value of all the provided items.
        /// </summary>
        /// <typeparam name="T">The type of the items.</typeparam>
        /// <param name="valueSelector">A transform function to apply to each item that returns the value to compare items with.</param>
        /// <param name="items">The items.</param>
        public static List<T> SelectThoseWithMax<T>(Func<T, double> valueSelector, params T[] items)
        {
            if (items == null)
            {
                throw new ArgumentNullException(nameof(items));
            }
            if (items.Length == 0)
            {
                throw new ArgumentOutOfRangeException(nameof(items), "At least one item must be provided.");
            }
            if (items.Length == 1)
            {
                return new List<T> { items[0] };
            }

            var itemsWithMax = new List<T> { items[0] };
            double max = valueSelector(items[0]);
            for (int i = 1; i < items.Length; ++i)
            {
                T item = items[i];
                double current = valueSelector(item);
                if (current > max)
                {
                    max = current;
                    itemsWithMax.Clear();
                    itemsWithMax.Add(item);
                }
                else if (current == max)
                {
                    itemsWithMax.Add(item);
                }
            }
            return itemsWithMax;
        }

        /// <summary>
        /// Swaps the values in both provided variables.
        /// </summary>
        /// <typeparam name="T">The type of the variables.</typeparam>
        /// <param name="A">First variable.</param>
        /// <param name="B">Second variable.</param>
        public static void Swap<T>(ref T A, ref T B)
        {
            T swap = A;
            A = B;
            B = swap;
        }
    }
}
