using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LeftPartColllider : DamageSystem
{
    public float CubeHealthLP = 100f;
    Renderer rendLP;
    MeshFilter thisMeshLp;
    // Start is called before the first frame update
    private void Start()
    {
        rgb = GetComponent<Rigidbody>();
        thisMeshLp = GetComponent<MeshFilter>();
        rendLP = normalParts[0].GetComponent<Renderer>();

        currentRenderB = rendLP;
        rendLP.material.SetColor("_Color", Color.green);



    }

    // Update is called once per frame
    void Update()
    {
        if (CubeHealthLP <= 80 && extraPartsB[0] != null)
        {
            rendLP.enabled = false;
            extraPartsB[0].SetActive(true);
            currentRenderB = extraPartsB[0].GetComponent<Renderer>();
            currentRenderB.enabled = true;


            currentRenderB.sharedMaterial.SetColor("_Color", Color.yellow);
        }
        if (CubeHealthLP <= 60 && extraPartsB[0] != null)
        {
            //    Parucl[0].gameObject.SetActive(true);
            currentRenderB.sharedMaterial.SetColor("_Color", Color.yellow);
        }
        if (CubeHealthLP <= 25 && extraPartsB[0] != null)
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


            CubeHealthLP -= 15f;
            DamageManager.instance.DamagePoints -= 5
                ;

            StartCoroutine("TurnOn");
        }
        if (CubeHealthLP <= 15)
        {
            currentRenderB.enabled = false;

        }

    }
    private void OnCollisionStay(Collision collision)
    {
        if (Player.GetComponent<Rigidbody>().velocity.magnitude > 2f && collision.gameObject.tag != "Ground")
            //rgb.constraints = RigidbodyConstraints.FreezeRotationX & RigidbodyConstraints.FreezeRotationZ;
            CubeHealthLP -= Time.deltaTime * 1;
    }
}
