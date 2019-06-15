using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CarEngine : MonoBehaviour
{
    public float turnSpeed = 15f;
    public Transform path;
    public float maxSteerAngle = 45f;
    public WheelCollider wheelFL;
    public WheelCollider wheelFR;
    public WheelCollider wheelRL;
    public WheelCollider wheelRR;
    public float maxMotorTorque = 80f;
    public float maxBrakeTorque = 10000f;
    public float currentSpeed;
    public float maxSpeed = 100f;
    public Vector3 centerOfMass;
    public bool isBraking = false;
    public Texture2D textureNormal;
    public Texture2D textureBraking;
    public Renderer carRenderer;
    private float m_CurrentTorque;
    [Range(0, 1)] [SerializeField] private float m_TractionControl; // 0 is no traction control, 1 is full interference
    [SerializeField] private float m_FullTorqueOverAllWheels;
    private Rigidbody rgb;
    [Header("Sensors")]
    public float sensorLength = 3f;
    public Vector3 frontSensorPosition = new Vector3(0f, 0.2f, 0.5f);
    public float frontSideSensorPosition = 0.2f;
    public float frontSensorAngle = 30f;
    public bool col = false;
    private List<Transform> nodes;
    private int currectNode = 0;
    private bool avoiding = false;
    private float targerSteerAngle = 0;

    public GameObject Player;


    private void Start()
    {
        GetComponent<Rigidbody>().centerOfMass = centerOfMass;
        rgb = gameObject.GetComponent<Rigidbody>();
        Transform[] pathTransforms = path.GetComponentsInChildren<Transform>();
        nodes = new List<Transform>();

        for (int i = 0; i < pathTransforms.Length; i++)
        {
            if (pathTransforms[i] != path.transform)
            {
                nodes.Add(pathTransforms[i]);
            }
        }
        m_CurrentTorque = m_FullTorqueOverAllWheels - (m_TractionControl * m_FullTorqueOverAllWheels);
    }

    private void FixedUpdate()
    {

        ApplySteer();
        if (gameObject.GetComponent<EnemySight>().state != State.chase)//чекаем есть погоня или нет
        {
            Sensors();
        }
        else
        {
            Chase();
        }
        Drive();


        CheckWaypointDistance();
        Braking();
        LerpToSteerAngle();
    }
    private void OnTriggerStay(Collider other)
    {


        if (rgb.velocity.magnitude < 5f && !other.CompareTag("Ground"))

        {


            StartCoroutine("Timer");
            StartCoroutine("Back");
        }

    }
    IEnumerator Timer()
    {
        isBraking = true;
        yield return new WaitForSeconds(4);
        isBraking = false;
    }

    IEnumerator Back()
    {
        col = true;
        int i = 0;
        while (i <= 2)
        {
            yield return new WaitForSeconds(1);


            rgb.AddForce(0, 0, -200);
            i++;
        }

        col = false;

    }
    private void Sensors()
    {
        RaycastHit hit;
        Vector3 sensorStartPos = transform.position;
        sensorStartPos += transform.forward * frontSensorPosition.z;
        sensorStartPos += transform.up * frontSensorPosition.y;
        float avoidMultiplier = 0;
        avoiding = false;

        //front right sensor
        sensorStartPos += transform.right * frontSideSensorPosition;
        if (Physics.Raycast(sensorStartPos, transform.forward, out hit, sensorLength))
        {
            if (!hit.collider.CompareTag("Ground"))
            {
                Debug.DrawLine(sensorStartPos, hit.point);
                avoiding = true;
                avoidMultiplier -= 1f;
            }
            else if (hit.collider.CompareTag("CarAi"))
            {
                Debug.DrawLine(sensorStartPos, hit.point);
                Debug.Log("CAR!");
                isBraking = true;
            }
        }

        //front right angle sensor
        else if (Physics.Raycast(sensorStartPos, Quaternion.AngleAxis(frontSensorAngle, transform.up) * transform.forward, out hit, sensorLength))
        {
            if (!hit.collider.CompareTag("Ground"))
            {
                Debug.DrawLine(sensorStartPos, hit.point);
                avoiding = true;
                avoidMultiplier -= 0.5f;
            }         
        }

        //front left sensor
        sensorStartPos -= transform.right * frontSideSensorPosition * 2;
        if (Physics.Raycast(sensorStartPos, transform.forward, out hit, sensorLength))
        {
            if (!hit.collider.CompareTag("Ground"))
            {
                Debug.DrawLine(sensorStartPos, hit.point);
                avoiding = true;
                avoidMultiplier += 1f;
            }
        
        }

        //front left angle sensor
        else if (Physics.Raycast(sensorStartPos, Quaternion.AngleAxis(-frontSensorAngle, transform.up) * transform.forward, out hit, sensorLength))
        {
            if (!hit.collider.CompareTag("Ground"))
            {
                Debug.DrawLine(sensorStartPos, hit.point);
                avoiding = true;
                avoidMultiplier += 0.5f;
            }         
        }

        //front center sensor
       
        if (Physics.Raycast(sensorStartPos, transform.forward, out hit, sensorLength))
        {
            if (!hit.collider.CompareTag("Ground"))
            {
                Debug.DrawLine(sensorStartPos, hit.point);
                avoiding = true;
                if (hit.normal.x < 0)
                {
                    avoidMultiplier = -1;
                }
                else
                {
                    avoidMultiplier = 1;
                }
            }
            //front center sensor
            if (Physics.Raycast(sensorStartPos, transform.forward, out hit, 18))//торможение при замечании другой машины
            {
                if (hit.collider.CompareTag("CarAi"))
                {

                    Debug.DrawLine(sensorStartPos, hit.point);
                    StartCoroutine("Timer");
                
                }
            }

            //front left angle sensor
            if (Physics.Raycast(sensorStartPos, Quaternion.AngleAxis(-frontSensorAngle, transform.up) * transform.forward, out hit, sensorLength))
            {
                if (hit.collider.CompareTag("CarAi"))
                {
                    Debug.DrawLine(sensorStartPos, hit.point);
                  
                }

               
            }
            //front right angle sensor
            if (Physics.Raycast(sensorStartPos, Quaternion.AngleAxis(frontSensorAngle, transform.up) * transform.forward, out hit, sensorLength))
            {
                if (hit.collider.CompareTag("CarAi"))
                {
                    Debug.DrawLine(sensorStartPos, hit.point);

                }
            }
          
            if (Physics.Raycast(sensorStartPos, transform.forward, out hit, sensorLength))
            {
                if (hit.collider.CompareTag("CarAi"))
                {
                    Debug.DrawLine(sensorStartPos, hit.point);
                    StartCoroutine("Timer");
                }

            }
           
            if (Physics.Raycast(sensorStartPos, transform.forward, out hit, sensorLength))
            {
                if (hit.collider.CompareTag("CarAi"))
                {
                    Debug.DrawLine(sensorStartPos, hit.point);
                    StartCoroutine("Timer");
                }

            }
              

        }
        if (Physics.Raycast(sensorStartPos, transform.forward, out hit, sensorLength)&&gameObject.GetComponent<EnemySight>().state==State.chase)//отключение уклонения
        {
            if (hit.collider.CompareTag("Player"))
            {

                Debug.DrawLine(sensorStartPos, hit.point);
          

            }
            sensorStartPos += transform.right * frontSideSensorPosition;
            if (Physics.Raycast(sensorStartPos, transform.forward, out hit, sensorLength) && gameObject.GetComponent<EnemySight>().state == State.chase)
            {
                if (!hit.collider.CompareTag("Ground"))
                {
                    Debug.DrawLine(sensorStartPos, hit.point);
                    avoiding = true;
                    avoidMultiplier -= 1f;
                }
                else if (hit.collider.CompareTag("CarAi"))
                {
                    Debug.DrawLine(sensorStartPos, hit.point);
                    Debug.Log("CAR!");
                    isBraking = true;
                }
            }
            sensorStartPos -= transform.right * frontSideSensorPosition * 2;
            if (Physics.Raycast(sensorStartPos, transform.forward, out hit, sensorLength) && gameObject.GetComponent<EnemySight>().state == State.chase)
            {
                if (!hit.collider.CompareTag("Ground"))
                {
                    Debug.DrawLine(sensorStartPos, hit.point);
                    avoiding = true;
                    avoidMultiplier += 1f;
                }

            }
        }
        


            //}

            if (avoiding)
        {
            targerSteerAngle = maxSteerAngle * avoidMultiplier;
            wheelFL.steerAngle = maxSteerAngle * avoidMultiplier;
            wheelFR.steerAngle = maxSteerAngle * avoidMultiplier;
        }

    }
  
    private void Chase()
    {
        RaycastHit hit;
        Vector3 sensorStartPos = transform.position;
        sensorStartPos += transform.forward * frontSensorPosition.z;
        sensorStartPos += transform.up * frontSensorPosition.y;
        float avoidMultiplier = 0;
        avoiding = false;
        if (Physics.Raycast(sensorStartPos, transform.forward, out hit, sensorLength))//(центральный луч)
        {
            if (hit.collider.CompareTag("Player"))
            {

                Debug.DrawLine(sensorStartPos, hit.point);


            }
            if (hit.collider.CompareTag("Obstacle"))
            {

                Debug.DrawLine(sensorStartPos, hit.point);


            }
        }
        sensorStartPos += transform.right * frontSideSensorPosition;
        if (Physics.Raycast(sensorStartPos, transform.forward, out hit, sensorLength))//(правый центральный луч)
        {
            if (hit.collider.CompareTag("Player"))
            {
                Debug.DrawLine(sensorStartPos, hit.point);

            }
        }
            sensorStartPos += transform.right * frontSideSensorPosition;
        if (Physics.Raycast(sensorStartPos, transform.forward, out hit, sensorLength))//(правый центральный луч)
        {
            if (hit.collider.CompareTag("Obstacle"))
            {
                Debug.DrawLine(sensorStartPos, hit.point);
                avoiding = true;
                avoidMultiplier -= 1f;
            }
        }

            sensorStartPos -= transform.right * frontSideSensorPosition * 2;

        if (Physics.Raycast(sensorStartPos, transform.forward, out hit, sensorLength))//(левый центральный луч)
        {
            if (hit.collider.CompareTag("Player"))
            {
                Debug.DrawLine(sensorStartPos, hit.point);


            }
        }
        sensorStartPos -= transform.right * frontSideSensorPosition * 2;


        if (Physics.Raycast(sensorStartPos, transform.forward, out hit, sensorLength))//(левый центральный луч)
        { if (hit.collider.CompareTag("Obstacle"))
            {
                Debug.DrawLine(sensorStartPos, hit.point);
                avoiding = true;
                avoidMultiplier += 1f;
            }
        }



        if (avoiding)
            {
                targerSteerAngle = maxSteerAngle * avoidMultiplier;
                wheelFL.steerAngle = maxSteerAngle * avoidMultiplier;
                wheelFR.steerAngle = maxSteerAngle * avoidMultiplier;
            }
        
    }

    private void ApplySteer()
    {
        if (avoiding) return;
        if (gameObject.GetComponent<EnemySight>().state == State.chase)
        {
            Vector3 relativeVector = transform.InverseTransformPoint(Player.transform.position);
            float newSteer = (relativeVector.x / relativeVector.magnitude) * maxSteerAngle;
            targerSteerAngle = newSteer;
        }

        else
        {
            Vector3 relativeVector = transform.InverseTransformPoint(nodes[currectNode].position);
            float newSteer = (relativeVector.x / relativeVector.magnitude) * maxSteerAngle;
            targerSteerAngle = newSteer;
        }

        if (col == true)
        {

            wheelFL.steerAngle = 0f;
            wheelFR.steerAngle = 0f;
        }
    }

    private void Drive()
    {
        currentSpeed = 2 * Mathf.PI * wheelFL.radius * wheelFL.rpm * 60 / 1000;

        if (currentSpeed < maxSpeed && !isBraking && col == false)
        {   maxMotorTorque = 80f;
            wheelFL.motorTorque = maxMotorTorque;
            wheelFR.motorTorque = maxMotorTorque;

            //wheelFR.transform.Rotate(-wheelFL.rpm / 60 * 360 * Time.deltaTime, 0f, 0f);


        }
        else if (currentSpeed < maxSpeed && !isBraking && col == true)
        {
            wheelFL.motorTorque = 0;
            wheelFR.motorTorque = 0;

            //wheelFR.transform.Rotate(-wheelFL.rpm / 60 * 360 * Time.deltaTime, 0f, 0f);


        }

        else
        {
         
            wheelFL.motorTorque = 0;
            wheelFR.motorTorque = 0;
            wheelRL.motorTorque = 0;
            wheelRR.motorTorque = 0;
        }
         if (currentSpeed < maxSpeed && !isBraking && gameObject.GetComponent<EnemySight>().state == State.chase && col == false)
        {
            maxMotorTorque = 250f;
            maxSpeed = 300f;


            wheelFL.motorTorque = maxMotorTorque * 10000;
            wheelFR.motorTorque = maxMotorTorque * 10000;


            //wheelRL.attachedRigidbody.AddForce(transform.up*10000 * wheelRL.attachedRigidbody.velocity.magnitude,ForceMode.Acceleration);
            //wheelFL.attachedRigidbody.AddForce(transform.up*10000 * wheelRL.attachedRigidbody.velocity.magnitude, ForceMode.Acceleration);





        }
        else
        {
            maxSpeed = 100f;
            maxMotorTorque = 80f;
            wheelFL.motorTorque = maxMotorTorque;
            wheelFR.motorTorque = maxMotorTorque;
        }
    
    }

    private void CheckWaypointDistance()
    {
        if (Vector3.Distance(transform.position, nodes[currectNode].position) < 5f)
        {
            if (currectNode == nodes.Count - 1)
            {
                currectNode = 0;
            }
            else
            {
                currectNode++;
            }
        }
       

    }

    private void Braking()
    {
        if (isBraking) 
        {


    
            wheelRL.brakeTorque = maxBrakeTorque*5;
            wheelRR.brakeTorque = maxBrakeTorque*5;
  
        }
        else
        {
      
            wheelRL.brakeTorque = 0;
            wheelRR.brakeTorque = 0;
            wheelFL.brakeTorque =0;
            wheelFR.brakeTorque =0;
        }
    }
    private Vector3 SetTarget()
    {
        if (gameObject.GetComponent<EnemySight>().state == State.chase)
        {
            return Player.transform.position;
        }
        else
        {
            return nodes[currectNode].position;
        }
    }
    private void LerpToSteerAngle()
    {
        wheelFL.steerAngle = Mathf.Lerp(wheelFL.steerAngle, targerSteerAngle, Time.deltaTime * turnSpeed);
        wheelFR.steerAngle = Mathf.Lerp(wheelFL.steerAngle, targerSteerAngle, Time.deltaTime * turnSpeed);
    }
}
