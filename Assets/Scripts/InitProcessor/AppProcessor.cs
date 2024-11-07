using System;
using SNShien.Common.AudioTools;
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

        public StageSettingContent CurrentStageSettingContent { get; private set; }

        private readonly Debugger debugger = new Debugger(DEBUGGER_KEY);
        private bool isInit;

        public void ExecuteEnterStage(string audioKey)
        {
            CurrentStageSettingContent = stageSetting.GetStageSettingContent(audioKey) ?? throw new NullReferenceException();
            eventInvoker.SendEvent(new SwitchSceneEvent(GameConst.SCENE_REPOSITION_ACTION_ENTER_GAME));
        }

        public void EnterSelectionMenu()
        {
            debugger.ShowLog($"EnterSelectionMenu, isInit: {isInit}");
            
            if (isInit)
                BackToSelectionMenu();
            else
                FirstEnterSelectionMenu();
        }

        private void SetGameEventRegister(bool isListen)
        {
            debugger.ShowLog($"SetGameEventRegister: {isListen}");

            eventRegister.Unregister<GameOverEvent>(OnGameOver);

            if (isListen)
                eventRegister.Register<GameOverEvent>(OnGameOver);
        }

        private void FirstEnterSelectionMenu()
        {
            audioManager.InitCollectionFromProject();
            RegisterBaseEvent();

            isInit = true;
        }

        private void RegisterBaseEvent()
        {
            debugger.ShowLog("RegisterBaseEvent");

            eventRegister.Unregister<SwitchSceneEvent>(OnSwitchScene);
            eventRegister.Register<SwitchSceneEvent>(OnSwitchScene);
        }

        private void BackToSelectionMenu()
        {
            CurrentStageSettingContent = null;
        }

        private void OnGameOver(GameOverEvent eventInfo)
        {
            debugger.ShowLog("OnGameOver");
            audioManager.Stop();
            eventInvoker.SendEvent(new SwitchSceneEvent(GameConst.SCENE_REPOSITION_ACTION_BACK_TO_SELECTION_MENU));
        }

        private void OnSwitchScene(SwitchSceneEvent eventInfo)
        {
            debugger.ShowLog($"OnSwitchScene: {eventInfo.RepositionActionKey}");

            switch (eventInfo.RepositionActionKey)
            {
                case GameConst.SCENE_REPOSITION_ACTION_ENTER_GAME:
                    SetGameEventRegister(true);
                    break;

                case GameConst.SCENE_REPOSITION_ACTION_ENTER_SELECTION_MENU:
                case GameConst.SCENE_REPOSITION_ACTION_BACK_TO_SELECTION_MENU:
                    SetGameEventRegister(false);
                    break;
            }
        }
    }
}