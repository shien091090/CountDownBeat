using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;

namespace GameCore.UnitTests
{
    public class ScoreBallFlagChangeScriptableObjectTest
    {
        private ScoreBallFlagChangeScriptableObject scoreBallFlagChangeScriptableObject;

        [SetUp]
        public void Setup()
        {
            scoreBallFlagChangeScriptableObject = ScriptableObject.CreateInstance<ScoreBallFlagChangeScriptableObject>();
        }

        [Test]
        //原旗標+新旗標判斷通過, 輸出旗標結果
        public void change_old_flag_by_new_flag_to_result_success()
        {
            List<FlagChangeDefine> defineList = new List<FlagChangeDefine>
            {
                new FlagChangeDefine(5, 10, 110)
            };

            scoreBallFlagChangeScriptableObject.SetDefineList(defineList);
            FlagChangeResult resultInfo = scoreBallFlagChangeScriptableObject.GetChangeFlagNumberInfo(5, 10);

            Assert.AreEqual(true, resultInfo.IsChangeSuccess);
            Assert.AreEqual(110, resultInfo.FinalFlagNum);
        }

        //原旗標+新旗標判斷不通過, 輸出失敗
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