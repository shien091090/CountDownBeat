using System.Collections;
using SNShien.Common.MonoBehaviorTools;
using UnityEngine;

namespace GameCore
{
    public class BeaterView : ArchitectureView, IBeaterView
    {
        [SerializeField] private float beatHintStaySeconds;
        [SerializeField] private GameObject go_beatHint;

        private IBeaterPresenter beaterPresenter;

        public override void UpdateView()
        {
        }

        public override void OpenView(params object[] parameters)
        {
            beaterPresenter = parameters[0] as IBeaterPresenter;
            beaterPresenter.BindView(this);
        }

        public override void ReOpenView(params object[] parameters)
        {
        }

        public override void CloseView()
        {
            StopAllCoroutines();
            beaterPresenter.UnbindView();
        }

        public void PlayBeatAnimation()
        {
            StartCoroutine(Cor_PlayBeatAnimation());
        }

        public void SetBeatHintActive(bool isActive)
        {
            go_beatHint.SetActive(isActive);
        }

        private IEnumerator Cor_PlayBeatAnimation()
        {
            SetBeatHintActive(true);

            yield return new WaitForSeconds(beatHintStaySeconds);

            SetBeatHintActive(false);
        }
    }
}