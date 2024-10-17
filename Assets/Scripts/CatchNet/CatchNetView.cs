using System;
using SNShien.Common.AdapterTools;
using TMPro;
using UnityEngine;

namespace GameCore
{
    [RequireComponent(typeof(Collider2DAdapterComponent))]
    public class CatchNetView : MonoBehaviour, ICatchNetView
    {
        [SerializeField] private TextMeshProUGUI tmp_catchNumber;
        
        private Collider2DAdapterComponent colliderComponent;

        private ICatchNetPresenter presenter;

        private void Awake()
        {
            colliderComponent = GetComponent<Collider2DAdapterComponent>();
            colliderComponent.SetHandlerType(ColliderHandleType.Trigger);
        }

        public void BindPresenter(ICatchNetPresenter presenter)
        {
            this.presenter = presenter;
            presenter.BindView(this);

            colliderComponent.InitHandler(presenter);
        }

        public void SetCatchNumber(string catchNumberText)
        {
            tmp_catchNumber.text = catchNumberText;
        }

        public void Close()
        {
            gameObject.SetActive(false);
        }
    }
}