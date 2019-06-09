using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LeftBackCollder : DamageSystem
{
    public float CubeHealthLPB = 100f;
    Renderer rendLPB;
    MeshFilter thisMeshLpb;
    // Start is called before the first frame update
    private void Start()
    {
        rgb = GetComponent<Rigidbody>();
        thisMeshLpb = GetComponent<MeshFilter>();
        rendLPB = normalParts[0].GetComponent<Renderer>();

        currentRenderB = rendLPB;
        rendLPB.material.SetColor("_Color", Color.green);



    }

    // Update is called once per frame
    void Update()
    {
        if (CubeHealthLPB <= 80 && extraPartsB[0] != null)
        {
            rendLPB.enabled = false;
            extraPartsB[0].SetActive(true);
            currentRenderB = extraPartsB[0].GetComponent<Renderer>();
            currentRenderB.enabled = true;


            currentRenderB.sharedMaterial.SetColor("_Color", Color.yellow);
        }
        if (CubeHealthLPB <= 60 && extraPartsB[0] != null)
        {
            //    Parucl[0].gameObject.SetActive(true);
            currentRenderB.sharedMaterial.SetColor("_Color", Color.yellow);
        }
        if (CubeHealthLPB <= 25 && extraPartsB[0] != null)
        {
            //Parucl[0].gameObject.SetActive(false);
            //Parucl[1].gameObject.SetActive(true);
            currentRenderB.sharedMaterial.SetColor("_Color", Color.red);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        float power = collision.relativeVelocity.magnitude;
        //rgb.freezeRotation = true;
        //rgb.constraints = RigidbodyConstraints.FreezeRotationX;
        if (power > 10f)
        {
            rgb.detectCollisions = false;


            CubeHealthLPB -= 15f;
            DamageManager.instance.DamagePoints -= 5
                ;

            StartCoroutine("TurnOn");
        }
        if (CubeHealthLPB <= 15)
        {
            currentRenderB.enabled = false;

        }

    }
    private void OnCollisionStay(Collision collision)
    {
        if (Player.GetComponent<Rigidbody>().velocity.magnitude > 2f && collision.gameObject.tag != "Ground")
            //rgb.constraints = RigidbodyConstraints.FreezeRotationX & RigidbodyConstraints.FreezeRotationZ;
            CubeHealthLPB -= Time.deltaTime * 1;
    }
}
