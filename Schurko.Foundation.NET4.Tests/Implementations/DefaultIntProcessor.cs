using System.ComponentModel.Composition;
using Schurko.Foundation.Tests.Interfaces;

namespace Schurko.Foundation.Tests.Implementations
{
    [Export(typeof(IIntProcessor))]
    [ExportMetadata("Default", "IIntProcessor")]
    public class DefaultIntProcessor : IIntProcessor
    {
        #region IIntProcessor Members

        public int ProcessInt(int inValue)
        {
            return inValue * inValue;
        }

        #endregion
    }
}
