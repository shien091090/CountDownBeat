using System;
using System.Collections.Generic;
using System.Linq;
using SNShien.Common.MathTools;
using SNShien.Common.ProcessTools;
using SNShien.Common.TesterTools;
using Zenject;

namespace GameCore
{
    public class CatchNetHandler : ICatchNetHandler
    {
        [Inject] private IEventRegister eventRegister;
        [Inject] private IEventInvoker eventInvoker;
        [Inject] private IGameSetting gameSetting;
        [Inject] private ICatchNetHandlerPresenter presenter;
        [Inject] private IFeverEnergyBarModel feverEnergyBarModel;
        [Inject] private IScoreBallHandler scoreBallHandler;
        [Inject] private IAppProcessor appProcessor;

        public int CurrentCatchNetLimit { get; private set; }

        private readonly List<CatchNet> inFieldCatchNetList = new List<CatchNet>();
        private readonly DynamicMVPBinder dynamicMVPBinder = new DynamicMVPBinder();

        private Debugger debugger = new Debugger(GameConst.DEBUGGER_KEY_CATCH_NET_HANDLER);

        public event Action<ICatchNet> OnSpawnCatchNet;
        public int CurrentInFieldCatchNetAmount => inFieldCatchNetList.Count(x => x.CurrentState == CatchNetState.Working);

        private List<int> CurrentInFieldCatchNetCatchFlagNumberList =>
            inFieldCatchNetList.Where(x => x.CurrentState == CatchNetState.Working).Select(x => x.CatchFlagNumber).ToList();

        public event Action OnInit;
        public event Action OnRelease;
        public event Action<ICatchNetPresenter> OnSettleCatchNet;

        public void ExecuteModelInit()
        {
            Init();
        }

        public void Release()
        {
            ClearData();
            SetEventRegister(false);

            OnRelease?.Invoke();
        }

        public void SettleCatchNet(ICatchNet catchNet)
        {
            UpdateCurrentCatchNetLimit();

            if (CurrentInFieldCatchNetAmount <= CurrentCatchNetLimit)
                SpawnCatchNet();

            ICatchNetPresenter catchNetPresenter = dynamicMVPBinder.GetPresenter<ICatchNetPresenter>(catchNet);
            OnSettleCatchNet?.Invoke(catchNetPresenter);

            eventInvoker.SendEvent(new GetScoreEvent(appProcessor.CurrentStageSettingContent.SuccessSettleScore));
        }

        private void Init()
        {
            SetEventRegister(true);
            UpdateCurrentCatchNetLimit();

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

        private void ClearData()
        {
            OnSpawnCatchNet = null;
            inFieldCatchNetList.Clear();
        }

        private void UpdateCurrentCatchNetLimit()
        {
            CurrentCatchNetLimit = appProcessor.CurrentStageSettingContent.CatchNetLimitByFeverStageSetting?.GetValueOrDefault(feverEnergyBarModel.CurrentFeverStage, 0) ?? 0;
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

            catchNetPresenter.Init(spawnIndex, fadeInMode, gameSetting.ScoreBallFrameColorByFlagSetting);
            catchNet.Init(CreateTargetFlagNumber());

            OnSpawnCatchNet?.Invoke(catchNet);
        }

        private int CreateTargetFlagNumber()
        {
            List<int> currentFlagNumList = scoreBallHandler.CurrentInFieldScoreBallFlagNumberList;
            foreach (int flagNum in currentFlagNumList)
            {
                if (CurrentInFieldCatchNetCatchFlagNumberList.Contains(flagNum) == false)
                    return flagNum;
            }

            Dictionary<int, int> flagWeightSetting = appProcessor.CurrentStageSettingContent.GetScoreBallFlagWeightSetting(feverEnergyBarModel.CurrentFeverStage);

            if (flagWeightSetting.Count == 0)
                throw new NullReferenceException("CatchNetFlagWeightSetting is empty");

            return RandomAlgorithm.GetRandomNumberByWeight(flagWeightSetting);
        }

        private void OnBeatEvent(BeatEvent eventInfo)
        {
            UpdateCurrentCatchNetLimit();

            if (CurrentInFieldCatchNetAmount >= CurrentCatchNetLimit)
                return;

            SpawnCatchNet();
        }
    }
}