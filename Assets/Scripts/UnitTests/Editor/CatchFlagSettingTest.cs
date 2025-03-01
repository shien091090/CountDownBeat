using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;

namespace GameCore.UnitTests
{
    public class CatchFlagSettingTest
    {
        private CatchFlagSetting flagSetting;
        private List<CatchFlagDefine> flagDefineList;

        [SetUp]
        public void Setup()
        {
            flagDefineList = new List<CatchFlagDefine>();
            flagSetting = new CatchFlagSetting();
        }

        [Test]
        //指定旗標融合判斷通過, 輸出旗標結果
        public void specific_flag_specific_type_merge_success()
        {
            GivenCatchFlagSetting(5,
                (TriggerFlagMergingType.DirectionWall_DownToUp, 110),
                (TriggerFlagMergingType.DirectionWall_UpToDown, 120),
                (TriggerFlagMergingType.DirectionWall_LeftToRight, 130),
                (TriggerFlagMergingType.DirectionWall_RightToLeft, 140));

            CatchFlagMergeResult resultInfo = flagSetting.GetCatchFlagMergeResult(5, TriggerFlagMergingType.DirectionWall_LeftToRight);
            ShouldFlagMergeSuccess(resultInfo, 130);
        }

        [Test]
        //指定旗標指定類型融合判斷不通過, 輸出失敗
        public void specific_flag_specific_type_merge_fail()
        {
            CatchFlagMergeResult resultInfo = flagSetting.GetCatchFlagMergeResult(5, TriggerFlagMergingType.DirectionWall_LeftToRight);
            ShouldFlagMergeFailed(resultInfo);
        }

        [Test]
        //指定旗標指定類型融合時有多個設定判斷通過, 輸出第一個設定的旗標結果
        public void specific_flag_specific_type_merge_success_by_multiple_settings()
        {
            GivenCatchFlagSetting(7, (TriggerFlagMergingType.DirectionWall_LeftToRight, 110));
            GivenCatchFlagSetting(7, (TriggerFlagMergingType.DirectionWall_LeftToRight, 120));

            CatchFlagMergeResult resultInfo = flagSetting.GetCatchFlagMergeResult(7, TriggerFlagMergingType.DirectionWall_LeftToRight);
            ShouldFlagMergeSuccess(resultInfo, 110);
        }

        [Test]
        [TestCase(1)]
        [TestCase(10)]
        [TestCase(110)]
        //任意旗標指定類型融合判斷通過, 輸出旗標結果
        public void any_flag_specific_type_merge_success(int flagNum)
        {
            GivenAnyFlagNumCatchFlagSetting((TriggerFlagMergingType.DirectionWall_LeftToRight, 110));

            CatchFlagMergeResult resultInfo = flagSetting.GetCatchFlagMergeResult(flagNum, TriggerFlagMergingType.DirectionWall_LeftToRight);
            ShouldFlagMergeSuccess(resultInfo, 110);
        }

        [Test]
        [TestCase(1)]
        [TestCase(10)]
        [TestCase(110)]
        //任意旗標指定類型融合時有多個設定判斷通過, 輸出第一個設定的旗標結果
        public void any_flag_specific_type_merge_success_by_multiple_settings(int flagNum)
        {
            GivenAnyFlagNumCatchFlagSetting((TriggerFlagMergingType.DirectionWall_LeftToRight, 14));
            GivenAnyFlagNumCatchFlagSetting((TriggerFlagMergingType.DirectionWall_LeftToRight, 12));
            GivenAnyFlagNumCatchFlagSetting((TriggerFlagMergingType.DirectionWall_LeftToRight, 13));

            CatchFlagMergeResult resultInfo = flagSetting.GetCatchFlagMergeResult(flagNum, TriggerFlagMergingType.DirectionWall_LeftToRight);
            ShouldFlagMergeSuccess(resultInfo, 14);
        }

        [Test]
        [TestCase(TriggerFlagMergingType.DirectionWall_LeftToRight)]
        [TestCase(TriggerFlagMergingType.DirectionWall_DownToUp)]
        [TestCase(TriggerFlagMergingType.DirectionWall_UpToDown)]
        //指定旗標不限類型融合判斷通過, 輸出旗標結果
        public void specific_flag_any_type_merge_success(TriggerFlagMergingType triggerFlagMergingType)
        {
            GivenCatchFlagSetting(5, (TriggerFlagMergingType.Any, 15));

            CatchFlagMergeResult resultInfo = flagSetting.GetCatchFlagMergeResult(5, triggerFlagMergingType);
            ShouldFlagMergeSuccess(resultInfo, 15);
        }

        [Test]
        [TestCase(TriggerFlagMergingType.DirectionWall_LeftToRight)]
        [TestCase(TriggerFlagMergingType.DirectionWall_DownToUp)]
        [TestCase(TriggerFlagMergingType.DirectionWall_UpToDown)]
        //指定旗標不限類型融合時有多個設定判斷通過, 輸出第一個設定的旗標結果
        public void specific_flag_any_type_merge_success_by_multiple_settings(TriggerFlagMergingType triggerFlagMergingType)
        {
            GivenCatchFlagSetting(2, (TriggerFlagMergingType.Any, 165));
            GivenCatchFlagSetting(2, (TriggerFlagMergingType.Any, 164));
            GivenCatchFlagSetting(2, (TriggerFlagMergingType.Any, 170));

            CatchFlagMergeResult resultInfo = flagSetting.GetCatchFlagMergeResult(2, triggerFlagMergingType);
            ShouldFlagMergeSuccess(resultInfo, 165);
        }

        [Test]
        [TestCase(1, TriggerFlagMergingType.DirectionWall_LeftToRight)]
        [TestCase(2, TriggerFlagMergingType.DirectionWall_DownToUp)]
        [TestCase(3, TriggerFlagMergingType.DirectionWall_RightToLeft)]
        //任意旗標不限類型融合判斷通過, 輸出旗標結果
        public void any_flag_any_type_merge_success(int flagNum, TriggerFlagMergingType triggerFlagMergingType)
        {
            GivenAnyFlagNumCatchFlagSetting((TriggerFlagMergingType.Any, 15));

            CatchFlagMergeResult resultInfo = flagSetting.GetCatchFlagMergeResult(flagNum, triggerFlagMergingType);
            ShouldFlagMergeSuccess(resultInfo, 15);
        }

        [Test]
        [TestCase(1, TriggerFlagMergingType.DirectionWall_LeftToRight)]
        [TestCase(2, TriggerFlagMergingType.DirectionWall_DownToUp)]
        [TestCase(3, TriggerFlagMergingType.DirectionWall_RightToLeft)]
        //任意旗標不限類型融合時有多個設定判斷通過, 輸出第一個設定的旗標結果
        public void any_flag_any_type_merge_success_by_multiple_settings(int flagNum, TriggerFlagMergingType triggerFlagMergingType)
        {
            GivenAnyFlagNumCatchFlagSetting((TriggerFlagMergingType.Any, 15));
            GivenAnyFlagNumCatchFlagSetting((TriggerFlagMergingType.Any, 14));
            GivenAnyFlagNumCatchFlagSetting((TriggerFlagMergingType.Any, 13));

            CatchFlagMergeResult resultInfo = flagSetting.GetCatchFlagMergeResult(flagNum, triggerFlagMergingType);
            ShouldFlagMergeSuccess(resultInfo, 15);
        }

        [Test]
        [TestCase(1, TriggerFlagMergingType.DirectionWall_LeftToRight)]
        [TestCase(5, TriggerFlagMergingType.DirectionWall_LeftToRight)]
        [TestCase(5, TriggerFlagMergingType.DirectionWall_UpToDown)]
        [TestCase(1, TriggerFlagMergingType.DirectionWall_UpToDown)]
        //若有多個設定判斷通過, 優先選擇任意旗標不限類型融合的設定
        public void use_any_flag_any_type_merge_setting_when_multiple_settings_check_success(int flagNum, TriggerFlagMergingType triggerFlagMergingType)
        {
            GivenCatchFlagSetting(5,
                (TriggerFlagMergingType.DirectionWall_LeftToRight, 11),
                (TriggerFlagMergingType.DirectionWall_DownToUp, 12),
                (TriggerFlagMergingType.Any, 13));

            GivenAnyFlagNumCatchFlagSetting(
                (TriggerFlagMergingType.DirectionWall_LeftToRight, 35),
                (TriggerFlagMergingType.DirectionWall_DownToUp, 36),
                (TriggerFlagMergingType.Any, 37));

            GivenCatchFlagSetting(1,
                (TriggerFlagMergingType.DirectionWall_LeftToRight, 21),
                (TriggerFlagMergingType.DirectionWall_DownToUp, 22),
                (TriggerFlagMergingType.Any, 23));

            CatchFlagMergeResult resultInfo = flagSetting.GetCatchFlagMergeResult(flagNum, triggerFlagMergingType);
            ShouldFlagMergeSuccess(resultInfo, 37);
        }

        [Test]
        [TestCase(1, TriggerFlagMergingType.DirectionWall_LeftToRight)]
        [TestCase(5, TriggerFlagMergingType.DirectionWall_LeftToRight)]
        [TestCase(5, TriggerFlagMergingType.DirectionWall_UpToDown)]
        [TestCase(1, TriggerFlagMergingType.DirectionWall_UpToDown)]
        //若有多個設定判斷通過, 沒有任意旗標不限類型融合的設定時, 次優先選擇指定旗標不限融合類型的設定
        public void use_specific_flag_any_type_merge_setting_when_multiple_settings_check_success(int flagNum, TriggerFlagMergingType triggerFlagMergingType)
        {
            GivenCatchFlagSetting(5,
                (TriggerFlagMergingType.DirectionWall_LeftToRight, 11),
                (TriggerFlagMergingType.DirectionWall_DownToUp, 12),
                (TriggerFlagMergingType.Any, 100));

            GivenAnyFlagNumCatchFlagSetting(
                (TriggerFlagMergingType.DirectionWall_LeftToRight, 35),
                (TriggerFlagMergingType.DirectionWall_DownToUp, 36));

            GivenCatchFlagSetting(1,
                (TriggerFlagMergingType.DirectionWall_LeftToRight, 21),
                (TriggerFlagMergingType.DirectionWall_DownToUp, 22),
                (TriggerFlagMergingType.Any, 100));

            CatchFlagMergeResult resultInfo = flagSetting.GetCatchFlagMergeResult(flagNum, triggerFlagMergingType);
            ShouldFlagMergeSuccess(resultInfo, 100);
        }

        [Test]
        [TestCase(1, TriggerFlagMergingType.DirectionWall_LeftToRight, 35)]
        [TestCase(5, TriggerFlagMergingType.DirectionWall_LeftToRight, 35)]
        [TestCase(5, TriggerFlagMergingType.DirectionWall_DownToUp, 36)]
        [TestCase(1, TriggerFlagMergingType.DirectionWall_DownToUp, 36)]
        //若有多個設定判斷通過, 沒有任意旗標不限類型融合 或 指定旗標不限融合類型的設定時, 則次次優先選擇任意旗標指定類型融合的設定
        public void use_any_flag_specific_type_merge_setting_when_multiple_settings_check_success(int flagNum, TriggerFlagMergingType triggerFlagMergingType,
            int expectedResultFlagNum)
        {
            GivenCatchFlagSetting(5,
                (TriggerFlagMergingType.DirectionWall_LeftToRight, 11),
                (TriggerFlagMergingType.DirectionWall_DownToUp, 12));

            GivenAnyFlagNumCatchFlagSetting(
                (TriggerFlagMergingType.DirectionWall_LeftToRight, 35),
                (TriggerFlagMergingType.DirectionWall_DownToUp, 36));

            GivenCatchFlagSetting(1,
                (TriggerFlagMergingType.DirectionWall_LeftToRight, 21),
                (TriggerFlagMergingType.DirectionWall_DownToUp, 22));

            CatchFlagMergeResult resultInfo = flagSetting.GetCatchFlagMergeResult(flagNum, triggerFlagMergingType);
            ShouldFlagMergeSuccess(resultInfo, expectedResultFlagNum);
        }

        [Test]
        [TestCase(5, TriggerFlagMergingType.DirectionWall_LeftToRight, 11)]
        [TestCase(5, TriggerFlagMergingType.DirectionWall_DownToUp, 12)]
        [TestCase(1, TriggerFlagMergingType.DirectionWall_LeftToRight, 21)]
        [TestCase(1, TriggerFlagMergingType.DirectionWall_DownToUp, 22)]
        //若有多個設定判斷通過, 沒有任意旗標不限類型融合 或 指定旗標不限融合類型 或 任意旗標指定類型融合的設定時, 最後選擇指定旗標指定類型融合
        public void use_specific_flag_specific_type_merge_setting_when_multiple_settings_check_success(int flagNum, TriggerFlagMergingType triggerFlagMergingType,
            int expectedResultFlagNum)
        {
            GivenCatchFlagSetting(5,
                (TriggerFlagMergingType.DirectionWall_LeftToRight, 11),
                (TriggerFlagMergingType.DirectionWall_DownToUp, 12));

            GivenCatchFlagSetting(1,
                (TriggerFlagMergingType.DirectionWall_LeftToRight, 21),
                (TriggerFlagMergingType.DirectionWall_DownToUp, 22));

            CatchFlagMergeResult resultInfo = flagSetting.GetCatchFlagMergeResult(flagNum, triggerFlagMergingType);
            ShouldFlagMergeSuccess(resultInfo, expectedResultFlagNum);
        }

        private void GivenCatchFlagSetting(int flagNum, params (TriggerFlagMergingType triggerFlagChangingType, int resultFlagNum)[] mergeDefines)
        {
            AddFlagDefineList(flagNum, mergeDefines);
        }

        private void GivenAnyFlagNumCatchFlagSetting(params (TriggerFlagMergingType DirectionWall_LeftToRight, int)[] mergeDefines)
        {
            AddFlagDefineList(mergeDefines: mergeDefines);
        }

        private void ShouldFlagMergeSuccess(CatchFlagMergeResult resultInfo, int expectedResultFlagNum)
        {
            Assert.AreEqual(true, resultInfo.IsMergeSuccess);
            Assert.AreEqual(expectedResultFlagNum, resultInfo.ResultFlagNum);
        }

        private void ShouldFlagMergeFailed(CatchFlagMergeResult resultInfo)
        {
            Assert.AreEqual(false, resultInfo.IsMergeSuccess);
        }

        private void AddFlagDefineList(int flagNum = -1, params (TriggerFlagMergingType DirectionWall_LeftToRight, int)[] mergeDefines)
        {
            if (flagDefineList == null)
                flagDefineList = new List<CatchFlagDefine>();

            CatchFlagDefine newFlagDefine = flagNum >= 0 ?
                CatchFlagDefine.CreateSpecificFlagInstance(flagNum) :
                CatchFlagDefine.CreateAnyFlagInstance();

            foreach ((TriggerFlagMergingType triggerFlagChangingType, int resultFlagNum) mergeDefine in mergeDefines)
            {
                newFlagDefine.AddCatchFlagMergeDefine(mergeDefine.triggerFlagChangingType, mergeDefine.resultFlagNum);
            }

            flagDefineList.Add(newFlagDefine);
            flagSetting.SetDefineList(flagDefineList);
        }
    }
}