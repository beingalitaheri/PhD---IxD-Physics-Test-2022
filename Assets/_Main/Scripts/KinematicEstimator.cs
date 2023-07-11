using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using System;

namespace _VIRAL._03_Scripts
{
    public class KinematicEstimator : MonoBehaviour
    {
        private IDisposable _velocityEstimatorDisposable;
        private Rigidbody _rigidbody;

        private Vector3 _referencePosition;

        private int sampleCount;
        private Vector3[] velocitySamples;
        private Vector3[] angularVelocitySamples;

        private Vector3 _priviousPosition;
        private Quaternion _priviousRotation;

        private int _velocitySampleCount = 5;
        private int _angularVelocitySampleCount = 5;

        public int VelocitySampleCount
        {
            get => _velocitySampleCount;
            set
            {
                angularVelocitySamples = new Vector3[value];
                _velocitySampleCount = value;
            }

        }

        public int angularVelocitySampleCount
        {
            get => _angularVelocitySampleCount;
            set
            {
                velocitySamples = new Vector3[value];
                angularVelocitySampleCount = value;
            }
        }

        private void Awake()
        {
            velocitySamples = new Vector3[_velocitySampleCount];
            angularVelocitySamples = new Vector3[_angularVelocitySampleCount];
        }
        // Start is called before the first frame update
        void Start()
        {
            _rigidbody = GetComponent<Rigidbody>();
        }

        public void StartEstimatingVelocity()
        {
            _velocityEstimatorDisposable?.Dispose();

            sampleCount = 0;
            _priviousPosition = _referencePosition;
            _priviousRotation = transform.rotation;

            _velocityEstimatorDisposable = Observable.EveryUpdate().Subscribe(_ =>
            {
                EstimateVelocity();
            }).AddTo(this);

        }

        public void StopEstimatingVelocity()
        {
            _velocityEstimatorDisposable?.Dispose();
        }

        private void EstimateVelocity()
        {
            int v = sampleCount % velocitySamples.Length;
            int w = sampleCount % angularVelocitySamples.Length;

            sampleCount = Math.Max(0, sampleCount + 1);

            _referencePosition = _rigidbody == null ? transform.position : (transform.position + _rigidbody.centerOfMass);

            float velocityFactor = 1.0f / Time.deltaTime;

            velocitySamples[v] = velocityFactor * (_referencePosition - _priviousPosition);

            Quaternion deltaRotation = transform.rotation * Quaternion.Inverse(_priviousRotation);

            float theta = 2.0f * Mathf.Acos(Mathf.Clamp(deltaRotation.w, -1.0f, 1.0f));

            if (theta > Math.PI)
            {
                theta -= 2.0f * Mathf.PI;
            }

            Vector3 angularVelocity = new Vector3(deltaRotation.x, deltaRotation.y, deltaRotation.z);

            if (angularVelocity.sqrMagnitude > 0.0f)
            {
                angularVelocity = theta * velocityFactor * angularVelocity.normalized;
            }

            angularVelocitySamples[w] = angularVelocity;

            _priviousPosition = _referencePosition;
            _priviousRotation = transform.rotation;
        }
        public Vector3 GetEstimatedVelocity()
        {
            Vector3 velocity = Vector3.zero;

            int velocitySampleCount = Mathf.Min(sampleCount, velocitySamples.Length);

            if (VelocitySampleCount != 0)
            {
                for (int i = 0; i < velocitySampleCount; i++)
                {
                    velocity += velocitySamples[i];
                }
                velocity *= (1.0f / velocitySampleCount);
            }
            if (velocity == Vector3.negativeInfinity || velocity == Vector3.positiveInfinity || float.IsNaN(velocity.x) || float.IsNaN(velocity.y) || float.IsNaN(velocity.z))
            {
                return Vector3.zero;
            }
            return velocity;
        }

        public Vector3 GetEstimatedAngularVelocity()
        {
            Vector3 angularVelocity = Vector3.zero;

            int angluarVelocitySampleCount = Mathf.Min(sampleCount, angularVelocitySamples.Length);


            if (_angularVelocitySampleCount != 0)
            {
                for (int i = 0; i < _angularVelocitySampleCount; i++)
                {
                    angularVelocity += angularVelocitySamples[i];
                }
                angularVelocity *= (1.0f / angularVelocitySampleCount);

            }

            return angularVelocity;
        }


    }
}