using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

public class LocomotionManager : MonoBehaviour
{
    [Space]
    [SerializeField] public BoolReactiveProperty TeleportationActive = new BoolReactiveProperty(true);
    [SerializeField] public BoolReactiveProperty MimotionActive = new BoolReactiveProperty(true);

    [Space]
    [SerializeField] private Teleportation _teleportation;
    [SerializeField] private Mimotion _mimotion;


    public Transform CameraRig => _cameraRig;
    public Transform CenterEye => _centerEyeAnchor;

    public Hand LeftHand => _leftHand;
    public Hand RightHand => _rightHand;

    private Transform _cameraRig;
    private Transform _centerEyeAnchor;

    private Hand _leftHand;
    private Hand _rightHand;

    private IDisposable _handleTeleporationDisposable;
    private IDisposable _handleMimotionDisposable;

    private void Awake()
    {
        _cameraRig = FindObjectOfType<OVRCameraRig>().transform;
        _centerEyeAnchor = FindObjectOfType<CenterEyeAnchor>().transform;
        _leftHand = FindObjectOfType<Hand>().GetComponent<Hand>();
        _rightHand = FindObjectOfType<Hand>().GetComponent<Hand>();

    }

    private void Start()
    {
        StartCheckingTeleportation();
        StartCheckingMimotion();
        InitalizeMimotionEvents();
    }

    private void InitalizeMimotionEvents()
    {
        _mimotion.OnStartMoving.Subscribe(_m =>
        {

            _handleTeleporationDisposable?.Dispose();
            _teleportation.CancelTeleportation();

        }

        );

        _mimotion.OnStopMoving.Subscribe(_m =>

        {
            Observable.Timer(TimeSpan.FromSeconds(0.5f)).Subscribe(_t =>
            {
                StartCheckingTeleportation();
            }
                    );
        }
        );

    }

    private void StartCheckingMimotion()
    {
        _handleMimotionDisposable?.Dispose();
        _handleMimotionDisposable = Observable.EveryUpdate().Subscribe(_t =>
        {
            if (MimotionActive.Value && _leftHand.IsTrackingGood() && _rightHand.IsTrackingGood())
            {
                _mimotion.HandleMimotion();
            }


        }

        );

    }

    private void StartCheckingTeleportation()
    {
        _handleTeleporationDisposable?.Dispose();

        _handleTeleporationDisposable = Observable.EveryUpdate().Subscribe(_t =>

        {
            if (TeleportationActive.Value && _rightHand.IsTrackingGood())
            {
                _teleportation.HandleTeleportation();

            }

            else
            {
                _teleportation.CancelTeleportation();
            }



        });
    }
}
