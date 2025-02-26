using System;
using System.Collections.Generic;
using System.Linq;
using SNShien.Common.MathTools;
using SNShien.Common.ProcessTools;
using SNShien.Common.TesterTools;
using Zenject;

namespace GameCore
{
    public class ScoreBallHandler : IScoreBallHandler
    {
        [Inject] private IScoreBallHandlerPresenter presenter;
        [Inject] private IEventRegister eventRegister;
        [Inject] private IEventInvoker eventInvoker;
        [Inject] private IGameSetting gameSetting;
        [Inject] private IAppProcessor appProcessor;
        [Inject] private IBeaterModel beaterModel;
        [Inject] private IFeverEnergyBarModel feverEnergyBarModel;

        public int CurrentInFieldScoreBallAmount => inFieldScoreBallList?.Count(x => x.CurrentState != ScoreBallState.Hide && x.CurrentState != ScoreBallState.None) ?? 0;
        public List<int> CurrentInFieldScoreBallFlagNumberList => inFieldScoreBallList?.Select(x => x.CurrentFlagNumber).ToList() ?? new List<int>();

        private DynamicMVPBinder dynamicMVPBinder = new DynamicMVPBinder();
        private List<int> tempSpawnBeatIndexList;
        private List<ScoreBall> inFieldScoreBallList;
        private int currentBeatIndex;

        private readonly Debugger debugger = new Debugger("ScoreBallHandler");

        public event Action<ScoreBall> OnSpawnScoreBall;

        public event Action OnRelease;
        public event Action OnInit;

        public void ExecuteModelInit()
        {
            Init();
        }

        public void Release()
        {
            SetEventRegister(false);
            OnSpawnScoreBall = null;

            OnRelease?.Invoke();
        }

        private void Init()
        {
            InitData();
            SetEventRegister(true);

            presenter.BindModel(this);

            OnInit?.Invoke();
        }

        private void InitData()
        {
            currentBeatIndex = -1;

            inFieldScoreBallList = new List<ScoreBall>();

            List<int> spawnBeatIndexList = appProcessor.CurrentStageSettingContent.SpawnBeatIndexList;
            if (spawnBeatIndexList == null || spawnBeatIndexList.Count == 0)
                throw new NullReferenceException("SpawnBeatIndexList is empty");

            tempSpawnBeatIndexList = new List<int>();
            tempSpawnBeatIndexList.AddRange(spawnBeatIndexList);
        }

        private bool TryGetHiddenScoreBall(out ScoreBall hiddenScoreBall)
        {
            if (inFieldScoreBallList.Count > 0 &&
                inFieldScoreBallList.Exists(x => x.CurrentState == ScoreBallState.Hide))
            {
                hiddenScoreBall = inFieldScoreBallList.First(x => x.CurrentState == ScoreBallState.Hide);
                return hiddenScoreBall != null;
            }
            else
            {
                hiddenScoreBall = null;
                return false;
            }
        }

        private void SetEventRegister(bool isListen)
        {
            eventRegister.Unregister<BeatEvent>(OnBeatEvent);

            if (isListen)
            {
                eventRegister.Register<BeatEvent>(OnBeatEvent);
            }
        }

        private void SpawnScoreBall()
        {
            IScoreBallView scoreBallView = presenter.Spawn();

            if (TryGetHiddenScoreBall(out ScoreBall hiddenScoreBall))
            {
                dynamicMVPBinder.RebindView(hiddenScoreBall, scoreBallView);

                hiddenScoreBall.Reactivate(CreateFlagNumber());
                OnSpawnScoreBall?.Invoke(hiddenScoreBall);
            }
            else
            {
                ScoreBallPresenter scoreBallPresenter = new ScoreBallPresenter();
                ScoreBall scoreBall = new ScoreBall(eventRegister, eventInvoker, appProcessor.CurrentStageSettingContent.FlagChangeSetting);

                dynamicMVPBinder.MultipleBind(scoreBall, scoreBallPresenter, scoreBallView);

                scoreBallView.Init();
                scoreBallPresenter.Init(beaterModel, gameSetting.ScoreBallTextColorSetting, gameSetting.ScoreBallFrameColorByFlagSetting);
                scoreBall.Init(appProcessor.CurrentStageSettingContent.ScoreBallStartCountDownValue, CreateFlagNumber());

                inFieldScoreBallList.Add(scoreBall);
                OnSpawnScoreBall?.Invoke(scoreBall);
            }
        }

        private int CreateFlagNumber()
        {
            List<ScoreBallFlagWeightDefine> flagWeightSetting =
                appProcessor.CurrentStageSettingContent.GetScoreBallFlagWeightSetting(feverEnergyBarModel.CurrentFeverStage);

            if (flagWeightSetting.Count == 0)
                throw new NullReferenceException("ScoreBallFlagWeightSetting is empty");

            Dictionary<int, int> flagWeightSettingDict = flagWeightSetting.ToDictionary(x => x.FlagNumber, x => x.Weight);
            return RandomAlgorithm.GetRandomNumberByWeight(flagWeightSettingDict);
        }

        private void OnBeatEvent(BeatEvent eventInfo)
        {
            currentBeatIndex++;

            if (tempSpawnBeatIndexList.Contains(currentBeatIndex))
            {
                tempSpawnBeatIndexList.Remove(currentBeatIndex);
                SpawnScoreBall();
            }
        }
    }
}