using SNShien.Common.ProcessTools;

namespace GameCore
{
    public interface IFeverEnergyBarModel : IArchitectureModel
    {
        int CurrentFeverStage { get; }
        void Hit();
    }
}