using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;

public class Mimotion : MonoBehaviour
{
    private Vector3 _cameraRigCurrentPosition;
    private bool _isMovingWithLeftHand = false;
    private bool _isMovingWithRightHand = false;
    private Vector3 _handStart;
    private Vector3 _currentHandPosition;

    private Hand _leftHand;
    private Hand _rightHand;
    private Transform _cameraRig;

    public Subject<Mimotion> OnStartMoving = new Subject<Mimotion>();
    public Subject<Mimotion> OnStopMoving = new Subject<Mimotion>();
    

    // Start is called before the first frame update
    void Awake()
    {
        _leftHand = GameObject.FindGameObjectWithTag("Left_hand").GetComponent<Hand>();
        _rightHand = GameObject.FindGameObjectWithTag("Right_hand").GetComponent<Hand>();


        _cameraRig = FindObjectOfType<OVRCameraRig>().transform;
        
    }

    // Update is called once per frame
    void Start()
    {
        _cameraRigCurrentPosition = _cameraRig.transform.position;

        _leftHand.OnStartGraping.Subscribe(h =>
        {

            StartMoving(_leftHand.GetLocalPosition(),_leftHand.hand_type);

        }

        );

        _leftHand.onStopGrasping.Subscribe(h =>
       {
           CheckStop();

       }

       );


        _rightHand.OnStartGraping.Subscribe(h =>
       {
           StartMoving(_rightHand.GetLocalPosition(),_rightHand.hand_type);

       }

        );

        _rightHand.onStopGrasping.Subscribe( h=>

        {
            CheckStop();
        
        }

        );

        
    }

    private void StartMoving(Vector3 handPosition, OVRHand.Hand handtype)
    {
        _cameraRigCurrentPosition = _cameraRig.transform.position;
        _handStart = handPosition;

        _isMovingWithRightHand = (handtype == OVRHand.Hand.HandRight);
        _isMovingWithLeftHand = (handtype == OVRHand.Hand.HandLeft);

        OnStartMoving.OnNext(this);
    }

    private void CheckStop()
    {
        if (!_leftHand.ISGrasping && !_rightHand.ISGrasping)
        {
            _isMovingWithLeftHand = false;
            _isMovingWithRightHand = false;

            OnStopMoving.OnNext(this);

        }
    
    }

    public void HandleMimotion()
    {
        if (_isMovingWithLeftHand || _isMovingWithRightHand)
        {
            _currentHandPosition = _isMovingWithLeftHand ? _leftHand.GetLocalPosition() : _rightHand.GetLocalPosition();

            Vector3 delta = _handStart - _currentHandPosition;
            Vector3 newPos = _cameraRigCurrentPosition + new Vector3(delta.x, 0, delta.z);

            _cameraRig.transform.position = Vector3.Lerp(_cameraRig.transform.position, newPos, 0.1f);
        }
    
    }
}
