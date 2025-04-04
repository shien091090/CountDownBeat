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
    public class FeverEnergyBarModelTest : ZenjectUnitTestFixture
    {
        private FeverEnergyBarModel feverEnergyBarModel;
        private IBeaterModel beaterModel;
        private IGameSetting gameSetting;
        private IEventRegister eventRegister;
        private IFeverEnergyBarPresenter feverEnergyBarPresenter;
        private IAppProcessor appProcessor;
        private IStageSettingContent stageSettingContent;

        private Action<HalfBeatEvent> halfBeatEventCallback;
        private Action<UpdateFeverEnergyBarEvent> updateFeverEnergyBarEventCallback;
        private Action<int> updateFeverStageEventCallback;

        [SetUp]
        public override void Setup()
        {
            base.Setup();

            InitAppProcessorMock();
            InitBeaterModelMock();
            InitGameSettingMock();
            InitEventRegisterMock();
            InitFeverEnergyBarPresenterMock();

            Container.Bind<FeverEnergyBarModel>().AsSingle();
            feverEnergyBarModel = Container.Resolve<FeverEnergyBarModel>();

            updateFeverEnergyBarEventCallback = Substitute.For<Action<UpdateFeverEnergyBarEvent>>();
            feverEnergyBarModel.OnUpdateFeverEnergyValue += updateFeverEnergyBarEventCallback;

            updateFeverStageEventCallback = Substitute.For<Action<int>>();
            feverEnergyBarModel.OnUpdateFeverStage += updateFeverStageEventCallback;

            feverEnergyBarModel.ExecuteModelInit();
        }

        private void InitAppProcessorMock()
        {
            appProcessor = Substitute.For<IAppProcessor>();
            stageSettingContent = Substitute.For<IStageSettingContent>();

            appProcessor.CurrentStageSettingContent.Returns(stageSettingContent);

            Container.Bind<IAppProcessor>().FromInstance(appProcessor).AsSingle();
        }

        private void InitFeverEnergyBarPresenterMock()
        {
            feverEnergyBarPresenter = Substitute.For<IFeverEnergyBarPresenter>();
            Container.Bind<IFeverEnergyBarPresenter>().FromInstance(feverEnergyBarPresenter).AsSingle();
        }

        private void InitGameSettingMock()
        {
            gameSetting = Substitute.For<IGameSetting>();

            Container.Bind<IGameSetting>().FromInstance(gameSetting).AsSingle();

            GivenFeverEnergyBarSetting(100);
        }

        private void InitEventRegisterMock()
        {
            halfBeatEventCallback = Substitute.For<Action<HalfBeatEvent>>();
            eventRegister = Substitute.For<IEventRegister>();

            eventRegister.When(x => x.Register(Arg.Any<Action<HalfBeatEvent>>())).Do(x =>
            {
                Action<HalfBeatEvent> callback = (Action<HalfBeatEvent>)x.Args()[0];
                halfBeatEventCallback += callback;
            });

            eventRegister.When(x => x.Unregister(Arg.Any<Action<HalfBeatEvent>>())).Do(x =>
            {
                Action<HalfBeatEvent> callback = (Action<HalfBeatEvent>)x.Args()[0];
                halfBeatEventCallback -= callback;
            });

            Container.Bind<IEventRegister>().FromInstance(eventRegister).AsSingle();
        }

        private void InitBeaterModelMock()
        {
            beaterModel = Substitute.For<IBeaterModel>();

            Container.Bind<IBeaterModel>().FromInstance(beaterModel).AsSingle();
        }

        private void GivenFeverEnergyBarSetting(params int[] energyBarSettingArray)
        {
            stageSettingContent.FeverEnergyPhaseSettings.Returns(energyBarSettingArray);
        }

        private void GivenFeverEnergyDecrease(int feverEnergyDecrease)
        {
            stageSettingContent.FeverEnergyDecrease.Returns(feverEnergyDecrease);
        }

        private void GivenFeverEnergyIncrease(int feverEnergyIncrease)
        {
            stageSettingContent.FeverEnergyIncrease.Returns(feverEnergyIncrease);
        }

        private void GivenBeatAccuracyResult(BeatTimingDirection beatTimingDirection, float accuracyValue)
        {
            beaterModel.DetectBeatAccuracyCurrentTime().Returns(new BeatAccuracyResult(accuracyValue, beatTimingDirection));
        }

        private void GivenAccuracyPassThreshold(float accuracyPassThreshold)
        {
            gameSetting.AccuracyPassThreshold.Returns(accuracyPassThreshold);
        }

        private void CallHalfBeatEvent()
        {
            halfBeatEventCallback.Invoke(new HalfBeatEvent());
        }

        private void CurrentFeverStageShouldBe(int expectedFeverStage)
        {
            Assert.AreEqual(expectedFeverStage, feverEnergyBarModel.CurrentFeverStage);
        }

        private void EnergyValueShouldBe(int expectedEnergyValue)
        {
            Assert.AreEqual(expectedEnergyValue, feverEnergyBarModel.EnergyValue);
        }

        private void LastUpdateFeverStageEventShouldBe(int expectedFeverStage)
        {
            int newFeverStage = (int)updateFeverStageEventCallback.ReceivedCalls().Last().GetArguments()[0];
            Assert.AreEqual(expectedFeverStage, newFeverStage);
        }

        private void LastUpdateFeverEnergyBarEventShouldBe(int expectedBeforeEnergyValue, int expectedAfterEnergyValue)
        {
            UpdateFeverEnergyBarEvent eventInfo = (UpdateFeverEnergyBarEvent)updateFeverEnergyBarEventCallback.ReceivedCalls().Last().GetArguments()[0];
            Assert.AreEqual(expectedBeforeEnergyValue, eventInfo.BeforeEnergyValue);
            Assert.AreEqual(expectedAfterEnergyValue, eventInfo.AfterEnergyValue);
        }

        private void ShouldNotSendUpdateFeverEnergyBarEvent()
        {
            updateFeverEnergyBarEventCallback.DidNotReceive().Invoke(Arg.Any<UpdateFeverEnergyBarEvent>());
        }

        private void ShouldNotSendUpdateFeverStageEvent()
        {
            updateFeverStageEventCallback.DidNotReceive().Invoke(Arg.Any<int>());
        }

        private void ShouldSendUpdateFeverStageEvent(int expectedCallTimes)
        {
            updateFeverStageEventCallback.Received(expectedCallTimes).Invoke(Arg.Any<int>());
        }

        private void ShouldSendUpdateFeverEnergyBarEvent(int expectedCallTimes)
        {
            updateFeverEnergyBarEventCallback.Received(expectedCallTimes).Invoke(Arg.Any<UpdateFeverEnergyBarEvent>());
        }

        #region 初始化

        [Test]
        //初始化時, 能量條為0
        public void energy_bar_is_zero_when_init()
        {
            EnergyValueShouldBe(0);
        }

        [Test]
        //初始化時, Fever階段為第0階
        public void fever_stage_is_zero_when_init()
        {
            CurrentFeverStageShouldBe(0);
        }

        #endregion

        #region 打擊拍點

        [Test]
        [TestCase(BeatTimingDirection.Early, 0.7f)]
        [TestCase(BeatTimingDirection.Early, 1)]
        [TestCase(BeatTimingDirection.Late, 1)]
        [TestCase(BeatTimingDirection.Late, 0.7f)]
        //打擊節拍點時, 若符合準度條件, 則能量條增加
        public void energy_bar_increase_when_hit_beat_in_accuracy(BeatTimingDirection beatTimingDirection, float accuracyValue)
        {
            GivenAccuracyPassThreshold(0.3f);
            GivenFeverEnergyIncrease(5);

            GivenBeatAccuracyResult(beatTimingDirection, accuracyValue);
            feverEnergyBarModel.Hit();
            EnergyValueShouldBe(5);
        }

        [Test]
        //能量條增加到滿時, 不會再增加
        public void energy_bar_not_increase_when_energy_is_full()
        {
            GivenAccuracyPassThreshold(0.3f);
            GivenBeatAccuracyResult(BeatTimingDirection.Early, 1);
            GivenFeverEnergyIncrease(25);
            GivenFeverEnergyBarSetting(10, 15, 25);

            feverEnergyBarModel.Hit();
            EnergyValueShouldBe(25);

            feverEnergyBarModel.Hit();
            EnergyValueShouldBe(50);

            feverEnergyBarModel.Hit();
            EnergyValueShouldBe(50);
        }

        [Test]
        [TestCase(BeatTimingDirection.Early, 0.69f)]
        [TestCase(BeatTimingDirection.Early, 0)]
        [TestCase(BeatTimingDirection.Late, 0)]
        [TestCase(BeatTimingDirection.Late, 0.69f)]
        //打擊拍點時, 若不符合準度條件, 則能量條減少
        public void energy_bar_decrease_when_hit_beat_not_in_accuracy(BeatTimingDirection beatTimingDirection, float accuracyValue)
        {
            GivenAccuracyPassThreshold(0.3f);
            GivenFeverEnergyIncrease(10);
            GivenFeverEnergyDecrease(2);

            GivenBeatAccuracyResult(BeatTimingDirection.Early, 0.8f);
            feverEnergyBarModel.Hit();
            EnergyValueShouldBe(10);

            GivenBeatAccuracyResult(beatTimingDirection, accuracyValue);
            feverEnergyBarModel.Hit();
            EnergyValueShouldBe(8);
        }

        [Test]
        //能量條減少到0時, 不會再減少
        public void energy_bar_not_decrease_when_energy_is_zero()
        {
            GivenAccuracyPassThreshold(0.3f);
            GivenFeverEnergyIncrease(10);
            GivenFeverEnergyDecrease(5);

            GivenBeatAccuracyResult(BeatTimingDirection.Early, 0.8f);
            feverEnergyBarModel.Hit();
            EnergyValueShouldBe(10);

            GivenBeatAccuracyResult(BeatTimingDirection.Early, 0.1f);
            feverEnergyBarModel.Hit();
            EnergyValueShouldBe(5);

            GivenBeatAccuracyResult(BeatTimingDirection.Early, 0.1f);
            feverEnergyBarModel.Hit();
            EnergyValueShouldBe(0);

            GivenBeatAccuracyResult(BeatTimingDirection.Early, 0.1f);
            feverEnergyBarModel.Hit();
            EnergyValueShouldBe(0);
        }

        #endregion

        #region 漏拍處理

        [Test]
        //每次打擊後, 經過一次半拍, 能量條不變
        public void energy_bar_not_change_when_pass_half_beat()
        {
            GivenFeverEnergyIncrease(50);
            GivenFeverEnergyDecrease(10);

            GivenBeatAccuracyResult(BeatTimingDirection.Early, 1);
            feverEnergyBarModel.Hit();
            EnergyValueShouldBe(50);

            CallHalfBeatEvent();

            EnergyValueShouldBe(50);
        }

        [Test]
        //每次打擊後, 經過兩次半拍, 能量條減少
        public void energy_bar_decrease_when_pass_two_half_beat()
        {
            GivenFeverEnergyIncrease(50);
            GivenFeverEnergyDecrease(10);

            GivenBeatAccuracyResult(BeatTimingDirection.Early, 1);
            feverEnergyBarModel.Hit();
            EnergyValueShouldBe(50);

            CallHalfBeatEvent();
            CallHalfBeatEvent();

            EnergyValueShouldBe(40);
        }

        [Test]
        //連續Miss(兩次半拍扣能量條), 後續每次半拍能量條都會再次減少
        public void energy_bar_decrease_when_pass_half_beat_everytime_after_two_miss()
        {
            GivenFeverEnergyIncrease(50);
            GivenFeverEnergyDecrease(10);

            GivenBeatAccuracyResult(BeatTimingDirection.Early, 1);
            feverEnergyBarModel.Hit();
            EnergyValueShouldBe(50);

            CallHalfBeatEvent();
            CallHalfBeatEvent();
            EnergyValueShouldBe(40);

            CallHalfBeatEvent();
            EnergyValueShouldBe(30);

            CallHalfBeatEvent();
            EnergyValueShouldBe(20);
        }

        [Test]
        //連續Miss(兩次半拍扣能量條), 若有正確打擊則下次經過半拍不會扣能量條
        public void energy_bar_not_decrease_when_hit_correct_beat_after_two_miss()
        {
            GivenFeverEnergyIncrease(100);
            GivenFeverEnergyDecrease(10);

            GivenBeatAccuracyResult(BeatTimingDirection.Early, 1);
            feverEnergyBarModel.Hit();
            EnergyValueShouldBe(100);

            CallHalfBeatEvent();
            CallHalfBeatEvent();
            EnergyValueShouldBe(90);

            GivenBeatAccuracyResult(BeatTimingDirection.Early, 1);
            feverEnergyBarModel.Hit();
            EnergyValueShouldBe(100);

            CallHalfBeatEvent();
            EnergyValueShouldBe(100);
        }

        [Test]
        //連續Miss(兩次半拍扣能量條), 若有錯誤打擊則下次經過半拍仍會扣能量條
        public void energy_bar_decrease_when_hit_wrong_beat_after_two_miss()
        {
            GivenFeverEnergyIncrease(100);
            GivenFeverEnergyDecrease(10);

            GivenBeatAccuracyResult(BeatTimingDirection.Early, 1);
            feverEnergyBarModel.Hit();
            EnergyValueShouldBe(100);

            CallHalfBeatEvent();
            CallHalfBeatEvent();
            EnergyValueShouldBe(90);

            GivenBeatAccuracyResult(BeatTimingDirection.Late, 0);
            feverEnergyBarModel.Hit();
            EnergyValueShouldBe(80);

            CallHalfBeatEvent();
            EnergyValueShouldBe(70);
        }

        #endregion

        #region Fever階段

        [Test]
        //能量條增加時若跨越Fever階段門檻, 則更新Fever階段
        public void update_fever_stage_when_energy_value_add_to_cross_fever_threshold()
        {
            GivenFeverEnergyIncrease(9);
            GivenFeverEnergyDecrease(5);
            GivenFeverEnergyBarSetting(10, 20, 30);
            GivenBeatAccuracyResult(BeatTimingDirection.Early, 1);

            feverEnergyBarModel.Hit();

            EnergyValueShouldBe(9);
            CurrentFeverStageShouldBe(0);

            feverEnergyBarModel.Hit();

            EnergyValueShouldBe(18);
            CurrentFeverStageShouldBe(1);

            feverEnergyBarModel.Hit();

            EnergyValueShouldBe(27);
            CurrentFeverStageShouldBe(1);

            feverEnergyBarModel.Hit();

            EnergyValueShouldBe(36);
            CurrentFeverStageShouldBe(2);
        }

        [Test]
        //能量條減少時若跨越Fever階段門檻, 則更新Fever階段
        public void update_fever_stage_when_energy_value_subtract_to_cross_fever_threshold()
        {
            GivenFeverEnergyIncrease(15);
            GivenFeverEnergyDecrease(5);
            GivenFeverEnergyBarSetting(5, 5, 5);
            GivenBeatAccuracyResult(BeatTimingDirection.Early, 1);

            feverEnergyBarModel.Hit();

            EnergyValueShouldBe(15);
            CurrentFeverStageShouldBe(2);

            CallHalfBeatEvent();
            CallHalfBeatEvent();

            EnergyValueShouldBe(10);
            CurrentFeverStageShouldBe(1);

            CallHalfBeatEvent();

            EnergyValueShouldBe(5);
            CurrentFeverStageShouldBe(0);
        }

        #endregion

        #region 事件發送

        [Test]
        //初始化時, 會發送能量條更新事件
        public void send_energy_bar_update_event_when_init()
        {
            LastUpdateFeverEnergyBarEventShouldBe(0, 0);
        }

        [Test]
        //初始化時, 會發送Fever階段更新事件
        public void send_fever_stage_update_event_when_init()
        {
            LastUpdateFeverStageEventShouldBe(0);
        }

        [Test]
        //能量條增減時, 會發送能量條更新事件
        public void send_energy_bar_update_event_when_energy_value_change()
        {
            GivenFeverEnergyIncrease(10);
            GivenFeverEnergyDecrease(5);
            GivenAccuracyPassThreshold(0.5f);
            GivenBeatAccuracyResult(BeatTimingDirection.Early, 1);

            feverEnergyBarModel.Hit();
            LastUpdateFeverEnergyBarEventShouldBe(0, 10);

            feverEnergyBarModel.Hit();
            LastUpdateFeverEnergyBarEventShouldBe(10, 20);

            GivenBeatAccuracyResult(BeatTimingDirection.Early, 0.3f);
            feverEnergyBarModel.Hit();
            LastUpdateFeverEnergyBarEventShouldBe(20, 15);
        }

        [Test]
        [TestCase(6, 1)]
        [TestCase(15, 1)]
        [TestCase(16, 2)]
        [TestCase(30, 2)]
        [TestCase(50, 2)]
        //Fever階段升階時, 會發送Fever階段更新事件
        public void send_fever_stage_update_event_when_increase_fever_stage(int feverEnergyIncrease, int expectedFeverStage)
        {
            GivenFeverEnergyIncrease(feverEnergyIncrease);
            GivenFeverEnergyDecrease(5);
            GivenFeverEnergyBarSetting(5, 10, 15);
            GivenBeatAccuracyResult(BeatTimingDirection.Early, 1);

            feverEnergyBarModel.Hit();

            LastUpdateFeverStageEventShouldBe(expectedFeverStage);
        }

        [Test]
        [TestCase(20, 2)]
        [TestCase(21, 2)]
        [TestCase(35, 1)]
        [TestCase(45, 0)]
        [TestCase(50, 0)]
        [TestCase(100, 0)]
        //Fever階段降階時, 會發送Fever階段更新事件
        public void send_fever_stage_update_event_when_decrease_fever_stage(int feverEnergyDecrease, int expectedFeverStage)
        {
            GivenFeverEnergyIncrease(50);
            GivenFeverEnergyDecrease(feverEnergyDecrease);
            GivenFeverEnergyBarSetting(5, 10, 15, 20);
            GivenBeatAccuracyResult(BeatTimingDirection.Early, 1);

            feverEnergyBarModel.Hit();

            CurrentFeverStageShouldBe(3);

            CallHalfBeatEvent();
            CallHalfBeatEvent();

            LastUpdateFeverStageEventShouldBe(expectedFeverStage);
        }

        [Test]
        [TestCase(1)]
        [TestCase(0.8f)]
        [TestCase(0.1f)]
        //打擊拍點但能量條未增減時, 不會發送能量條更新事件
        public void not_send_energy_bar_update_event_when_hit_beat_but_energy_value_not_change(float hitAccuracyValue)
        {
            ShouldSendUpdateFeverEnergyBarEvent(1);

            GivenFeverEnergyIncrease(0);
            GivenFeverEnergyDecrease(5);
            GivenAccuracyPassThreshold(0.3f);
            GivenBeatAccuracyResult(BeatTimingDirection.Early, hitAccuracyValue);

            feverEnergyBarModel.Hit();

            ShouldSendUpdateFeverEnergyBarEvent(1);
        }

        [Test]
        //打擊拍點但Fever階段未更新時, 不會發送Fever階段更新事件
        public void not_send_fever_stage_update_event_when_hit_beat_but_fever_stage_not_change()
        {
            ShouldSendUpdateFeverStageEvent(1);

            GivenFeverEnergyIncrease(1);
            GivenFeverEnergyDecrease(5);
            GivenFeverEnergyBarSetting(5, 10, 15);
            GivenBeatAccuracyResult(BeatTimingDirection.Early, 1);

            feverEnergyBarModel.Hit();

            ShouldSendUpdateFeverStageEvent(1);
        }

        #endregion
    }
}