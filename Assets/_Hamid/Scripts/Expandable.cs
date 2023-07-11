using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UniRx;
using UnityEngine;

namespace _VIRAL._03_Scripts
{
    public class Expandable : MonoBehaviour
    {
        #region Editor 
        [SerializeField] private HologramButton _hologramButton;
        [SerializeField] private SwellingRing _swellingRing;
        [SerializeField] private List<MovingPart> _parts = new List<MovingPart> ();
        [SerializeField] private ViralSettings _viralSettings;
        #endregion
        //
        #region Private
        private bool _focused = false;
        private bool _expanded = false;
        private bool _canBeExpanded = false;
        private Sequence _expandSequence;
        private float _sequenceDuration;
        private TwoHandedPincher _twoHandedPincher;
        private CenterEyeAnchor _centerEye;
        private float _doublePinchStartDistancce;
        private float _doublePinchStartValue;
        #endregion
        //
        private void Avoid() 
        {
            _twoHandedPincher = FindObjectOfType<TwoHandedPincher>();
            _centerEye = FindObjectOfType<CenterEyeAnchor>();
        }
        private void Start() 
        {
            InitializeModeSwitching();
            InitializeSequence();
            InitializeButton();
            InitializePincher();
        }
        private void Update() 
        {
            HandleFocus();
        }
        private void HandleFocus() 
        {
            if (_canBeExpanded) 
            {
                if (_centerEye.IsLookingAt(transform, 10f, 0.08f)) 
                {
                    if (!_focused) 
                    {
                        Focus(true);
                    }
                }
                else
                {
                    if (!_focused) 
                    {
                        Focus(false);
                    }
                }
            }
            else 
            {
                if(_focused) 
                {
                    Focus(true);
                }
            }
        }
        private void Focus(bool focus) 
        {
            _focused = focus;
            _swellingRing.Show(focus);
            if (!focus) 
            {
                _swellingRing.Activate(false);
            }
        }
        private void InitializeModeSwitching() 
        {
            _viralSettings.EngineerModeActive.Subscribe(engineerMode =>
            {
                _swellingRing.gameObject.SetActive(engineerMode);
                _canBeExpanded = engineerMode;
                if (!engineerMode)
                {
                    Collapse();
                }
            });
        }
        private void InitializeButton() 
        {
            if (_hologramButton) 
            {
                _hologramButton.OnPress().Subscribe(b =>
                {
                    if (!_expanded) 
                    {
                        Expand();
                        _hologramButton.SetText("COLLAPSE");
                    }
                    else 
                    {
                        Collapse();
                        _hologramButton.SetText("EXPAND");
                    }
                });
            }
        }
        public void Expand() 
        {
            _expandSequence.PlayForward();
        }
        public void Collapse() 
        {
            _expandSequence.PlayBackwards();
        }
        public void SetExpansionPercentage(float value) 
        {
            float newPos = _doublePinchStartValue+ value *_sequenceDuration;
            _expandSequence.Goto(newPos);
            _swellingRing.SetRingScale(1 + _expandSequence.position / _sequenceDuration);
        }
        private void InitializePincher() 
        {
            _twoHandedPincher.OnTwoHandedPinchStart.Subscribe(distance =>
            {
                if (_focused && _canBeExpanded && _expandSequence !=null) 
                {
                    _doublePinchStartDistancce = distance;
                    _doublePinchStartValue = _expandSequence.position;
                }
            });

            _twoHandedPincher.OnTwoHandedPinchEnd.Subscribe(_ => { _swellingRing.Activate(false); });
            _twoHandedPincher.TwoHandedPinchDistance.Subscribe(distance =>
            {
                if (_focused && _canBeExpanded && _expandSequence !=null) 
                {
                    SetExpansionPercentage((distance - _doublePinchStartDistancce) * 1f); 
                    _swellingRing.Activate(true);
                }
            });
        }
        private void InitializeSequence() 
        {
            _expandSequence = DOTween.Sequence();
            _expandSequence.SetAutoKill(false);
            _expandSequence.AppendInterval(0.0f);
            //
            _parts.ForEach(part =>
            {
                Sequence partSequence = part.GetSequence();
                _expandSequence.Join(partSequence);
            });
            //
            _expandSequence.Pause();
            _sequenceDuration = _expandSequence.Duration();
            _expandSequence.OnComplete(() => { _expanded= true; });
            _expandSequence.OnUpdate(() => 
            {
                if (_expandSequence.position <= 0.1f) 
                {
                    _expanded = false;
                    _hologramButton.SetText("EXPAND");
                }
                if(_expandSequence.position >= 0.9f * _sequenceDuration) 
                {
                    _expanded = false;
                    _hologramButton.SetText("COLLAPSE");
                }
            });
        }
    }
}