using System;
using System.Linq;
using NSubstitute;
using NUnit.Framework;
using SNShien.Common.AdapterTools;
using SNShien.Common.ProcessTools;
using Zenject;

namespace GameCore.UnitTests
{
    public class HpBarModelTest : ZenjectUnitTestFixture
    {
        private HpBarModel hpBarModel;
        private IAppProcessor appProcessor;
        private IGameSetting gameSetting;
        private IEventRegister eventRegister;
        private IEventInvoker eventInvoker;
        private IHpBarPresenter presenter;
        private IDeltaTimeGetter deltaTimeGetter;

        private Action<GetScoreEvent> getScoreEventCallback;
        private Action<float> refreshHpEventCallback;
        private Action initEventCallback;
        private Action releaseEventCallback;

        [SetUp]
        public override void Setup()
        {
            base.Setup();

            InitAppProcessorMock();
            Container.Bind<IAppProcessor>().FromInstance(appProcessor).AsSingle();

            InitEventRegisterMock();
            Container.Bind<IEventRegister>().FromInstance(eventRegister).AsSingle();

            eventInvoker = Substitute.For<IEventInvoker>();
            Container.Bind<IEventInvoker>().FromInstance(eventInvoker).AsSingle();

            gameSetting = Substitute.For<IGameSetting>();
            Container.Bind<IGameSetting>().FromInstance(gameSetting).AsSingle();

            presenter = Substitute.For<IHpBarPresenter>();
            Container.Bind<IHpBarPresenter>().FromInstance(presenter).AsSingle();

            deltaTimeGetter = Substitute.For<IDeltaTimeGetter>();
            Container.Bind<IDeltaTimeGetter>().FromInstance(deltaTimeGetter).AsSingle();

            Container.Bind<HpBarModel>().AsSingle();
            hpBarModel = Container.Resolve<HpBarModel>();

            refreshHpEventCallback = Substitute.For<Action<float>>();
            hpBarModel.OnRefreshHp += refreshHpEventCallback;

            initEventCallback = Substitute.For<Action>();
            hpBarModel.OnInit += initEventCallback;

            releaseEventCallback = Substitute.For<Action>();
            hpBarModel.OnRelease += releaseEventCallback;
        }

        [Test]
        [TestCase(1)]
        [TestCase(1.5f)]
        [TestCase(100)]
        //初始化時會依據設定將血量設為全滿
        public void init_and_set_full_hp(float hpMaxSetting)
        {
            GivenHpMax(hpMaxSetting);

            hpBarModel.Init();

            CurrentHpShouldBe(hpMaxSetting);
        }

        [Test]
        //初始化找不到關卡設定時, 會報錯
        public void init_and_throw_exception_when_no_stage_setting()
        {
            GivenCurrentStageSettingContentNull();

            Assert.Throws<System.NullReferenceException>(() => hpBarModel.Init());
        }

        [Test]
        //初始化若血量設定為0時, 會報錯
        public void init_and_throw_exception_when_hp_max_is_0()
        {
            GivenHpMax(0);

            Assert.Throws<System.Exception>(() => hpBarModel.Init());
        }

        [Test]
        //初始化時, 會發送初始化事件
        public void send_init_event_when_init()
        {
            GivenHpMax(100);
            
            hpBarModel.Init();

            ShouldSendInitEvent(1);
        }

        [Test]
        //每幀刷新時, 血量會依照關卡設定減少
        public void decrease_hp_when_update_by_frame()
        {
            GivenHpMax(100);
            GivenDeltaTime(0.1f);
            GivenHpDecreasePerSecondSetting(10);

            hpBarModel.Init();
            hpBarModel.UpdateFrame();

            CurrentHpShouldBe(99);
        }

        [Test]
        [TestCase(50)]
        [TestCase(52.5f)]
        //Beat減少血量至0時, 會發送GameOver事件
        public void send_game_over_event_when_hp_is_0(float hpDecreasePerSecond)
        {
            GivenHpMax(100);
            GivenDeltaTime(1);
            GivenHpDecreasePerSecondSetting(hpDecreasePerSecond);

            hpBarModel.Init();
            hpBarModel.UpdateFrame();

            ShouldSendGameOverEvent(0);

            hpBarModel.UpdateFrame();

            ShouldSendGameOverEvent(1);
            CurrentHpShouldBe(0);
        }

        [Test]
        [TestCase(1.5f, 51.5f)]
        [TestCase(10, 60)]
        [TestCase(0, 50)]
        //每次捕獲分數球時, 血量會依據關卡設定增加
        public void increase_hp_when_catch_score_ball(float hpIncreasePerCatch, float expectedFinalHp)
        {
            GivenHpMax(100);
            GivenDeltaTime(1);
            GivenHpDecreasePerSecondSetting(50);
            GivenHpIncreasePerCatchSetting(hpIncreasePerCatch);

            hpBarModel.Init();
            hpBarModel.UpdateFrame();
            CallGetScoreEventCallback();

            CurrentHpShouldBe(expectedFinalHp);
        }

        [Test]
        //捕獲分數球血量增加時, 不會超過全滿血量
        public void hp_will_not_exceed_hp_max_when_catch_score_ball()
        {
            GivenHpMax(100);
            GivenDeltaTime(1);
            GivenHpDecreasePerSecondSetting(50);
            GivenHpIncreasePerCatchSetting(100);

            hpBarModel.Init();
            hpBarModel.UpdateFrame();
            CallGetScoreEventCallback();

            CurrentHpShouldBe(100);
        }

        [Test]
        //每次更新血量時, 會發送刷新血量事件
        public void send_refresh_hp_event_when_hp_changed()
        {
            GivenHpMax(100);
            GivenDeltaTime(1);
            GivenHpDecreasePerSecondSetting(5);
            GivenHpIncreasePerCatchSetting(3);

            hpBarModel.Init();
            ShouldSendRefreshHpEvent(1, 100);

            hpBarModel.UpdateFrame();
            ShouldSendRefreshHpEvent(2, 95);

            CallGetScoreEventCallback();
            ShouldSendRefreshHpEvent(3, 98);
        }

        [Test]
        //釋放時, 會發送釋放事件
        public void send_release_event_when_release()
        {
            hpBarModel.Release();

            ShouldSendReleaseEvent(1);
        }

        private void InitEventRegisterMock()
        {
            getScoreEventCallback = null;

            eventRegister = Substitute.For<IEventRegister>();

            eventRegister.When(x => x.Register(Arg.Any<Action<GetScoreEvent>>())).Do(x =>
            {
                Action<GetScoreEvent> callback = (Action<GetScoreEvent>)x.Args()[0];
                getScoreEventCallback += callback;
            });

            eventRegister.When(x => x.Unregister(Arg.Any<Action<GetScoreEvent>>())).Do(x =>
            {
                Action<GetScoreEvent> callback = (Action<GetScoreEvent>)x.Args()[0];
                getScoreEventCallback -= callback;
            });
        }

        private void InitAppProcessorMock()
        {
            appProcessor = Substitute.For<IAppProcessor>();

            appProcessor.CurrentStageSettingContent.Returns(new StageSettingContent());
        }

        private void GivenDeltaTime(float deltaTime)
        {
            deltaTimeGetter.deltaTime.Returns(deltaTime);
        }

        private void GivenHpIncreasePerCatchSetting(float hpIncreasePerCatch)
        {
            appProcessor.CurrentStageSettingContent.SetHpIncreasePerCatch(hpIncreasePerCatch);
        }

        private void GivenHpDecreasePerSecondSetting(float hpDecreasePerSecond)
        {
            appProcessor.CurrentStageSettingContent.SetHpDecreasePerSecond(hpDecreasePerSecond);
        }

        private void GivenCurrentStageSettingContentNull()
        {
            appProcessor.CurrentStageSettingContent.Returns((StageSettingContent)null);
        }

        private void GivenHpMax(float hpMax)
        {
            gameSetting.HpMax.Returns(hpMax);
        }

        private void CallGetScoreEventCallback()
        {
            getScoreEventCallback.Invoke(new GetScoreEvent(1));
        }

        private void ShouldSendReleaseEvent(int expectedCallTimes)
        {
            if (expectedCallTimes == 0)
                releaseEventCallback.DidNotReceive().Invoke();
            else
                releaseEventCallback.Received(expectedCallTimes).Invoke();
        }

        private void ShouldSendInitEvent(int expectedCallTimes)
        {
            if (expectedCallTimes == 0)
                initEventCallback.DidNotReceive().Invoke();
            else
                initEventCallback.Received(expectedCallTimes).Invoke();
        }

        private void ShouldSendRefreshHpEvent(int expectedCallTimes, float expectedHp)
        {
            refreshHpEventCallback.Received(expectedCallTimes).Invoke(Arg.Any<float>());

            float hpArg = (float)refreshHpEventCallback
                .ReceivedCalls()
                .Last()
                .GetArguments()[0];

            Assert.AreEqual(expectedHp, hpArg);
        }

        private void ShouldSendGameOverEvent(int expectedSendTimes)
        {
            if (expectedSendTimes == 0)
                eventInvoker.DidNotReceive().SendEvent(Arg.Any<GameOverEvent>());
            else
                eventInvoker.Received(expectedSendTimes).SendEvent(Arg.Any<GameOverEvent>());
        }

        private void CurrentHpShouldBe(float expectedHp)
        {
            Assert.AreEqual(expectedHp, hpBarModel.CurrentHp);
        }
    }
}