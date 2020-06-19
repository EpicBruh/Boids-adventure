using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class Vehicle : MonoBehaviour
{
    Rigidbody2D rb;
    public float MaxSpeed = .5f;
    public float MaxForce = 1f;
    //public float NearDistance = 3f;
    public float radius = 3f;
    public float wanderDistance = 5f;
    public bool debug = false;
    
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.velocity = new Vector2(0, 1);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void FixedUpdate()
    {
        wander();
        







        //Rotate body
        Vector2 v = rb.velocity;
        float angle = Mathf.Atan2(v.y, v.x) * Mathf.Rad2Deg - 90;
        transform.rotation = Quaternion.AngleAxis(angle, new Vector3(0, 0, 1));

    }

    public void seek(Vector2 target)
    {
        //Steering Force = Desired velocity - Actual velocity
        Vector2 desiredVel = target - (Vector2)transform.position;
        float d = desiredVel.magnitude;
        desiredVel.Normalize();
        if (d < radius)
        {
            d = map(d, 0, radius, 0, MaxSpeed);
            desiredVel *= d;
        }
        else
        {
            desiredVel *= MaxSpeed;
        }

        Vector2 SteerForce = desiredVel - rb.velocity;
        SteerForce = Vector2.ClampMagnitude(SteerForce, MaxForce);
        rb.AddForce(SteerForce);
    }



    //Wander Function
    public void wander() {
        Vector2 desiredLoc = rb.velocity;
        desiredLoc.Normalize();
        desiredLoc *= wanderDistance;
        desiredLoc += (Vector2)transform.position;
        Vector2 v = rb.velocity;
        float angle = Mathf.Atan2(v.y, v.x);
        float wanderTheta = Random.Range(-0.3f, 0.3f);
        Vector2 circleOffset = new Vector2(radius * Mathf.Cos(angle + wanderTheta), radius * Mathf.Sin(angle + wanderTheta));
        Debug.Log(circleOffset);
        Vector2 target = desiredLoc + circleOffset;
        seek(target);

    }








    public float map(float OldValue, float OldMin, float OldMax, float NewMin, float NewMax)
    {
        float OldRange = OldMax - OldMin;
        float NewRange = NewMax - NewMin;
        float NewValue = ((OldValue - OldMin) * NewRange / OldRange) + NewMin;
        return NewValue;
    }


    private void OnDrawGizmos()
    {
        //if(debug)
            //
    }

}
