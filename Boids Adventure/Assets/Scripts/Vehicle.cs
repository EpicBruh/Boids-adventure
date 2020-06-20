using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class Vehicle : MonoBehaviour
{
    float wanderTheta = 0;
    public bool debug = false;
    public Vector2 velocity;
    public Vector2 acceleration;
    [SerializeField]
    float MaxSpeed = 1f;
    [SerializeField]
    float MaxForce = .3f;
   



    private void FixedUpdate()
    {
        wander();
        Move();
        rot();
    }





    public void rot() {
        Vector2 v = velocity;
        float angle = Mathf.Atan2(v.y, v.x) * Mathf.Rad2Deg - 90;
        transform.rotation = Quaternion.AngleAxis(angle, new Vector3(0, 0, 1));
    }

    public void applyForce(Vector2 force)
    {
        acceleration += force;
    }

    public void Move()
    {
        velocity += acceleration;
        velocity = Vector2.ClampMagnitude(velocity, MaxSpeed);
        transform.position += (Vector3)velocity * Time.deltaTime;
        acceleration *= 0;
    }




    //Steering Force = Desired velocity - Actual velocity
    public void seek(Vector2 target)
    {        
        Vector2 desiredVel = target - (Vector2)transform.position;
        desiredVel.Normalize();
        desiredVel *= MaxSpeed;
        Vector2 SteerForce = desiredVel - velocity;
        SteerForce = Vector2.ClampMagnitude(SteerForce, MaxForce);
        applyForce(SteerForce);
    }




    //Wander Function
    public void wander() {
        float wanderR = 1.3f;
        float wanderD = 4f;
        float change = 0.3f;
        wanderTheta += Random.Range(-change, change);

        Vector2 circlePos = velocity;
        circlePos.Normalize();
        circlePos *= wanderD;
        circlePos += (Vector2)transform.position;

        Vector2 v = velocity.normalized;
        float h = Mathf.Atan2(v.y, v.x);
        
        Vector2 circleOffset = new Vector2(wanderR * Mathf.Cos(h + wanderTheta), wanderR * Mathf.Sin(h + wanderTheta));
        Vector2 target = circlePos + circleOffset;
        seek(target);
        drawWanderStuff(transform.position, circlePos, target, wanderR);
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
        if (debug)
        {

        }
            
    }


    void drawWanderStuff(Vector3 position, Vector2 circle, Vector2 target, float rad)
    {


        if (debug)
        {
            Debug.DrawLine(position, circle);
            Debug.DrawLine(circle, target);
        }
        
    }

    
}
