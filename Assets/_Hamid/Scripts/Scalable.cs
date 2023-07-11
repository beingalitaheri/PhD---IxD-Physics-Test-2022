using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;

namespace _VIRAL._03_Scripts
{
    [RequireComponent(typeof(Interactable))]
    public class Scalable : MonoBehaviour
    {
        [SerializeField] private float _minScaleFactor = 0.5f;
        [SerializeField] private float _maxScaleFactor = 1.5f;
        [SerializeField] private float _pinchDistanceFactor = 1.5f;


        private float _currentScaleFactor = 1f;
        private Vector3 _initialScale;
        private Interactable _interactable;
        private TwoHandedPincher _twoHandedPincher;

        private bool _isScaling = false;

        private float _doublePinchStartDistance;
        private float _doublePinchStartValue;


        private void Awake()
        {
            _twoHandedPincher = FindObjectOfType<TwoHandedPincher>();
            _interactable = GetComponent<Interactable>();
            _initialScale = transform.localScale;
        }


        // Start is called before the first frame update
        private void Start()
        {
            InitializePincher();
            UpdateRing();
        }





        private bool CanBeScaled()
        {
            bool isEnabled = gameObject.activeSelf && enabled;
            bool isFocused = _interactable.Focused;
            bool isCaptured = _interactable.Holdable && _interactable.Holdable.IsCaptured;

            return isEnabled && isFocused && !isCaptured;
        }

        private void InitializePincher()
        {
            _twoHandedPincher.OnTwoHandedPinchStart.Subscribe(distance =>
            {
                if (CanBeScaled() && !_isScaling)
                {
                    StartScaling(distance);
                }


            }).AddTo(this);

            _twoHandedPincher.OnTwoHandedPinchEnd.Subscribe(distance =>
            {
                StopScaling();
            }).AddTo(this);

            //********** I changed TwoHandedPinchDistance
            _twoHandedPincher.TwoHandedPinchDistance.Subscribe(distance =>
            {
                if (_isScaling)
                {
                    if (CanBeScaled())
                    {
                        float pinchDelta = distance - _doublePinchStartDistance;
                        AdjustScale(pinchDelta);
                    }
                    else
                    {
                        StopScaling();
                    }

                }

            }).AddTo(this);

        }


        private void StartScaling(float distance)
        {
            _isScaling = true;
            _doublePinchStartDistance = distance;
            _doublePinchStartValue = _currentScaleFactor;
            _interactable.Ring.ShowScaleRings(true);
        }

        private void AdjustScale(float delta)
        {
            _interactable.Ring.ShowScaleRings(true);

            float newScaleFactor = _doublePinchStartValue + (delta * _pinchDistanceFactor);

            if (newScaleFactor > _minScaleFactor && newScaleFactor < _maxScaleFactor)
            {
                _interactable.Ring.ShowScaleRings(true);
                _currentScaleFactor = newScaleFactor;
                transform.localScale = _initialScale * newScaleFactor;

                _interactable.Holdable.RefreshPhysics();

                UpdateRing();
            }
        }

        private void UpdateRing()
        {
            _interactable.Ring.SetRingScale(1 + (_currentScaleFactor - _minScaleFactor) / (_maxScaleFactor - _minScaleFactor));
        }

        private void StopScaling()
        {
            _isScaling = false;
            _doublePinchStartValue = 0;
            _interactable.Ring.Activate(false);
            _interactable.Ring.ShowScaleRings(false);
        }



    }
}