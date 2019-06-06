using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageSystem : MonoBehaviour
{
    public float CubeHealth = 100f;
    Renderer rend ;
    MeshFilter thisMesh;
    //Set the main Color of the Material to green
    public Rigidbody rgb;
    public List<GameObject> damageParts;
    public List<GameObject>extraPartsB;
    public GameObject[] normalParts;
    public GameObject Player;
    public Renderer currentRenderB;

    private void Start()
    {
        rgb = GetComponent<Rigidbody>();
        thisMesh = GetComponent<MeshFilter>();
        rend = normalParts[0].GetComponent<Renderer>();
       
        currentRenderB = rend;
        rend.material.SetColor("_Color",Color.green);
        ExtraPart();
  
      
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.LeftAlt))
        {
            Restore();
        }
       
        if (CubeHealth <= 80&&extraPartsB[0]!=null)
        {
            rend.enabled = false;
            extraPartsB[0].SetActive(true);
            currentRenderB = extraPartsB[0].GetComponent<Renderer>();
            currentRenderB.enabled = true;


            currentRenderB.sharedMaterial.SetColor("_Color", Color.yellow);
        }
        if (CubeHealth <= 60 && extraPartsB[0] != null)
        {
            //    Parucl[0].gameObject.SetActive(true);
            currentRenderB.sharedMaterial.SetColor("_Color", Color.yellow);
        }
        if(CubeHealth <= 25 && extraPartsB[0] != null)
        {
            //Parucl[0].gameObject.SetActive(false);
            //Parucl[1].gameObject.SetActive(true);
            currentRenderB.sharedMaterial.SetColor("_Color", Color.red);
        }
      
        
        
    }

    private void Restore()
    {
     
        extraPartsB.Clear();
        ExtraPart();
        normalParts[0].GetComponent<Renderer>().enabled = true;
        rend = normalParts[0].GetComponent<Renderer>();
        rend.sharedMaterial.SetColor("_Color", Color.green);

        //damageParts[0] = extraParts[0];
        //damageParts[0].enabled = false;
        CubeHealth = 100f;
    }

    private void OnCollisionEnter(Collision collision)
    {
        float power = collision.relativeVelocity.magnitude;
        //rgb.freezeRotation = true;
        //rgb.constraints = RigidbodyConstraints.FreezeRotationX;
        if (power>10f)
        {
            rgb.detectCollisions=false;

      
            CubeHealth -= 30f;
            DamageManager.instance.DamagePoints -= 5;

            StartCoroutine("TurnOn");
        }
        if(CubeHealth<=15)
        {
            DamageManager.instance.DamagePoints -= 5;
            if(extraPartsB[0]!=null)
            DetachPart();
        }

      
    }
    IEnumerator TurnOn()
    {
        yield return new WaitForSeconds(1);
        rgb.detectCollisions = true;
    }
    private void OnCollisionStay(Collision collision)
    {
        if(Player.GetComponent<Rigidbody>().velocity.magnitude>2f&&collision.gameObject.tag!="Ground")
        //rgb.constraints = RigidbodyConstraints.FreezeRotationX & RigidbodyConstraints.FreezeRotationZ;
        CubeHealth -= Time.deltaTime * 1;
    }
    private void OnCollisionExit(Collision collision)
    {
        //rgb.WakeUp();
        //rgb.freezeRotation = false;
        //rgb.constraints = RigidbodyConstraints.None;


    }

    internal virtual void ExtraPart()
    {
        extraPartsB.Add(Instantiate(damageParts[0], new Vector3(transform.position.x, transform.position.y, transform.position.z),
            Quaternion.Euler(gameObject.transform.rotation.x, gameObject.transform.rotation.y +90, gameObject.transform.rotation.z)));
        extraPartsB[0].transform.SetParent(Player.transform);
        extraPartsB[0].transform.localRotation = Quaternion.identity;
        extraPartsB[0].transform.Rotate(0, 90, 0);
        extraPartsB[0].SetActive(false);
    }

    //void ExtraP()
    //{
    //    extraParts.Add(Instantiate(damageParts[0], new Vector3(transform.position.x, transform.position.y, transform.position.z),
    //        Quaternion.Euler(0, 90, 0)));
    //    extraParts[0].transform.SetParent(Player.transform);

    //    extraParts[0].SetActive(false);
    //}
   internal virtual void DetachPart()
    {


        
        //damageParts[0].SetActive(false);
        extraPartsB[0].gameObject.AddComponent<BoxCollider>();
        //damageParts[0].gameObject.AddComponent<Rigidbody>();
        StartCoroutine("Rgb");
        currentRenderB = null;
        Destroy(extraPartsB[0].gameObject, 3f);
    
    }
    public IEnumerator Rgb()
    {
        yield return new WaitForSeconds(0.1f);
        extraPartsB[0].gameObject.AddComponent<Rigidbody>();
    
    }
}
