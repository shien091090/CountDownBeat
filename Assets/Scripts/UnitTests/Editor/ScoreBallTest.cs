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
        private ICatchFlagSetting flagSetting;

        private Action<BeatEvent> beatEventCallback;
        private Action scoreBallBeatEventCallback;
        private Action<ScoreBallState> updateStateEventCallback;
        private Action initEventCallback;
        private Action<int> updateCountDownValueEventCallback;
        private Action<int> updateFlagNumberEventCallback;

        [SetUp]
        public void Setup()
        {
            InitEventHandlerMock();
            flagSetting = Substitute.For<ICatchFlagSetting>();

            scoreBall = new ScoreBall(eventRegister, eventInvoker, flagSetting);

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

            updateFlagNumberEventCallback = Substitute.For<Action<int>>();
            scoreBall.OnUpdateCatchFlagNumber += updateFlagNumberEventCallback;
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

        private void GivenCatchFlagMergeResultIsFailed()
        {
            flagSetting.GetCatchFlagMergeResult(Arg.Any<int>(), Arg.Any<TriggerFlagMergingType>()).Returns(CatchFlagMergeResult.CreateFailInstance());
        }

        private void GivenCatchFlagMergeResult(int flagNum, TriggerFlagMergingType triggerFlagMergingType, int expectedResultFlagNum)
        {
            flagSetting.GetCatchFlagMergeResult(flagNum, triggerFlagMergingType).Returns(CatchFlagMergeResult.CreateSuccessInstance(expectedResultFlagNum));
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

        private void ShouldSendUpdateFlagNumberEvent(int expectedCallTimes)
        {
            if (expectedCallTimes == 0)
                updateFlagNumberEventCallback.DidNotReceive().Invoke(Arg.Any<int>());
            else
                updateFlagNumberEventCallback.Received(expectedCallTimes).Invoke(Arg.Any<int>());
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
            scoreBall.Init(20, 1);

            CurrentCountDownValueShouldBe(20);
            CurrentStateShouldBe(ScoreBallState.InCountDown);
        }

        [Test]
        //收到Beat事件時, 倒數數字減至0, 切換狀態為"Hide"
        public void count_down_to_zero_then_change_state_to_hide()
        {
            scoreBall.Init(1, 1);

            CallBeatEventCallback();

            CurrentCountDownValueShouldBe(0);
            CurrentStateShouldBe(ScoreBallState.Hide);
        }

        [Test]
        //設定凍結狀態並切換狀態為"Freeze"
        public void set_freeze_state_then_change_state_to_freeze()
        {
            scoreBall.Init(20, 1);
            scoreBall.SetFreezeState(true);

            CurrentStateShouldBe(ScoreBallState.Freeze);
        }

        [Test]
        //取消凍結狀態並切換狀態為"InCountDown"
        public void unset_freeze_state_then_change_state_to_in_count_down()
        {
            scoreBall.Init(10, 1);
            scoreBall.SetFreezeState(true);
            scoreBall.SetFreezeState(false);

            CurrentStateShouldBe(ScoreBallState.InCountDown);
        }

        [Test]
        //成功結算, 切換狀態為"Hide"
        public void success_settle_then_change_state_to_hide()
        {
            scoreBall.Init(10, 1);
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
            scoreBall.Init(10, 1);

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

            scoreBall.Reactivate(1);

            CurrentStateShouldBe(ScoreBallState.InCountDown);
        }

        #endregion

        #region 事件註冊&發送事件

        [Test]
        //設定初始化時, 監聽事件
        public void verify_register_event()
        {
            scoreBall.Init(20, 1);

            ShouldRegisterBeatEvent();
            ShouldRegisterHalfBeatEvent();
        }

        [Test]
        //初始化時, 發送初始化事件
        public void send_init_event_when_init()
        {
            scoreBall.Init(20, 1);

            ShouldSendInitEvent(1);
        }

        [Test]
        //初始化時, 發送旗標更新事件
        public void send_update_catch_flag_number_event_when_init()
        {
            scoreBall.Init(20, 4);

            ShouldSendUpdateFlagNumberEvent(1);
        }

        [Test]
        //更新倒數數字時, 發送倒數數字更新事件
        public void send_update_count_down_value_event_when_update_count_down_value()
        {
            scoreBall.Init(20, 1);

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
            scoreBall.Init(20, 1);

            ShouldSendInitEvent(1);

            scoreBall.SuccessSettle();

            ShouldSendInitEvent(1);

            scoreBall.Reactivate(1);

            ShouldSendInitEvent(2);
        }

        [Test]
        //分數球重新激活時, 發送旗標更新事件
        public void send_update_flag_number_event_when_reactivate()
        {
            scoreBall.Init(20, 1);

            ShouldSendUpdateFlagNumberEvent(1);

            scoreBall.SuccessSettle();

            ShouldSendUpdateFlagNumberEvent(1);

            scoreBall.Reactivate(2);

            ShouldSendUpdateFlagNumberEvent(2);
        }

        [Test]
        //收到Beat事件時, 發送分數球Beat事件
        public void send_score_ball_beat_event_when_receive_beat_event()
        {
            scoreBall.Init(20, 1);

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
            scoreBall.Init(20, 1);

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
            scoreBall.Init(10, 1);
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
            scoreBall.Init(1, 1);

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
            scoreBall.Init(startCountDownValue, 1);

            CurrentCountDownValueShouldBe(0);
            CurrentStateShouldBe(ScoreBallState.None);
        }

        [Test]
        //收到Beat事件時, 若為倒數拍點, 則倒數數字減一
        public void count_down_when_receive_beat_event_and_is_count_down_beat()
        {
            scoreBall.Init(20, 1);

            CallBeatEventCallback(true);

            CurrentCountDownValueShouldBe(19);
        }

        [Test]
        //收到Beat事件時, 若不是倒數拍點, 則不做事
        public void do_nothing_when_receive_beat_event_and_not_count_down_beat()
        {
            scoreBall.Init(20, 1);

            CallBeatEventCallback(false);

            CurrentCountDownValueShouldBe(20);
        }

        [Test]
        //設為凍結狀態時, 收到Beat事件不會倒數
        public void do_not_count_down_when_freeze()
        {
            scoreBall.Init(20, 1);
            scoreBall.SetFreezeState(true);

            CurrentCountDownValueShouldBe(20);

            CallBeatEventCallback();

            CurrentCountDownValueShouldBe(20);
        }

        [Test]
        //取消凍結狀態, 收到Beat事件數字繼續倒數
        public void continue_count_down_when_unfreeze()
        {
            scoreBall.Init(10, 1);
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
            scoreBall.Init(10, 1);

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
            scoreBall.Init(10, 1);
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
            scoreBall.Init(10, 1);

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

            scoreBall.Reactivate(1);

            CurrentCountDownValueShouldBe(10);
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        //結算或倒數完畢後再度激活, 收到Beat事件倒數數字減一
        public void reactivate_and_count_down(bool isSuccessSettle)
        {
            scoreBall.Init(10, 1);

            if (isSuccessSettle)
                scoreBall.SuccessSettle();
            else
            {
                for (int i = 0; i < 10; i++)
                {
                    CallBeatEventCallback();
                }
            }

            scoreBall.Reactivate(1);

            CallBeatEventCallback();

            CurrentCountDownValueShouldBe(9);
        }

        #endregion

        #region 旗標融合

        [Test]
        //旗標融合時, 若旗標融合設定判斷為通過則輸出新旗標
        public void merge_flag_success_when_check_setting_pass()
        {
            GivenCatchFlagMergeResult(5, TriggerFlagMergingType.DirectionWall_LeftToRight, 11);

            scoreBall.Init(10, 5);
            scoreBall.MergeFlagWith(TriggerFlagMergingType.DirectionWall_LeftToRight);

            CurrentFlagNumberShouldBe(11);
        }

        [Test]
        //旗標融合時, 若旗標融合設定判斷為不通過則維持原旗標
        public void merge_flag_failed_when_check_setting_not_pass()
        {
            GivenCatchFlagMergeResultIsFailed();

            scoreBall.Init(10, 5);
            scoreBall.MergeFlagWith(TriggerFlagMergingType.DirectionWall_UpToDown);

            CurrentFlagNumberShouldBe(5);
        }

        #endregion
    }
}