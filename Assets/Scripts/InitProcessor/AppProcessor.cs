using System;
using SNShien.Common.AudioTools;
using SNShien.Common.MonoBehaviorTools;
using SNShien.Common.ProcessTools;
using SNShien.Common.TesterTools;
using Zenject;

namespace GameCore
{
    public class AppProcessor : IAppProcessor
    {
        private const string DEBUGGER_KEY = "AppProcessor";

        [Inject] private IAudioManager audioManager;
        [Inject] private IStageSetting stageSetting;
        [Inject] private IEventRegister eventRegister;
        [Inject] private IEventInvoker eventInvoker;
        [Inject] private IViewManager viewManager;

        public IStageSettingContent CurrentStageSettingContent { get; private set; }

        private readonly Debugger debugger = new Debugger(DEBUGGER_KEY);
        private bool isInit;

        public void ExecuteEnterStage(string audioKey)
        {
            CurrentStageSettingContent = stageSetting.GetStageSettingContent(audioKey) ?? throw new NullReferenceException();
            SetGameEventRegister(true);
            eventInvoker.SendEvent(new SwitchSceneEvent(SceneRepositionActionConst.ENTER_GAME));
        }

        public void EnterSelectionMenu()
        {
            if (isInit == false)
                Init();
            else
                debugger.ShowLog("is already init");
        }

        private void Init()
        {
            debugger.ShowLog("app processor init");

            audioManager.InitCollectionFromProject();
            SetGameEventRegister(false);

            isInit = true;
        }

        private void SetGameEventRegister(bool isListen)
        {
            debugger.ShowLog($"SetGameEventRegister: {isListen}");

            eventRegister.Unregister<GameOverEvent>(OnGameOver);

            if (isListen)
                eventRegister.Register<GameOverEvent>(OnGameOver);
        }

        private void OnGameOver(GameOverEvent eventInfo)
        {
            debugger.ShowLog("close all view", true);

            audioManager.Stop();
            viewManager.ClearAllView();
            SetGameEventRegister(false);

            debugger.ShowLog("back to selection menu", true);
            eventInvoker.SendEvent(new SwitchSceneEvent(SceneRepositionActionConst.BACK_TO_SELECTION_MENU));
        }
    }
}