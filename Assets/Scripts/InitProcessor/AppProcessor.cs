using System;
using SNShien.Common.AudioTools;
using SNShien.Common.TesterTools;
using Zenject;

namespace GameCore
{
    public class AppProcessor : IAppProcessor
    {
        private const string DEBUGGER_KEY = "InitProcessorModel";

        [Inject] private IAudioManager audioManager;
        [Inject] private IStageSetting stageSetting;

        public StageSettingContent CurrentStageSettingContent { get; private set; }

        private readonly Debugger debugger = new Debugger(DEBUGGER_KEY);
        private bool isInit;
        private string enterStageAudioKey;

        public void SetEnterStageAudioKey(string audioKey)
        {
            enterStageAudioKey = audioKey;
            CurrentStageSettingContent = stageSetting.GetStageSettingContent(audioKey) ?? throw new NullReferenceException();
        }

        public void CheckInit()
        {
            if (isInit)
            {
                debugger.ShowLog("is already init", true);
                return;
            }

            debugger.ShowLog("init app processor");
            audioManager.InitCollectionFromProject();

            isInit = true;
        }

        private void OnSwitchScene(SwitchSceneEvent eventInfo)
        {
            debugger.ShowLog($"reposition action key: {eventInfo.RepositionActionKey}");
            if (eventInfo.RepositionActionKey == GameConst.SCENE_REPOSITION_ACTION_ENTER_SELECTION_MENU)
                CheckInit();
        }
    }
}