using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.Vehicles.Car;

public class traffic_light_test : MonoBehaviour
{
    
    public enum Lights
    {
        red, yellow, green
    }
 	
    public Lights lights;

 	public GameObject RedLight, YellowLight, GreenLight;
	public float RedLightTime, YellowLightTime, GreenLightTime, DelayLightTime;
	
	private void Awake()
    { 
        lights = Lights.red;
    	RedLight.transform.Rotate (0,180,0);
    	YellowLight.transform.Rotate (0,0,0);
    	GreenLight.transform.Rotate (0,0,0);
    	LIghtManager.startlight += StartLight;
    }

   public void StartLight()
    {  
        StartCoroutine("Switch");
    }

    IEnumerator Switch()
    {
        lights = Lights.red;
    	RedLight.transform.Rotate (0,0,0);
        yield return new WaitForSeconds(RedLightTime);
        StartCoroutine("Switch2");
    }

    IEnumerator Switch2()
    {
        StopCoroutine("Switch");
        lights = Lights.yellow;
        RedLight.transform.Rotate (0,180,0);
        YellowLight.transform.Rotate (0,180,0);
        yield return new WaitForSeconds(YellowLightTime);
        StartCoroutine("Switch3");
    }

    IEnumerator Switch3()
    {
        StopCoroutine("Switch2");
        lights = Lights.green;
        YellowLight.transform.Rotate (0,180,0);
        GreenLight.transform.Rotate (0,180,0);
        yield return new WaitForSeconds(GreenLightTime);
        StartCoroutine("Switch4");
    }
    
    IEnumerator Switch4()
    {
        StopCoroutine("Switch3");
        GreenLight.transform.Rotate (0,180,0);
        RedLight.transform.Rotate (0,180,0);
        yield return new WaitForSeconds(DelayLightTime);
        StartCoroutine("Switch");
    }

    private void OnTriggerStay(Collider other)
    {

        if (other.gameObject.tag == "CarAi" && lights == Lights.red  )
        {
            other.GetComponent<CarEngine>().isBraking=true;
        }
        else if (other.gameObject.tag == "CarAi" && lights == Lights.yellow)
        {
            other.GetComponent<CarEngine>().isBraking = true;
        }
        else if  (other.gameObject.tag == "CarAi" && lights == Lights.green) 
                { other.GetComponent<CarEngine>().isBraking = false; }
    }
}
