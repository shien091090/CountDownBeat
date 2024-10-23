using System;
using FMOD.Studio;
using SNShien.Common.AudioTools;
using SNShien.Common.MonoBehaviorTools;
using SNShien.Common.ProcessTools;
using Zenject;

namespace GameCore
{
    public class BeaterModel : IBeaterModel
    {
        [Inject] private IViewManager viewManager;
        [Inject] private IEventInvoker eventInvoker;
        [Inject] private IAudioManager audioManager;
        [Inject] private IStageSetting stageSetting;

        private BeaterPresenter presenter;
        private int beatCounter;

        public StageSettingContent CurrentStageSettingContent { get; private set; }

        public void ExecuteModelInit()
        {
            InitView();

            StartStage(GameConst.AUDIO_NAME_BGM_1);
        }

        private void InitView()
        {
            presenter = new BeaterPresenter(this);
            viewManager.OpenView<BeaterView>(presenter);
        }

        private void StartStage(string audioKey)
        {
            CurrentStageSettingContent = stageSetting.GetStageSettingContent(audioKey) ?? throw new NullReferenceException();

            audioManager
                .PlayWithCallback(audioKey)
                .Register(EVENT_CALLBACK_TYPE.TIMELINE_BEAT, OnBeat);

            beatCounter = 0;
        }

        private void OnBeat()
        {
            beatCounter++;
            
            bool isCountDownBeat = false;
            if (beatCounter >= CurrentStageSettingContent.CountDownBeatFreq)
            {
                isCountDownBeat = true;
                beatCounter = 0;
            }

            eventInvoker.SendEvent(new BeatEvent(isCountDownBeat));
        }
    }
}