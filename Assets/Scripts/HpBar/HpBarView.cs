﻿using System;
using SNShien.Common.MonoBehaviorTools;
using UnityEngine;
using UnityEngine.UI;

namespace GameCore
{
    public class HpBarView : ArchitectureView, IHpBarView
    {
        [SerializeField] private Slider sld_hpBar;

        private IHpBarPresenter presenter;
        private bool isStartUpdateHp;

        private void Update()
        {
            if (isStartUpdateHp)
                presenter.UpdateFrame();
        }

        public override void UpdateView()
        {
        }

        public override void OpenView(params object[] parameters)
        {
            presenter = parameters[0] as IHpBarPresenter;
            presenter.BindView(this);

            isStartUpdateHp = true;
        }

        public override void ReOpenView(params object[] parameters)
        {
        }

        public override void CloseView()
        {
            presenter.UnbindView();
            isStartUpdateHp = false;
        }

        public void RefreshHpSliderValue(float value)
        {
            sld_hpBar.value = value;
        }
    }
}