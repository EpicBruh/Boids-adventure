using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class Vehicle : MonoBehaviour
{
    [SerializeField]
    GridManager gridManager;
    float wanderTheta = 0;
    public bool debug = false;
    public Vector2 velocity;
    public Vector2 acceleration;
    [SerializeField]
    float MaxSpeed = 1f;
    [SerializeField]
    float MaxForce = .3f;

    [SerializeField]
    bool selfMove = false;
    [SerializeField]
    Path p;

    [SerializeField]
    bool followThePath = false;

    [SerializeField]
    bool randomVelocity = false;

    private void Start()
    {
        if (randomVelocity)
        {
            velocity = new Vector2(Random.Range(0,4), Random.Range(0,2));
        }
        else
        {
            velocity = new Vector2();
        }
        acceleration = new Vector2();
    }




    private void FixedUpdate()
    {
        if (selfMove)
            wander();
        else
            applyForce(gridManager.FlowField(transform.position));
        
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

    public void followPath(Path p)
    {
        Vector2 predictLoc = (Vector2)transform.position + velocity;

        Vector2 ap = predictLoc - (Vector2)p.start.position;
        Vector2 ab = (Vector2)(p.end.position - p.start.position);
        float d = (Vector2.Dot(ap, ab))/ ab.magnitude;
        Vector2 an = ab.normalized * d;
        Vector2 normalPoint = an + (Vector2)p.start.position;

        float distance = Vector2.Distance(predictLoc, normalPoint);
        if(distance > p.radius)
        {
            seek(normalPoint);
        }
        /*else
        {
            Vector2 target = (Vector2)p.end.position - normalPoint;
            Vector2 target2 = (Vector2)p.start.position - normalPoint;
            if (target.magnitude > target2.magnitude)
            {
                target.Normalize();
                target *= MaxSpeed;
                seek(target);
            }
            else
            {
                target2.Normalize();
                target2 *= MaxSpeed;
                seek(target2);
            }
        }*/
    }



    public float map(float OldValue, float OldMin, float OldMax, float NewMin, float NewMax)
    {
        float OldRange = OldMax - OldMin;
        float NewRange = NewMax - NewMin;
        float NewValue = ((OldValue - OldMin) * NewRange / OldRange) + NewMin;
        return NewValue;
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
