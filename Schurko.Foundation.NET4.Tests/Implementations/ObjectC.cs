using System.ComponentModel.Composition;
using Schurko.Foundation.Tests.Interfaces;

namespace Schurko.Foundation.Tests.Implementations
{
    [Export(typeof(IObjectX))]
    public class ObjectC : IObjectX
    {
        public string Name { get; private set; }

        public ObjectC()
        {
            Name = "ObjectC";
        }
    }
}