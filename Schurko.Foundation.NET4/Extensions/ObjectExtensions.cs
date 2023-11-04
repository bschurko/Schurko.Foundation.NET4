using System;


#nullable enable
namespace Schurko.Foundation.Extensions
{
    public static class ObjectExtensions
    {
        public static IfStatement<TSource> If<TSource>(
          this TSource value,
          Func<TSource, bool> condition,
          Action<TSource> action)
        {
            if (!condition(value))
                return IfStatement.False(value);
            action(value);
            return IfStatement.True(value);
        }

        public static IfStatement<TSource> ElseIf<TSource>(
          this IfStatement<TSource> statement,
          Func<TSource, bool> condition,
          Action<TSource> action)
        {
            if (statement.State || !condition(statement.Context))
                return statement;
            action(statement.Context);
            return IfStatement.True(statement.Context);
        }

        public static void Else<TSource>(this IfStatement<TSource> statement, Action<TSource> action)
        {
            if (statement.State)
                return;
            action(statement.Context);
        }

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

        public static IfStatement<TSource> IfNotNull<TSource>(
          this TSource value,
          Action<TSource> operation)
          where TSource : class
        {
            if (value == null)
                return IfStatement.False<TSource>(value);
            operation(value);
            return IfStatement.True(value);
        }
    }
}
