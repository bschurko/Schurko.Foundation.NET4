using Schurko.Foundation.Hash;
using System;
using System.Collections;
using System.Text;


#nullable enable
namespace Schurko.Foundation.Extensions
{
    public static class StringExtensions
    {
        public static string AsParam(this string param) => param.IndexOf('@') != 0 ? string.Format("@{0}", param) : param;

        public static bool Compare(this string strA, string strCompare, bool ignoreCase) => string.Equals(strA, strCompare, ignoreCase ? StringComparison.CurrentCultureIgnoreCase : StringComparison.CurrentCulture);

        public static string FormatString(this string val, params object[] args) => string.Format(val, args);

        public static string XmlDecode(this string value)
        {
            value = value.Replace("&lt;", "<");
            value = value.Replace("&gt;", ">");
            value = value.Replace("&quot;", "\"");
            value = value.Replace("&amp;", "&");
            return value;
        }

        public static string XmlEncode(this string value)
        {
            value = value.Replace("&", "&amp;");
            value = value.Replace("<", "&lt;");
            value = value.Replace(">", "&gt;");
            value = value.Replace("\"", "&quot;");
            return value;
        }

        public static T ParseEnum<T>(this string value) where T : struct => (T)Enum.Parse(typeof(T), value, true);

        public static bool TryParseEnum<T>(this string source, out T value) where T : struct => Enum.TryParse(source, true, out value);

        public static T AsEnum<T>(this string value, T defaultValue) where T : struct
        {
            T obj;
            return !value.TryParseEnum(out obj) ? defaultValue : obj;
        }

        public static TTarget IfNotNullOrEmpty<TSource, TTarget>(
          this TSource value,
          Func<TSource, TTarget> operation)
          where TSource : IEnumerable
        {
            return value == null || !value.GetEnumerator().MoveNext() ? default : operation(value);
        }

        //public static TTarget IfNotNullOrDefault<TSource, TTarget>(
        //  this TSource value,
        //  Func<TSource, TTarget> operation)
        //  where TSource : IEquatable<TSource>
        //{
        //  if ((object) value != null)
        //  {
        //    ref TSource local = ref value;
        //    if ((object) default (TSource) == null)
        //    {
        //      TSource source = local;
        //      local = ref source;
        //    }
        //    TSource other = default (TSource);
        //    if (!local.Equals(other))
        //      return operation(value);
        //  }
        //  return default (TTarget);
        //}

        //public static IfStatement<TSource> IfNotNullOrDefault<TSource>(
        //  this TSource value,
        //  Action<TSource> operation)
        //  where TSource : IEquatable<TSource>
        //{
        //  if ((object) value != null)
        //  {
        //    ref TSource local = ref value;
        //    if ((object) default (TSource) == null)
        //    {
        //      TSource source = local;
        //      local = ref source;
        //    }
        //    TSource other = default (TSource);
        //    if (!local.Equals(other))
        //    {
        //      operation(value);
        //      return IfStatement.True<TSource>(value);
        //    }
        //  }
        //  return IfStatement.False<TSource>(value);
        //}

        public static IfStatement<TSource> IfNotNullOrEmpty<TSource>(
          this TSource value,
          Action<TSource> operation)
          where TSource : IEnumerable
        {
            if (value == null || !value.GetEnumerator().MoveNext())
                return IfStatement.False(value);
            operation(value);
            return IfStatement.True(value);
        }

        public static string Indent(this string str) => str.Indent(4);

        public static string Indent(this string str, int level)
        {
            StringBuilder stringBuilder = new StringBuilder();
            for (int index = 0; index < level; ++index)
                stringBuilder.Append(' ');
            string str1 = stringBuilder.ToString();
            string str2 = str1 + str.Replace("\n", Environment.NewLine + str1);
            if (str.EndsWith("\n"))
                str2 = str2.Substring(0, str2.Length - level);
            return str2;
        }

        public static ulong Hash(this string value, HashType hashType)
        {
            switch (hashType)
            {
                case HashType.FNV:
                    return FNVHash.HashString(value);
                case HashType.MD5:
                    throw new NotImplementedException("MD5 Hash algorithm has not been implemented");
                case HashType.SHA1:
                    throw new NotImplementedException("SHA1 Hash algorithm has not been implemented");
                default:
                    throw new ArgumentOutOfRangeException(nameof(hashType));
            }
        }
    }
}
