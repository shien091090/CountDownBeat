using System;
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

        private Action<HalfBeatEvent> halfBeatEventCallback;

        [SetUp]
        public override void Setup()
        {
            base.Setup();

            InitBeaterModelMock();
            InitGameSettingMock();
            InitEventRegisterMock();

            Container.Bind<FeverEnergyBarModel>().AsSingle();
            feverEnergyBarModel = Container.Resolve<FeverEnergyBarModel>();
            feverEnergyBarModel.Init();
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
            gameSetting.FeverEnergyBarSetting.Returns(energyBarSettingArray);
        }

        private void GivenFeverEnergyDecrease(int feverEnergyDecrease)
        {
            gameSetting.FeverEnergyDecrease.Returns(feverEnergyDecrease);
        }

        private void GivenFeverEnergyIncrease(int feverEnergyIncrease)
        {
            gameSetting.FeverEnergyIncrease.Returns(feverEnergyIncrease);
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

        private void EnergyValueShouldBe(object expectedEnergyValue)
        {
            Assert.AreEqual(expectedEnergyValue, feverEnergyBarModel.EnergyValue);
        }

        #region 初始化

        [Test]
        //初始化時, 能量條為0
        public void energy_bar_is_zero_when_init()
        {
            EnergyValueShouldBe(0);
        }

        [Test]
        //初始化時, Fever階段為第一階
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
            feverEnergyBarModel.HitBeat();
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

            feverEnergyBarModel.HitBeat();
            EnergyValueShouldBe(25);

            feverEnergyBarModel.HitBeat();
            EnergyValueShouldBe(50);

            feverEnergyBarModel.HitBeat();
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
            feverEnergyBarModel.HitBeat();
            EnergyValueShouldBe(10);

            GivenBeatAccuracyResult(beatTimingDirection, accuracyValue);
            feverEnergyBarModel.HitBeat();
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
            feverEnergyBarModel.HitBeat();
            EnergyValueShouldBe(10);

            GivenBeatAccuracyResult(BeatTimingDirection.Early, 0.1f);
            feverEnergyBarModel.HitBeat();
            EnergyValueShouldBe(5);

            GivenBeatAccuracyResult(BeatTimingDirection.Early, 0.1f);
            feverEnergyBarModel.HitBeat();
            EnergyValueShouldBe(0);

            GivenBeatAccuracyResult(BeatTimingDirection.Early, 0.1f);
            feverEnergyBarModel.HitBeat();
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
            feverEnergyBarModel.HitBeat();
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
            feverEnergyBarModel.HitBeat();
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
            feverEnergyBarModel.HitBeat();
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
        public void energy_bar_not_decrease_when_hit_beat_after_two_miss()
        {
            GivenFeverEnergyIncrease(100);
            GivenFeverEnergyDecrease(10);

            GivenBeatAccuracyResult(BeatTimingDirection.Early, 1);
            feverEnergyBarModel.HitBeat();
            EnergyValueShouldBe(100);

            CallHalfBeatEvent();
            CallHalfBeatEvent();
            EnergyValueShouldBe(90);

            GivenBeatAccuracyResult(BeatTimingDirection.Early, 1);
            feverEnergyBarModel.HitBeat();
            EnergyValueShouldBe(100);

            CallHalfBeatEvent();
            EnergyValueShouldBe(90);
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

            feverEnergyBarModel.HitBeat();

            EnergyValueShouldBe(9);
            CurrentFeverStageShouldBe(0);

            feverEnergyBarModel.HitBeat();

            EnergyValueShouldBe(18);
            CurrentFeverStageShouldBe(1);

            feverEnergyBarModel.HitBeat();

            EnergyValueShouldBe(27);
            CurrentFeverStageShouldBe(1);

            feverEnergyBarModel.HitBeat();

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

            feverEnergyBarModel.HitBeat();

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
    }
}