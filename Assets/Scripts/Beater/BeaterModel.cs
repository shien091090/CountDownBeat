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
        [Inject] private IAppProcessor appProcessor;
        [Inject] private IBeaterPresenter presenter;

        private int beatCounter;
        private int totalBeatCounter;

        public void ExecuteModelInit()
        {
            InitPresenter();

            StartStage(appProcessor.CurrentStageSettingContent);
        }

        public void Release()
        {
            presenter.UnbindModel();
        }

        public void TriggerHalfBeat()
        {
            eventInvoker.SendEvent(new HalfBeatEvent());
            presenter.PlayHalfBeatAnimation();
        }

        private void InitPresenter()
        {
            presenter.BindModel(this);
            presenter.OpenView();
        }

        private void CheckSetHalfBeatOffset()
        {
            float halfBeatTimeOffset = presenter.CurrentTimer / totalBeatCounter / 2;
            presenter.SetHalfBeatTimeOffset(halfBeatTimeOffset);
        }

        private void SendBeatEvent()
        {
            beatCounter++;

            bool isCountDownBeat = false;
            if (beatCounter >= appProcessor.CurrentStageSettingContent.CountDownBeatFreq)
            {
                isCountDownBeat = true;
                beatCounter = 0;
            }

            eventInvoker.SendEvent(new BeatEvent(isCountDownBeat));
        }

        private void StartStage(StageSettingContent stageSetting)
        {
            audioManager
                .PlayWithCallback(stageSetting.AudioKey)
                .Register(EVENT_CALLBACK_TYPE.TIMELINE_BEAT, OnBeat);

            beatCounter = 0;
        }

        private void OnBeat()
        {
            totalBeatCounter++;

            SendBeatEvent();
            CheckSetHalfBeatOffset();
            presenter.PlayBeatAnimation();
        }
    }
}