namespace Schurko.Foundation.Tests.Interfaces
{
    public interface IObjectA
    {
        string Name { get; }

        IObjectB SubObject { get; }
    }
}