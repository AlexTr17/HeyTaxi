using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.Vehicles.Car;

public class LightScript : MonoBehaviour
{
    public enum Lights
    {
        red,
        yellow,
        green
    }
    public Lights lights;
    private void Awake()
    { lights = Lights.red;
        transform.GetChild(0).gameObject.SetActive(false);
        transform.GetChild(1).gameObject.SetActive(false);
            LIghtManager.startlight += StartLight;
    }
    // Start is called before the first frame update
    void Start()
    {
       
    
        
    }
    public void StartLight()
    {  
        StartCoroutine("Switch");

    }

    // Update is called once per frame
    IEnumerator Switch()
    {
        lights = Lights.green;
        transform.GetChild(0).gameObject.SetActive(false);
        transform.GetChild(2).gameObject.SetActive(true);
        yield return new WaitForSeconds(15);
        StartCoroutine("Switch2");

    }
    IEnumerator Switch2()
    {
        
        StopCoroutine("Switch");
        lights = Lights.yellow;
        transform.GetChild(2).gameObject.SetActive(false);
        transform.GetChild(1).gameObject.SetActive(true);
        yield return new WaitForSeconds(2);
        StartCoroutine("Switch3");


    }
    IEnumerator Switch3()
    {
        StopCoroutine("Switch2");
        lights = Lights.red;
        transform.GetChild(1).gameObject.SetActive(false);
        transform.GetChild(0).gameObject.SetActive(true);
        yield return new WaitForSeconds(8);
        StartCoroutine("Switch4");


    }
    IEnumerator Switch4()
    {
        StopCoroutine("Switch3");
        lights = Lights.yellow;
        transform.GetChild(0).gameObject.SetActive(false);
        transform.GetChild(1).gameObject.SetActive(true);
        yield return new WaitForSeconds(2);
        transform.GetChild(1).gameObject.SetActive(false);
        StartCoroutine("Switch");
        //Debug.Log("yellow");



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
