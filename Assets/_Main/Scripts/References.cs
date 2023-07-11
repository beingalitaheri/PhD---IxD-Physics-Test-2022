using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class References : MonoBehaviour
{

    [SerializeField]private Transform _head;

    private Transform _centerEyeAnchor;
    // Start is called before the first frame update
    void Start()
    {
        _centerEyeAnchor = FindObjectOfType<CenterEyeAnchor>().transform;   
    }

    // Update is called once per frame
    void Update()
    {
        _head.position = new Vector3(_centerEyeAnchor.position.x,0.001f,_centerEyeAnchor.position.z);
        _head.rotation = Quaternion.Euler(0,_centerEyeAnchor.rotation.eulerAngles.y,0);
    }

    public Transform GetHead()
    {
        return _head;
    }
}
