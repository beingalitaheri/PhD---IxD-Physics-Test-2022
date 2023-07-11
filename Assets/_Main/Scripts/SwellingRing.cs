using DG.Tweening;
using System;
using UnityEngine;

public class SwellingRing : MonoBehaviour
{

    [SerializeField]private bool _lookCamera;
    [SerializeField]private SpriteRenderer _spriteRenderer;
    [SerializeField] private Transform _container;
    [SerializeField] private Transform _ring;
    [SerializeField] private Transform _faceContainer;
    [SerializeField] private Transform _dot;

    [SerializeField] private Material _materialDefault;
    [SerializeField] private Material _materialActive;


    private Transform _centerEye;
    private Sequence _swellSequence;

    private void Awake()
    {
        _centerEye = FindObjectOfType<CenterEyeAnchor>().transform;
        Show(false, 0.0f);
    }

    private void Update()
    {
        if (_lookCamera)
        {
            _faceContainer.LookAt(_centerEye);
            _dot.LookAt(_centerEye);
        }
    }

    public void Show(bool show, float time = 0.3f)
    {
        if (show)
        {
            Expand(time);
        }
        else
        {
            Collapse(time);
        }
    }
    public void SetRingScale(float scale)
    {
        _ring.localScale = Vector3.one * scale;
    }
    public void Activate(bool activate)
    {
        _spriteRenderer.material  = activate? _materialDefault : _materialActive;
    }

    private void Expand(float time)
    { 
        _container.gameObject.SetActive(true);
        _swellSequence?.Kill();
        _swellSequence = DOTween.Sequence();
        _swellSequence.Append(_container.DOScale(Vector3.one, time).SetEase(Ease.OutQuad));
        _swellSequence.Join(_dot.DOScale(Vector3.one * 0.01f, time)).SetEase(Ease.InBack);
    }

    private void Collapse(float time)
    {
        _swellSequence?.Kill();
        _swellSequence= DOTween.Sequence();
        _swellSequence.Append(_container.DOScale(Vector3.one * 0.01f, time)).SetEase(Ease.InQuad).OnComplete(() =>
       {
            _container.gameObject.SetActive(false);
        
       }
        );
        _swellSequence.Join(_dot.DOScale(Vector3.one, time).SetEase(Ease.OutBack));
    }

    public void ShowScaleRings(bool v)
    {
        
    }
}
