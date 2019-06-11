using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PedState
{
    walk,
    crash
}

public class PedestrianScript : MonoBehaviour
{
    public PedState currentState;
    public Transform path;
    private List<Transform> nodes;
    public int currectNode = 0;
    public Vector3 centerOfMass;
    public int number = 0;
    public Vector3 frontSensorPosition = new Vector3(0f, 0.2f, 0.5f);
    public float frontSideSensorPosition = 0.2f;
    private bool avoiding = false;
    public float sensorLength = 3f;
    // Start is called before the first frame update
    void Start()
    {
        currentState = PedState.walk;


        Transform[] pathTransforms = path.GetComponentsInChildren<Transform>();
        nodes = new List<Transform>();
        for (int i = 0; i < pathTransforms.Length; i++)
        {
            if (pathTransforms[i] != path.transform)
            {
                nodes.Add(pathTransforms[i]);
            }
        }
        float lastDist = Vector3.Distance(transform.position, pathTransforms[0].position);
        for (int i = 1; i < pathTransforms.Length; i++)
        {
            float thisDist = Vector3.Distance(transform.position, pathTransforms[i].transform.position);
            if (lastDist > thisDist)
            {
                lastDist = thisDist;
                currectNode = i--;
                if(currectNode<=0)
                {
                    currectNode = 0;
                }
            }
           
        }
    }
        
      
    private void Update()
    {

        if (currentState == PedState.walk)
        {
            CheckWaypointDistance();
            Walk();
            Sensors();
        }
        if (currentState == PedState.crash)
            return;
     
     
    }

    private void Sensors()
    {
        RaycastHit hit;
        Vector3 sensorStartPos = transform.position;
        sensorStartPos += transform.forward * frontSensorPosition.z;
        sensorStartPos += transform.up * frontSensorPosition.y;
      
        avoiding = false;

        //front right sensor
        sensorStartPos += transform.right * frontSideSensorPosition;
        if (Physics.Raycast(sensorStartPos, transform.forward, out hit, sensorLength))
        {
            if (hit.collider.CompareTag("Player"))
            {
                Debug.DrawLine(sensorStartPos, hit.point);
         
                if (Vector3.Distance(transform.position, hit.transform.position) < 7f)
                { Dodge(); }
            }
         
        }

    }

    private void Dodge()
    {

        Debug.Log("DODGE");
        gameObject.AddComponent<Rigidbody>();
        Rigidbody rgb = gameObject.GetComponent<Rigidbody>();
        rgb.AddForce(transform.localPosition.x+7f*Time.deltaTime, transform.localPosition.y , transform.localPosition.z);
     
 
      

        
    }
    private void Walk()
    {
        //transform.Translate(Vector3.forward * Time.deltaTime, Space.Self); 
        transform.LookAt(nodes[currectNode].position);
        transform.position = Vector3.MoveTowards(transform.position, nodes[currectNode].position, Time.deltaTime*5);

       
    }
    private void CheckWaypointDistance()
    {
        if (Vector3.Distance(transform.position, nodes[currectNode].position) < 5f)
        {
            if (currectNode == nodes.Count - 1)//если послдений элемент
            {
                currectNode = 0;
            }
            else
            {
                currectNode++;
            }
        }  
        
    }
    private void OnTriggerEnter(Collider other)
    {
        if(other.tag=="Player"&&currentState==PedState.walk)
        {
            currentState = PedState.crash;
            gameObject.AddComponent<Rigidbody>();
            StartCoroutine("DestroyRigidbody");
        }
    }
    IEnumerator DestroyRigidbody()
    {
        yield return new WaitForSeconds(5f);
        
        Destroy(gameObject.GetComponent<Rigidbody>());
        gameObject.GetComponent<CapsuleCollider>().radius = 0.01f;
    }

}
