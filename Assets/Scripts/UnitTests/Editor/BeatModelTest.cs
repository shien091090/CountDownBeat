using FMOD.Studio;
using NSubstitute;
using NUnit.Framework;
using SNShien.Common.AdapterTools;
using SNShien.Common.AudioTools;
using SNShien.Common.MonoBehaviorTools;
using SNShien.Common.ProcessTools;
using Zenject;

namespace GameCore.UnitTests
{
    [TestFixture]
    public class BeatModelTest : ZenjectUnitTestFixture
    {
        [SetUp]
        public override void Setup()
        {
            base.Setup();

            InitViewManagerMock();
            Container.Bind<IViewManager>().FromInstance(viewManager).AsSingle();

            InitEventInvokerMock();
            Container.Bind<IEventInvoker>().FromInstance(eventInvoker).AsSingle();

            InitAudioManagerMock();
            Container.Bind<IAudioManager>().FromInstance(audioManager).AsSingle();

            Container.Bind<BeaterModel>().AsSingle();
            beaterModel = Container.Resolve<BeaterModel>();
        }

        private void InitAudioManagerMock()
        {
            audioManager = Substitute.For<IAudioManager>();
            callbackSetting = new FmodAudioCallbackSetting();

            audioManager.PlayWithCallback(Arg.Any<string>()).Returns(callbackSetting);
        }

        private BeaterModel beaterModel;
        private IViewManager viewManager;
        private IEventInvoker eventInvoker;
        private IAudioManager audioManager;
        private FmodAudioCallbackSetting callbackSetting;

        [Test]
        //當收到音樂節拍事件時，應該發送BeatEvent事件
        public void send_beat_event_when_receive_bgm_beat_callback()
        {
            beaterModel.ExecuteModelInit();

            ShouldPlayWithCallBack(GameConst.AUDIO_NAME_BGM_1, 0);

            CallAudioCallback(EVENT_CALLBACK_TYPE.TIMELINE_BEAT);

            ShouldSendBeatEvent(1);
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
        }
    }
}