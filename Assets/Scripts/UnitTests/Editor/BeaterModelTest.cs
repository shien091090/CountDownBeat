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

        #region 節拍事件

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
    }
}