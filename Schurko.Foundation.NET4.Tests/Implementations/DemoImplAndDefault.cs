using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Schurko.Foundation.Tests.Interfaces;

namespace Schurko.Foundation.Tests.Implementations
{
    /// <summary>
    /// The demo impl and default.
    /// </summary>
    //[Export(typeof(IDoubleProcessor))]
    //[Export(typeof(IIntProcessor))]
    //[ExportMetadata("Default", "IIntProcessor")]
    public class DemoImplAndDefault : IDoubleProcessor, IIntProcessor
    {
        public double Process(double value)
        {
            throw new NotImplementedException();
        }

        public int ProcessInt(int inValue)
        {
            throw new NotImplementedException();
        }

    }
}
