using System;
using System.Linq;
using DG.Tweening;
using SNShien.Common.AdapterTools;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace GameCore
{
    [RequireComponent(typeof(Collider2DAdapterComponent))]
    public class CatchNetView : MonoBehaviour, ICatchNetView
    {
        private const string ANIM_KEY_BEAT = "catch_net_beat";

        [SerializeField] private float spawnAnimationDuration;
        [SerializeField] private Ease spawnAnimationEaseType;
        [SerializeField] private RectTransform go_root;
        [SerializeField] private Image img_flag;
        [SerializeField] private CatchNetNumberPosSetting[] catchNumberPosSettings;

        public Vector3 Position => gameObject.transform.position;

        private Collider2DAdapterComponent colliderComponent;
        private ICatchNetPresenter presenter;
        private Tween spawnCatchNetTween;
        private Animator animator;

        public void SetFlagColor(Color flagColor)
        {
            img_flag.color = flagColor;
        }

        public void Close()
        {
            gameObject.SetActive(false);
        }

        public void PlaySpawnAnimation(CatchNetSpawnFadeInMode fadeInMode, Action callback)
        {
            Vector3 startLocalPos = Vector3.zero;

            switch (fadeInMode)
            {
                case CatchNetSpawnFadeInMode.FromTop:
                    startLocalPos = new Vector3(0, go_root.rect.height, 0);
                    break;

                case CatchNetSpawnFadeInMode.FromBottom:
                    startLocalPos = new Vector3(0, -go_root.rect.height, 0);
                    break;

                case CatchNetSpawnFadeInMode.FromLeft:
                    startLocalPos = new Vector3(-go_root.rect.width, 0, 0);
                    break;

                case CatchNetSpawnFadeInMode.FromRight:
                    startLocalPos = new Vector3(go_root.rect.width, 0, 0);
                    break;
            }

            go_root.localPosition = startLocalPos;
            spawnCatchNetTween = go_root
                .DOLocalMove(Vector3.zero, spawnAnimationDuration)
                .SetEase(spawnAnimationEaseType)
                .OnComplete(() =>
                {
                    callback?.Invoke();
                });
        }

        public void PlayBeatAnimation()
        {
            animator.Play(ANIM_KEY_BEAT, 0, 0);
        }

        public void ResetPos()
        {
            go_root.localPosition = Vector3.zero;
        }

        public void BindPresenter(IMVPPresenter mvpPresenter)
        {
            presenter = (ICatchNetPresenter)mvpPresenter;
            mvpPresenter.BindView(this);

            colliderComponent.InitHandler(presenter);
        }

        public void UnbindPresenter()
        {
            presenter = null;
        }

        [ContextMenu("Play Spawn Animation")]
        public void EditorTest_PlaySpawnAnimation()
        {
            spawnCatchNetTween?.Kill();
            ResetPos();
            PlaySpawnAnimation(CatchNetSpawnFadeInMode.FromLeft, null);
        }

        private void Awake()
        {
            animator = GetComponent<Animator>();

            colliderComponent = GetComponent<Collider2DAdapterComponent>();
            colliderComponent.SetHandlerType(ColliderHandleType.Trigger);
        }
    }
}