using System;
using SNShien.Common.AdapterTools;
using SNShien.Common.TesterTools;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
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
        [SerializeField] private Color inCountDownColor;
        [SerializeField] private Color freezeColor;
        [SerializeField] private TextMeshProUGUI tmp_countDownNum;
        [SerializeField] private Image img_back;

        private Debugger debugger;
        private ScoreBallPresenter presenter;

        private float waitDoubleClickTimer;
        private float doubleClickCoolDownTimer;
        private bool isClicked;
        private bool isWaitDoubleClick;
        private bool isDoubleClickCoolDown;

        public void SetCountDownNumberText(string text)
        {
            tmp_countDownNum.text = text;
        }

        public void SetInCountDownColor()
        {
            img_back.color = inCountDownColor;
        }

        public void SetFreezeColor()
        {
            img_back.color = freezeColor;
        }

        public void Close()
        {
            gameObject.SetActive(false);
        }

        public void InitData()
        {
            waitDoubleClickTimer = 0;
            doubleClickCoolDownTimer = 0;
            isClicked = false;
            isWaitDoubleClick = false;
            isDoubleClickCoolDown = false;
            SetInCountDownColor();

            // debugger.ShowLog($"startCountDownValue: {gameSetting.ScoreBallStartCountDownValue}");
        }

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

        public void BindPresenter(ScoreBallPresenter presenter)
        {
            this.presenter = presenter;
            presenter.BindView(this);
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
            presenter.OnDrag();
        }
    }
}