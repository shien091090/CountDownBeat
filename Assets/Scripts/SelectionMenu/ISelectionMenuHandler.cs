using SNShien.Common.ProcessTools;

namespace GameCore
{
    public interface ISelectionMenuHandler : IArchitectureModel
    {
        void EnterStage(int stageIndex);
    }
}