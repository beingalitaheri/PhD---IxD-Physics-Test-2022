using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using DG.Tweening;
using System;
using UnityEngine.UI;
using TMPro;

namespace _VIRAL._03_Scripts
{
    public class HologramButton : HologramUiComponent
    {
        private Subject<HologramButton> _onButtonPress = new Subject<HologramButton>();
        public IObservable<HologramButton> OnPress()
        {
            return _onButtonPress ?? (_onButtonPress = new Subject<HologramButton>());
        }
        public override void EnterAction(Vector3 point)
        {
            base.EnterAction(point);
            transform.DOScale(_initialScale * 1.1f, 0.1f);
        }
        public override void ExitAction(Vector3 point)
        {
            base.ExitAction(point);
            PressButton();
        }
        private void PressButton()
        {
            _onButtonPress.OnNext(this);
            PlaySound();
            if (_deactivateAfterActionTime > 0)
            {
                DeactivateFor(_deactivateAfterActionTime);
            }
        }
        public void SetText(string v)
        {
            throw new NotImplementedException();
        }
    }
}