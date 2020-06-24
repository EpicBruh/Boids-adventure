using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VehicleManager : MonoBehaviour
{
    public Vehicle[] vehicles;
    public bool debug = false;
    public bool cohesion = false;
    public bool seperation = false;


    void Update()
    {
        foreach(Vehicle v in vehicles)
        {
            if(seperation)
                v.seperate(vehicles);
            if(cohesion)
                v.cohesion(vehicles);
            v.debug = debug;
        }
    }
}
