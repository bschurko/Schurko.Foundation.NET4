

#nullable enable


namespace Schurko.Foundation.Extensions
{
    public class IfStatement
    {
        internal static IfStatement<TSource> True<TSource>(TSource data) => new IfStatement<TSource>(true, data);

        internal static IfStatement<TSource> False<TSource>(TSource data) => new IfStatement<TSource>(false, data);
    }

    public class IfStatement<TSource>
    {
        public bool State { get; private set; }

        public TSource Context { get; private set; }

        internal IfStatement(bool state, TSource context)
        {
            State = state;
            Context = context;
        }

        public static implicit operator bool(IfStatement<TSource> obj) => obj.State;
    }
}
