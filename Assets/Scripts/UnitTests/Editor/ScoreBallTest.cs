using System;
using System.Collections.Generic;
using System.Linq;
using NSubstitute;
using NUnit.Framework;
using SNShien.Common.ProcessTools;

namespace GameCore.UnitTests
{
    public class ScoreBallTest
    {
        private ScoreBall scoreBall;
        private IEventRegister eventRegister;
        private IEventInvoker eventInvoker;
        private IScoreBallPresenter presenter;
        private IGameSetting gameSetting;
        private IFeverEnergyBarModel feverEnergyBarModel;

        private Action<BeatEvent> beatEventCallback;
        private Action scoreBallBeatEventCallback;
        private Action<ScoreBallState> updateStateEventCallback;
        private Action initEventCallback;
        private Action<int> updateCountDownValueEventCallback;

        [SetUp]
        public void Setup()
        {
            InitEventHandlerMock();
            InitGameSettingMock();
            feverEnergyBarModel = Substitute.For<IFeverEnergyBarModel>();

            scoreBall = new ScoreBall(eventRegister, eventInvoker, gameSetting, feverEnergyBarModel);

            presenter = Substitute.For<IScoreBallPresenter>();
            scoreBall.BindPresenter(presenter);

            scoreBallBeatEventCallback = Substitute.For<Action>();
            scoreBall.OnScoreBallBeat += scoreBallBeatEventCallback;

            updateStateEventCallback = Substitute.For<Action<ScoreBallState>>();
            scoreBall.OnUpdateState += updateStateEventCallback;

            initEventCallback = Substitute.For<Action>();
            scoreBall.OnInit += initEventCallback;

            updateCountDownValueEventCallback = Substitute.For<Action<int>>();
            scoreBall.OnUpdateCountDownValue += updateCountDownValueEventCallback;
        }

        private void InitGameSettingMock()
        {
            gameSetting = Substitute.For<IGameSetting>();

            GivenScoreBallFlagWeightSetting(0, new Dictionary<int, int>
            {
                { 1, 1 }
            });
        }

        private void InitEventHandlerMock()
        {
            beatEventCallback = null;

            eventInvoker = Substitute.For<IEventInvoker>();
            eventRegister = Substitute.For<IEventRegister>();

            eventRegister.When(x => x.Register(Arg.Any<Action<BeatEvent>>())).Do(x =>
            {
                Action<BeatEvent> callback = (Action<BeatEvent>)x.Args()[0];
                beatEventCallback += callback;
            });

            eventRegister.When(x => x.Unregister(Arg.Any<Action<BeatEvent>>())).Do(x =>
            {
                Action<BeatEvent> callback = (Action<BeatEvent>)x.Args()[0];
                beatEventCallback -= callback;
            });
        }

        private void GivenStartCountDownValue(int startCountDownValue)
        {
            gameSetting.ScoreBallStartCountDownValue.Returns(startCountDownValue);
        }

        private void GivenCurrentFeverStage(int feverStage)
        {
            feverEnergyBarModel.CurrentFeverStage.Returns(feverStage);
        }

        private void GivenScoreBallFlagWeightSetting(int currentFeverStage, Dictionary<int, int> flagWeightSetting)
        {
            gameSetting.GetScoreBallFlagWeightSetting(currentFeverStage).Returns(flagWeightSetting);
        }

        private void GivenScoreBallFlagWeightSettingIsEmpty()
        {
            gameSetting.GetScoreBallFlagWeightSetting(Arg.Any<int>()).Returns(new Dictionary<int, int>());
        }

        private void CallBeatEventCallback(bool isCountDownBeat = true)
        {
            beatEventCallback?.Invoke(new BeatEvent(isCountDownBeat));
        }

        private void LastSendUpdateStateEventShouldBe(ScoreBallState expectedState)
        {
            ScoreBallState arg = (ScoreBallState)updateStateEventCallback.ReceivedCalls().Last().GetArguments()[0];
            Assert.AreEqual(expectedState, arg);
        }

        private void ShouldSendInitEvent(int expectedCallTimes)
        {
            if (expectedCallTimes == 0)
                initEventCallback.DidNotReceive().Invoke();
            else
                initEventCallback.Received(expectedCallTimes).Invoke();
        }

        private void ShouldSendScoreBallBeatEvent(int expectedCallTimes)
        {
            if (expectedCallTimes == 0)
                scoreBallBeatEventCallback.DidNotReceive().Invoke();
            else
                scoreBallBeatEventCallback.Received(expectedCallTimes).Invoke();
        }

        private void ShouldSendDamageEvent(int expectedCallTimes = 1)
        {
            eventInvoker.Received(expectedCallTimes).SendEvent(Arg.Any<DamageEvent>());
        }

        private void ShouldRegisterBeatEvent()
        {
            eventRegister.Received().Unregister(Arg.Any<Action<BeatEvent>>());
            eventRegister.Received().Register(Arg.Any<Action<BeatEvent>>());
        }

        private void ShouldRegisterHalfBeatEvent()
        {
            eventRegister.Received().Unregister(Arg.Any<Action<HalfBeatEvent>>());
            eventRegister.Received().Register(Arg.Any<Action<HalfBeatEvent>>());
        }

        private void CurrentStateShouldBe(ScoreBallState expectedState)
        {
            ScoreBallState currentState = scoreBall.CurrentState;
            Assert.AreEqual(expectedState, currentState);
        }

        private void CurrentCountDownValueShouldBe(int expectedCurrentCountDownValue)
        {
            int currentCountDownValue = scoreBall.CurrentCountDownValue;
            Assert.AreEqual(expectedCurrentCountDownValue, currentCountDownValue);
        }

        private void ShouldSendUpdateCountDownValueEvent(int expectedCallTimes)
        {
            if (expectedCallTimes == 0)
                updateCountDownValueEventCallback.DidNotReceive().Invoke(Arg.Any<int>());
            else
                updateCountDownValueEventCallback.Received(expectedCallTimes).Invoke(Arg.Any<int>());
        }

        private void CurrentFlagNumberShouldBe(int expectedFlagNumber)
        {
            Assert.AreEqual(expectedFlagNumber, scoreBall.CurrentFlagNumber);
        }

        #region 狀態變化

        [Test]
        //設定初始倒數數字並切換狀態為"InCountDown"
        public void init_then_change_state_to_in_count_down()
        {
            GivenStartCountDownValue(20);

            scoreBall.Init();

            CurrentCountDownValueShouldBe(20);
            CurrentStateShouldBe(ScoreBallState.InCountDown);
        }

        [Test]
        //收到Beat事件時, 倒數數字減至0, 切換狀態為"Hide"
        public void count_down_to_zero_then_change_state_to_hide()
        {
            GivenStartCountDownValue(1);

            scoreBall.Init();

            CallBeatEventCallback();

            CurrentCountDownValueShouldBe(0);
            CurrentStateShouldBe(ScoreBallState.Hide);
        }

        [Test]
        //設定凍結狀態並切換狀態為"Freeze"
        public void set_freeze_state_then_change_state_to_freeze()
        {
            GivenStartCountDownValue(20);

            scoreBall.Init();
            scoreBall.SetFreezeState(true);

            CurrentStateShouldBe(ScoreBallState.Freeze);
        }

        [Test]
        //取消凍結狀態並切換狀態為"InCountDown"
        public void unset_freeze_state_then_change_state_to_in_count_down()
        {
            GivenStartCountDownValue(10);

            scoreBall.Init();
            scoreBall.SetFreezeState(true);
            scoreBall.SetFreezeState(false);

            CurrentStateShouldBe(ScoreBallState.InCountDown);
        }

        [Test]
        //成功結算, 切換狀態為"Hide"
        public void success_settle_then_change_state_to_hide()
        {
            GivenStartCountDownValue(10);

            scoreBall.Init();
            scoreBall.SuccessSettle();

            CurrentCountDownValueShouldBe(0);
            CurrentStateShouldBe(ScoreBallState.Hide);
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        //結算或倒數完畢後再度激活, 狀態切換為"InCountDown"
        public void reactivate_then_change_state_to_in_count_down(bool isSuccessSettle)
        {
            GivenStartCountDownValue(10);

            scoreBall.Init();

            if (isSuccessSettle)
                scoreBall.SuccessSettle();
            else
            {
                for (int i = 0; i < 10; i++)
                {
                    CallBeatEventCallback();
                }
            }

            CurrentStateShouldBe(ScoreBallState.Hide);

            scoreBall.Reactivate();

            CurrentStateShouldBe(ScoreBallState.InCountDown);
        }

        #endregion

        #region 事件註冊&發送事件

        [Test]
        //設定初始化時, 監聽事件
        public void verify_register_event()
        {
            GivenStartCountDownValue(20);

            scoreBall.Init();

            ShouldRegisterBeatEvent();
            ShouldRegisterHalfBeatEvent();
        }

        [Test]
        //初始化時, 發送初始化事件
        public void send_init_event_when_init()
        {
            GivenStartCountDownValue(20);

            scoreBall.Init();

            ShouldSendInitEvent(1);
        }

        [Test]
        //更新倒數數字時, 發送倒數數字更新事件
        public void send_update_count_down_value_event_when_update_count_down_value()
        {
            GivenStartCountDownValue(20);

            scoreBall.Init();

            ShouldSendUpdateCountDownValueEvent(1);

            CallBeatEventCallback(true);

            ShouldSendUpdateCountDownValueEvent(2);

            CallBeatEventCallback(false);

            ShouldSendUpdateCountDownValueEvent(2);

            CallBeatEventCallback(true);

            ShouldSendUpdateCountDownValueEvent(3);
        }

        [Test]
        //分數球重新激活時, 發送Init事件
        public void send_score_ball_beat_event_when_reactivate()
        {
            GivenStartCountDownValue(20);

            scoreBall.Init();

            ShouldSendInitEvent(1);

            scoreBall.SuccessSettle();

            ShouldSendInitEvent(1);

            scoreBall.Reactivate();

            ShouldSendInitEvent(2);
        }

        [Test]
        //收到Beat事件時, 發送分數球Beat事件
        public void send_score_ball_beat_event_when_receive_beat_event()
        {
            GivenStartCountDownValue(20);

            scoreBall.Init();

            ShouldSendScoreBallBeatEvent(0);

            CallBeatEventCallback();

            ShouldSendScoreBallBeatEvent(1);

            CallBeatEventCallback();

            ShouldSendScoreBallBeatEvent(2);
        }

        [Test]
        //收到Beat事件時, 若狀態為"Hide"則不會發送分數球Beat事件
        public void do_not_send_score_ball_beat_event_when_hide()
        {
            GivenStartCountDownValue(20);

            scoreBall.Init();

            CallBeatEventCallback();

            ShouldSendScoreBallBeatEvent(1);

            scoreBall.SuccessSettle();
            CurrentStateShouldBe(ScoreBallState.Hide);

            CallBeatEventCallback();

            ShouldSendScoreBallBeatEvent(1);
        }

        [Test]
        //變更狀態時, 會發送變更狀態事件
        public void send_update_state_event_when_state_change()
        {
            GivenStartCountDownValue(10);

            scoreBall.Init();
            LastSendUpdateStateEventShouldBe(ScoreBallState.InCountDown);

            scoreBall.SetFreezeState(true);
            LastSendUpdateStateEventShouldBe(ScoreBallState.Freeze);

            scoreBall.SetFreezeState(false);
            LastSendUpdateStateEventShouldBe(ScoreBallState.InCountDown);

            scoreBall.SuccessSettle();
            LastSendUpdateStateEventShouldBe(ScoreBallState.Hide);
        }

        [Test]
        //收到Beat事件時, 倒數數字減至0, 發送Damage事件
        public void count_down_to_zero_and_send_damage_event()
        {
            GivenStartCountDownValue(1);

            scoreBall.Init();

            CallBeatEventCallback();

            CurrentCountDownValueShouldBe(0);
            ShouldSendDamageEvent();
        }

        #endregion

        #region 倒數數字

        [Test]
        [TestCase(0)]
        [TestCase(-1)]
        [TestCase(-10)]
        //設定初始倒數數字, 若數字小於0則不做事
        public void init_with_negative_value(int startCountDownValue)
        {
            GivenStartCountDownValue(startCountDownValue);

            scoreBall.Init();

            CurrentCountDownValueShouldBe(0);
            CurrentStateShouldBe(ScoreBallState.None);
        }

        [Test]
        //收到Beat事件時, 若為倒數拍點, 則倒數數字減一
        public void count_down_when_receive_beat_event_and_is_count_down_beat()
        {
            GivenStartCountDownValue(20);

            scoreBall.Init();

            CallBeatEventCallback(true);

            CurrentCountDownValueShouldBe(19);
        }

        [Test]
        //收到Beat事件時, 若不是倒數拍點, 則不做事
        public void do_nothing_when_receive_beat_event_and_not_count_down_beat()
        {
            GivenStartCountDownValue(20);

            scoreBall.Init();

            CallBeatEventCallback(false);

            CurrentCountDownValueShouldBe(20);
        }

        [Test]
        //設為凍結狀態時, 收到Beat事件不會倒數
        public void do_not_count_down_when_freeze()
        {
            GivenStartCountDownValue(20);

            scoreBall.Init();
            scoreBall.SetFreezeState(true);

            CurrentCountDownValueShouldBe(20);

            CallBeatEventCallback();

            CurrentCountDownValueShouldBe(20);
        }

        [Test]
        //取消凍結狀態, 收到Beat事件數字繼續倒數
        public void continue_count_down_when_unfreeze()
        {
            GivenStartCountDownValue(10);

            scoreBall.Init();
            scoreBall.SetFreezeState(true);

            CallBeatEventCallback();

            scoreBall.SetFreezeState(false);

            CurrentCountDownValueShouldBe(10);

            CallBeatEventCallback();

            CurrentCountDownValueShouldBe(9);
        }

        [Test]
        //重設倒數數字, 數字恢復到起始值
        public void reset_count_down_value()
        {
            GivenStartCountDownValue(10);

            scoreBall.Init();

            CallBeatEventCallback();
            CallBeatEventCallback();
            CallBeatEventCallback();

            CurrentCountDownValueShouldBe(7);

            scoreBall.ResetToBeginning();

            CurrentCountDownValueShouldBe(10);
        }

        [Test]
        //重設倒數數字, 若狀態為"Hide"則不做事
        public void do_not_reset_count_down_value_when_hide()
        {
            GivenStartCountDownValue(10);

            scoreBall.Init();
            scoreBall.SuccessSettle();

            scoreBall.ResetToBeginning();

            CurrentCountDownValueShouldBe(0);
            CurrentStateShouldBe(ScoreBallState.Hide);
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        //結算或倒數完畢後再度激活, 倒數數字恢復到起始值
        public void reactivate_then_reset_count_down_value(bool isSuccessSettle)
        {
            GivenStartCountDownValue(10);

            scoreBall.Init();

            if (isSuccessSettle)
                scoreBall.SuccessSettle();
            else
            {
                for (int i = 0; i < 10; i++)
                {
                    CallBeatEventCallback();
                }
            }

            CurrentStateShouldBe(ScoreBallState.Hide);

            scoreBall.Reactivate();

            CurrentCountDownValueShouldBe(10);
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        //結算或倒數完畢後再度激活, 收到Beat事件倒數數字減一
        public void reactivate_and_count_down(bool isSuccessSettle)
        {
            GivenStartCountDownValue(10);

            scoreBall.Init();

            if (isSuccessSettle)
                scoreBall.SuccessSettle();
            else
            {
                for (int i = 0; i < 10; i++)
                {
                    CallBeatEventCallback();
                }
            }

            scoreBall.Reactivate();

            CallBeatEventCallback();

            CurrentCountDownValueShouldBe(9);
        }

        #endregion

        #region 捕獲旗標

        [Test]
        //初始化時, 依據當前Fever階段對應的旗標編號權重設定, 設定當前旗標編號
        public void init_with_flag_number()
        {
            GivenStartCountDownValue(10);
            GivenCurrentFeverStage(2);
            GivenScoreBallFlagWeightSetting(2, new Dictionary<int, int>
            {
                { 4, 1 }
            });

            scoreBall.Init();

            CurrentFlagNumberShouldBe(4);
        }

        [Test]
        //初始化時, 依據當前Fever階段對應的旗標編號權重設定, 若有多個權重設定則按照權重隨機抽取旗標編號
        public void init_with_multiple_flag_number()
        {
            GivenStartCountDownValue(10);
            GivenCurrentFeverStage(2);
            GivenScoreBallFlagWeightSetting(2, new Dictionary<int, int>
            {
                { 4, 1 },
                { 5, 3 },
                { 6, 6 }
            });

            Dictionary<int, int> flagNumberCounterDict = new Dictionary<int, int>();
            for (int i = 0; i < 300; i++)
            {
                scoreBall.Init();

                if (flagNumberCounterDict.ContainsKey(scoreBall.CurrentFlagNumber))
                    flagNumberCounterDict[scoreBall.CurrentFlagNumber]++;
                else
                    flagNumberCounterDict[scoreBall.CurrentFlagNumber] = 1;
            }

            Assert.IsTrue(flagNumberCounterDict[4] > 0);
            Assert.IsTrue(flagNumberCounterDict[5] > 0);
            Assert.IsTrue(flagNumberCounterDict[6] > 0);
            Assert.IsTrue(flagNumberCounterDict[6] > flagNumberCounterDict[5]);
            Assert.IsTrue(flagNumberCounterDict[5] > flagNumberCounterDict[4]);
        }

        [Test]
        //初始化時, 若取不到旗標編號權重設定則回應錯誤
        public void init_with_no_flag_number_setting()
        {
            GivenStartCountDownValue(10);
            GivenScoreBallFlagWeightSettingIsEmpty();

            Assert.Throws<NullReferenceException>(() => scoreBall.Init());
        }

        [Test]
        //結算或倒數完畢後再度激活, 重新抽取旗標編號
        public void reactivate_then_reselect_flag_number()
        {
            GivenStartCountDownValue(10);
            GivenCurrentFeverStage(2);
            GivenScoreBallFlagWeightSetting(2, new Dictionary<int, int>
            {
                { 4, 1 },
            });

            scoreBall.Init();

            CurrentFlagNumberShouldBe(4);

            scoreBall.SuccessSettle();

            GivenCurrentFeverStage(3);
            GivenScoreBallFlagWeightSetting(3, new Dictionary<int, int>
            {
                { 6, 1 },
            });

            scoreBall.Reactivate();

            CurrentFlagNumberShouldBe(6);
        }

        #endregion
    }
}