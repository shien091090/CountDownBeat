namespace GameCore
{
    public interface IAppProcessor
    {
        IStageSettingContent CurrentStageSettingContent { get; }
        void ExecuteEnterStage(string audioKey);
        void EnterSelectionMenu();
    }
}