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
        private const string PREFAB_NAME_BEAT_EFFECT = "BeatEffect";

        [Inject] private IDeltaTimeGetter deltaTimeGetter;

        [SerializeField] private ObjectPoolManager objectPool;
        [SerializeField] private Color inCountDownColor;
        [SerializeField] private Color freezeColor;
        [SerializeField] private TextMeshProUGUI tmp_countDownNum;
        [SerializeField] private Image img_back;

        public int GetCurrentCountDownValue => presenter.CurrentCountDownValue;

        private Debugger debugger = new Debugger(GameConst.DEBUGGER_KEY_SCORE_BALL_VIEW);
        private IScoreBallPresenter presenter;
        private OperableUI operableUI;
        private Collider2DAdapterComponent colliderComponent;
        private Animator animator;

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

        public void SetTextColor(Color color)
        {
            tmp_countDownNum.color = color;
        }

        public void Close()
        {
            gameObject.SetActive(false);
        }

        public void TriggerCatch()
        {
            presenter.TriggerCatch();
        }

        public void BindPresenter(IMVPPresenter mvpPresenter)
        {
            presenter = (IScoreBallPresenter)mvpPresenter;
        }

        public void UnbindPresenter()
        {
            presenter = null;
        }

        public void CreateBeatEffectPrefab()
        {
            objectPool.SpawnGameObject<AutoParticleEffectObject>(PREFAB_NAME_BEAT_EFFECT);
        }

        public void PlayAnimation(string animKey)
        {
            animator.Play(animKey, 0, 0);
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
            animator = GetComponent<Animator>();

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