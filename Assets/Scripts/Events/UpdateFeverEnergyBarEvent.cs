using SNShien.Common.ProcessTools;

namespace GameCore
{
    public class UpdateFeverEnergyBarEvent : IArchitectureEvent
    {
        public int BeforeEnergyValue { get; }
        public int AfterEnergyValue { get; }

        public UpdateFeverEnergyBarEvent(int beforeEnergyValue, int afterEnergyValue)
        {
            BeforeEnergyValue = beforeEnergyValue;
            AfterEnergyValue = afterEnergyValue;
        }
    }
}