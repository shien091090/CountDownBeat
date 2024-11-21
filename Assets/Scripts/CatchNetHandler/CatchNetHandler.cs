using System;
using System.Collections.Generic;
using System.Linq;
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
        private List<CatchNet> inFieldCatchNetList = new List<CatchNet>();

        public event Action<CatchNet> OnSpawnCatchNet;
        public int CurrentInFieldCatchNetAmount { get; }

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

        private CatchNet GetHiddenCatchNet()
        {
            CatchNet hiddenCatchNet = inFieldCatchNetList.FirstOrDefault(x => x.CurrentState == CatchNetState.None || x.CurrentState == CatchNetState.SuccessSettle);
            return hiddenCatchNet;
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
            CatchNet catchNet = GetHiddenCatchNet();
            if (catchNet == null)
            {
                CatchNetPresenter catchNetPresenter = new CatchNetPresenter(eventRegister);
                catchNet = new CatchNet(catchNetPresenter, presenter, eventInvoker, gameSetting);

                presenter.SpawnCatchNet(catchNetPresenter);
                catchNet.Init(Random.Range(gameSetting.CatchNetNumberRange.x, gameSetting.CatchNetNumberRange.y + 1));
            }

            OnSpawnCatchNet?.Invoke(catchNet);
        }

        private void OnBeatEvent(BeatEvent eventInfo)
        {
            if (gameSetting.SpawnCatchNetFreq == 0)
                return;

            if (CurrentInFieldCatchNetAmount >= gameSetting.CatchNetLimit)
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