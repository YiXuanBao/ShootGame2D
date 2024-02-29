using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//相机跟随
public class CameraFollow : MonoBehaviour
{
    [Range(1, 10)]
    public float smooth = 10f; //跟随速度

    Transform target;

    Vector3 targetPosition;

    public void SetTarget(Transform obj)
    {
        target = obj;
    }

    void LateUpdate()
    {
        if (target != null)
        {
            targetPosition.x = transform.position.x + (target.position.x - transform.position.x) * smooth * Time.deltaTime;
            targetPosition.y = transform.position.y + (target.position.y - transform.position.y) * smooth * Time.deltaTime;
            targetPosition.z = transform.position.z;
            transform.position = targetPosition;
        }
    }
}
