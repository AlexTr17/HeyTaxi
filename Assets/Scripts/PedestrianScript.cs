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
        
        // for(int i=0;i<pathTransforms.Length;i++)
        //{
           
        //    if(Vector3.Distance(transform.position, pathTransforms[i].position)>Vector3.Distance(transform.position,pathTransforms[currectNode].position))
        //    {
        //        currectNode = number++;
        //        number++;
        //        if(currectNode>=pathTransforms.Length)
        //        {
        //            currectNode = 0;
        //        }
        //    }
        //    else
        //    {
        //        number++;
        //    }

        //}

    

 
    // Update is called once per frame
    private void Update()
    {  if (currentState == PedState.walk)

        {
            CheckWaypointDistance();
            Walk();
        }
        else
        {
          
        }
     
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
        if(other.tag=="Player")
        {
            currentState = PedState.crash;
            gameObject.AddComponent<Rigidbody>();
            this.enabled = false;
        }
    }

}
