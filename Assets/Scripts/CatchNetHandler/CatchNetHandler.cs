using System;
using System.Collections.Generic;
using System.Linq;
using SNShien.Common.ProcessTools;
using SNShien.Common.TesterTools;
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
        private readonly List<CatchNet> inFieldCatchNetList = new List<CatchNet>();
        private readonly DynamicMVPBinder dynamicMVPBinder = new DynamicMVPBinder();
        private Debugger debugger = new Debugger(GameConst.DEBUGGER_KEY_CATCH_NET_HANDLER);
        public event Action<ICatchNet> OnSpawnCatchNet;

        public int CurrentInFieldCatchNetAmount => inFieldCatchNetList.Count(x => x.CurrentState == CatchNetState.Working);

        public event Action OnInit;
        public event Action OnRelease;
        public event Action<ICatchNetPresenter> OnSettleCatchNet;

        public void ExecuteModelInit()
        {
            Init();
        }

        public void Release()
        {
            OnSpawnCatchNet = null;
            SetEventRegister(false);

            OnRelease?.Invoke();
        }

        public void SettleCatchNet(ICatchNet catchNet)
        {
            ICatchNetPresenter catchNetPresenter = dynamicMVPBinder.GetPresenter<ICatchNetPresenter>(catchNet);
            OnSettleCatchNet?.Invoke(catchNetPresenter);

            eventInvoker.SendEvent(new GetScoreEvent(gameSetting.SuccessSettleScore));
        }

        private void Init()
        {
            beatCounter = 0;
            SetEventRegister(true);

            presenter.BindModel(this);

            OnInit?.Invoke();
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
            if (presenter.TryOccupyPos(out int spawnIndex, out CatchNetSpawnFadeInMode fadeInMode) == false)
                return;

            CatchNet catchNet = GetHiddenCatchNet();
            ICatchNetView catchNetView = presenter.Spawn(spawnIndex);
            ICatchNetPresenter catchNetPresenter = null;
            if (catchNet == null)
            {
                catchNetPresenter = new CatchNetPresenter();
                catchNet = new CatchNet(this, eventRegister);

                dynamicMVPBinder.MultipleBind(catchNet, catchNetPresenter, catchNetView);
                inFieldCatchNetList.Add(catchNet);
            }
            else
            {
                dynamicMVPBinder.RebindView(catchNet, catchNetView);
                catchNetPresenter = dynamicMVPBinder.GetPresenter<ICatchNetPresenter>(catchNet);
            }

            catchNetPresenter.Init(spawnIndex, fadeInMode);
            catchNet.Init(Random.Range(gameSetting.CatchNetNumberRange.x, gameSetting.CatchNetNumberRange.y + 1));

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