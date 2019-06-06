using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityStandardAssets.Vehicles.Car;

public enum State
{
    patrol,
    suspicion,
    chase
}
public class EnemySight : MonoBehaviour
{
    public State state;
    public float viewRadius;
    [Range(0, 360)]
    public float viewAngle;
    public float count = 3f;
    public LayerMask targetMask;
    public LayerMask obstacleMask;
    CarAIControl carAi;
    public GameObject waypoint;
    public GameObject CarAI;

    public List<Transform> visibleTargets = new List<Transform>();
    GameObject Player;
    void Start()
    {
        state = State.suspicion;
        Player = GameObject.FindGameObjectWithTag("Player");
        StartCoroutine("FindTargetsWithDelay", .2f);
      
    }


    IEnumerator FindTargetsWithDelay(float delay)
    {
        while (true)
        {
            yield return new WaitForSeconds(delay);
            FindVisibleTargets();
        }
    }

    //private void Update()
    //{
    //    if (gameObject.GetComponent<EnemySight>().state == State.patrol && gameObject.tag == "Police")
    //    {
    //        gameObject.GetComponent<CarAIControl>().SetTarget(waypoint.transform);
    //    }
    //    else if (gameObject.GetComponent<EnemySight>().state == State.suspicion)
    //    { gameObject.GetComponent<CarAIControl>().SetTarget(Player.transform); ; }
    //    else if (gameObject.GetComponent<EnemySight>().state == State.chase )
    //    { gameObject.GetComponent<CarAIControl>().SetTarget(Player.transform); ; }
    //}
    void FindVisibleTargets()
    {
        
        visibleTargets.Clear();
        Collider[] targetsInViewRadius = Physics.OverlapSphere(transform.position, viewRadius, targetMask);

        for (int i = 0; i < targetsInViewRadius.Length; i++)
        {
            Transform target = targetsInViewRadius[i].transform;
              
            Vector3 dirToTarget = (target.position - transform.position).normalized;

            if (Vector3.Angle(transform.forward, dirToTarget) < viewAngle / 2)
            {
                float dstToTarget = Vector3.Distance(transform.position, target.position);

                if (!Physics.Raycast(transform.position, dirToTarget, dstToTarget, obstacleMask) && state == State.patrol)
                {
                    visibleTargets.Add(target);
                }
                else if (!Physics.Raycast(transform.position, dirToTarget, dstToTarget, obstacleMask) && state == State.suspicion)
                {
                    visibleTargets.Add(target);
                    state = State.chase;
                   StopCoroutine("Patrol");

                }
               
                else if (!Physics.Raycast(transform.position, dirToTarget, dstToTarget, obstacleMask) && state == State.chase)
                {
                  
                    visibleTargets.Add(target);
                    
                    state = State.chase;
                    StopCoroutine("Suspicion");
                }
               
            }  
             else if (Vector3.Angle(transform.forward, dirToTarget) > viewAngle / 2 && state==State.chase )
            {
               
                StartCoroutine("Suspicion",10f);
            }
            else if (Vector3.Angle(transform.forward, dirToTarget) >viewAngle / 2 && state == State.suspicion)
            {
              
                StartCoroutine("Patrol",3f);
            }
     
        }
       
    }

    IEnumerator Suspicion(float count)
    {
     
        yield return new WaitForSeconds(count);
        
            state = State.suspicion;
      
    }
    IEnumerator Patrol(float count)
    {
    
        yield return new WaitForSeconds(count);

        state = State.patrol;
    }

    public Vector3 DirFromAngle(float angleInDegrees, bool angleIsGlobal)
    {
        if (!angleIsGlobal)
        {
            angleInDegrees += transform.eulerAngles.y;
        }
        return new Vector3(Mathf.Sin(angleInDegrees * Mathf.Deg2Rad), 0, Mathf.Cos(angleInDegrees * Mathf.Deg2Rad));
    }
}