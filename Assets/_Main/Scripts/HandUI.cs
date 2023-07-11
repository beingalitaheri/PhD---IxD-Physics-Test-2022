using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.PackageManager.UI;
using UnityEngine;
using UniRx;
using TMPro;
using DG.Tweening;
using System;
namespace _VIRAL._03_Scripts
{
    public class HandUI : MonoBehaviour
    {
        #region Editor
        [SerializeField] private ViralSettings _viralSetting;

        [Space]
        [SerializeField] private Transform _Pivot;
        [SerializeField] private Transform _Panel;
        [SerializeField] private Window _window;
        [Space]
        [SerializeField] private HologramButton _buttonOpen;
        [SerializeField] private HologramButton _buttonClose;
        [SerializeField] private HologramCheckbox _checkboxTeleportation;
        [SerializeField] private HologramCheckbox _checkboxMimotion;
        [SerializeField] private HologramButton _buttonMenu;
        [SerializeField] private HologramButton _buttonMode;
        #endregion
        #region Private
        private Transform _centorEyeAnchor;
        private ControlPanel _controlPanel;

        private bool _isFacing = false;
        private bool _panelOpened = false;
        private bool _panelMoving = false;

        private Vector3 _panelInitialScale;
        private Vector3 _buttonOpenInitialScale;

        private Sequence _panelSequence;
        private Tweener _buttonOpenTweener;

        private const string _engineerModeText = "ENGINEER";
        private const string _operatorModeText = "OPERATOR";

        private readonly Vector3 _openRotation = new Vector3(20, 0, 0);
        #endregion

        private void Awake()
        {
            Initialize();
        }
        private void Start()
        {
            Initialize();
        }
        private void Initialize()
        {
            InitializePanel();
            InitializeButtons();
        }
        private void Update()
        {
            CheckFacing();
            if (_isFacing && !_panelOpened && !_panelMoving)
            {
                OpenPanel();
            }
            if (!_isFacing && _panelOpened && !_panelMoving)
            {
                ClosePanel();
            }
        }
        private void CheckFacing()
        {
            Vector3 forward = transform.TransformDirection(Vector3.forward);
            Vector3 toOther = _centorEyeAnchor.position - transform.position;

            _isFacing = Vector3.Dot(forward, toOther) > 0.1f;
        }
        private void InitializePanel()
        {
            _Panel.gameObject.SetActive(false);
            _Pivot.localRotation = Quaternion.Euler(0, 0, 0);
            _panelInitialScale = _Panel.localScale;
            _Panel.localScale = _panelInitialScale * 0.01f;
            _buttonOpenInitialScale = _buttonOpen.transform.localScale;
            _window.Close();

        }

        private void OpenPanel()
        {
            _panelMoving = true;
            _Panel.gameObject.SetActive(true);
            _panelSequence?.Kill();
            _panelSequence = DOTween.Sequence();
            _panelSequence.Append(_Pivot.DOLocalRotate(_openRotation, 0.25f))
                .SetEase(Ease.InOutQuad)
                .OnComplete(() =>
                {
                    _panelMoving = false;
                    _panelOpened = true;

                });
            _panelSequence.Join(_Panel.DOScale(_panelInitialScale, 0.2f)
                .SetEase(Ease.InOutQuad));

        }
        private void ClosePanel()
        {
            _window.Close();
            _buttonOpenTweener?.Kill();
            _buttonOpenTweener = _buttonOpen.transform.DOScale(_buttonOpenInitialScale, 0.1f).OnComplete(() =>
            {
                _buttonOpen.gameObject.SetActive(true);
            });
            _panelMoving = true;
            _panelSequence?.Kill();
            _panelSequence = DOTween.Sequence(); ;

            _panelSequence.Append(
                _Pivot.DOLocalRotate(Vector3.zero, 0.1f)).
                SetEase(Ease.InOutQuad).OnComplete(
                () =>
                {
                    _buttonOpen.gameObject.SetActive(true);
                    _Panel.gameObject.SetActive(false);
                    _panelMoving = false;
                    _panelOpened = false;

                });
            _panelSequence.Join(_Panel.DOScale(_panelInitialScale * 0.01f, 0.1f))
                .SetEase(Ease.InOutQuad);

        }
        private void InitializeButtons()
        {
            _buttonOpen.OnPress().Subscribe(b =>
            {
                _window.Open();
                _buttonOpenTweener?.Kill();
                _buttonOpenTweener = _buttonOpen.transform.DOScale(_buttonOpenInitialScale * 0.01f, 0.1f)
                                                                .OnComplete(() =>
                                                                {
                                                                    _buttonOpen.gameObject.SetActive(false);
                                                                    _Panel.gameObject.SetActive(true);
                                                                }

                                                                );
                _buttonOpen.DeactivateFor(1);
                Debug.Log("Open Menu");
            });
            _buttonClose.OnPress().Subscribe(b =>
            {
                _window.Close();
                _buttonClose.DeactivateFor(1);
                _buttonOpen.gameObject.SetActive(true);

                Observable.Timer(TimeSpan.FromSeconds(0.5f)).Subscribe(t =>
                {
                    _buttonOpenTweener?.Kill();
                    _buttonOpenTweener = _buttonOpen.transform.DOScale(_buttonOpenInitialScale, 0.1f).OnComplete(()
                         =>
                    {
                        _buttonOpen.gameObject.SetActive(true);
                    });
                });
            });

            _buttonMenu.OnPress().Subscribe(b => { _controlPanel.Activated(true); });
            
            //Teleporation
            _checkboxTeleportation.SetValue(_viralSetting.TeleporationActive.Value);
            _viralSetting.TeleporationActive.Subscribe(_checkboxTeleportation.SetValue);
            _checkboxTeleportation.onSwitch.Subscribe(active => { _viralSetting.TeleporationActive.Value = active; });

            //Mimotion
            _checkboxMimotion.SetValue(_viralSetting.MimotionActive.Value);
            _viralSetting.MimotionActive.Subscribe(_checkboxMimotion.SetValue);
            _checkboxMimotion.onSwitch.Subscribe(value => { _viralSetting.MimotionActive.Value = value; });

            //Mode Switch
            _buttonMode.SetText(_viralSetting.EngineerModeActive.Value ? _operatorModeText : _engineerModeText);
            _viralSetting.EngineerModeActive.Subscribe((b =>
            {
                _buttonMode.SetText(b ? _operatorModeText : _engineerModeText);
            }
            ));

            _buttonMode.OnPress().Subscribe(_ =>
            {
                _viralSetting.EngineerModeActive.Value = !_viralSetting.EngineerModeActive.Value;

                Debug.Log("Switch Mode: " + (_viralSetting.EngineerModeActive.Value ? "ENGINEER" : "OPERATOR"));

            });

        }

    }
}