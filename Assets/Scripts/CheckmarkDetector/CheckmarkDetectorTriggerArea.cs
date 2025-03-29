using Sirenix.OdinInspector;
using SNShien.Common.AdapterTools;
using UnityEngine;

namespace GameCore
{
    [RequireComponent(typeof(Collider2DAdapterComponent))]
    [RequireComponent(typeof(BoxCollider2D))]
    [RequireComponent(typeof(Rigidbody2D))]
    public class CheckmarkDetectorTriggerArea : MonoBehaviour, ICheckmarkDetectorTriggerArea
    {
        [SerializeField] private bool isFirstTriggerType;
        [SerializeField] [ShowIf("$isFirstTriggerType")] private CheckmarkFirstTriggerAreaType firstTriggerAreaType;
        [SerializeField] [HideIf("$isFirstTriggerType")] private CheckmarkSecondTriggerAreaType secondTriggerAreaType;

        private BoxCollider2D boxCollider;
        private RectTransform rectTransform;
        private Collider2DAdapterComponent colliderComponent;

        public bool IsFirstTriggerType => isFirstTriggerType;

        public void Show()
        {
            gameObject.SetActive(true);
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }

        public CheckmarkFirstTriggerAreaType FirstTriggerAreaType()
        {
            if (isFirstTriggerType)
                return firstTriggerAreaType;
            else
                return CheckmarkFirstTriggerAreaType.None;
        }

        public CheckmarkSecondTriggerAreaType SecondTriggerAreaType()
        {
            if (isFirstTriggerType)
                return CheckmarkSecondTriggerAreaType.None;
            else
                return secondTriggerAreaType;
        }

        public void Init(ICheckmarkDetectorPresenter presenter)
        {
            rectTransform = GetComponent<RectTransform>();
            colliderComponent = GetComponent<Collider2DAdapterComponent>();
            InitBoxCollider();

            colliderComponent.SetHandlerType(ColliderHandleType.Trigger);
            colliderComponent.InitHandler(presenter.GetColliderHandler(this));
        }

        private void InitBoxCollider()
        {
            boxCollider = GetComponent<BoxCollider2D>();
            boxCollider.size = rectTransform.rect.size;

            float offsetX = rectTransform.pivot.x == 0 ?
                rectTransform.rect.width / 2 :
                -rectTransform.rect.width / 2;

            float offsetY = rectTransform.pivot.y == 0 ?
                rectTransform.rect.height / 2 :
                -rectTransform.rect.height / 2;

            boxCollider.offset = new Vector2(offsetX, offsetY);
        }
    }
}