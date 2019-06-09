using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RightFrontPart : DamageSystem
{
    public float CubeHealthFB = 100f;
    Renderer rendFB;
    MeshFilter thisMeshFB;
    //Set the main Color of the Material to green
    

    private void Start()
    {
        rgb = GetComponent<Rigidbody>();
        thisMeshFB = GetComponent<MeshFilter>();
        rendFB = normalParts[0].GetComponent<Renderer>();

        currentRenderB = rendFB;
        rendFB.material.SetColor("_Color", Color.green);



    }

    private void Update()
    {
      
        if (CubeHealthFB <= 80 && extraPartsB[0] != null)
        {
            rendFB.enabled = false;
            extraPartsB[0].SetActive(true);
            currentRenderB = extraPartsB[0].GetComponent<Renderer>();
            currentRenderB.enabled = true;


            currentRenderB.sharedMaterial.SetColor("_Color", Color.yellow);
        }
        if (CubeHealthFB <= 60 && extraPartsB[0] != null)
        {
            //    Parucl[0].gameObject.SetActive(true);
            currentRenderB.sharedMaterial.SetColor("_Color", Color.yellow);
        }
        if (CubeHealthFB <= 25 && extraPartsB[0] != null)
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
        rendFB = normalParts[0].GetComponent<Renderer>();
        rendFB.sharedMaterial.SetColor("_Color", Color.green);

        //damageParts[0] = extraParts[0];
        //damageParts[0].enabled = false;
        CubeHealthFB = 100f;
    }

    private void OnCollisionEnter(Collision collision)
    {
        float power = collision.relativeVelocity.magnitude;
        //rgb.freezeRotation = true;
        //rgb.constraints = RigidbodyConstraints.FreezeRotationX;
        if (power > 10f)
        {
            rgb.detectCollisions = false;


            CubeHealthFB -= 15f;
            DamageManager.instance.DamagePoints -= 5
                ;

            StartCoroutine("TurnOn");
        }
        if (CubeHealth <= 15)
        {
            currentRenderB.enabled = false;
          
        }

    }
    IEnumerator TurnOn()
    {
        yield return new WaitForSeconds(1);
        rgb.detectCollisions = true;
    }
    private void OnCollisionStay(Collision collision)
    {
        if (Player.GetComponent<Rigidbody>().velocity.magnitude > 2f && collision.gameObject.tag != "Ground")
            //rgb.constraints = RigidbodyConstraints.FreezeRotationX & RigidbodyConstraints.FreezeRotationZ;
            CubeHealth -= Time.deltaTime * 1;
    }
    private void OnCollisionExit(Collision collision)
    {
        //rgb.WakeUp();
        //rgb.freezeRotation = false;
        //rgb.constraints = RigidbodyConstraints.None;


    }

    

  
   
}
