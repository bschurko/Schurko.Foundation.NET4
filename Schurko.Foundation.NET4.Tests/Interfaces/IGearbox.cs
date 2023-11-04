namespace Schurko.Foundation.Tests.Interfaces
{
    public interface IGearbox
    {
        IGearSelector GearSelector { get; }

        int CurrentGear { get; }

        int NumberOfGears { get; }

        int SelectGear(int speed);
    }
}
