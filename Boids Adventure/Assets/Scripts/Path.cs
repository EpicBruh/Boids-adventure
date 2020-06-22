using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Path : MonoBehaviour
{
    public Transform start;
    public Transform end;
    public float radius = 4f;


    private void OnDrawGizmos()
    {
        Debug.DrawLine(start.position, end.position);
    }
}
