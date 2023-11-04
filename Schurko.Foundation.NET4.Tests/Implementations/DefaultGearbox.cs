using System.ComponentModel.Composition;
using Schurko.Foundation.IoC.MEF;
using Schurko.Foundation.Tests.Interfaces;

namespace Schurko.Foundation.Tests.Implementations
{
    [Export(typeof(IGearbox))]
    [ExportMetadata("Default", "IGearbox")]
    public class DefaultGearbox : IGearbox
    {
        #region IGearbox Members

        public IGearSelector GearSelector
        {
            get
            {
                return DependencyInjector.Resolve<IGearSelector>();
            }
        }

        public int CurrentGear { get; set; }

        public int NumberOfGears
        {
            get
            {
                return 4;
            }
        }

        public int SelectGear(int speed)
        {
            CurrentGear = GearSelector.GetBestGear(this, speed);
            return CurrentGear;
        }

        #endregion
    }
}
