using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using System;

namespace _VIRAL._03_Scripts
{
    public class TwoHandedPincher : MonoBehaviour
    {
        [SerializeField] private OVRHand.HandFinger _pinchFinger = OVRHand.HandFinger.Index;

        private Hand _leftHand;
        private Hand _rightHand;

        private IObservable<float> _twoHandedPinchDistance;
        private IObservable<bool> _onTwoHandedPinch;
        public IObservable<float> _TwoHandedPinchDistance;

        private void Awake()
        {
            //** I changed HandLeft and HandRight to Hand
            //_leftHand = FindObjectOfType<HandLeft>().GetComponent<Hand>();
            //_rightHand = FindObjectOfType<HandRight>().GetComponent<Hand>();


            _leftHand = GameObject.FindGameObjectWithTag("Left_hand").GetComponent<Hand>();
            _rightHand = GameObject.FindGameObjectWithTag("Right_hand").GetComponent<Hand>();
        }
        private IObservable<bool> CreateOnTwoHandedPinch()
        {
            var leftHandPinch = _leftHand.OnPinch(_pinchFinger);
            var rightHandPinch = _rightHand.OnPinch(_pinchFinger);
            var combinePinch = Observable.CombineLatest(leftHandPinch, rightHandPinch);

            return combinePinch.Select(p => p[0] && p[1]);
        }
        public IObservable<float> OnTwoHandedPinchStart
        {
            get
            {
                if (_onTwoHandedPinch == null)
                {
                    _onTwoHandedPinch = CreateOnTwoHandedPinch();
                }

                return _onTwoHandedPinch.Where(p => p).Select(_ => PinchDistance());

            }
        }
        //Week 4
        public IObservable<float> OnTwoHandedPinchEnd 
        {
            get 
            {
                var leftHandPinch = _leftHand.OnPinch(_pinchFinger);
                var rightHandPinch = _rightHand.OnPinch(_pinchFinger);

                return OnTwoHandedPinchStart.SelectMany(_ =>
                leftHandPinch.Merge(rightHandPinch)
                .Take(1)
                .Select(p => PinchDistance())
                );
            }
        }

        //Week 3
        /*public IObservable<float> OnTwoHandedPinchEnd
        {
            get
            {
                if (_onTwoHandedPinch == null) 
                {
                    _onTwoHandedPinch= CreateOnTwoHandedPinch();
                }
                return _onTwoHandedPinch.Where(p => !p).Select(_ => PinchDistance());
            }
        }*/
        //
        public IObservable<float> TwoHandedPinchDistance
    {
        get
        {
            var leftHandPinch = _leftHand.OnPinch(_pinchFinger);
            var rightHandPinch = _rightHand.OnPinch(_pinchFinger);

            return OnTwoHandedPinchStart
                .SelectMany(p =>
                Observable.EveryUpdate().Select(x => PinchDistance())
                .TakeUntil(leftHandPinch.Merge(rightHandPinch)));

        }
    }

        private float PinchDistance()
        {
            return Vector3.Distance(_leftHand.GetPosition(), _rightHand.GetPosition());


        }
    }
}
