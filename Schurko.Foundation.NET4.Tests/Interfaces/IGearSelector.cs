namespace Schurko.Foundation.Tests.Interfaces
{
    public interface IGearSelector
    {
        int GetBestGear(IGearbox gearbox, int speed);
    }
}
