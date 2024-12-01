using SNShien.Common.ProcessTools;

namespace GameCore
{
    public interface ICatchNetHandler : IArchitectureModel
    {
        void SettleCatchNet(ICatchNet catchNet);
    }
}