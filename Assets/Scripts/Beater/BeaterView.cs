using System.Collections;
using SNShien.Common.AdapterTools;
using SNShien.Common.MonoBehaviorTools;
using UnityEngine;
using Zenject;

namespace GameCore
{
    public class BeaterView : ArchitectureView, IBeaterView
    {
        [Inject] private IDeltaTimeGetter deltaTimeGetter;

        [SerializeField] private float beatHintStaySeconds;
        [SerializeField] private GameObject go_beatHint;
        [SerializeField] private GameObject go_halfBeatHint;

        private IBeaterPresenter presenter;

        public void PlayBeatAnimation()
        {
            StartCoroutine(Cor_PlayHintFlashAnimation(go_beatHint));
        }

        public void PlayHalfBeatAnimation()
        {
            StartCoroutine(Cor_PlayHintFlashAnimation(go_halfBeatHint));
        }

        public void HideAllHint()
        {
            go_beatHint.SetActive(false);
            go_halfBeatHint.SetActive(false);
        }

        public override void UpdateView()
        {
        }

        public override void OpenView(params object[] parameters)
        {
            presenter = parameters[0] as IBeaterPresenter;
            presenter.BindView(this);
        }

        public override void ReOpenView(params object[] parameters)
        {
        }

        public override void CloseView()
        {
            StopAllCoroutines();
            presenter.UnbindView();
        }

        private void Update()
        {
            presenter.UpdateFrame(deltaTimeGetter.deltaTime);
        }

        private IEnumerator Cor_PlayHintFlashAnimation(GameObject obj)
        {
            obj.SetActive(true);

            yield return new WaitForSeconds(beatHintStaySeconds);

            obj.SetActive(false);
        }
    }
}