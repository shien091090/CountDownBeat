using SNShien.Common.ProcessTools;
using Zenject;

namespace GameCore
{
    public class CatchNetHandler : ICatchNetHandler
    {
        [Inject] private IEventRegister eventRegister;
        [Inject] private IGameSetting gameSetting;
        [Inject] private ICatchNetHandlerPresenter presenter;

        private int beatCounter;

        public void ExecuteModelInit()
        {
            Init();
        }

        private void Init()
        {
            beatCounter = 0;
            RegisterEvent();
        }

        private void RegisterEvent()
        {
            eventRegister.Unregister<BeatEvent>(OnBeatEvent);
            eventRegister.Register<BeatEvent>(OnBeatEvent);
        }

        private void OnBeatEvent(BeatEvent eventInfo)
        {
            if (gameSetting.SpawnCatchNetFreq == 0)
                return;

            if (presenter.CurrentCatchNetCount >= gameSetting.CatchNetLimit)
            {
                beatCounter = 0;
                return;
            }

            beatCounter++;

            if (beatCounter >= gameSetting.SpawnCatchNetFreq)
            {
                beatCounter = 0;
                presenter.SpawnCatchNet();
            }
        }
    }
}