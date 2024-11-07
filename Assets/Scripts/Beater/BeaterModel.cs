using FMOD.Studio;
using SNShien.Common.AudioTools;
using SNShien.Common.MonoBehaviorTools;
using SNShien.Common.ProcessTools;
using SNShien.Common.TesterTools;
using Zenject;

namespace GameCore
{
    public class BeaterModel : IBeaterModel
    {
        [Inject] private IViewManager viewManager;
        [Inject] private IEventInvoker eventInvoker;
        [Inject] private IAudioManager audioManager;
        [Inject] private IAppProcessor appProcessor;

        private BeaterPresenter presenter;
        private int beatCounter;

        public void ExecuteModelInit()
        {
            InitView();

            StartStage(appProcessor.CurrentStageSettingContent);
        }

        private void InitView()
        {
            presenter = new BeaterPresenter(this);
            viewManager.OpenView<BeaterView>(presenter);
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
            Debugger debugger = new Debugger("BeaterModel");
            debugger.ShowLog("OnBeat");
            
            beatCounter++;

            bool isCountDownBeat = false;
            if (beatCounter >= appProcessor.CurrentStageSettingContent.CountDownBeatFreq)
            {
                isCountDownBeat = true;
                beatCounter = 0;
            }

            eventInvoker.SendEvent(new BeatEvent(isCountDownBeat));
            presenter.PlayBeatAnimation();
        }
    }
}