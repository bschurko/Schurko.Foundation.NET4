using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.Composition;
using Schurko.Foundation.Tests.Interfaces;

namespace Schurko.Foundation.Tests.Implementations
{

    public class VehicleTrabant : IVehicle
    {
        public string Name { get; set; }

        [Import]
        public IEngine Engine { get; set; }

    }
}
