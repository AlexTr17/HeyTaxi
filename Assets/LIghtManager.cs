using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LIghtManager : MonoBehaviour
{

    //public static LIghtManager instance;
    public delegate void Light();
    public static event  Light startlight;

    // Start is called before the first frame update
    void Start()
    {
        if(startlight!=null)
        startlight.Invoke();
    }

    // Update is called once per frame
 
}
