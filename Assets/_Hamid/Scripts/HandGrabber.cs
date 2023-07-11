using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;

namespace _VIRAL._03_Scripts
{
    public class HandGrabber : Holder
    {
        [SerializeField] private Hand _hand;
        // Start is called before the first frame update

        public Hand Hand => _hand;
        void Start()
        {
            Initialize();

        }

        private void Initialize()
        {


            _hand.OnStartGraping.Subscribe(h =>
            {

                Capture();

            }

            );

            _hand.onStopGrasping.Subscribe(h =>
            {

                Release();

            });

            _onCaptured.Subscribe(h =>
            {
                bool useSnapHandRight = h._visualSnapHandRight && _hand.hand_type == OVRHand.Hand.HandRight;
                bool useSnapHandLeft = h._visualSnapHandLeft && _hand.hand_type == OVRHand.Hand.HandLeft;

                if (useSnapHandRight || useSnapHandLeft)
                {
                    h.ShowVisualSnapHand(true, _hand.hand_type);
                    _hand.ShowHand(false);
                }
                else
                {
                    _hand.MakeTransparent(true);
                    _hand.ShowHand(true);
                }



            });

            _onReleased.Subscribe(h =>

            {
                h.ShowVisualSnapHand(false, _hand.hand_type);
                _hand.ShowHand(true);
                _hand.MakeTransparent(false);
            }
            );
        }
    }
}