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

        public void Release()
        {
            OnSpawnCatchNet = null;
            SetEventRegister(false);
            presenter.UnbindModel();
        }

        private void Init()
        {
            beatCounter = 0;
            SetEventRegister(true);

            presenter.BindModel(this);
            presenter.OpenView();
            presenter.Init();
        }

        private void SetEventRegister(bool isListen)
        {
            eventRegister.Unregister<BeatEvent>(OnBeatEvent);

            if (isListen)
            {
                eventRegister.Register<BeatEvent>(OnBeatEvent);
            }
        }

        private void SpawnCatchNet()
        {
            CatchNetPresenter catchNetPresenter = new CatchNetPresenter();
            CatchNet catchNet = new CatchNet(catchNetPresenter, presenter, eventInvoker, gameSetting);

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