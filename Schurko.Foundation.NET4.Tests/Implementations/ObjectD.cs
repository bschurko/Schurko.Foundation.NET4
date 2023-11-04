using System.ComponentModel.Composition;
using Schurko.Foundation.Tests.Interfaces;

namespace Schurko.Foundation.Tests.Implementations
{
    [Export]
    public class ObjectD : IObjectX
    {
        public string Name { get; private set; }

        public ObjectD()
        {
            Name = "ObjectD";
        }
    }
}