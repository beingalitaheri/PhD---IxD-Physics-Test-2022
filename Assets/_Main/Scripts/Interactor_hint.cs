using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace _VIRAL._03_Scripts
{
    public class Interactor_hint : MonoBehaviour
    {
        private HologramUiComponent _hintedComponent;
        public HologramUiComponent HintedComponenet => _hintedComponent;

        public void SetComponenet(HologramUiComponent _componenet)
        {
            bool notNullAndDifferent = false;

            if (_hintedComponent && _componenet)
            {
                notNullAndDifferent = _hintedComponent.GetInstanceID() != _componenet.GetInstanceID();
            }

            if (!_hintedComponent || !_componenet || notNullAndDifferent)
            {
                if (_hintedComponent)
                {
                    _hintedComponent.Focus(false);
                }

                if (_componenet)
                {
                    _componenet.Focus(true);
                }
                _hintedComponent = _componenet;
            }
        }
    }
}