using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UniRx;
using System;
using UnityEngine.UI;
using DG.Tweening;

namespace _VIRAL._03_Scripts
{
    public class HologramUiComponent : MonoBehaviour
    {
        [SerializeField] protected bool _enableOnStart = true;
        [SerializeField] protected TextMeshPro _text;
        [SerializeField] protected SpriteRenderer _icon;

        [SerializeField] protected float _actiondelay = 0f;
        [SerializeField] protected float _deactivateAfterActionTime = 0f;

        [SerializeField] protected Color _colorDisabled = new Color(1.000f, 1.000f, 1.000f, 0.3f);
        [SerializeField] protected Color _colorEnabled = new Color(1.000f, 1.000f, 1.000f, 0.7f);
        [SerializeField] protected Color _colorFocus = new Color(1.000f, 1.000f, 1.000f, 1.000f);
        [SerializeField] protected Color _colorActive = new Color(0.000f, 0.779f, 1.000f, 1.000f);


        [SerializeField] public AudioSource AudioSource;
        [SerializeField] public AudioClip ButtonClip;

        public bool Enabled => _enabled;
        public bool Focused => _focus;
        public bool Active => _active;


        protected Vector3 _iconInitialScale;
        protected Vector3 _initialScale;

        protected bool _enabled = true;
        protected bool _focus = false;
        protected bool _active = false;

        protected Interactor _interactor;



        //my addon
        private HologramHint _hologramHint;


        [SerializeField] private GameObject _hintPrefab;


        [SerializeField] private string _hintText;

        [SerializeField] private double _showHintDelay;

        [SerializeField] private bool _showhint = true;

        public Subject<HologramHint> hintDisposable = new Subject<HologramHint>();

        private IDisposable _hintDisposable => hintDisposable;

        protected virtual void Awake()
        {
            _initialScale = transform.localScale;
            _iconInitialScale = _icon.transform.localScale;
            Enable(_enableOnStart);
            //****** i add this 
            InitializeHint();
        }

        public void DeactivateFor(float seconds = 0.4f)
        {
            Enable(false);
            Observable.Timer(TimeSpan.FromSeconds(seconds)).Subscribe(t =>
            {
                Enable(true);
            }
        );
        }

        private void OnTriggerEnter(Collider other)
        {
            //_enabled = true;
            if (!_enabled) return;

            Interactor interactor = other.gameObject.GetComponent<Interactor>();
            Interactor_hint my_intereactior_hint = other.gameObject.GetComponentInChildren<Interactor_hint>();

            if (interactor && !_interactor)
            {
                SetInteractor(interactor);
                EnterAction(other.transform.position);
            }

            if (my_intereactior_hint)
            {
                my_intereactior_hint.SetComponenet(this);
                Focus(true);
            }
            //_enabled = true;
        }

        private void InitializeHint()
        {
            if (_hintPrefab != null)
            {
                if (_hintPrefab != null)
                {
                    GameObject hint = Instantiate(_hintPrefab, transform.position, transform.rotation);

                    _hologramHint = hint.GetComponent<HologramHint>();

                    _hologramHint.SetUiComponent(this);
                    _hologramHint.SetText(_hintText.Length > 0 ? _hintText : _text.text);
                }

            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (!_enabled) return;

            Interactor interactor = other.gameObject.GetComponent<Interactor>();

            if (interactor && _interactor)
            {
                SetInteractor(null);
                ExitAction(other.transform.position);
            }


        }

        public void SetInteractor(Interactor interactor)
        {
            _interactor = interactor;
        }

        public void Enable(bool enable)
        {
            _enabled = enable;
            UpdateColor();
        }

        private void OnDisable()
        {
            _hologramHint.ShowHint(false);
        }

        private void OnDestroy()
        {
            Destroy(_hologramHint);
        }

        private void UpdateColor()
        {
            SetColor(_active ? _colorActive : _focus ? _colorFocus : _enabled ? _colorEnabled : _colorDisabled);
        }

        private void SetColor(Color my_color)
        {
            _text.color = my_color;

            if (_icon)
            {
                _icon.color = my_color;
            }
        }

        public void Activate(bool active)
        {
            _active = active;
            UpdateColor();
        }

        //public void Focus(bool focus)
        //{
        //    _focus = focus;
        //    UpdateColor();
        //}

        public void Focus(bool focus)
        {
            if (focus == _focus) return;

            _focus = focus;
            UpdateColor();

            if (focus)
            {
                ShowHint(true);
            }
            else
            {
                _hologramHint.ShowHint(false);
            }
        }

        private void ShowHint(bool show)
        {
            if (show && _showhint)
            {
                _hintDisposable.Dispose();


                //_hintDisposable = Observable.Timer(TimeSpan.FromSeconds(_showHintDelay)).Subscribe(_ =>
                //{
                //    if (_focus)
                //    {
                //        _hologramHint.ShowHint(true);
                //    }
                //}
                //).AddTo(this);
            }
            else
            {
                _hologramHint.ShowHint(false);
            }
        }

        public virtual void EnterAction(Vector3 point)
        {
            Activate(true);
        }

        public virtual void ExitAction(Vector3 point)
        {
            transform.DOScale(_initialScale, 0.1f);

            Observable.Timer(TimeSpan.FromSeconds(_actiondelay)).Subscribe(t =>
            {
                Activate(false);
            });
        }

        public virtual void Click(Vector3 Position)
        {

            EnterAction(Position);

            Observable.Timer(TimeSpan.FromSeconds(_actiondelay)).Subscribe(_ =>
            {
                ExitAction(Position);

            });
        }

        protected virtual void PlaySound()
        {
            if (AudioSource != null && ButtonClip != null)
            {
                AudioSource.PlayOneShot(ButtonClip, 1.0f);
            }

        }

    }
}