using System;
using SNShien.Common.ProcessTools;
using Zenject;
using Random = UnityEngine.Random;

namespace GameCore
{
    public class CatchNetHandler : ICatchNetHandler
    {
        [Inject] private IEventRegister eventRegister;
        [Inject] private IEventInvoker eventInvoker;
        [Inject] private IGameSetting gameSetting;
        [Inject] private ICatchNetHandlerPresenter presenter;

        private int beatCounter;
        
        public event Action<CatchNet> OnSpawnCatchNet;

        public void ExecuteModelInit()
        {
            Init();
        }

        private void Init()
        {
            beatCounter = 0;
            RegisterEvent();

            presenter.BindModel(this);
            presenter.OpenView();
        }

        private void RegisterEvent()
        {
            eventRegister.Unregister<BeatEvent>(OnBeatEvent);
            eventRegister.Register<BeatEvent>(OnBeatEvent);
        }

        private void SpawnCatchNet()
        {
            CatchNetPresenter catchNetPresenter = new CatchNetPresenter();
            CatchNet catchNet = new CatchNet(catchNetPresenter, eventInvoker, gameSetting);

            presenter.SpawnCatchNet(catchNetPresenter);
            catchNet.Init(Random.Range(gameSetting.CatchNetNumberRange.x, gameSetting.CatchNetNumberRange.y + 1));

            OnSpawnCatchNet?.Invoke(catchNet);
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

                SpawnCatchNet();
            }
        }
    }
}