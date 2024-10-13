using NSubstitute;
using NUnit.Framework;
using SNShien.Common.AdapterTools;
using SNShien.Common.MonoBehaviorTools;
using SNShien.Common.ProcessTools;
using UnityEngine;
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

            gameSetting = Substitute.For<IGameSetting>();
            Container.Bind<IGameSetting>().FromInstance(gameSetting).AsSingle();

            InitViewManagerMock();
            Container.Bind<IViewManager>().FromInstance(viewManager).AsSingle();

            deltaTimeGetter = Substitute.For<IDeltaTimeGetter>();
            Container.Bind<IDeltaTimeGetter>().FromInstance(deltaTimeGetter).AsSingle();

            InitEventInvokerMock();
            Container.Bind<IEventInvoker>().FromInstance(eventInvoker).AsSingle();

            Container.Bind<BeaterModel>().AsSingle();
            beaterModel = Container.Resolve<BeaterModel>();
        }

        private BeaterModel beaterModel;
        private IGameSetting gameSetting;
        private IViewManager viewManager;
        private IDeltaTimeGetter deltaTimeGetter;
        private IEventInvoker eventInvoker;

        private int beatEventTriggerTimes;
        private int halfBeatEventTriggerTimes;

        private void InitEventInvokerMock()
        {
            beatEventTriggerTimes = 0;
            halfBeatEventTriggerTimes = 0;

            eventInvoker = Substitute.For<IEventInvoker>();

            eventInvoker.When(x => x.SendEvent(Arg.Is<BeatEvent>(x => x is BeatEvent))).Do(callInfo =>
            {
                beatEventTriggerTimes++;
            });

            eventInvoker.When(x => x.SendEvent(Arg.Is<HalfBeatEvent>(x => x is HalfBeatEvent))).Do(callInfo =>
            {
                halfBeatEventTriggerTimes++;
            });
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

        [Test]
        //更新幀畫面後未達到節拍時間點, 不發送事件
        public void update_frame_not_reach_beat_time()
        {
            GivenBeatTimeThresholdSetting(1);
            GivenDeltaTime(0.9f);

            beaterModel.ExecuteModelInit();
            beaterModel.UpdatePerFrame();

            ShouldSendBeatEvent(0);
        }

        [Test]
        //更新幀畫面後達節拍時間點, 發送事件
        public void update_frame_reach_beat_time()
        {
            GivenBeatTimeThresholdSetting(1);
            GivenDeltaTime(1);

            beaterModel.ExecuteModelInit();
            beaterModel.UpdatePerFrame();

            ShouldSendBeatEvent(1);
        }

        [Test]
        //更新幀畫面後達節拍時間點, 且超過的時間會計算至下次節拍
        public void update_frame_reach_beat_time_and_exceed()
        {
            GivenBeatTimeThresholdSetting(1);
            GivenDeltaTime(0.7f);

            beaterModel.ExecuteModelInit();
            beaterModel.UpdatePerFrame();
            beaterModel.UpdatePerFrame();
            beaterModel.UpdatePerFrame();

            ShouldSendBeatEvent(2);
        }

        [Test]
        //節拍頻率設為0, 更新幀畫面時不做事
        public void beat_time_threshold_is_0()
        {
            GivenBeatTimeThresholdSetting(0);
            GivenDeltaTime(0.7f);

            beaterModel.ExecuteModelInit();
            beaterModel.UpdatePerFrame();
            beaterModel.UpdatePerFrame();
            beaterModel.UpdatePerFrame();

            ShouldSendBeatEvent(0);
        }

        [Test]
        //更新幀畫面後未達到一半節拍時間點, 不發送事件
        public void update_frame_not_reach_half_beat_time()
        {
            GivenBeatTimeThresholdSetting(1);
            GivenDeltaTime(0.4f);

            beaterModel.ExecuteModelInit();
            beaterModel.UpdatePerFrame();

            ShouldSendHalfBeatEvent(0);
        }

        [Test]
        //更新幀畫面後達一半節拍時間點, 發送事件
        public void update_frame_reach_half_beat_time()
        {
            GivenBeatTimeThresholdSetting(1);
            GivenDeltaTime(0.5f);

            beaterModel.ExecuteModelInit();
            beaterModel.UpdatePerFrame();

            ShouldSendHalfBeatEvent(1);
        }

        [Test]
        //更新幀畫面後達一半節拍時間點, 且超過的時間會計算至下次節拍
        public void update_frame_reach_half_beat_time_and_exceed()
        {
            GivenBeatTimeThresholdSetting(1);
            GivenDeltaTime(0.4f);

            beaterModel.ExecuteModelInit();
            beaterModel.UpdatePerFrame();
            beaterModel.UpdatePerFrame();
            beaterModel.UpdatePerFrame();
            beaterModel.UpdatePerFrame();

            ShouldSendHalfBeatEvent(2);
        }

        private void ShouldSendHalfBeatEvent(int expectedSendTimes)
        {
            Assert.AreEqual(expectedSendTimes, halfBeatEventTriggerTimes);
        }

        private void ShouldSendBeatEvent(int expectedSendTimes)
        {
            Assert.AreEqual(expectedSendTimes, beatEventTriggerTimes);
        }

        private void GivenDeltaTime(float deltaTime)
        {
            deltaTimeGetter.deltaTime.Returns(deltaTime);
        }

        private void GivenBeatTimeThresholdSetting(float beatTimeThreshold)
        {
            gameSetting.BeatTimeThreshold.Returns(beatTimeThreshold);
        }
    }
}