using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

namespace _VIRAL._03_Scripts
{
    [RequireComponent(typeof(Rigidbody))]
    public class Interactable : MonoBehaviour
    {
        [SerializeField] private ObjectType _objectType = ObjectType.undefined;
        [SerializeField] private GameObject _ringPrefab;
        [SerializeField] private float _ringScale = 0.1f;
        [SerializeField] private bool _canBeFocused = true;

        private Holdable _holdable;
        private Scalable _scalable;
        public ObjectType ObjectType => _objectType;


        private bool _focused;
        private SwellingRing _ring;

        public Holdable Holdable => _holdable;
        public Scalable Scalable => _scalable;

        public bool Focused => _focused;

        public SwellingRing Ring => _ring;


        //private Sequence _ringScaleTweener;
        private Tweener _ringScaleTweener;

        private CenterEyeAnchor _centerEye;

        private Rigidbody _rb;

        private void Awake()
        {
            _rb = GetComponent<Rigidbody>();
            _ring = Instantiate(_ringPrefab, transform.position, transform.rotation).GetComponent<SwellingRing>();
            _ring.transform.localScale = Vector3.one * _ringScale;
            _centerEye = FindObjectOfType<CenterEyeAnchor>();

            _holdable = GetComponent<Holdable>();
            _scalable = GetComponent<Scalable>();
        }

        private void Start()
        {
            Focus(false);
            _ring.ShowScaleRings(false);
        }
        private void Update()
        {
            _ring.transform.position = _rb.worldCenterOfMass;
            CheckGaze();
        }

        private void CheckGaze()
        {

            if (_centerEye.IsLookingAt(transform, 2f, 0.05f))
            {
                if (!_focused)
                {
                    Focus(true);
                }

            }
            else
            {
                if (_focused)
                {
                    Focus(false);
                }
            }
        }

        private void Focus(bool focus)
        {
            _focused = focus;
            _ring.Show(focus);

            if (!focus)
            {
                _ring.Activate(false);
            }

        }

        public void AdjustRingScale(float multiplier)
        {
            _ringScaleTweener?.Kill();
            _ringScaleTweener = _ring.transform.DOScale(Vector3.one * _ringScale * multiplier, 0.2f).SetEase(Ease.OutBack);
            // Assign the Tweener object directly
            //Tweener _ringScaleTweener = _ring.transform.DOScale(Vector3.one * _ringScale * multiplier, 0.2f).SetEase(Ease.OutBack);


        }
    }
}