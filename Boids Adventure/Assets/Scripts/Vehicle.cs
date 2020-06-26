using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class Vehicle : MonoBehaviour
{
    //Global Variables
    [SerializeField]
    GridManager gridManager;
    public Transform roomBorderMax;
    public Transform roomBorderMin;
    [SerializeField]
    Path p;


    //Debug Variables
    public bool limitedBorders = false;
    public bool debug = false;
    [SerializeField]
    float MaxSpeed = 1f;
    [SerializeField]
    float MaxForce = .3f;
    [SerializeField]
    [Range(0, 10)]
    float radius = 2f;
    [SerializeField]
    [Range(0, 10)]
    float cohesionMultiplier = 4f;
    [SerializeField]
    bool randomVelocity = false;
    [SerializeField]
    bool selfMove = false;
    [SerializeField]
    bool followThePath = false;


    //Placeholder Variables
    float wanderTheta = 0;
    public Vector2 velocity;
    public Vector2 acceleration;
    
    








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

        if (!followThePath)
        {
            if (selfMove)
                wander();
            else
                applyForce(gridManager.FlowField(transform.position));
        }
        else
            followPath(p);

        if (limitedBorders)
            wrapAround();
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

    public void wrapAround() {
        if (transform.position.x > roomBorderMax.position.x)
            transform.position = new Vector3(roomBorderMin.position.x, transform.position.y, 0);
        else if (transform.position.x < roomBorderMin.position.x)
            transform.position = new Vector3(roomBorderMax.position.x, transform.position.y, 0);
        if (transform.position.y > roomBorderMax.position.y)
            transform.position = new Vector3(transform.position.x, roomBorderMin.position.y, 0);
        else if (transform.position.y < roomBorderMin.position.y)
            transform.position = new Vector3(transform.position.x, roomBorderMax.position.y, 0);
    }



    //Steering Force = Desired velocity - Actual velocity
    public Vector2 seek(Vector2 target)
    {        
        Vector2 desiredVel = target - (Vector2)transform.position;
        desiredVel.Normalize();
        desiredVel *= MaxSpeed;
        Vector2 SteerForce = desiredVel - velocity;
        
        SteerForce = Vector2.ClampMagnitude(SteerForce, MaxForce);

        return SteerForce;
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
        applyForce(seek(target));
        drawWanderStuff(transform.position, circlePos, target, wanderR);
    }

    public void followPath(Path p)
    {
        float worldRecord = 10000000;
        Vector2 target = new Vector2();
        Vector2 predict = velocity;
        predict.Normalize();
        predict *= .5f;
        Vector2 predictLoc = (Vector2)transform.position + predict;
        

        for(int i = 0; i < p.points.Length - 1; i++)
        {
            Vector2 a = p.points[i].position;
            Vector2 b = p.points[i + 1].position;
            Vector2 normalPoint = getNormalPoint(predictLoc, a, b);

            if (normalPoint.x < a.x || normalPoint.x > b.x)
                normalPoint = b;

            float distance = Vector2.Distance(predictLoc, normalPoint);

            if (distance < worldRecord)
            {
                worldRecord = distance;
                target = normalPoint;              
            }
        }
        
        if (worldRecord > p.radius && worldRecord != 10000000)
        {
            applyForce(seek(target) *5f);
        }
       
        
    }

    public Vector2 separate(Vehicle[] boids ) {
        float desiredSeparation = radius * 2;
        Vector2 sum = new Vector2();
        int count = 0;

        foreach(Vehicle v in boids)
        {
            float d = Vector2.Distance(transform.position, v.transform.position);
            if ((d > 0) && (d < desiredSeparation))
            {
                Vector2 diff = transform.position - v.transform.position; //This implies from obstacle to gameObject
                diff.Normalize();
                diff /= d; //As it will be more the closer they are
                sum += diff;
                count++;
            }
        }
        if (count > 0)
        {
            sum /= count;
            sum.Normalize();
            sum *= MaxSpeed;
            Vector2 steerForce = sum - velocity; //SteerForce = desired - current velocity
            steerForce = Vector2.ClampMagnitude(steerForce, MaxForce);
            return steerForce;
        }
        return new Vector2(0,0);
    }

    public Vector2 cohesion(Vehicle[] boids)
    {
        float desiredCohesion = radius * 2 * cohesionMultiplier;
        Vector2 sum = new Vector2();
        int count = 0;

        foreach (Vehicle v in boids)
        {
            float d = Vector2.Distance(transform.position, v.transform.position);
            if ((d > 0) && (d > desiredCohesion))
            {
                Vector2 diff = v.transform.position - transform.position; //This implies from gameObject to obstacle
                diff.Normalize();
                diff *= d; //As it will be more the closer they are
                sum += diff;
                count++;
            }
        }
        if (count > 0)
        {
            sum /= count;
            sum.Normalize();
            sum *= MaxSpeed;
            Vector2 steerForce = sum - velocity; //SteerForce = desired - current velocity
            steerForce = Vector2.ClampMagnitude(steerForce, MaxForce);
            return steerForce;
        }
        return new Vector2(0, 0);
    }

    public Vector2 align(Vehicle[] boids)
    {
        float neighborDist = radius * 2 * cohesionMultiplier;

        Vector2 sum = new Vector2();
        int count = 0;
        foreach(Vehicle v in boids)
        {
            float d = Vector2.Distance(transform.position, v.transform.position);
            if((d>0) && d < neighborDist)
            {
                sum += v.velocity;
                count++;
            }
        }
        if (count > 0)
        {
            sum /= count;
            sum.Normalize();
            sum *= MaxSpeed;
            Vector2 steerForce = sum - velocity;
            steerForce = Vector2.ClampMagnitude(steerForce, MaxForce);
            return steerForce;
        }
        else
            return new Vector2();
    }



    public void applyBehaviors(Vehicle[] boids) {
        Vector2 separationForce = separate(boids);
        Vector2 cohesionForce = cohesion(boids);
        Vector2 alignForce = align(boids);

        separationForce *= 2.5f;
        cohesionForce *= 1.8f;
        alignForce *= 1.7f;

        applyForce(separationForce);
        applyForce(cohesionForce);
        applyForce(alignForce);
    }



    




    

    Vector2 getNormalPoint(Vector2 p, Vector2 a, Vector2 b)
    {
        Vector2 ap = p - a;
        Vector2 ab = b - a;
        ab.Normalize();
        ab *= Vector2.Dot(ap, ab);
        Vector2 normalPoint = a + ab;
        return normalPoint;
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

    private void OnDrawGizmos()
    {
        if (debug)
        {
            Handles.DrawWireDisc(transform.position, new Vector3(0, 0, 1), radius);
            Handles.DrawWireDisc(transform.position, new Vector3(0, 0, 1), radius * cohesionMultiplier);
        }
    }


}
