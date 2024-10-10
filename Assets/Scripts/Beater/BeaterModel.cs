using SNShien.Common.AdapterTools;
using SNShien.Common.MonoBehaviorTools;
using SNShien.Common.ProcessTools;
using Zenject;

namespace GameCore
{
    public class BeaterModel : IBeaterModel
    {
        [Inject] private IViewManager viewManager;
        [Inject] private IGameSetting gameSetting;
        [Inject] private IDeltaTimeGetter deltaTimeGetter;
        [Inject] private IEventInvoker eventInvoker;

        private BeaterPresenter presenter;
        private float beatTimeThreshold;
        private float currentTimer;

        public void ExecuteModelInit()
        {
            Init();

            presenter = new BeaterPresenter(this);
            viewManager.OpenView<BeaterView>(presenter);
        }

        public void UpdatePerFrame()
        {
            currentTimer += deltaTimeGetter.deltaTime;
            if (currentTimer >= beatTimeThreshold)
            {
                currentTimer -= beatTimeThreshold;
                eventInvoker.SendEvent(new BeatEvent());
                presenter.PlayBeatAnimation();
            }
        }

        private void Init()
        {
            beatTimeThreshold = gameSetting.BeatTimeThreshold;
            currentTimer = 0;
        }
    }
}