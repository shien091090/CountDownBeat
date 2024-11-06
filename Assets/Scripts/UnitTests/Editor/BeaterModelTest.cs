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
        [SetUp]
        public override void Setup()
        {
            base.Setup();

            InitViewManagerMock();
            InitEventInvokerMock();
            InitAudioManagerMock();
            InitAppProcessorMock();

            Container.Bind<BeaterModel>().AsSingle();
            beaterModel = Container.Resolve<BeaterModel>();
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

        private BeaterModel beaterModel;
        private IViewManager viewManager;
        private IEventInvoker eventInvoker;
        private IAudioManager audioManager;
        private FmodAudioCallbackSetting callbackSetting;
        private IAppProcessor appProcessor;

        [Test]
        //初始化時, 撥放當前關卡音樂
        public void play_current_stage_audio_when_init()
        {
            GivenStageSettingContent(GameConst.AUDIO_NAME_BGM_1);

            beaterModel.ExecuteModelInit();

            ShouldPlayWithCallBack(GameConst.AUDIO_NAME_BGM_1, 0);
        }

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
        //初始化若取不到關卡設定, 會報錯
        public void throw_error_when_cannot_get_stage_setting()
        {
            GivenStageSettingContentIsNull();

            Assert.Throws<NullReferenceException>(() => beaterModel.ExecuteModelInit());
        }

        private void LastBeatEventShouldBe(bool expectedIsCountDownBeat)
        {
            IArchitectureEvent eventInfo = (IArchitectureEvent)eventInvoker.ReceivedCalls().Last(x => x.GetMethodInfo().Name == "SendEvent").GetArguments()[0];
            BeatEvent beatEvent = eventInfo as BeatEvent;
            Assert.AreEqual(expectedIsCountDownBeat, beatEvent.isCountDownBeat);
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

        private void ShouldSendBeatEvent(int expectedCallTimes)
        {
            eventInvoker.Received(expectedCallTimes).SendEvent(Arg.Any<BeatEvent>());
        }

        private void CallAudioCallback(EVENT_CALLBACK_TYPE type)
        {
            callbackSetting.TryCallback(type);
        }

        private void ShouldPlayWithCallBack(string expectedAudioKey, int expectedTrackIndex, int expectedCallTimes = 1)
        {
            audioManager.Received(expectedCallTimes).PlayWithCallback(expectedAudioKey, expectedTrackIndex);
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
    }
}