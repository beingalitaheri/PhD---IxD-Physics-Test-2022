using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using System;
using _VIRAL._03_Scripts;

public class HandCursor : MonoBehaviour
{
    [SerializeField]private OVRHand.HandFinger _fingerPinch = OVRHand.HandFinger.Index;

    [SerializeField]private Interactor _interactor;

    [SerializeField] private Transform _startpoint;
    [SerializeField] private Laser_Pointer _laserPointer;
    [SerializeField] private Transform _ring;
    [SerializeField] private SpriteRenderer _ringSprite;

    [SerializeField] private float _minDistance = 0.3f;
    [SerializeField] private float _maxDistance = 3;

    [SerializeField] private Color _colorDefault;
    [SerializeField] private Color _colorPinch;

    [SerializeField] private Gradient _gradientDefault;
    [SerializeField] private Gradient _gradientPinch;

    public Subject<HandCursor> OnStartAiming = new Subject<HandCursor>();
    public Subject<HandCursor> OnStopAiming = new Subject<HandCursor>();

    public bool IsAiming => _isAiming;

    private Hand _hand;

    private int _layerMask = 1 << 8;

    private Vector3 _direction;

    private bool _isPinching = false;
    private bool _isAiming = false;

    private Vector3 _hitPoint;
    private HologramUiComponent _focusedComponent;

    private void Awake()
    {
        _hand = GetComponentInParent<Hand>();
    }

    private void Start()
    {
        _hand.OnPinchStarted(_fingerPinch).Subscribe(_ =>
        {
            HandlePinch();
            _isPinching = true;
        });

        _hand.OnPinchStopped(_fingerPinch).Subscribe(_ =>

        {
            HandleUnpinch();
            _isPinching = false;
        } );
    }
    private void Update()
    {
        HandleCursor();

        if (_hand.isPinching(_fingerPinch))
        {
            HandlePinching();
        }
    }

    private void HandleCursor()
    {
        _direction = Vector3.Lerp(_direction, _startpoint.forward, 0.025f);

        if (_focusedComponent)
        {
            _focusedComponent.Activate(false);
            _focusedComponent.Focus(false);
            _focusedComponent.SetInteractor(null);
            _focusedComponent = null;
        }

        RaycastHit hit;
        if (Physics.Raycast(_startpoint.position, _direction, out hit, _maxDistance, _layerMask))
        {
            _hitPoint = hit.point;

            if (hit.distance > _minDistance)
            {
                if (!_isAiming)
                {
                    ShowCursor(true);
                }

                _ring.position = hit.point;
                _ring.LookAt(hit.point + hit.normal);

                Gradient gradient = _isPinching ? _gradientPinch : _gradientDefault;
                _laserPointer.DrawLine(_startpoint.position, hit.point, gradient);

                _ringSprite.color = _isPinching ? _colorPinch : _colorDefault;

                HologramUiComponent uiComponenet = hit.collider.GetComponent<HologramUiComponent>();

                if (uiComponenet && uiComponenet.Enabled)
                {
                    _focusedComponent = uiComponenet;
                    _focusedComponent.Focus(true);
                }
            }
            else
            {
                if (_isAiming)
                {
                    ShowCursor(false);
                }
            }
        }
        else
        {
            if (_isAiming)
            {
                ShowCursor(false);
            }
        }

    }

    private void ShowCursor(bool show)
    {
        _isAiming = show;

        _laserPointer.gameObject.SetActive(show);
        _ring.gameObject.SetActive(show);

        if (show)
        {
            OnStartAiming.OnNext(this);
        }
        else
        {
            OnStopAiming.OnNext(this);
        }
    }

    private void HandlePinch()
    {
        if (_focusedComponent && _focusedComponent.Enabled)
        {
            _focusedComponent.Click(_hitPoint);
        }

    }

    private void HandleUnpinch()
    {


    }

    private void HandlePinching()
    {

        if (_focusedComponent)
        {
            _focusedComponent.SetInteractor(_interactor);

            if (_interactor)
            {
                _focusedComponent.Activate(true);
            }
        }
    }
}
