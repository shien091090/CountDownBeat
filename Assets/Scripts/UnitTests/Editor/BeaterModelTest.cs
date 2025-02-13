using System;
using System.Collections.Generic;
using System.Linq;
using FMOD.Studio;
using NSubstitute;
using NUnit.Framework;
using SNShien.Common.AudioTools;
using SNShien.Common.MonoBehaviorTools;
using SNShien.Common.ProcessTools;
using Zenject;

namespace GameCore.UnitTests
{
    [TestFixture]
    public class BeaterModelTest : ZenjectUnitTestFixture
    {
        private BeaterModel beaterModel;
        private IViewManager viewManager;
        private IEventInvoker eventInvoker;
        private IAudioManager audioManager;
        private FmodAudioCallbackSetting callbackSetting;
        private IAppProcessor appProcessor;
        private IBeaterPresenter beaterPresenter;

        [SetUp]
        public override void Setup()
        {
            base.Setup();

            InitViewManagerMock();
            InitEventInvokerMock();
            InitAudioManagerMock();
            InitAppProcessorMock();
            InitBeaterPresenterMock();

            Container.Bind<BeaterModel>().AsSingle();
            beaterModel = Container.Resolve<BeaterModel>();
        }

        private void InitBeaterPresenterMock()
        {
            beaterPresenter = Substitute.For<IBeaterPresenter>();

            Container.Bind<IBeaterPresenter>().FromInstance(beaterPresenter).AsSingle();
        }

        private void InitAppProcessorMock()
        {
            appProcessor = Substitute.For<IAppProcessor>();

            Container.Bind<IAppProcessor>().FromInstance(appProcessor).AsSingle();
        }

        private void InitAudioManagerMock()
        {
            audioManager = Substitute.For<IAudioManager>();
            callbackSetting = new FmodAudioCallbackSetting();

            audioManager.PlayWithCallback(Arg.Any<string>()).Returns(callbackSetting);

            Container.Bind<IAudioManager>().FromInstance(audioManager).AsSingle();
        }

        private void InitEventInvokerMock()
        {
            eventInvoker = Substitute.For<IEventInvoker>();

            Container.Bind<IEventInvoker>().FromInstance(eventInvoker).AsSingle();
        }

        private void InitViewManagerMock()
        {
            viewManager = Substitute.For<IViewManager>();

            viewManager.When(x => x.OpenView<BeaterView>(Arg.Any<object>())).Do(callInfo =>
            {
                IBeaterView view = Substitute.For<IBeaterView>();
                object[] args = (object[])callInfo.Args()[0];
                BeaterPresenter beaterPresenter = (BeaterPresenter)args[0];
                beaterPresenter.BindView(view);
            });

            Container.Bind<IViewManager>().FromInstance(viewManager).AsSingle();
        }

        private void GivenStageSettingContentIsNull()
        {
            appProcessor.CurrentStageSettingContent.Returns((StageSettingContent)null);
        }

        private void GivenStageSettingContent(string audioKey, int bpm = 1, int countDownBeatFreq = 1, List<int> spawnBeatIndexList = null)
        {
            StageSettingContent stageSettingContent = new StageSettingContent();
            stageSettingContent.SetAudioKey(audioKey);
            stageSettingContent.SetBpm(bpm);
            stageSettingContent.SetSpawnBeatIndexList(spawnBeatIndexList);
            stageSettingContent.SetCountDownBeatFreq(countDownBeatFreq);

            appProcessor.CurrentStageSettingContent.Returns(stageSettingContent);
        }

        private void GivenCurrentTimer(float timer)
        {
            beaterPresenter.CurrentTimer.Returns(timer);
        }

        private void CallAudioCallback(EVENT_CALLBACK_TYPE type)
        {
            callbackSetting.TryCallback(type);
        }

        private void AccuracyResultShouldBe(float currentTime, BeatTimingDirection expectedBeatTimingDirection, float expectedAccuracy)
        {
            BeatAccuracyResult accuracyResult = beaterModel.DetectBeatAccuracy(currentTime);
            Assert.AreEqual(expectedBeatTimingDirection, accuracyResult.BeatTimingDirection);

            //使用近似值的方式比對, 只要是在expectedAccuracy的加減0.01範圍內都算正確
            Assert.IsTrue(Math.Abs(expectedAccuracy - accuracyResult.AccuracyValue) < 0.01f);
        }

        private void LastSetHalfBeatTimeOffsetShouldBe(float expectedHalfBeatTimeOffset)
        {
            float arg = (float)beaterPresenter
                .ReceivedCalls()
                .Last(x => x.GetMethodInfo().Name == "SetHalfBeatTimeOffset")
                .GetArguments()[0];

            Assert.AreEqual(expectedHalfBeatTimeOffset, arg);
        }

        private void LastBeatEventShouldBe(bool expectedIsCountDownBeat)
        {
            IArchitectureEvent eventInfo = (IArchitectureEvent)eventInvoker.ReceivedCalls().Last(x => x.GetMethodInfo().Name == "SendEvent").GetArguments()[0];
            BeatEvent beatEvent = eventInfo as BeatEvent;
            Assert.AreEqual(expectedIsCountDownBeat, beatEvent.isCountDownBeat);
        }

        private void ShouldSendBeatEvent(int expectedCallTimes)
        {
            eventInvoker.Received(expectedCallTimes).SendEvent(Arg.Any<BeatEvent>());
        }

        private void ShouldPlayWithCallBack(string expectedAudioKey, int expectedTrackIndex, int expectedCallTimes = 1)
        {
            audioManager.Received(expectedCallTimes).PlayWithCallback(expectedAudioKey, expectedTrackIndex);
        }

        private void AccuracyResultShouldBeInvalid(float currentTime)
        {
            BeatAccuracyResult accuracyResult = beaterModel.DetectBeatAccuracy(currentTime);
            Assert.AreEqual(BeatTimingDirection.Invalid, accuracyResult.BeatTimingDirection);
            Assert.AreEqual(0, accuracyResult.AccuracyValue);
        }

        private void NextBeatTimingShouldBe(float expectedBeatTiming)
        {
            Assert.IsTrue(Math.Abs(expectedBeatTiming - beaterModel.GetNextBeatTiming()) < 0.01f);
        }

        #region 初始化

        [Test]
        //初始化時, 撥放當前關卡音樂
        public void play_current_stage_audio_when_init()
        {
            GivenStageSettingContent(GameConst.AUDIO_NAME_BGM_1);

            beaterModel.ExecuteModelInit();

            ShouldPlayWithCallBack(GameConst.AUDIO_NAME_BGM_1, 0);
        }

        [Test]
        //初始化若取不到關卡設定, 會報錯
        public void throw_error_when_cannot_get_stage_setting()
        {
            GivenStageSettingContentIsNull();

            Assert.Throws<NullReferenceException>(() => beaterModel.ExecuteModelInit());
        }

        #endregion

        #region 整拍事件

        [Test]
        //當收到音樂節拍事件時，若沒有達到分數球倒數拍點, 驗證BeatEvent事件中的參數
        public void send_beat_event_when_receive_bgm_beat_callback_and_not_count_down_beat()
        {
            GivenStageSettingContent(GameConst.AUDIO_NAME_BGM_1, countDownBeatFreq: 2);

            beaterModel.ExecuteModelInit();

            CallAudioCallback(EVENT_CALLBACK_TYPE.TIMELINE_BEAT);

            ShouldSendBeatEvent(1);
            LastBeatEventShouldBe(false);
        }

        [Test]
        //當收到音樂節拍事件時, 若達到分數球倒數拍點, 驗證BeatEvent事件中的參數
        public void send_beat_event_when_receive_bgm_beat_callback_and_is_count_down_beat()
        {
            GivenStageSettingContent(GameConst.AUDIO_NAME_BGM_1, countDownBeatFreq: 1);

            beaterModel.ExecuteModelInit();

            CallAudioCallback(EVENT_CALLBACK_TYPE.TIMELINE_BEAT);

            ShouldSendBeatEvent(1);
            LastBeatEventShouldBe(true);
        }

        #endregion

        #region 半拍事件

        [Test]
        [TestCase(0.5f, 1f, 0.25f)]
        [TestCase(1f, 2f, 0.5f)]
        [TestCase(0.8f, 1.6f, 0.4f)]
        //收到兩個音樂節拍事件時, 會開始計算半拍時間並通知Presenter半拍時間差
        public void calculate_half_beat_time_offset_when_receive_two_bgm_beat_callback(float firstBeatTime, float secondBeatTime, float expectedHalfBeatTimeOffset)
        {
            GivenStageSettingContent(GameConst.AUDIO_NAME_BGM_1, countDownBeatFreq: 2);

            beaterModel.ExecuteModelInit();

            GivenCurrentTimer(firstBeatTime);
            CallAudioCallback(EVENT_CALLBACK_TYPE.TIMELINE_BEAT);

            GivenCurrentTimer(secondBeatTime);
            CallAudioCallback(EVENT_CALLBACK_TYPE.TIMELINE_BEAT);

            LastSetHalfBeatTimeOffsetShouldBe(expectedHalfBeatTimeOffset);
        }

        [Test]
        //持續收到音樂節拍事件時, 會動態調整半拍時間差並通知Presenter
        public void dynamic_adjust_half_beat_time_offset_when_receive_bgm_beat_callback()
        {
            GivenStageSettingContent(GameConst.AUDIO_NAME_BGM_1, countDownBeatFreq: 2);

            beaterModel.ExecuteModelInit();

            GivenCurrentTimer(0.5f); //+0.5
            CallAudioCallback(EVENT_CALLBACK_TYPE.TIMELINE_BEAT);

            GivenCurrentTimer(1f); //+0.5
            CallAudioCallback(EVENT_CALLBACK_TYPE.TIMELINE_BEAT);

            LastSetHalfBeatTimeOffsetShouldBe(0.25f);

            GivenCurrentTimer(1.2f); //+0.2
            CallAudioCallback(EVENT_CALLBACK_TYPE.TIMELINE_BEAT);

            LastSetHalfBeatTimeOffsetShouldBe(0.2f);
        }

        #endregion

        #region 拍點準度偵測

        [Test]
        [TestCase(2.5f, 0)]
        [TestCase(2.75f, 0.5f)]
        [TestCase(2.975f, 0.95f)]
        //以兩個半拍為範圍偵測拍點準度, 偵測點在下一個節拍前
        public void detect_beat_accuracy_and_detect_point_before_next_beat(float currentTime, float expectedAccuracy)
        {
            GivenStageSettingContent(GameConst.AUDIO_NAME_BGM_1, countDownBeatFreq: 2);

            beaterModel.ExecuteModelInit();

            GivenCurrentTimer(1f);
            CallAudioCallback(EVENT_CALLBACK_TYPE.TIMELINE_BEAT);

            GivenCurrentTimer(2f);
            CallAudioCallback(EVENT_CALLBACK_TYPE.TIMELINE_BEAT);

            //下一拍預計時間為3秒, 故下一拍偵測點為2.5~3.5秒間, 下一個節拍前則是2.5~3秒間
            AccuracyResultShouldBe(currentTime, BeatTimingDirection.Early, expectedAccuracy);
        }

        [Test]
        [TestCase(2, 1)]
        [TestCase(2.25f, 0.5f)]
        [TestCase(2.495f, 0.01f)]
        //以兩個半拍為範圍偵測拍點準度, 偵測點在目前節拍後
        public void detect_beat_accuracy_and_detect_point_after_current_beat(float currentTime, float expectedAccuracy)
        {
            GivenStageSettingContent(GameConst.AUDIO_NAME_BGM_1, countDownBeatFreq: 2);

            beaterModel.ExecuteModelInit();

            GivenCurrentTimer(1f);
            CallAudioCallback(EVENT_CALLBACK_TYPE.TIMELINE_BEAT);

            GivenCurrentTimer(2f);
            CallAudioCallback(EVENT_CALLBACK_TYPE.TIMELINE_BEAT);

            //目前節拍為2秒, 下一拍預計時間為3秒, 故目前節拍後偵測點為1.5~2.5秒間, 目前節拍後則是2~2.5秒間
            AccuracyResultShouldBe(currentTime, BeatTimingDirection.Late, expectedAccuracy);
        }
        
        //

        [Test]
        //尚未收到音樂節拍事件時, 偵測拍點準度回傳Invalid
        public void detect_beat_accuracy_but_not_receive_beat_callback()
        {
            GivenStageSettingContent(GameConst.AUDIO_NAME_BGM_1, countDownBeatFreq: 2);

            beaterModel.ExecuteModelInit();

            AccuracyResultShouldBeInvalid(0.5f);
        }

        [Test]
        [TestCase(3.9f, BeatTimingDirection.Late, 0.5f)]
        [TestCase(4.2f, BeatTimingDirection.Late, 0)]
        [TestCase(4.5f, BeatTimingDirection.Early, 0.5f)]
        [TestCase(4.65f, BeatTimingDirection.Early, 0.75f)]
        //收到節拍事件後更新平均半拍時間差, 並且偵測拍點準度
        public void update_half_beat_time_offset_and_detect_beat_accuracy_when_receive_beat_callback(float currentTime, BeatTimingDirection expectedBeatTimingDirection,
            float expectedAccuracy)
        {
            GivenStageSettingContent(GameConst.AUDIO_NAME_BGM_1, countDownBeatFreq: 2);

            beaterModel.ExecuteModelInit();

            GivenCurrentTimer(1f);
            CallAudioCallback(EVENT_CALLBACK_TYPE.TIMELINE_BEAT);

            GivenCurrentTimer(2f); //平均半拍時間差為0.5
            CallAudioCallback(EVENT_CALLBACK_TYPE.TIMELINE_BEAT);

            GivenCurrentTimer(3.6f); //平均半拍時間差更新為0.6
            CallAudioCallback(EVENT_CALLBACK_TYPE.TIMELINE_BEAT);

            NextBeatTimingShouldBe(4.8f);
            AccuracyResultShouldBe(currentTime, expectedBeatTimingDirection, expectedAccuracy);
        }

        #endregion
    }
}