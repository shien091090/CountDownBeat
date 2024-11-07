namespace GameCore
{
    public interface IAppProcessor
    {
        StageSettingContent CurrentStageSettingContent { get; }
        void ExecuteEnterStage(string audioKey);
        void EnterSelectionMenu();
    }
}