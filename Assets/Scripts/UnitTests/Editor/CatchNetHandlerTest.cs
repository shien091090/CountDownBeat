using System;
using NSubstitute;
using NUnit.Framework;
using SNShien.Common.ProcessTools;
using Zenject;

namespace GameCore.UnitTests
{
    [TestFixture]
    public class CatchNetHandlerTest : ZenjectUnitTestFixture
    {
        [SetUp]
        public override void Setup()
        {
            base.Setup();

            InitGameSettingMock();
            Container.Bind<IGameSetting>().FromInstance(gameSetting).AsSingle();

            InitCatchNetPresenterMock();
            Container.Bind<ICatchNetHandlerPresenter>().FromInstance(presenter).AsSingle();

            InitEventHandlerMock();
            Container.Bind<IEventRegister>().FromInstance(eventRegister).AsSingle();

            Container.Bind<CatchNetHandler>().AsSingle();
            catchNetHandler = Container.Resolve<CatchNetHandler>();
        }

        private void InitGameSettingMock()
        {
            gameSetting = Substitute.For<IGameSetting>();

            GivenCatchNetLimit(10);
        }

        private void InitCatchNetPresenterMock()
        {
            presenter = Substitute.For<ICatchNetHandlerPresenter>();

            presenter.When(x => x.SpawnCatchNet()).Do(callInfo =>
            {
                GivenCurrentCatchNetCount(presenter.CurrentCatchNetCount + 1);
            });
        }

        private CatchNetHandler catchNetHandler;
        private IEventRegister eventRegister;
        private IGameSetting gameSetting;

        private Action<BeatEvent> beatEventCallback;
        private ICatchNetHandlerPresenter presenter;

        [Test]
        //每固定次數Beat時, 生成捕獲網
        public void spawn_catch_net_when_beat()
        {
            GivenSpawnCatchNetFreqSetting(3);

            catchNetHandler.ExecuteModelInit();

            CallBeatEventCallback();
            CallBeatEventCallback();
            ShouldSpawnCatchNet(0);

            CallBeatEventCallback();
            ShouldSpawnCatchNet(1);
        }

        [Test]
        //設定生成頻率為0時, 不會生成捕獲網
        public void not_spawn_catch_net_when_freq_is_0()
        {
            GivenSpawnCatchNetFreqSetting(0);

            catchNetHandler.ExecuteModelInit();

            CallBeatEventCallback();

            ShouldSpawnCatchNet(0);
        }

        [Test]
        //捕獲網達上限數量時, 即使達Beat次數門檻也不會生成捕獲網
        public void not_spawn_catch_net_when_reach_limit()
        {
            GivenSpawnCatchNetFreqSetting(1);
            GivenCatchNetLimit(4);

            catchNetHandler.ExecuteModelInit();

            CallBeatEventCallback();
            CallBeatEventCallback();
            CallBeatEventCallback();
            CallBeatEventCallback();

            ShouldSpawnCatchNet(4);

            CallBeatEventCallback();

            ShouldSpawnCatchNet(4);
        }

        [Test]
        //捕獲網達上限數量後, 若有捕獲網成功結算, 則後續會再生成捕獲網
        public void spawn_catch_net_after_success_settle()
        {
            GivenSpawnCatchNetFreqSetting(1);
            GivenCatchNetLimit(4);

            catchNetHandler.ExecuteModelInit();

            CallBeatEventCallback();
            CallBeatEventCallback();
            CallBeatEventCallback();
            CallBeatEventCallback();

            ShouldSpawnCatchNet(4);

            GivenCurrentCatchNetCount(presenter.CurrentCatchNetCount - 1);

            CallBeatEventCallback();

            ShouldSpawnCatchNet(5);
        }

        private void GivenCurrentCatchNetCount(int count)
        {
            presenter.CurrentCatchNetCount.Returns(count);
        }

        private void GivenCatchNetLimit(int catchNetLimit)
        {
            gameSetting.CatchNetLimit.Returns(catchNetLimit);
        }

        private void ShouldSpawnCatchNet(int expectedCallTimes)
        {
            if (expectedCallTimes == 0)
                presenter.DidNotReceive().SpawnCatchNet();
            else
                presenter.Received(expectedCallTimes).SpawnCatchNet();
        }

        private void InitEventHandlerMock()
        {
            beatEventCallback = null;

            eventRegister = Substitute.For<IEventRegister>();

            eventRegister.When(x => x.Register(Arg.Any<Action<BeatEvent>>())).Do(x =>
            {
                Action<BeatEvent> callback = (Action<BeatEvent>)x.Args()[0];
                beatEventCallback = callback;
            });
        }

        private void GivenSpawnCatchNetFreqSetting(int spawnCatchNetFreq)
        {
            gameSetting.SpawnCatchNetFreq.Returns(spawnCatchNetFreq);
        }

        private void CallBeatEventCallback()
        {
            beatEventCallback.Invoke(new BeatEvent());
        }
    }
}