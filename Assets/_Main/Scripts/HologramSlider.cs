using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using DG.Tweening;
using UniRx;
using System;

namespace _VIRAL._03_Scripts
{
    public class HologramSlider : HologramUiComponent
    {
        [SerializeField] private float _min = 0;
        [SerializeField] private float _max = 10;
        [SerializeField] private float _value = 5;

        [Space]
        [SerializeField] private Transform _ring;
        [SerializeField] private Transform _minPos;
        [SerializeField] private Transform _maxPos;

        [SerializeField] private TextMeshPro _valueText;
        [SerializeField] private TextMeshPro _minText;
        [SerializeField] private TextMeshPro _maxText;

        private Tweener _sliderTweener;

        private bool _sliding = false;

        private float _ringPosition;

        public Subject<float> OnValueChanged = new Subject<float>();

        public float Value => _value;

        protected override void Awake()
        {
            base.Awake();

            _minText.text = _min.ToString();
            _maxText.text = _max.ToString();

            SetValue(_value);
            AdjustSlider();

        }
        private void Update()
        {
            if (!_sliding && _interactor)
            {
                _sliding = true;
                _icon.transform.DOScale(_iconInitialScale * 1.4f, 0.3f);

            }
            if (_sliding && !_interactor)
            {
                _sliding = false;
                _icon.transform.DOScale(_iconInitialScale, 0.3f);


            }
            if (_interactor)
            {
                CalculateValue(_interactor.transform.position);
                AdjustSlider();
            }
            Vector3 currentPos = _ring.localPosition;
            _ring.localPosition = Vector3.Lerp(currentPos, new Vector3(_ringPosition, currentPos.y, currentPos.z), 0.1f);
        }

        private void CalculateValue(Vector3 hitposition)
        {
            Vector3 hitLocalPosition = transform.InverseTransformPoint(hitposition);
            SetValue((hitLocalPosition.x - _minPos.localPosition.x) / GetScale() + _min);

            AdjustSlider();
        }

        private void SetValue(float value)
        {
            _value = Mathf.Clamp(value, _min, _max);

            OnValueChanged.OnNext(_value);

            _valueText.text = Math.Round(_value, 1).ToString();
        }

        private void AdjustSlider()
        {
            _ringPosition = _minPos.localPosition.x + (_value - _min) * GetScale();
        }

        private float GetScale()
        {
            return GetLength() / (_max - _min);
            //float a = GetLength() / (_max - _min);
            //return a;
        }

        private float GetLength()
        {
            return _maxPos.localPosition.x - _minPos.localPosition.x;
            //float a = _maxPos.localPosition.z - _minPos.localPosition.z;
            //return a;
        }
        public override void Click(Vector3 Position)
        {

        }
    }
}