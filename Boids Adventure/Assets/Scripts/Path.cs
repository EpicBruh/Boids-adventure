using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Path : MonoBehaviour
{
    public float radius = .5f;
    public Transform[] points;
    
    private void OnDrawGizmos()
    {

        for(int i = 0; i < points.Length - 1; i++)
        {
            Debug.DrawLine(points[i].position, points[i + 1].position, Color.red);
        }
    }
}
