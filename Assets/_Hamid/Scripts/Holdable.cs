using System;
using UnityEngine;
using UniRx;
using System.Collections.Generic;

namespace _VIRAL._03_Scripts
{
    [RequireComponent(typeof(Interactable))]
    public class Holdable : MonoBehaviour
    {

        [SerializeField] protected bool _captureKinematically = true;
        [SerializeField] public List<HolderType> _allowedHolderTypes = new List<HolderType>();
        [SerializeField] protected List<Holdable> _subParts = new List<Holdable>();
        [SerializeField] protected Joint _linkedJoint;

        [SerializeField] private bool _kinemtaicOnCaptured = true;

        public Vector3 InitialLocalScale => _initialLocalScale;
        public Holder Holder => _holder;
        public Rigidbody Rb => _rb;

        public ConfigurableJoint Joint => _joint;
        public Interactable Interactable => _interactable;
        public bool IsCaptured => _isCaptured;

        public bool MarkedForDestruction => _markedForDestruction;
        public IObservable<Holdable> OnCaptured => _onCaptured;
        public IObservable<Holdable> OnReleased => _onReleased;



        private Interactable _interactable;
        private Holder _holder;

        private Rigidbody _rb;
        private ConfigurableJoint _joint;

        private Vector3 _capturePoint;


        private bool _markedForDestruction = false;
        private bool _isCaptured = false;

        private Transform _initialParent;
        private Vector3 _initialLocalScale = Vector3.one;
        private Vector3 __initialLocalPosition;

        private bool _initialKinematicState;
        private bool _initialGravityState;

        private Vector3 _linkedJointConnectedAnchor;

        private KinematicEstimator _kinematicEstimator;


        private readonly Subject<Holdable> _onCaptured = new Subject<Holdable>();
        private readonly Subject<Holdable> _onReleased = new Subject<Holdable>();
        // Start is called before the first frame update


        public Transform _visualSnapHandRight;
        public Transform _visualSnapHandLeft;


        [SerializeField] private int handSpring = 3000;
        [SerializeField] private int hand_angular = 1000;
        [SerializeField] private int hand_damper = 50;
        [SerializeField] private int hand_breakForce = 10000;



        private void Awake()
        {
            _interactable = GetComponent<Interactable>();
            _rb = GetComponent<Rigidbody>();
            _kinematicEstimator = gameObject.AddComponent<KinematicEstimator>();
            Initialize();

            ShowVisualSnapHand(false, OVRHand.Hand.HandLeft);
            ShowVisualSnapHand(false, OVRHand.Hand.HandRight);

        }
        void Start()
        {
             _interactable = GetComponent<Interactable>();
             _rb = GetComponent<Rigidbody>();
             _kinematicEstimator = gameObject.AddComponent<KinematicEstimator>();

             Initialize();

        }

        private void Update()
        {
            CheckDetachJoint();
        }

        private void CheckDetachJoint()
        {
            if (!_joint || !_holder) return;

            if (!_captureKinematically || !_holder._captureKinematically)
            {
                float distance = Vector3.Distance(transform.TransformPoint(_capturePoint), _holder.transform.position);

                if (distance > _holder._releaseDistance)
                {
                    _holder.Release();
                }
            }

        }

        private void OnJointBreak(float breakForce)
        {
            if (!_captureKinematically || (_holder || !_holder._captureKinematically))
            {
                _holder.Release();
            }
        }

        private void Initialize()
        {
            _initialLocalScale = transform.localScale;
            __initialLocalPosition = transform.localPosition;
            _initialKinematicState = _rb.isKinematic;
            _initialGravityState = _rb.useGravity;

            if (_linkedJoint)
            {
                _linkedJointConnectedAnchor = _linkedJoint.connectedAnchor;
            }
        }
        public void Highlight(bool highlight)
        {
            _interactable.AdjustRingScale(highlight ? 1.3f : 1f);
        }

        public void Capture(Holder holder)
        {
            if (_holder)
            {
                _holder.Release();
            }

            if (_captureKinematically || holder._captureKinematically)
            {
                _rb.isKinematic = true;
                _rb.useGravity = false;

                _kinematicEstimator.StartEstimatingVelocity();

                transform.parent = holder.transform;
                _rb.interpolation = RigidbodyInterpolation.None;

            }
            else
            {
                _rb.useGravity = false;
                CreateJoint(holder);
            }

            _isCaptured = true;
            _holder = holder;

            //_interactable.Ring.gameObject.SetActive(false);
            //transform.parent = holder.transform;

            _onCaptured.OnNext(this);
        }

        private void CreateJoint(Holder holder)
        {

            _joint = gameObject.AddComponent<ConfigurableJoint>();
            _joint.connectedBody = holder._rb;

            float spring, angularspring, damper, maximumForce = 0;

            if (holder.GetComponent<HandGrabber>())
            {
                _joint.autoConfigureConnectedAnchor = true;
                spring = handSpring;
                angularspring = hand_angular;
                damper = hand_damper;
                maximumForce = 10000;
                _capturePoint = transform.InverseTransformPoint(holder.transform.position);


                _joint.breakForce = hand_breakForce;
                _joint.breakTorque = hand_breakForce;
            }
            else
            {
                _joint.autoConfigureConnectedAnchor = false;
                _joint.anchor = holder.transform.position;
                _joint.connectedAnchor = Vector3.zero;
                spring = 1000;
                angularspring = 100;
                damper = 100;
                maximumForce = 1000;
                _capturePoint = _rb.centerOfMass;

            }

            ConfigurableJointMotion motion = ConfigurableJointMotion.Free;

            _joint.xMotion = _joint.yMotion = _joint.zMotion = motion;
            _joint.angularXMotion = _joint.angularYMotion = _joint.angularZMotion = motion;

            JointDrive motionDrive = new JointDrive();
            motionDrive.positionSpring = spring;
            motionDrive.positionDamper = damper;
            motionDrive.maximumForce = maximumForce;



            JointDrive angularDrive = new JointDrive();
            angularDrive.positionSpring = angularspring;
            angularDrive.positionDamper = damper;
            angularDrive.maximumForce = maximumForce;


            _joint.xDrive = _joint.yDrive = _joint.zDrive = motionDrive;
            _joint.angularXDrive = _joint.angularYZDrive = angularDrive;

            _joint.breakForce = 5000;
            _joint.breakTorque = 5000;


        }

        public void Release()
        {
            if(_kinemtaicOnCaptured) 
            {
                _rb.isKinematic = _initialKinematicState;
                _rb.useGravity = _initialGravityState;
            }
            _interactable.Ring.gameObject.SetActive(false);
            _isCaptured= false;
            _holder = null;
            transform.parent = _initialParent;

            if (_rb.useGravity) 
            {
                _rb.velocity = _kinematicEstimator.GetEstimatedVelocity();
                _rb.angularVelocity = _kinematicEstimator.GetEstimatedAngularVelocity();
            }

            _kinematicEstimator.StopEstimatingVelocity();

           /* if (!_holder)
            {
                return;
            }

            if (_captureKinematically) //** || _holder.CaptureKinematically
            {
                _rb.isKinematic = _initialKinematicState;
                transform.parent = _initialParent;


                if (_rb.useGravity)
                {
                    _rb.velocity = _kinematicEstimator.GetEstimatedVelocity();
                    _rb.angularVelocity = _kinematicEstimator.GetEstimatedAngularVelocity();
                }



                _kinematicEstimator.StopEstimatingVelocity();

            }

            else
            {
                Destroy(_joint);
                _joint = null;
            }
            _rb.useGravity = _initialGravityState;
            _rb.interpolation = RigidbodyInterpolation.Interpolate;


            //_interactable.Ring.gameObject.SetActive(true);
            _isCaptured = false;
            _holder = null;
            //  transform.parent = _initialParent;

            //if (_rb.useGravity)
            //{
            //    _rb.velocity = _kinematicEstimator.GetEstimatedVelocity();
            //    _rb.angularVelocity = _kinematicEstimator.GetEstimatedAngularVelocity();
            //}

            //_kinematicEstimator.StopEstimatingVelocity(); */

            _onReleased.OnNext(this);

        }


        public void RefreshPhysics()
        {
            RefreshSubJoints();
        }

        public void RefreshSubJoints()
        {

            _subParts.ForEach(p =>
            {
                p.Refreshjoint();
                p.RefreshSubJoints();

            });

        }

        public void Refreshjoint()
        {
            if (_linkedJoint)
            {
                _linkedJoint.connectedAnchor = _linkedJointConnectedAnchor;
            }

        }


        public void CollapseAndDestroy(float time)
        {

        }

        public void ShowVisualSnapHand(bool show, OVRHand.Hand handType)
        {
            if (_visualSnapHandRight && handType == OVRHand.Hand.HandRight)
            {
                _visualSnapHandRight.gameObject.SetActive(show);
            }

            if (_visualSnapHandLeft && handType == OVRHand.Hand.HandLeft)
            {
                _visualSnapHandLeft.gameObject.SetActive(show);

            }

        }

    }
}
