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

        private BeaterPresenter presenter;
        private float currentTimer;
        private bool isAlreadyBeatHalfEvent;

        public void ExecuteModelInit()
        {
            InitView();
            InitPlayBgm();
        }

        private void InitPlayBgm()
        {
            audioManager
                .PlayWithCallback(GameConst.AUDIO_NAME_BGM_1)
                .Register(EVENT_CALLBACK_TYPE.TIMELINE_BEAT, OnBeat);
        }

        private void OnBeat()
        {
            eventInvoker.SendEvent(new BeatEvent());
        }

        private void InitView()
        {
            presenter = new BeaterPresenter(this);
            viewManager.OpenView<BeaterView>(presenter);
        }
    }
}