using SNShien.Common.AdapterTools;
using SNShien.Common.MonoBehaviorTools;
using SNShien.Common.ProcessTools;
using Zenject;

namespace GameCore
{
    public class ScoreBallHandler : IScoreBallHandler
    {
        [Inject] private IEventRegister eventRegister;
        [Inject] private IEventInvoker eventInvoker;
        [Inject] private IGameSetting gameSetting;
        [Inject] private IViewManager viewManager;

        private IScoreBallHandlerPresenter presenter;
        private int beatCounter;
        private float spawnBeatFreq;

        public void ExecuteModelInit()
        {
            Init();

            this.presenter = new ScoreBallHandlerPresenter(this);
            viewManager.OpenView<ScoreBallHandlerView>(presenter);
        }

        private void Init()
        {
            beatCounter = 0;
            spawnBeatFreq = gameSetting.ScoreBallSpawnBeatFreq;
            RegisterEvent();
        }

        private void RegisterEvent()
        {
            eventRegister.Unregister<BeatEvent>(OnBeatEvent);
            eventRegister.Register<BeatEvent>(OnBeatEvent);
        }

        private void SpawnScoreBall()
        {
            presenter.Spawn(new ScoreBallPresenter(eventRegister, eventInvoker, gameSetting));
        }

        private void OnBeatEvent(BeatEvent eventInfo)
        {
            beatCounter++;
            
            if (beatCounter >= spawnBeatFreq)
            {
                beatCounter = 0;
                SpawnScoreBall();
            }
        }
    }
}