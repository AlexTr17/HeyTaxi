using System;
using UnityEngine;

namespace UnityStandardAssets.Vehicles.Car
{
    // this script is specific to the supplied Sample Assets car, which has mudguards over the front wheels
    // which have to turn with the wheels when steering is applied.

    public class Mudguard : MonoBehaviour
    {
        public CarController carController; // car controller to get the steering angle

        private Quaternion m_OriginalRotation;

        public float sensorLength = 3f;
        public Vector3 frontSensorPosition = new Vector3(0f, 0.2f, 0.5f);
        public float frontSideSensorPosition = 0.2f;
        public float frontSensorAngle = 30f;
        public bool avoiding = false;
     
        private void Start()
        {
            m_OriginalRotation = transform.localRotation;
        }


        private void Update()
        {
            transform.localRotation = m_OriginalRotation*Quaternion.Euler(0, carController.CurrentSteerAngle, 0);
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
                if (!hit.collider.CompareTag("Obstacle"))
                {
                    Debug.DrawLine(sensorStartPos, hit.point);
                    avoiding = true;
                    avoidMultiplier -= 1f;
                }
            }

            //front right angle sensor
            else if (Physics.Raycast(sensorStartPos, Quaternion.AngleAxis(frontSensorAngle, transform.up) * transform.forward, out hit, sensorLength))
            {
                if (!hit.collider.CompareTag("Obstacle"))
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
                if (!hit.collider.CompareTag("Obstacle"))
                {
                    Debug.DrawLine(sensorStartPos, hit.point);
                    avoiding = true;
                    avoidMultiplier += 1f;
                }
            }

            //front left angle sensor
            else if (Physics.Raycast(sensorStartPos, Quaternion.AngleAxis(-frontSensorAngle, transform.up) * transform.forward, out hit, sensorLength))
            {
                if (!hit.collider.CompareTag("Obstacle"))
                {
                    Debug.DrawLine(sensorStartPos, hit.point);
                    avoiding = true;
                    avoidMultiplier += 0.5f;
                }
            }

            //front center sensor
            if (avoidMultiplier == 0)
            {
                if (Physics.Raycast(sensorStartPos, transform.forward, out hit, sensorLength))
                {
                    if (!hit.collider.CompareTag("Obstacle"))
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
                }
            }

            if (avoiding)
            {
                transform.localRotation = m_OriginalRotation * Quaternion.Euler(0, carController.CurrentSteerAngle*avoidMultiplier, 0); ;
                //wheelFL.steerAngle = maxSteerAngle * avoidMultiplier;
                //wheelFR.steerAngle = maxSteerAngle * avoidMultiplier;
            }

        }
    }
}
