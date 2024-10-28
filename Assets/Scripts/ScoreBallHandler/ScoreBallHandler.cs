using System;
using System.Collections.Generic;
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
        [Inject] private IBeaterModel beaterModel;

        private List<int> tempSpawnBeatIndexList;
        private int currentBeatIndex;

        public event Action OnSpawnScoreBall;

        public void ExecuteModelInit()
        {
            Init();
        }

        private void Init()
        {
            InitData();
            InitPresenter();
            RegisterEvent();
        }

        private void InitPresenter()
        {
            presenter.BindModel(this);
            presenter.OpenView();
        }

        private void InitData()
        {
            currentBeatIndex = 0;

            tempSpawnBeatIndexList = new List<int>();
            tempSpawnBeatIndexList.AddRange(beaterModel.CurrentStageSettingContent.SpawnBeatIndexList);
        }

        private void RegisterEvent()
        {
            eventRegister.Unregister<BeatEvent>(OnBeatEvent);
            eventRegister.Register<BeatEvent>(OnBeatEvent);
        }

        private void SpawnScoreBall()
        {
            IScoreBallView scoreBallView = presenter.Spawn();
            scoreBallView.Init();

            if (scoreBallView.CheckCreatePresenter(out IScoreBallPresenter scoreBallPresenter))
            {
                ScoreBall scoreBallModel = new ScoreBall(eventRegister, eventInvoker);
                scoreBallModel.BindPresenter(scoreBallPresenter);
                scoreBallModel.Init();
            }
            else
            {
            }

            OnSpawnScoreBall?.Invoke();
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