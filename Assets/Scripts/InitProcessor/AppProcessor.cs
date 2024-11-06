using System;
using SNShien.Common.AudioTools;
using SNShien.Common.ProcessTools;
using SNShien.Common.TesterTools;
using Zenject;

namespace GameCore
{
    public class AppProcessor : IAppProcessor
    {
        private const string DEBUGGER_KEY = "InitProcessorModel";

        public StageSettingContent CurrentStageSettingContent { get; private set; }

        private readonly IAudioManager audioManager;
        private readonly IStageSetting stageSetting;
        private readonly IEventRegister eventRegister;

        private readonly Debugger debugger = new Debugger(DEBUGGER_KEY);
        private bool isInit;
        private string enterStageAudioKey;

        public AppProcessor(IAudioManager audioManager, IStageSetting stageSetting, IEventRegister eventRegister)
        {
            this.audioManager = audioManager;
            this.stageSetting = stageSetting;
            this.eventRegister = eventRegister;

            RegisterEvent();
        }

        public void SetEnterStageAudioKey(string audioKey)
        {
            enterStageAudioKey = audioKey;
            CurrentStageSettingContent = stageSetting.GetStageSettingContent(audioKey) ?? throw new NullReferenceException();
        }

        private void CheckInit()
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

        private void RegisterEvent()
        {
            eventRegister.Unregister<SwitchSceneEvent>(OnSwitchScene);
            eventRegister.Register<SwitchSceneEvent>(OnSwitchScene);
        }

        private void OnSwitchScene(SwitchSceneEvent eventInfo)
        {
            if (eventInfo.RepositionActionKey == GameConst.SCENE_REPOSITION_ACTION_ENTER_SELECTION_MENU)
                CheckInit();
        }
    }
}