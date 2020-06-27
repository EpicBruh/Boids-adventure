using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Path : MonoBehaviour
{
    public float radius = .1f;
    public Transform[] points;
    public bool debug = false;

    private void OnDrawGizmos()
    {
        if (debug)
        {
            for (int i = 0; i < points.Length - 1; i++)
            {
                Debug.DrawLine(points[i].position, points[i + 1].position, Color.red);
            }
        }
    }
}
