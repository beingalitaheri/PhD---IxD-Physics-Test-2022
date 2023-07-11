using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FaceTowardPlayer : MonoBehaviour
{
    private Transform _centerEye;
    private Quaternion _rotation;

    // Start is called before the first frame update
    void Start()
    {
        _centerEye = FindObjectOfType<CenterEyeAnchor>().transform;
    }

    // Update is called once per frame
    void Update()
    {
        if (_centerEye)
        {
            Vector3 relativePos = _centerEye.transform.position - transform.position;
            _rotation = Quaternion.Euler(0, Quaternion.LookRotation(relativePos,Vector3.up).eulerAngles.y,0);
            transform.rotation = Quaternion.Slerp(transform.rotation, _rotation, 0.1f);
        }
        
    }
}
