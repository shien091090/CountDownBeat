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
        private float currentTimer;
        private bool isAlreadyBeatHalfEvent;

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
            audioManager
                .PlayWithCallback(audioKey)
                .Register(EVENT_CALLBACK_TYPE.TIMELINE_BEAT, OnBeat);

            CurrentStageSettingContent = stageSetting.GetStageSettingContent(audioKey);
        }

        private void OnBeat()
        {
            eventInvoker.SendEvent(new BeatEvent());
        }
    }
}