using System;
using System.Collections.Generic;
using System.Linq;
using SNShien.Common.ProcessTools;
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

        private List<int> tempSpawnBeatIndexList;
        private List<ScoreBall> inFieldScoreBallList;
        private int currentBeatIndex;

        public event Action<ScoreBall> OnSpawnScoreBall;

        public int CurrentInFieldScoreBallAmount => inFieldScoreBallList?.Count(x => x.CurrentState != ScoreBallState.Hide && x.CurrentState != ScoreBallState.None) ?? 0;

        public void ExecuteModelInit()
        {
            Init();
        }

        public void Release()
        {
            SetEventRegister(false);
            presenter.UnbindModel();
            OnSpawnScoreBall = null;
        }

        private void Init()
        {
            InitData();
            InitPresenter();
            SetEventRegister(true);
        }

        private void InitPresenter()
        {
            presenter.BindModel(this);
            presenter.OpenView();
        }

        private void InitData()
        {
            currentBeatIndex = -1;

            inFieldScoreBallList = new List<ScoreBall>();

            tempSpawnBeatIndexList = new List<int>();
            tempSpawnBeatIndexList.AddRange(appProcessor.CurrentStageSettingContent.SpawnBeatIndexList);
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
            if (TryGetHiddenScoreBall(out ScoreBall hiddenScoreBall))
            {
                hiddenScoreBall.Reactivate();
                OnSpawnScoreBall?.Invoke(hiddenScoreBall);
            }
            else
            {
                IScoreBallView scoreBallView = presenter.Spawn();
                scoreBallView.Init();

                ScoreBallPresenter scoreBallPresenter = new ScoreBallPresenter();
                ScoreBall scoreBall = new ScoreBall(eventRegister, eventInvoker, gameSetting.ScoreBallTextColorSetting);
                scoreBall.BindPresenter(scoreBallPresenter);
                scoreBallPresenter.BindView(scoreBallView);

                scoreBall.Init(gameSetting.ScoreBallStartCountDownValue);

                inFieldScoreBallList.Add(scoreBall);
                OnSpawnScoreBall?.Invoke(scoreBall);
            }
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