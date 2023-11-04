using System;
using System.ComponentModel.Composition;
using Schurko.Foundation.Tests.Interfaces;

namespace Schurko.Foundation.Tests.Implementations
{
    [Export(typeof(IDoubleProcessor))]
    [ExportMetadata("Default", "IDoubleProcessor")]
    public class DefaultDoubleProcessor : IDoubleProcessor
    {
        #region IDoubleProcessor Members

        public double Process(double value)
        {
            // Should never get called.
            throw new NotImplementedException();
        }

        #endregion
    }
}
