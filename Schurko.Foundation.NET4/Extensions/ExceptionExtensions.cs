using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;


#nullable enable
namespace Schurko.Foundation.Extensions
{
    public static class ExceptionExtensions
    {
        public static string FormatToString(this Exception ex, int? depth) => BasicStringFormat(ex, depth ?? 10);

        private static string BasicStringFormat(Exception ex, int depth) => StringException(new StringBuilder(), ex, 0, depth).ToString();

        private static StringBuilder StringException(
          StringBuilder curValue,
          Exception ex,
          int curDepth,
          int maxDepth)
        {
            curValue.Append("[");
            string str = curDepth != 0 ? "Inner" : "Outer";
            string source = ex.Source;
            if (ex.TargetSite != null)
                source += string.Format(".{0}", ex.TargetSite.Name);
            curValue.Append(string.Format("Exception occured in [{0}].\n Type: [{1}::{2}]\n Message: [{3}]\n Stack: [{4}]\n", source, str, ex.GetType().Name, ex.Message, ex.StackTrace));
            if (ex.InnerException != null && curDepth < maxDepth)
                curValue = StringException(curValue, ex.InnerException, curDepth + 1, maxDepth);
            curValue.Append("]");
            return curValue;
        }

        public static string GetFullExceptionMessage(Exception ex, string prepend = "")
        {
            if (ex == null)
                return string.Empty;
            return string.Format("{3}{4} Exception Details:{3}{4} Message - '{0}'{3}{4} Stack Trace - '{1}'{3}Type - {5}{3}{4}{2}{3}", ex.Message, ex.StackTrace, ex.InnerException != null ? GetFullExceptionMessage(ex.InnerException, "Inner Exception: " + prepend) : (object)" - No Inner Exception", Environment.NewLine, prepend, ex.GetType());
        }

        public static void ThrowIfNull(this object o, string name)
        {
            if (o == null)
                throw new ArgumentNullException(name);
        }

        public static void ThrowIfNull(this object o, string paramName, string message)
        {
            if (o == null)
                throw new ArgumentNullException(paramName, message);
        }

        public static void ThrowIfNullOrEmpty(this string str, string paramName)
        {
            if (str == null)
                throw new ArgumentNullException(paramName);
            if (str.Length <= 0)
                throw new ArgumentException(paramName);
        }

        public static void ThrowIfNullOrWhiteSpace(this string str, string paramName)
        {
            if (str == null)
                throw new ArgumentNullException(paramName);
            if (string.IsNullOrWhiteSpace(str))
                throw new ArgumentException(paramName);
        }

        public static void StringThrowIfNullOrWhiteSpace(this string str, string message)
        {
            if (string.IsNullOrWhiteSpace(str))
                throw new ArgumentException(message);
        }

        public static void ThrowIfNull(this object o, string name, Type type, string funcationName)
        {
            if (o == null)
                throw new ArgumentNullException(string.Format("Type:{0}, Method:{1} ", type, funcationName));
        }

        public static void CollectionThrowIfNullOrEmpty<T>(
          this IEnumerable<T> collection,
          string message)
        {
            collection.ThrowIfNull(message);
            if (!collection.Any())
                throw new ArgumentException(message);
        }
    }
}
