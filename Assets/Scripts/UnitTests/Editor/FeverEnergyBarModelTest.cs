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


            beaterModel = Substitute.For<IBeaterModel>();
            Container.Bind<IBeaterModel>().FromInstance(beaterModel).AsSingle();

            gameSetting = Substitute.For<IGameSetting>();
            Container.Bind<IGameSetting>().FromInstance(gameSetting).AsSingle();

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
            GivenBeatAccuracyResult(beatTimingDirection, accuracyValue);
            GivenFeverEnergyIncrease(5);

            feverEnergyBarModel.HitBeat();
            EnergyValueShouldBe(5);
        }

        private void GivenFeverEnergyIncrease(float feverEnergyIncrease)
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

        //能量條增加到滿時, 不會再增加
        //打擊拍點時, 若不符合準度條件, 則能量條減少
        //節拍事件發生時但沒有打擊時, 能量條減少
        //能量條減少到0時, 不會再減少
        //能量條增減時若跨越Fever階段門檻, 則更新Fever狀態
    }
}