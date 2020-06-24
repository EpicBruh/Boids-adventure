using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [SerializeField]
    Transform target;

    [SerializeField]
    bool interp = false;
    [SerializeField]
    [Range(0, 1)]
    float smoothing = 0.1f;
    
    
    void FixedUpdate()
    {
        if (!interp)
        {
            if (target.position.x >= transform.position.x + 17.7852f)
            {
                transform.position += new Vector3(2 * 17.7852f, 0, 0);
            }
            else if (target.position.x <= transform.position.x - 17.7852f)
            {
                transform.position -= new Vector3(2 * 17.7852f, 0, 0);
            }
            if (target.position.y >= transform.position.y + 10f)
            {
                transform.position += new Vector3(0, 20, 0);
            }
            else if (target.position.y <= transform.position.y - 10f)
            {
                transform.position -= new Vector3(0, 20, 0);
            }
        }
        else
        {
            Vector3 tar = new Vector3(target.position.x,target.position.y, -10);
            transform.position = Vector3.Lerp(transform.position, tar, smoothing);
        }
    }
}
