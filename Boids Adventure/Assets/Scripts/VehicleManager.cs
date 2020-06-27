using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VehicleManager : MonoBehaviour
{
    
    public bool debug = false;
    [SerializeField]
    bool applyBehaviors = true;
    [SerializeField]
    GameObject flock;
    public List<Vehicle> vehicles;

    private void Start()
    {
        vehicles = new List<Vehicle>();
        foreach(GameObject g in GameObject.FindGameObjectsWithTag("boid")){
            vehicles.Add(g.GetComponent<Vehicle>());
            Debug.Log(vehicles);
        }
    }

    void Update()
    {
        
        if (Input.GetMouseButton(0))
        {
            Vector3 target = new Vector3(Camera.main.ScreenToWorldPoint(Input.mousePosition).x, Camera.main.ScreenToWorldPoint(Input.mousePosition).y, 0);
            vehicles.Add(Instantiate(flock,target , Quaternion.identity).GetComponent<Vehicle>());
            
        }
    }

    private void FixedUpdate()
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
