using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UniRx;
using System;
using UnityEngine.UI;
using TMPro;

namespace _VIRAL._03_Scripts { 
public class HologramCheckbox : HologramUiComponent
{
    [SerializeField] public SpriteRenderer _iconCheckMark;
    public Vector3 _LocalScale;
    private bool _isChecked = false;
    private Tweener _checkMarkTweener;
    private Vector3 _iconCheckmarkInitialScale;
    public Subject<bool> onSwitch = new Subject<bool>();

    protected override void Awake()
    {
        base.Awake();

        _iconCheckmarkInitialScale = _iconCheckMark.transform.localScale;
        _iconCheckMark.transform.localScale = Vector3.one * 1.1f;
        _iconCheckMark.gameObject.SetActive(false);
    }

    public override void ExitAction(Vector3 point)
    {
        base.ExitAction(point);
        _icon.transform.DOScale(_LocalScale, 0.1f);
    }
    public override void EnterAction(Vector3 point)
    {
        base.EnterAction(point);
        _icon.transform.DOScale(_iconInitialScale * 1.2f, 0.1f);

        PlaySound();
        PhysicalSwitch(); 
    }

    private void PhysicalSwitch()
    {
        PhysicalCheck(!_isChecked);
    }

    public void PhysicalCheck(bool value)
    {
        _isChecked = value;
        onSwitch.OnNext(value);

        _checkMarkTweener?.Kill();

        if (value)
        {
            _iconCheckMark.gameObject.SetActive(true);
            _checkMarkTweener = _iconCheckMark.transform.DOScale(_iconCheckmarkInitialScale, 0.25f).SetEase(Ease.OutBack);
        }
        else
        {
            _checkMarkTweener = _iconCheckMark.transform.DOScale(_iconCheckmarkInitialScale * 0.01f, 0.25f)
                .SetEase(Ease.OutBack)
                .OnComplete(() =>
                {
                    _iconCheckMark.gameObject.SetActive(false);

                });
        }
    }

    public void SetValue(bool value)
    {
        _isChecked = value;
        if (value)
        {
            _iconCheckMark.gameObject.SetActive(true);
            _iconCheckMark.transform.localScale = _iconCheckmarkInitialScale;
        }
        else
        {
            _iconCheckMark.gameObject.SetActive(false);
            _iconCheckMark.transform.localScale = _iconCheckmarkInitialScale * 0.01f;
        }
    
    }
}
}