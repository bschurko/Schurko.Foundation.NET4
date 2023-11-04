namespace Schurko.Foundation.Tests.Interfaces
{
    public interface IObjectB
    {
        string Name { get; }

        IObjectX SubItem1 { get; }

        IObjectX SubItem2 { get; }
    }
}