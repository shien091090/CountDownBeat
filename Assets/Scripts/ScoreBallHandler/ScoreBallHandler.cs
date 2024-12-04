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

        private DynamicMVPBinder dynamicMVPBinder = new DynamicMVPBinder();
        private List<int> tempSpawnBeatIndexList;
        private List<ScoreBall> inFieldScoreBallList;
        private int currentBeatIndex;

        public event Action<ScoreBall> OnSpawnScoreBall;

        public int CurrentInFieldScoreBallAmount => inFieldScoreBallList?.Count(x => x.CurrentState != ScoreBallState.Hide && x.CurrentState != ScoreBallState.None) ?? 0;

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
            IScoreBallView scoreBallView = presenter.Spawn();

            if (TryGetHiddenScoreBall(out ScoreBall hiddenScoreBall))
            {
                dynamicMVPBinder.RebindView(hiddenScoreBall, scoreBallView);

                hiddenScoreBall.Reactivate();
                OnSpawnScoreBall?.Invoke(hiddenScoreBall);
            }
            else
            {
                ScoreBallPresenter scoreBallPresenter = new ScoreBallPresenter();
                ScoreBall scoreBall = new ScoreBall(eventRegister, eventInvoker);

                dynamicMVPBinder.MultipleBind(scoreBall, scoreBallPresenter, scoreBallView);

                scoreBallView.Init();
                scoreBallPresenter.Init(gameSetting.ScoreBallTextColorSetting);
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