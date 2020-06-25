using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class Vehicle : MonoBehaviour
{
    //Variables
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


    [SerializeField]
    float radius = 2f;


    public Transform roomBorderMax;
    public Transform roomBorderMin;




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
            //else
                //applyForce(gridManager.FlowField(transform.position));
        }
        else
            followPath(p);


        Move();
        rot();
        wrapAround();
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
        if (transform.position.x < roomBorderMin.position.x)
            transform.position = new Vector3(roomBorderMax.position.x, transform.position.y, 0);
        if (transform.position.y > roomBorderMax.position.y)
            transform.position = new Vector3(transform.position.x, roomBorderMin.position.y, 0);
        if (transform.position.y < roomBorderMin.position.y)
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
        predict *= 1f;
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
            seek(target);
        }
       
        
    }

    public Vector2 seperate(Vehicle[] boids ) {
        float desiredSeparation = radius * 2;
        Vector2 sum = new Vector2();
        int count = 0;

        foreach(Vehicle v in boids)
        {
            float d = Vector2.Distance(transform.position, v.transform.position);
            if ((d > 0) && (d < desiredSeparation))
            {
                Vector2 diff = transform.position - v.transform.position;
                diff.Normalize();
                diff /= d;
                sum += diff;
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
            //applyForce(steerForce);
            return steerForce;
        }

        return new Vector2();
    }

    public void applyBehaviors(Vehicle[] boids)
    {
        Vector2 separateForce = seperate(boids);
        Vector2 cohesionForce = cohesion(boids);
        Vector2 seekForce = seek((Vector2)Camera.main.ScreenToWorldPoint(Input.mousePosition));

        cohesionForce *= 1.0f;
        separateForce *= 1.5f;
        seekForce *= 0.5f;

        applyForce(cohesionForce);
        applyForce(separateForce);
        applyForce(seekForce);
    }

    public void flock(Vehicle[] boids)
    {
        Vector2 separateForce = seperate(boids);
        Vector2 cohesionForce = cohesion(boids);
        Vector2 alignForce = align(boids);

        cohesionForce *= 1.0f;
        separateForce *= 1.1f;
        alignForce *= 1.0f;

        
        applyForce(separateForce);
        applyForce(alignForce);
        applyForce(cohesionForce);
        
    }

    public Vector2 align(Vehicle[] boids)
    {
        float neighbourDist = radius * 2 * 4;
        Vector2 sum = new Vector2();
        int count = 0;
        foreach(Vehicle boid in boids)
        {
            float d = Vector2.Distance(transform.position, boid.transform.position);
            if(d > 0 && d < neighbourDist)
            {
                sum += boid.velocity;
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

    public Vector2 cohesion(Vehicle[] boids)
    {
        float desiredCohesion = radius * 2 * 5;
        float desiredSeparation = radius * 2;
        Vector2 sum = new Vector2();
        int count = 0;

        foreach (Vehicle v in boids)
        {

            float d = Vector2.Distance(transform.position, v.transform.position);
            if ((d > 0) && (d < desiredCohesion) && (d > desiredSeparation))
            {
                Vector2 diff = v.transform.position;
                sum += diff;
                count++;
            }
        }
        if (count > 0)
        {
            sum /= count;
            return sum;
        }
        else
            return new Vector2();
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
        }
    }


}
