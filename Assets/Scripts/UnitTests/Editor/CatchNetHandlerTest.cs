using System;
using System.Linq;
using NSubstitute;
using NUnit.Framework;
using SNShien.Common.ProcessTools;
using UnityEngine;
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

            IEventInvoker eventInvoker = Substitute.For<IEventInvoker>();
            Container.Bind<IEventInvoker>().FromInstance(eventInvoker).AsSingle();

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
            catchNetView = Substitute.For<ICatchNetView>();

            presenter.When(x => x.SpawnCatchNet(Arg.Any<ICatchNetPresenter>())).Do(callInfo =>
            {
                GivenCurrentCatchNetCount(presenter.CurrentCatchNetCount + 1);

                ICatchNetPresenter catchNetPresenter = (ICatchNetPresenter)callInfo.Args()[0];
                catchNetPresenter.BindView(catchNetView);
            });
        }

        private CatchNetHandler catchNetHandler;
        private IEventRegister eventRegister;
        private IGameSetting gameSetting;
        private ICatchNetHandlerPresenter presenter;
        private ICatchNetView catchNetView;

        private Action<BeatEvent> beatEventCallback;

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

        [Test]
        //生成捕獲網, 驗證捕獲數字
        public void spawn_catch_net_then_verify_target_number()
        {
            GivenCatchNetNumberRange(1, 5);

            catchNetHandler.ExecuteModelInit();

            CallBeatEventCallback();

            // LastRefreshCatchNetNumberShouldBe(1, 5);
        }

        private void GivenCatchNetNumberRange(int min, int max)
        {
            gameSetting.CatchNetNumberRange.Returns(new Vector2Int(min, max));
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
                presenter.DidNotReceive().SpawnCatchNet(Arg.Any<CatchNetPresenter>());
            else
                presenter.Received(expectedCallTimes).SpawnCatchNet(Arg.Any<CatchNetPresenter>());
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