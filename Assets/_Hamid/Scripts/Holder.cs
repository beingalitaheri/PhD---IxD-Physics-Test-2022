using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UniRx;
using TMPro;

namespace _VIRAL._03_Scripts
{
    public class Holder : MonoBehaviour
    {

        [SerializeField] protected HolderType _holderType = HolderType.Undefined;
        [SerializeField] public bool _captureKinematically = false;
        //[SerializeField] protected Transform _textContainer;
        //[SerializeField] protected TextMeshPro _tmp;
        [SerializeField] public float _releaseDistance = 0.1f;
        [SerializeField] private bool _restrictAllowedObjects = false;
        [SerializeField] private ObjectType[] _allowedObjectTypes;


        public IObservable<Holdable> OnCaptured => _onCaptured;
        public IObservable<Holdable> OnReleased => _onReleased;


        protected Holdable _capturedObject;
        protected Holdable _closestHoldable;


        private List<Holdable> _reachableHoldables = new List<Holdable>();

        protected readonly Subject<Holdable> _onCaptured = new Subject<Holdable>();
        protected readonly Subject<Holdable> _onReleased = new Subject<Holdable>();


        protected readonly Subject<Holdable> _newClosestDetected = new Subject<Holdable>();
        protected readonly Subject<Holdable> _noMoreObjectDetected = new Subject<Holdable>();



        public Rigidbody _rb;
        private CenterEyeAnchor _centerEye;



        protected void Awake()
        {
            _rb = GetComponent<Rigidbody>();
            _centerEye = FindObjectOfType<CenterEyeAnchor>();
        }



        // Start is called before the first frame update
        protected virtual void Start()
        {

        }

        private void OnTriggerEnter(Collider other)
        {
            if (_capturedObject) return;
            //if (!other.attachedArticulationBody) return;

            Holdable holdable = other.attachedRigidbody.GetComponent<Holdable>();

            bool allowedToHold = CheckObjectAllowed(holdable) && CheckIfCanHoldThis(holdable);
            if (holdable && !_reachableHoldables.Contains(holdable))
            {

                _reachableHoldables.Add(holdable);
                CheckClosest();
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (!other.attachedRigidbody) return;
            Holdable holdable = other.attachedRigidbody.GetComponent<Holdable>();
            if (holdable && _reachableHoldables.Contains(holdable))
            {
                _reachableHoldables.Remove(holdable);
                holdable.Highlight(false);
                CheckClosest();
            }


        }

        private void CheckClosest()
        {
            float closestDistance = 1000;
            _closestHoldable = null;

            _reachableHoldables.ForEach(Holdable =>
             {
                 Holdable.Highlight(false);

                 float distance = Vector3.Distance(Holdable.transform.position, transform.position);

                 if (distance < closestDistance)
                 {
                     closestDistance = distance;
                     _closestHoldable = Holdable;
                 }

             });

            if (_closestHoldable == null)
            {
                _noMoreObjectDetected.OnNext(null);
            }
            else
            {
                _closestHoldable.Highlight(true);
                _newClosestDetected.OnNext(_closestHoldable);
            }
        }


        internal bool CheckIfCanHoldThis(Holdable holdabale)
        {
            return holdabale._allowedHolderTypes.Contains(_holderType);
        }

        internal bool CheckObjectAllowed(Holdable holdabale)
        {
            if (!_restrictAllowedObjects) return true;
            return _allowedObjectTypes.Contains(holdabale.Interactable.ObjectType);
        }


        public void Capture()
        {
            if (_capturedObject) return;

            if (_closestHoldable)
            {
                _closestHoldable.Capture(this);
                _capturedObject = _closestHoldable;

                _onCaptured.OnNext(_capturedObject);
            }
        }
        public void Release()
        {
            if (_capturedObject)
            {
                _capturedObject.Release();
                _capturedObject = null;
                CheckClosest();

            }

        }
    }
}