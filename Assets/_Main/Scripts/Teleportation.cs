using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UniRx;
using DG.Tweening;

public class Teleportation : MonoBehaviour
{

    [SerializeField] private Laser_Pointer _laserPointer;  
    [SerializeField] private Transform _startPoint;
    [SerializeField] private Transform _target;
    [SerializeField] private Transform _virtualRig;
    [SerializeField] private Transform _virtualPosition;

    private LocomotionManager _locomotion_manager;


    private bool _startAiming = false;
    private bool _isAimingTeleportation = false;
    private bool _canTeleport = false;
    private bool _targetVisible = false;
    private Vector3 _aimingPoint;
    private IDisposable _delayAimDisposable;
    private int _layerMask = 1 << 4;

    //tweeners
    private Tween _targetTweener;
    private Sequence _movementSequence;

    public Subject<Teleportation> OnStartAiming = new Subject<Teleportation>();
    public Subject<Teleportation> OnStopAiming = new Subject<Teleportation>();
    public Subject<Teleportation> onShowTarget = new Subject<Teleportation>();
    public Subject<Teleportation> onHideTarget = new Subject<Teleportation>();
    public Subject<Teleportation> OnStartTeleporting = new Subject<Teleportation>();
    public Subject<Teleportation> OnStopTeleporting = new Subject<Teleportation>();



    [Space]
    [SerializeField]private AudioSource _audioSource;
    [SerializeField]private AudioClip _soundStartAiming;
    [SerializeField]private AudioClip _soundTeleport;


    private void Awake()
    {
        _locomotion_manager = GetComponentInParent<LocomotionManager>();

        CancelTeleportation();

        _target.localScale = Vector3.one * 0.01f;

        InitializeAudio();

    }

    public void HandleTeleportation()
    {

        if (_locomotion_manager.RightHand.isPinching(OVRHand.HandFinger.Index))
        {

            HandlePinch();
        }

        if (!_locomotion_manager.RightHand.isPinching(OVRHand.HandFinger.Index))
        {
            HandleRelease();
        }


    
    }

    //private void HandleRelease()
    //{
    //    if (!_startAiming && !_isAimingTeleportation)
    //    {
    //        _startAiming = true;
    //        OnStartAiming.OnNext(this);
    //        _delayAimDisposable?.Dispose();
    //        _delayAimDisposable = Observable.Timer(TimeSpan.FromSeconds(0.05f)).Subscribe(c =>

    //        {

    //            _isAimingTeleportation = true;
    //            _startAiming = false;


    //        }

    //        );

    //    }

    //    if (_isAimingTeleportation)
    //    {
    //       // _laserPointer.
            
    //    }

    //}

    private void HandlePinch()
    {
        if (!_startAiming && !_isAimingTeleportation)
        {
            _startAiming = true;
            OnStartAiming.OnNext(this);
            _delayAimDisposable?.Dispose();
            _delayAimDisposable = Observable.Timer(TimeSpan.FromSeconds(0.05f)).Subscribe(c =>
              {
                  _isAimingTeleportation = true;
                  _startAiming = false;

              });
        }

        if (_isAimingTeleportation)
        {
            //_laserPointer.
            _laserPointer.Activate(true);
            
        }

        RaycastHit hit;

        if (Physics.Raycast(_startPoint.position,_startPoint.forward,out hit,10,_layerMask))
        {
            if (_isAimingTeleportation)
            {
                _laserPointer.DrawCurveValid(_startPoint.position, hit.point);

                ShowTarget(true);
                _canTeleport = true;
                _target.position = hit.point;
                _target.position = new Vector3(_target.position.x, _target.position.y + 0.02f, _target.position.z);
                _aimingPoint = hit.point;
            }

        }
        else
        {
            _canTeleport = false;
            ShowTarget(false);
            _laserPointer.DrawCurveDenied(_startPoint.position, _startPoint.position + _startPoint.forward * 2);
        }

        UpdateVirtualRig();
    }

    private void HandleRelease()
    {
        if (_isAimingTeleportation && _canTeleport)
        {
            _isAimingTeleportation = false;
            OnStopAiming.OnNext(this);
            _target.gameObject.SetActive(false);

            //Teleport(_aimingPoint);
            Teleport();

            ShowTarget(false);

        }
        _laserPointer.Activate(false);
    }

    private void UpdateVirtualRig()
    {
        _virtualRig.localPosition = new Vector3(_locomotion_manager.CenterEye.localPosition.x, 0, _locomotion_manager.CenterEye.localPosition.z);
        _virtualPosition.localRotation = Quaternion.Euler(0, _locomotion_manager.CenterEye.localRotation.eulerAngles.y, 0);
      
    }

    private void ShowTarget(bool show)
    {
        if (show && !_targetVisible)
        {
            onShowTarget.OnNext(this);
            _target.gameObject.SetActive(true);
            _targetTweener?.Kill();
            _targetTweener = _target.DOScale(1, 0.25f).SetEase(Ease.OutBack);
            _targetVisible = true;
        }
        if (!show && _targetVisible)
        {
            onHideTarget.OnNext(this);
            _targetVisible = false;
            _targetTweener?.Kill();
            _targetTweener = _target.DOScale(0.01f, 0.25f).SetEase(Ease.OutBack).OnComplete(() =>
                {
                    _target.gameObject.SetActive(false);

                }
                );

        }
    }

    public void CancelTeleportation()
    {
        _movementSequence?.Kill();

        if (_isAimingTeleportation)
        {
            _isAimingTeleportation = false;
            ShowTarget(false);
            _laserPointer.Activate(false);
        }
    
    
    }

    private void Teleport()
    {
        OnStartTeleporting.OnNext(this);

        Vector3 adjustedPosition = _virtualRig.position;

        _locomotion_manager.CameraRig.transform.position = adjustedPosition;
        _locomotion_manager.CameraRig.transform.rotation = _virtualRig.rotation;

        OnStopTeleporting.OnNext(this); 
    
    }



    private void InitializeAudio()
    {
        onShowTarget.Subscribe( _=>
            {

            PlaySound(_soundStartAiming, 0.3f);
        
            }
            );
        OnStartTeleporting.Subscribe(_=>
            {
                PlaySound(_soundTeleport,0.7f);
        
            }

            );
    }

    private void PlaySound(AudioClip clip,float volumn)
    {

        _audioSource.Stop();
        _audioSource.volume = volumn;
        _audioSource.PlayOneShot(clip);
    }

}
