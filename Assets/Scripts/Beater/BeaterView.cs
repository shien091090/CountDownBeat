using System;
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

        public float CurrentTimer { get; private set; }

        private IBeaterPresenter beaterPresenter;
        private float halfBeatTimeOffset;
        private float halfBeatEventTimer;

        public void SetHalfBeatTimeOffset(float halfBeatTimeOffset)
        {
            this.halfBeatTimeOffset = halfBeatTimeOffset;
        }

        public void ClearHalfBeatEventTimer()
        {
            halfBeatEventTimer = 0;
        }

        public void PlayBeatAnimation()
        {
            StartCoroutine(Cor_PlayBeatAnimation());
        }

        public void SetBeatHintActive(bool isActive)
        {
            go_beatHint.SetActive(isActive);
        }

        public override void UpdateView()
        {
        }

        public override void OpenView(params object[] parameters)
        {
            beaterPresenter = parameters[0] as IBeaterPresenter;
            beaterPresenter.BindView(this);
            ClearData();
        }

        public override void ReOpenView(params object[] parameters)
        {
        }

        public override void CloseView()
        {
            StopAllCoroutines();
            beaterPresenter.UnbindView();
            ClearData();
        }

        private void Update()
        {
            CurrentTimer += deltaTimeGetter.deltaTime;
            halfBeatEventTimer += deltaTimeGetter.deltaTime;
            if (halfBeatEventTimer >= halfBeatTimeOffset)
            {
                halfBeatEventTimer = 0;
                beaterPresenter.TriggerHalfBeat();
            }
        }

        private void ClearData()
        {
            halfBeatEventTimer = 0;
            CurrentTimer = 0;
            halfBeatTimeOffset = 0;
        }

        private IEnumerator Cor_PlayBeatAnimation()
        {
            SetBeatHintActive(true);

            yield return new WaitForSeconds(beatHintStaySeconds);

            SetBeatHintActive(false);
        }
    }
}