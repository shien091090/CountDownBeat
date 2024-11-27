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
        [Inject] private IMVPArchitectureHandler mvpArchitectureHandler;

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
            bool haveIdlePos = presenter.HaveIdlePos();
            if (haveIdlePos == false)
                return;

            CatchNet catchNet = GetHiddenCatchNet();
            if (catchNet == null)
            {
                ICatchNetView catchNetView = presenter.Spawn();
                CatchNetPresenter catchNetPresenter = new CatchNetPresenter();
                catchNet = new CatchNet(presenter, eventInvoker, gameSetting);

                if (presenter.TryOccupyPos(out int spawnIndex, out CatchNetSpawnFadeInMode fadeInMode) == false)
                    return;
                
                mvpArchitectureHandler.MultipleBind(catchNet, catchNetPresenter, catchNetView);
                catchNetPresenter.Init(spawnIndex, fadeInMode);
                catchNet.Init(Random.Range(gameSetting.CatchNetNumberRange.x, gameSetting.CatchNetNumberRange.y + 1));
            }
            else
            {
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