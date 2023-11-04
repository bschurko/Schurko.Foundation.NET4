using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Schurko.Foundation.Tests.Interfaces;

namespace Schurko.Foundation.Tests.Implementations
{
    public enum SpaceShipClass
    {
        Civilian,
        Military,
        Hybrid
    }

    [Export(typeof(ISpaceship))]
    [ExportMetadata("Speed", "Medium")]
    [ExportMetadata("Class", SpaceShipClass.Civilian)]
    public class SpaceshipCruiser : ISpaceship
    {
        #region ISpaceship Members

        public string Name
        {
            get { throw new NotImplementedException(); }
        }

        #endregion
    }
}
