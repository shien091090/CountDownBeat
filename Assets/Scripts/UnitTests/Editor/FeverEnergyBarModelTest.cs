using NSubstitute;
using NUnit.Framework;
using Zenject;

namespace GameCore.UnitTests
{
    [TestFixture]
    public class FeverEnergyBarModelTest : ZenjectUnitTestFixture
    {
        private FeverEnergyBarModel feverEnergyBarModel;
        private IBeaterModel beaterModel;
        private IGameSetting gameSetting;

        [SetUp]
        public override void Setup()
        {
            base.Setup();

            InitBeaterModelMock();
            InitGameSettingMock();

            Container.Bind<FeverEnergyBarModel>().AsSingle();
            feverEnergyBarModel = Container.Resolve<FeverEnergyBarModel>();
        }

        [Test]
        //初始化時, 能量條為0
        public void energy_bar_is_zero_when_init()
        {
            EnergyValueShouldBe(0);
        }

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

        //節拍事件發生時但沒有打擊時, 能量條減少
        //能量條增減時若跨越Fever階段門檻, 則更新Fever狀態

        private void InitGameSettingMock()
        {
            gameSetting = Substitute.For<IGameSetting>();

            Container.Bind<IGameSetting>().FromInstance(gameSetting).AsSingle();

            GivenFeverEnergyBarSetting(100);
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

        private void EnergyValueShouldBe(object expectedEnergyValue)
        {
            Assert.AreEqual(expectedEnergyValue, feverEnergyBarModel.EnergyValue);
        }
    }
}