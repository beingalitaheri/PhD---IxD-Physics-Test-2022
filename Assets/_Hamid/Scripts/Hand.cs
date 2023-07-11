using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

public class Hand : MonoBehaviour
{
    [SerializeField] private OVRHand my_ovr_hand;
    [SerializeField] private OVRHand.Hand which_hand;

    [Space]
    [SerializeField] public Transform IndexProximal;
    [SerializeField] public Transform MiddleProximal;
    [SerializeField] public Transform RingProximal;
    [SerializeField] public Transform PinkyProximal;

    [Space]
    [SerializeField] public Transform indexMiddle;
    [SerializeField] public Transform indexEnd;


    [Space]
    [SerializeField]public Transform IndexTip;
    [SerializeField]public Transform RingTip;
    [SerializeField]public Transform PinkyTip;
    [SerializeField]public Transform MiddleTip;

    [Space]
    [SerializeField]private ParticleSystem _plop;
    [SerializeField]private SkinnedMeshRenderer _meshRenderer;
    [SerializeField]private Material _materialDefault;
    [SerializeField]private Material _materialTransparent;

    public Subject<Hand> _OnStartGrasping = new Subject<Hand>();
    public Subject<Hand> _OnStopGrasping = new Subject<Hand>();


    public IObservable<Hand> OnStartGraping => _OnStartGrasping;
    public IObservable<Hand> onStopGrasping => _OnStopGrasping;

    public bool ISGrasping => _isGrasping;


    public OVRHand.Hand hand_type => which_hand;


    private bool _isGrasping;

    private Dictionary<OVRHand.HandFinger,IObservable<bool>> _onPinches = new Dictionary<OVRHand.HandFinger, IObservable<bool>>();

    private Dictionary<OVRHand.HandFinger, IObservable<float>> _onPinchEnded = new Dictionary<OVRHand.HandFinger, IObservable<float>>();


    public IObservable<float> OnPinchEnded(OVRHand.HandFinger finger)
    {
        

        if (!_onPinchEnded.ContainsKey(finger))
        {
            _onPinchEnded.Add(finger, CreateOnPinch(finger).Where(p=>!p).Select(_=>Time.time));
        }
        return _onPinchEnded[finger];


    }
    public IObservable<bool> OnPinch(OVRHand.HandFinger finger)
    {
        if (!_onPinches.ContainsKey(finger))
        {
            _onPinches.Add(finger, CreateOnPinch(finger));
        }
        return _onPinches[finger]; 
    
    }

    public IObservable<bool> CreateOnPinch(OVRHand.HandFinger finger)
    {
      var lastPinchStream =
            Observable.EveryUpdate().Select(_ => isPinching(finger));

     var currentPinchSteam =
            Observable.EveryUpdate().Select(_ => isPinching(finger)).Skip(1);

        return Observable.Zip(lastPinchStream, currentPinchSteam)
            .Where(p => p[0] != p[1])
            .Select(p => !p[0] && p[1]);
    }

    private void Start()
    {
        OnPinchStarted(OVRHand.HandFinger.Index).Subscribe(_ =>
        {
            Plop(IndexTip.position);

        }

        );

        OnPinchStarted(OVRHand.HandFinger.Middle).Subscribe(_ =>
        {
            Plop(MiddleTip.position);

        }

       );


        OnPinchStarted(OVRHand.HandFinger.Ring).Subscribe(_ =>
        {
            Plop(RingTip.position);

        }

       );

        OnPinchStarted(OVRHand.HandFinger.Pinky).Subscribe(_ =>
        {
            Plop(PinkyTip.position);

        }

       );

        OnPinchStarted(OVRHand.HandFinger.Index).Subscribe(_ =>
        {
            Plop(IndexTip.position);

        }

       );



    }

    private void Update()
    {
        CheckGrasping();
    }


    public bool IsTrackingGood()
    {
        return my_ovr_hand.HandConfidence == OVRHand.TrackingConfidence.High;
    }

    public Vector3 GetPosition()
    {

        return my_ovr_hand.transform.position;
    }

    public Quaternion GetRotation()
    {

        return my_ovr_hand.transform.rotation;

    }
    public Vector3 GetLocalPosition()
    {
        return my_ovr_hand.transform.localPosition;
    }
    public bool isPinching(OVRHand.HandFinger finger)

    {
        return my_ovr_hand.GetFingerIsPinching(finger);
    }

    public void CheckGrasping()
    {
        bool wasGrasping = _isGrasping;

        bool indexFlexed = CheckInRange(IndexProximal.localRotation.eulerAngles.z, 200, 320);
        bool middleFlexed = CheckInRange(MiddleProximal.localRotation.eulerAngles.z, 200, 310);
        bool ringFlexed = CheckInRange(RingProximal.localRotation.eulerAngles.z, 200, 310);
        bool pinkFlexed = CheckInRange(PinkyProximal.localRotation.eulerAngles.z, 200, 310);

        _isGrasping = indexFlexed && middleFlexed && ringFlexed && pinkFlexed;

        if (_isGrasping !=wasGrasping)

        {
            if (_isGrasping)
            {
                _OnStartGrasping.OnNext(this);
            }
            else
            {
                _OnStopGrasping.OnNext(this);
            }

        }





    }

    private bool CheckInRange(float value, float min, float max)
    {
        return value > min && value < max;
    }



    public IObservable<Unit> OnPinchStarted(OVRHand.HandFinger finger)
    {
        var lastPinchStream = Observable.EveryUpdate().Select(_ => isPinching(finger));

        var currentPinchStream = Observable.EveryUpdate().Select(_ => isPinching(finger)).Skip(1);

        return Observable.Zip(currentPinchStream, lastPinchStream).Where(p => p[0] && !p[1]).Select(x => Unit.Default);

    }



    public IObservable<Unit> OnPinchStopped(OVRHand.HandFinger finger)
    {
        var lastPinchStream = Observable.EveryUpdate().Select(_ => isPinching(finger));

      var currentPinchStream  = Observable.EveryUpdate().Select(_ => isPinching(finger)).Skip(1);

        return Observable.Zip(currentPinchStream, lastPinchStream).Where(p => !p[0] && p[1]).Select(x => Unit.Default);

    }

    private void Plop(Vector3 position)
    {

        _plop.transform.position = position;
        _plop.Emit(1);
    }

    public void MakeTransparent(bool transparent)
    {
        _meshRenderer.material = transparent ? _materialTransparent : _materialDefault;
    }

    public void ShowHand(bool show)
    {
        _meshRenderer.gameObject.SetActive(show);
    }
}
