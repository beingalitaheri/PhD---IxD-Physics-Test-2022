using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeightAdjuster : MonoBehaviour
{
    [SerializeField] private float _heightRatio = 0.6f;
    [SerializeField] public float _thershold = 0.05f;
    [SerializeField] public float _lerpvalue = 0.9f;

    private Transform _centerEyeAnchor;



    void Start()
    {
        _centerEyeAnchor = FindObjectOfType<CenterEyeAnchor>().transform;
    }


    void Update()
    {
        float cameraheight = _centerEyeAnchor.position.y;
        float height = cameraheight * _heightRatio;
        float distance = Mathf.Abs(transform.position.y - height);

        if (distance > _thershold)
        {
            //Vector3 newHeight = new Vector3(transform.position.x, height, transform.position.z);
            //transform.position = Vector3.Lerp(transform.position, newHeight, Time.fixedDeltaTime * _lerpvalue);
        }
    }
}
