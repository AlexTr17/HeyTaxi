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
    public float maxBrakeTorque = 150f;
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
        Sensors();
        //AddForce(new Vector3(0, 0, -10));
        Drive(maxMotorTorque);


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

        yield return new WaitForSeconds(4);

    }

    IEnumerator Back()
    {
        col = true;
        int i = 0;
        while (i <= 2)
        {
            yield return new WaitForSeconds(1);


            rgb.AddForce(0, 0, -1000);
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
            //else if (hit.collider.CompareTag("CarAi"))
            //{
            //    Debug.DrawLine(sensorStartPos, hit.point);
            //    avoiding = true;
            //    Braking();
            //}
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
            //else if (hit.collider.CompareTag("CarAi"))
            //{
            //    Debug.DrawLine(sensorStartPos, hit.point);
            //    avoiding = true;
            //   Braking();
            //}
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
            //else if (hit.collider.CompareTag("CarAi"))
            //{
            //    Debug.DrawLine(sensorStartPos, hit.point);
            //    avoiding = true;
            //    Braking();
            //}
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

            //else if (hit.collider.CompareTag("CarAi"))
            //{
            //    Debug.DrawLine(sensorStartPos, hit.point);
            //    avoiding = true;
            //    Braking();
            //}
        }

        //front center sensor
        //if (avoidMultiplier == 0)
        //{
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
            if (!hit.collider.CompareTag("Ground"))
            {
                Braking();
            }
        }
        //else if (hit.collider.CompareTag("CarAi"))
        //{
        //    Debug.DrawLine(sensorStartPos, hit.point);
        //    avoiding = true;
        //    Braking();
        //}
        //}

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
            Debug.Log("OFFROTATE");
            wheelFL.steerAngle = 0f;
            wheelFR.steerAngle = 0f;
        }
    }

    private void Drive(float max)
    {
        currentSpeed = 2 * Mathf.PI * wheelFL.radius * wheelFL.rpm * 60 / 1000;

        if (currentSpeed < maxSpeed && !isBraking && col == false)
        {
            wheelFL.motorTorque = max * 5;
            wheelFR.motorTorque = max * 5;

            //wheelFR.transform.Rotate(-wheelFL.rpm / 60 * 360 * Time.deltaTime, 0f, 0f);


        }
        else if (currentSpeed < maxSpeed && !isBraking && col == true)
        {
            wheelFL.motorTorque = 0;
            wheelFR.motorTorque = 0;

            //wheelFR.transform.Rotate(-wheelFL.rpm / 60 * 360 * Time.deltaTime, 0f, 0f);


        }
        else if (currentSpeed < maxSpeed && !isBraking && gameObject.GetComponent<EnemySight>().state == State.chase && col == false)
        {
            wheelFL.motorTorque = max * 5;

            wheelFR.motorTorque = max * 5;

        }
        else
        {
            wheelFL.motorTorque = 0;
            wheelFR.motorTorque = 0;
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
            //carRenderer.material.mainTexture = textureBraking;
            wheelRL.brakeTorque = maxBrakeTorque;
            wheelRR.brakeTorque = maxBrakeTorque;
        }
        else
        {
            //carRenderer.material.mainTexture = textureNormal;
            wheelRL.brakeTorque = 0;
            wheelRR.brakeTorque = 0;
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
