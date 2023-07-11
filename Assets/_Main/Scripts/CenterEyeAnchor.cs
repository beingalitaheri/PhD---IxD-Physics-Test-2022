using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CenterEyeAnchor : MonoBehaviour
{
    public bool IsLookingAt(Transform other, float distance = 10f, float fov = 0.1f)
    {
        Vector3 direction = (transform.position - other.transform.position).normalized;

        float dot = Vector3.Dot(direction,transform.forward);
        float dist = Vector3.Distance(transform.position, other.position);

        return (dot < (-1 + fov) && dist < distance);
    }
}
