using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using DG.Tweening;
using System;

namespace _VIRAL._03_Scripts
{
    public class HologramHint : MonoBehaviour
    {
        [SerializeField] private Transform _container;
        [SerializeField] private TextMeshPro _textHint;

        private HologramUiComponent _uiComponent;
        private bool _hintVisible = true;
        private Tweener _hintTweener;
        private Transform _centerEye;
        private float _scaleMultiplier;

        private void Awake()
        {
            _centerEye = FindObjectOfType<CenterEyeAnchor>().transform;
        }
        private void Start()
        {
            ShowHint(false);
        }



        private void Update()
        {
            HandleOrientation();
        }

        public void ShowHint(bool show)
        {
            if (!_hintVisible && show)
            {
                _hintTweener?.Kill();
                gameObject.SetActive(true);
                _hintVisible = true;
                _hintTweener = transform.DOScale(Vector3.one * 1f, 0.1f);
            }

            if (_hintVisible && !show)
            {
                _hintTweener?.Kill();
                _hintVisible = false;
                _hintTweener = transform.DOScale(Vector3.one * 0.01f, 0.1f).
                    OnComplete(() =>
                    {
                        gameObject.SetActive(false);
                    }
                    );
            }
        }
        public void SetUiComponent(HologramUiComponent componenet)
        {
            _uiComponent = componenet;
        }

        public void SetText(string text)
        {
            _textHint.text = text;
        }

        private void HandleOrientation()
        {
            Vector3 Forward = (transform.position - _centerEye.position).normalized;

            Vector3 offset = new Vector3(0, 0.02f, 0);

            Vector3 newPos = _uiComponent.transform.position + offset - Forward.normalized * 0.02f;

            Vector3 releativePos = _centerEye.position - transform.position;
            float yAngle = Quaternion.LookRotation(releativePos, Vector3.up).eulerAngles.y;
            Quaternion newRot = Quaternion.Euler(0, yAngle, 0);


            transform.SetPositionAndRotation(newPos, newRot);

            float distance = Vector3.Distance(transform.position, _centerEye.position);
            _centerEye.localScale = Vector3.one * Math.Max(0.3f, distance);
        }
    }
}