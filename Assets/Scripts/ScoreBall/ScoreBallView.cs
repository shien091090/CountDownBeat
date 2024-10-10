using System;
using SNShien.Common.AdapterTools;
using SNShien.Common.TesterTools;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using Zenject;

namespace GameCore
{
    [RequireComponent(typeof(EventTrigger))]
    public class ScoreBallView : MonoBehaviour, IScoreBallView
    {
        [Inject] private IDeltaTimeGetter deltaTimeGetter;
        [Inject] private IGameSetting gameSetting;

        [SerializeField] private float checkDoubleClickTime;
        [SerializeField] private float checkDoubleClickCoolDownTime;
        [SerializeField] private TextMeshProUGUI tmp_countDownNum;

        private Debugger debugger;
        private ScoreBallPresenter presenter;

        private float waitDoubleClickTimer;
        private float doubleClickCoolDownTimer;
        private bool isClicked;
        private bool isWaitDoubleClick;
        private bool isDoubleClickCoolDown;

        private void Update()
        {
            if (isWaitDoubleClick)
                waitDoubleClickTimer += deltaTimeGetter.deltaTime;

            if (isDoubleClickCoolDown)
            {
                doubleClickCoolDownTimer += deltaTimeGetter.deltaTime;
                if (doubleClickCoolDownTimer >= checkDoubleClickCoolDownTime)
                    isDoubleClickCoolDown = false;
            }
        }

        private void InitData()
        {
            waitDoubleClickTimer = 0;
            doubleClickCoolDownTimer = 0;
            isClicked = false;
            isWaitDoubleClick = false;
            isDoubleClickCoolDown = false;

            // debugger.ShowLog($"startCountDownValue: {gameSetting.ScoreBallStartCountDownValue}");
        }

        public void SetCountDownNumberText(string text)
        {
            tmp_countDownNum.text = text;
        }

        public void BindPresenter(ScoreBallPresenter presenter)
        {
            this.presenter = presenter;
            presenter.BindView(this);

            InitData();
        }

        private void Awake()
        {
            debugger = new Debugger(GameConst.DEBUGGER_KEY_SCORE_BALL_VIEW);
        }

        private void MoveFollowMouse()
        {
            Vector3 mousePosition = Input.mousePosition;
            transform.position = mousePosition;
        }

        public void OnClickDown()
        {
            if (waitDoubleClickTimer <= checkDoubleClickTime && isWaitDoubleClick)
            {
                // debugger.ShowLog("OnDoubleClick");
                isDoubleClickCoolDown = true;
                doubleClickCoolDownTimer = 0;
            }
            // else
            // debugger.ShowLog(isDoubleClickCoolDown ?
            // "OnClickDown(DoubleClickCoolDown)" :
            // "OnClickDown");

            waitDoubleClickTimer = 0;
            isClicked = true;
            isWaitDoubleClick = false;
        }

        public void OnClickUp()
        {
            isClicked = false;

            if (isDoubleClickCoolDown == false)
                isWaitDoubleClick = true;
        }

        public void OnDrag()
        {
            if (isClicked == false)
                return;

            MoveFollowMouse();
        }
    }
}