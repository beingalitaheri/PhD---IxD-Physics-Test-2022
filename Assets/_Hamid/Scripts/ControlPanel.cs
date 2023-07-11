using UnityEngine;
using DG.Tweening;
using UniRx;
using System;

namespace _VIRAL._03_Scripts
{
    public class ControlPanel : MonoBehaviour
    {
        [SerializeField] private ViralSettings _viralSettings;

        [Space]
        [SerializeField] private LookAtHead _lookAtHead;
        [SerializeField] private HeightAdjuster _heightAdjuster;

        [Space]
        [SerializeField] private HologramButton _hologramButtonSelect;
        [SerializeField] private HologramButton _hologramButtonDeselect;

        [Space]
        [SerializeField] private HologramCheckbox hologramcheckboxFollowPlayer;
        [SerializeField] private HologramCheckbox hologramcheckboxTeleporation;
        [SerializeField] private HologramCheckbox hologramcheckboxMimotion;

        [Space]
        [SerializeField] private HologramSlider _sliderScale;

        [Space]
        [SerializeField] private Transform _container;

        private Tweener _panelTweener;

        private float _uiScale = 1.0f;

        private const string SAVE_FILE_NAME = "ViralSettings.json";

        private string _savepath => $"{Application.persistentDataPath}/{SAVE_FILE_NAME}";



        private void Awake()
        {
            _lookAtHead.SetDistance(_sliderScale.Value);
            AdjustScale(_sliderScale.Value);
        }

        private void Start()
        {
            _viralSettings.Load(_savepath);
            Initilaize();
            _lookAtHead.enabled = _viralSettings.MenuFollowPlayer.Value;
            _heightAdjuster.enabled = _viralSettings.MenuFollowPlayer.Value;
        }

        private void Update()
        {
            _container.localScale = Vector3.Lerp(_container.localScale, Vector3.one * _uiScale, 0.01f);
        }

        private void Initilaize()
        {
            _hologramButtonSelect.OnPress().Subscribe(_ =>
            {
                hologramcheckboxTeleporation.PhysicalCheck(true);
                hologramcheckboxMimotion.PhysicalCheck(true);
            }

            );

            _hologramButtonDeselect.OnPress().Subscribe(_ =>
            {
                hologramcheckboxTeleporation.PhysicalCheck(false);
                hologramcheckboxMimotion.PhysicalCheck(false);
            }
            );
            _viralSettings.MenuFollowPlayer.Subscribe(follow =>
            {
                hologramcheckboxFollowPlayer.SetValue(follow);
                _lookAtHead.enabled = follow;
            });

            hologramcheckboxFollowPlayer.onSwitch.Subscribe(value =>
            {
                _viralSettings.MenuFollowPlayer.Value = value;
            });

            //Teleporatation 
            _viralSettings.TeleporationActive.Subscribe(hologramcheckboxTeleporation.SetValue);

            hologramcheckboxTeleporation.onSwitch.Subscribe(active =>
            {
                _viralSettings.TeleporationActive.Value = active;
            });

            //Mimotion 
            hologramcheckboxMimotion.SetValue(_viralSettings.MimotionActive.Value);

            _viralSettings.MimotionActive.Subscribe(hologramcheckboxMimotion.SetValue);

            hologramcheckboxMimotion.onSwitch.Subscribe(value =>
            {
                _viralSettings.MimotionActive.Value = value;
            }

            );

            //slider
            _sliderScale.OnValueChanged.Subscribe(value =>
            {
                AdjustScale(value);
            }
            );
        }

        private void AdjustScale(float value)
        {
            _lookAtHead.SetDistance(value);
            _uiScale = value;

        }

        private void OnDisable()
        {
            _viralSettings.Save(_savepath);
        }

        public void Activated(bool which)
        {

        }
    }
}