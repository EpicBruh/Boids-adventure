using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [SerializeField]
    Transform target;
    
    
    void FixedUpdate()
    {
        if (target.position.x >= transform.position.x + 17.7852f)
        {
            transform.position += new Vector3(2*17.7852f,0,0);
        }
        else if (target.position.x <= transform.position.x - 17.7852f)
        {
            transform.position -= new Vector3(2*17.7852f, 0, 0);
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
}
