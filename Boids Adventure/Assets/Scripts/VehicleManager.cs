using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VehicleManager : MonoBehaviour
{
    public Vehicle[] vehicles;
    public bool debug = false;
    [SerializeField]
    bool applyBehaviors = true;
    


    void Update()
    {
        if (applyBehaviors)
        {
            foreach (Vehicle v in vehicles)
            {
                v.applyBehaviors(vehicles);
                v.debug = debug;
            }
        }
    }
}
