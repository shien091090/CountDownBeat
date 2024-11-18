using System;
using System.Linq;
using SNShien.Common.AdapterTools;
using TMPro;
using UnityEngine;

namespace GameCore
{
    [RequireComponent(typeof(Collider2DAdapterComponent))]
    public class CatchNetView : MonoBehaviour, ICatchNetView
    {
        [SerializeField] private TextMeshProUGUI tmp_catchNumber;
        [SerializeField] private CatchNetNumberPosSetting[] catchNumberPosSettings;

        private Collider2DAdapterComponent colliderComponent;

        private ICatchNetPresenter presenter;

        public void SetCatchNumber(string catchNumberText)
        {
            tmp_catchNumber.text = catchNumberText;
        }

        public void SetCatchNumberPosType(CatchNetSpawnFadeInMode fadeInMode)
        {
            CatchNetNumberPosSetting setting = catchNumberPosSettings.FirstOrDefault(x => x.FadeInMode == fadeInMode);
            if (setting != null)
                tmp_catchNumber.transform.localPosition = setting.LocalPosition;
        }

        public void Close()
        {
            gameObject.SetActive(false);
        }

        public void PlaySpawnAnimation(CatchNetSpawnFadeInMode fadeInMode)
        {
        }

        public void BindPresenter(ICatchNetPresenter presenter)
        {
            this.presenter = presenter;
            presenter.BindView(this);

            colliderComponent.InitHandler(presenter);
        }

        private void Awake()
        {
            colliderComponent = GetComponent<Collider2DAdapterComponent>();
            colliderComponent.SetHandlerType(ColliderHandleType.Trigger);
        }
    }
}