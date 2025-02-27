using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;

namespace GameCore.UnitTests
{
    public class ScoreBallFlagChangeSettingTest
    {
        private ScoreBallFlagChangeSetting flagChangeSetting;
        private List<FlagStartChangeDefine> flagChangeDefineList;

        [SetUp]
        public void Setup()
        {
            flagChangeDefineList = new List<FlagStartChangeDefine>();
            flagChangeSetting = new ScoreBallFlagChangeSetting();
        }

        [Test]
        //原旗標+新旗標判斷通過, 輸出旗標結果
        public void change_old_flag_by_new_flag_to_result_success()
        {
            GivenFlagChangeSetting(5, 10, 110);

            FlagChangeResult resultInfo = flagChangeSetting.GetChangeFlagNumberInfo(5, 10);
            ShouldFlagChangeSuccess(resultInfo, 110);
        }
        
        //原旗標+新旗標判斷不通過, 輸出失敗

        private void GivenFlagChangeSetting(int flagNum, int newFlagNum, int resultFlagNum)
        {
            if (flagChangeDefineList == null)
                flagChangeDefineList = new List<FlagStartChangeDefine>();

            FlagStartChangeDefine match = flagChangeDefineList.FirstOrDefault(x => x.FlagNum == flagNum);
            bool isMatch = match != null;

            FlagStartChangeDefine flagStartChangeDefine = isMatch ?
                match :
                new FlagStartChangeDefine(flagNum);

            flagStartChangeDefine.AddFlagChangeResultDefine(newFlagNum, resultFlagNum);

            if (isMatch == false)
                flagChangeDefineList.Add(flagStartChangeDefine);

            flagChangeSetting.SetSettingList(flagChangeDefineList);
        }

        private void ShouldFlagChangeSuccess(FlagChangeResult resultInfo, object expectedResultFlagNum)
        {
            Assert.AreEqual(true, resultInfo.IsChangeSuccess);
            Assert.AreEqual(expectedResultFlagNum, resultInfo.ResultFlagNum);
        }

        //原旗標+新旗標有多個設定判斷通過, 輸出第一個設定的旗標結果
        //任意旗標+新旗標判斷通過, 輸出旗標結果
        //任意旗標+新旗標判斷通過且有多個設定也判斷通過, 輸出第一個設定的旗標結果
        //原旗標+任意旗標判斷通過, 輸出旗標結果
        //原旗標+任意旗標判斷通過且有多個設定也判斷通過, 輸出第一個設定的旗標結果
        //任意旗標+任意旗標判斷通過, 輸出旗標結果
        //任意旗標+任意旗標判斷通過且有多個設定也判斷通過, 輸出第一個設定的旗標結果
        //若有多個設定判斷通過, 優先選擇任意旗標+任意旗標, 再者新旗標為任意旗標者, 再者原旗標為任意旗標者, 最後才是兩者皆為指定旗標者
    }
}