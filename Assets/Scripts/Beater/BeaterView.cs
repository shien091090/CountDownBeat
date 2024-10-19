using System;
using System.Collections;
using UnityEngine;

namespace GameCore
{
    public class BeaterView : MonoBehaviour, IBeaterView
    {
        [SerializeField] private GameObject go_beatHint;

        private IBeaterPresenter beaterPresenter;

        public void UpdateView()
        {
        }

        public void OpenView(params object[] parameters)
        {
            beaterPresenter = parameters[0] as IBeaterPresenter;
            beaterPresenter.BindView(this);
        }

        public void ReOpenView(params object[] parameters)
        {
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

            yield return new WaitForSeconds(0.5f);

            SetBeatHintActive(false);
        }
    }
}