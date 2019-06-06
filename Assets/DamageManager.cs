using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.Vehicles.Car;

public class DamageManager : MonoBehaviour
{
    public static DamageManager instance;
 
    public ParticleSystem[] Parucl = new ParticleSystem[3];
    Renderer rend;
    public float DamagePoints;
    CarController car;
    [SerializeField] GameObject Player;
    // Start is called before the first frame update
    private void Awake()
    {
        if (instance == null)
            instance = this;
    }
    void Start()
    {
        DamagePoints = 100f;
    

    }

    // Update is called once per frame
    void Update()
    {
       

        if (DamagePoints <= 60)
        {
            Parucl[0].gameObject.SetActive(true);
       
        }
        if (DamagePoints <= 25)
        {
            Parucl[0].gameObject.SetActive(false);
            Parucl[1].gameObject.SetActive(true);
         
        }
        if(DamagePoints<=0 && Player.GetComponent<Rigidbody>().velocity.magnitude>1f)
        {

            Player.GetComponent<CarUserControl>().enabled = false;
            Player.GetComponent<CarController>().BrakePer();
        }
    }
}
