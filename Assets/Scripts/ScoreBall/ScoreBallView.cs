using System;
using SNShien.Common.AdapterTools;
using SNShien.Common.MonoBehaviorTools;
using SNShien.Common.TesterTools;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace GameCore
{
    [RequireComponent(typeof(Collider2DAdapterComponent))]
    public class ScoreBallView : MonoBehaviour, IScoreBallView
    {
        [Inject] private IDeltaTimeGetter deltaTimeGetter;
        [SerializeField] private Color inCountDownColor;
        [SerializeField] private Color freezeColor;
        [SerializeField] private TextMeshProUGUI tmp_countDownNum;
        [SerializeField] private Image img_back;

        public int GetCurrentCountDownValue => presenter.CurrentCountDownValue;

        private Debugger debugger = new Debugger(GameConst.DEBUGGER_KEY_SCORE_BALL_VIEW);
        private IScoreBallPresenter presenter;
        private OperableUI operableUI;
        private Collider2DAdapterComponent colliderComponent;

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

        public void TriggerCatch()
        {
            presenter.TriggerCatch();
        }

        public void BindPresenter(IScoreBallPresenter presenter)
        {
            this.presenter = presenter;
        }

        public void Init()
        {
            operableUI = gameObject.GetComponent<OperableUI>();
            operableUI.Init();
            RegisterEvent();
            SetInCountDownColor();
        }

        private void Update()
        {
            operableUI.UpdatePerFrame(deltaTimeGetter.deltaTime);
        }

        private void Awake()
        {
            colliderComponent = GetComponent<Collider2DAdapterComponent>();
            colliderComponent.SetHandlerType(ColliderHandleType.Trigger);
        }

        private void RegisterEvent()
        {
            operableUI.OnStartDragEvent -= StartDrag;
            operableUI.OnStartDragEvent += StartDrag;

            operableUI.OnDragOverEvent -= DragOver;
            operableUI.OnDragOverEvent += DragOver;

            operableUI.OnDoubleClickEvent -= DoubleClick;
            operableUI.OnDoubleClickEvent += DoubleClick;
        }

        private void DoubleClick()
        {
            presenter.DoubleClick();
        }

        private void DragOver()
        {
            presenter.DragOver();
        }

        private void StartDrag()
        {
            presenter.StartDrag();
        }
    }
}