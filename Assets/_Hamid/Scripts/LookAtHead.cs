using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookAtHead : MonoBehaviour
{
    private References _references;


    [Space]
    [SerializeField]private Transform _translocator;

    [Space]
    [SerializeField]private float _lerpSpeedPosition = 0.05f;
    [SerializeField]private float _lerpSpeedRotation = 0.1f;
    [SerializeField]private float _lerpSpeedDistance = 0.01f;


    private float _distance;
    // Start is called before the first frame update
    void Start()
    {
        _references = FindObjectOfType<References>();
    }

    public void SetDistance(float distance)
    {
        _distance = distance * 0.1f ;
    
    }

    // Update is called once per frame
    private void Update()
    {
        Vector3 currentPos = transform.position;

        Vector3 newPos = _references.GetHead().position;
        newPos = Vector3.Lerp(currentPos, new Vector3(newPos.x, currentPos.y, newPos.z), _lerpSpeedPosition);

        Quaternion newRot = Quaternion.Euler(0,_references.GetHead().rotation.eulerAngles.y, 0);

        newRot = Quaternion.Lerp(transform.rotation, newRot, _lerpSpeedRotation);

        transform.SetPositionAndRotation(newPos, newRot);

        Vector3 pos = _translocator.localPosition;
        _translocator.localPosition = Vector3.Lerp(pos, new Vector3(0, 0, _distance), _lerpSpeedDistance);
        
    }
}
