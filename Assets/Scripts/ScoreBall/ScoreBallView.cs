using System;
using SNShien.Common.AdapterTools;
using SNShien.Common.MonoBehaviorTools;
using SNShien.Common.ProcessTools;
using SNShien.Common.TesterTools;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace GameCore
{
    [RequireComponent(typeof(Collider2DAdapterComponent))]
    [RequireComponent(typeof(ComputableCollider))]
    public class ScoreBallView : MonoBehaviour, IScoreBallView
    {
        private const string PREFAB_NAME_BEAT_EFFECT = "BeatEffect";

        [Inject] private IDeltaTimeGetter deltaTimeGetter;
        [Inject] private IEventInvoker eventInvoker;

        [Header("FlagColor")] [SerializeField] private Color redColor;
        [SerializeField] private Color greenColor;

        [Header("Reference")] [SerializeField] private ObjectPoolManager objectPool;
        [SerializeField] private TextMeshProUGUI tmp_countDownNum;
        [SerializeField] private Image img_back;
        [SerializeField] private GameObject go_directionFlagArrow;
        [SerializeField] private GameObject go_directionFlagCheckmark;

        public int CurrentFlagNumber => presenter.CurrentFlagNumber;
        public ComputableCollider ComputableCollider { get; private set; }

        private IScoreBallPresenter presenter;
        private OperableUI operableUI;
        private Collider2DAdapterComponent colliderComponent;
        private Animator animator;

        private readonly Debugger debugger = new Debugger(DebuggerKeyConst.SCORE_BALL_VIEW);

        public void SetCountDownNumberText(string text)
        {
            tmp_countDownNum.text = text;
        }

        public void SetFrameColor(int colorNum)
        {
            if (colorNum == Stage1ColorConst.RED_COLOR)
                img_back.color = redColor;

            if (colorNum == Stage1ColorConst.GREEN_COLOR)
                img_back.color = greenColor;
        }

        public void SetDirectionFlag(int directionFlagNum)
        {
            bool isArrowFlag = directionFlagNum == Stage1DirectionConst.DIRECTION_LEFT_TO_RIGHT ||
                               directionFlagNum == Stage1DirectionConst.DIRECTION_RIGHT_TO_LEFT;

            bool isCheckmarkFlag = directionFlagNum == Stage1DirectionConst.CHECKMARK_UP ||
                                   directionFlagNum == Stage1DirectionConst.CHECKMARK_DOWN ||
                                   directionFlagNum == Stage1DirectionConst.CHECKMARK_RIGHT ||
                                   directionFlagNum == Stage1DirectionConst.CHECKMARK_LEFT;

            go_directionFlagArrow.SetActive(isArrowFlag);
            go_directionFlagCheckmark.SetActive(isCheckmarkFlag);

            if (isArrowFlag)
            {
                switch (directionFlagNum)
                {
                    case Stage1DirectionConst.DIRECTION_LEFT_TO_RIGHT:
                        go_directionFlagArrow.transform.localRotation = Quaternion.Euler(0, 0, 180);
                        break;

                    case Stage1DirectionConst.DIRECTION_RIGHT_TO_LEFT:
                        go_directionFlagArrow.transform.localRotation = Quaternion.Euler(0, 0, 0);
                        break;
                }
            }

            if (isCheckmarkFlag)
            {
                switch (directionFlagNum)
                {
                    case Stage1DirectionConst.CHECKMARK_UP:
                        go_directionFlagCheckmark.transform.localRotation = Quaternion.Euler(0, 0, 0);
                        break;

                    case Stage1DirectionConst.CHECKMARK_DOWN:
                        go_directionFlagCheckmark.transform.localRotation = Quaternion.Euler(0, 0, 180);
                        break;

                    case Stage1DirectionConst.CHECKMARK_RIGHT:
                        go_directionFlagCheckmark.transform.localRotation = Quaternion.Euler(0, 0, -90);
                        break;

                    case Stage1DirectionConst.CHECKMARK_LEFT:
                        go_directionFlagCheckmark.transform.localRotation = Quaternion.Euler(0, 0, 90);
                        break;
                }
            }
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
            objectPool.SpawnGameObject(PREFAB_NAME_BEAT_EFFECT);
        }

        public void PlayAnimation(string animKey)
        {
            animator.Play(animKey, 0, 0);
        }

        public void HideAllDirectionFlag()
        {
            go_directionFlagArrow.SetActive(false);
            go_directionFlagCheckmark.SetActive(false);
        }

        public void TriggerCheckmark(CheckmarkSecondTriggerAreaType checkmarkType)
        {
            presenter.TriggerCheckmark(checkmarkType);
        }

        public void Init()
        {
            operableUI.Init();
            RegisterEvent();
        }

        private void Update()
        {
            operableUI.UpdatePerFrame(deltaTimeGetter.deltaTime);
        }

        public void CrossDirectionFlagWall(string crossDirectionKey)
        {
            if (Enum.TryParse(crossDirectionKey, out TriggerFlagMergingType type))
                presenter.CrossDirectionFlagWall(type);
        }

        private void Awake()
        {
            colliderComponent = GetComponent<Collider2DAdapterComponent>();
            colliderComponent.SetHandlerType(ColliderHandleType.Trigger);

            animator = GetComponent<Animator>();
            operableUI = gameObject.GetComponent<OperableUI>();
            ComputableCollider = GetComponent<ComputableCollider>();
        }

        private void RegisterEvent()
        {
            operableUI.OnStartDragEvent -= StartDrag;
            operableUI.OnStartDragEvent += StartDrag;

            operableUI.OnDragOverEvent -= DragOver;
            operableUI.OnDragOverEvent += DragOver;
        }

        private void DragOver()
        {
            presenter.DragOver();
            eventInvoker.SendEvent(new ScoreBallOperateEvent(false, this));
        }

        private void StartDrag()
        {
            presenter.StartDrag();
            eventInvoker.SendEvent(new ScoreBallOperateEvent(true, this));
        }
    }
}