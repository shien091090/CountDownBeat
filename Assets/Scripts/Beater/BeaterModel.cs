using FMOD.Studio;
using SNShien.Common.AudioTools;
using SNShien.Common.MonoBehaviorTools;
using SNShien.Common.ProcessTools;
using UnityEngine;
using Zenject;

namespace GameCore
{
    public class BeaterModel : IBeaterModel
    {
        [Inject] private IViewManager viewManager;
        [Inject] private IEventInvoker eventInvoker;
        [Inject] private IAudioManager audioManager;
        [Inject] private IAppProcessor appProcessor;
        [Inject] private IBeaterPresenter presenter;

        private int beatCounter;
        private int totalBeatCounter;
        private float avgBeatInterval;
        private float halfBeatTimeOffset;

        public void ExecuteModelInit()
        {
            InitPresenter();

            StartStage(appProcessor.CurrentStageSettingContent);
        }

        public void Release()
        {
            ClearData();
            presenter.UnbindModel();
        }

        public void TriggerHalfBeat()
        {
            eventInvoker.SendEvent(new HalfBeatEvent());
            presenter.PlayHalfBeatAnimation();
        }

        private void InitPresenter()
        {
            presenter.BindModel(this);
            presenter.OpenView();
        }

        public BeatAccuracyResult DetectBeatAccuracy(float currentTime)
        {
            if(totalBeatCounter == 0)
                return BeatAccuracyResult.CreateInvalidResult();
            
            //預估下一個beat的時間點
            float nextBeatTiming = (totalBeatCounter + 1) * avgBeatInterval;
            
            //以beat時間點為基準, 減半拍時間到加半拍時間的範圍內, 判斷currentTime的準度, 回應準度結果(0~1), 1為最準, 0為最不準, 在beat時間點上為1, 在半拍時間點上為0
            float accuracy = 1f - Mathf.Abs(nextBeatTiming - currentTime) / halfBeatTimeOffset;
            
            BeatTimingDirection direction = currentTime >= nextBeatTiming ? BeatTimingDirection.Late : BeatTimingDirection.Early;

            return new BeatAccuracyResult(accuracy, direction);
        }

        private void ClearData()
        {
            beatCounter = 0;
            totalBeatCounter = 0;
            avgBeatInterval = 0;
            halfBeatTimeOffset = 0;
        }

        private void UpdateBeatTimeInfo()
        {
            if (totalBeatCounter == 0)
                return;

            avgBeatInterval = presenter.CurrentTimer / totalBeatCounter;
            halfBeatTimeOffset = avgBeatInterval / 2;
        }

        private void SendBeatEvent()
        {
            beatCounter++;

            bool isCountDownBeat = false;
            if (beatCounter >= appProcessor.CurrentStageSettingContent.CountDownBeatFreq)
            {
                isCountDownBeat = true;
                beatCounter = 0;
            }

            eventInvoker.SendEvent(new BeatEvent(isCountDownBeat));
        }

        private void StartStage(StageSettingContent stageSetting)
        {
            audioManager
                .PlayWithCallback(stageSetting.AudioKey)
                .Register(EVENT_CALLBACK_TYPE.TIMELINE_BEAT, OnBeat);

            beatCounter = 0;
        }

        private void OnBeat()
        {
            totalBeatCounter++;

            SendBeatEvent();
            UpdateBeatTimeInfo();
            presenter.SetHalfBeatTimeOffset(halfBeatTimeOffset);
            presenter.PlayBeatAnimation();
        }
    }
}