using System;
using System.Linq;
using NSubstitute;
using NUnit.Framework;
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

        private Action<BeatEvent> beatEventCallback;
        private Action<GetScoreEvent> getScoreEventCallback;

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

            Container.Bind<HpBarModel>().AsSingle();
            hpBarModel = Container.Resolve<HpBarModel>();
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
        //每次Beat時, 血量會依照關卡設定減少
        public void decrease_hp_when_beat()
        {
            GivenHpMax(100);
            GivenHpDecreasePerBeatSetting(1.5f);

            hpBarModel.Init();
            CallBeatEventCallback();

            CurrentHpShouldBe(98.5f);
        }

        [Test]
        [TestCase(50)]
        [TestCase(52.5f)]
        //Beat減少血量至0時, 會發送GameOver事件
        public void send_game_over_event_when_hp_is_0(float hpDecreasePerBeat)
        {
            GivenHpMax(100);
            GivenHpDecreasePerBeatSetting(hpDecreasePerBeat);

            hpBarModel.Init();
            CallBeatEventCallback();

            ShouldSendGameOverEvent(0);

            CallBeatEventCallback();

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
            GivenHpDecreasePerBeatSetting(50);
            GivenHpIncreasePerCatchSetting(hpIncreasePerCatch);

            hpBarModel.Init();
            CallBeatEventCallback();
            CallGetScoreEventCallback();

            CurrentHpShouldBe(expectedFinalHp);
        }

        [Test]
        //捕獲分數球血量增加時, 不會超過全滿血量
        public void hp_will_not_exceed_hp_max_when_catch_score_ball()
        {
            GivenHpMax(100);
            GivenHpDecreasePerBeatSetting(50);
            GivenHpIncreasePerCatchSetting(100);

            hpBarModel.Init();
            CallBeatEventCallback();
            CallGetScoreEventCallback();

            CurrentHpShouldBe(100);
        }

        [Test]
        //每次更新血量時, 會通知presenter刷新血量顯示
        public void call_presenter_to_refresh_hp_when_hp_changed()
        {
            GivenHpMax(100);
            GivenHpDecreasePerBeatSetting(5);
            GivenHpIncreasePerCatchSetting(3);

            hpBarModel.Init();
            ShouldCallPresenterRefreshHp(1, 100);

            CallBeatEventCallback();
            ShouldCallPresenterRefreshHp(2, 95);

            CallGetScoreEventCallback();
            ShouldCallPresenterRefreshHp(3, 98);
        }

        private void InitEventRegisterMock()
        {
            beatEventCallback = null;
            getScoreEventCallback = null;

            eventRegister = Substitute.For<IEventRegister>();

            eventRegister.When(x => x.Register(Arg.Any<Action<BeatEvent>>())).Do(x =>
            {
                Action<BeatEvent> callback = (Action<BeatEvent>)x.Args()[0];
                beatEventCallback += callback;
            });

            eventRegister.When(x => x.Register(Arg.Any<Action<GetScoreEvent>>())).Do(x =>
            {
                Action<GetScoreEvent> callback = (Action<GetScoreEvent>)x.Args()[0];
                getScoreEventCallback += callback;
            });
        }

        private void InitAppProcessorMock()
        {
            appProcessor = Substitute.For<IAppProcessor>();

            appProcessor.CurrentStageSettingContent.Returns(new StageSettingContent());
        }

        private void GivenHpIncreasePerCatchSetting(float hpIncreasePerCatch)
        {
            appProcessor.CurrentStageSettingContent.SetHpIncreasePerCatch(hpIncreasePerCatch);
        }

        private void GivenHpDecreasePerBeatSetting(float hpDecreasePerBeat)
        {
            appProcessor.CurrentStageSettingContent.SetHpDecreasePerBeat(hpDecreasePerBeat);
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

        private void CallBeatEventCallback()
        {
            beatEventCallback.Invoke(new BeatEvent(false));
        }

        private void ShouldCallPresenterRefreshHp(int expectedCallTimes, float expectedHp)
        {
            presenter.Received(expectedCallTimes).RefreshHp(Arg.Any<float>());

            float hpArg = (float)presenter
                .ReceivedCalls()
                .Last(x => x.GetMethodInfo().Name == "RefreshHp")
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