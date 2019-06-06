using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackBumperDamage : DamageSystem
{
    float CubeHealthBD = 100f;
    Renderer rendB;
    MeshFilter thisMeshB;
    //Set the main Color of the Material to green
     Renderer currentRender;
    public List<GameObject> DamagePartsB;
    public List<GameObject> extraParts = new List<GameObject>();
    private void Start()
    {
        rgb = GetComponent<Rigidbody>();
        thisMeshB = GetComponent<MeshFilter>();
        rendB = normalParts[0].GetComponent<Renderer>();

        currentRender = rendB;
        rendB.material.SetColor("_Color", Color.green);
        ExtraPart();


    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.LeftAlt))
        {
            Restore();
        }

        if (CubeHealthBD <= 80 && extraParts[0] != null)
        {
            rendB.enabled = false;
            extraParts[0].SetActive(true);
            currentRender = extraParts[0].GetComponent<Renderer>();
            currentRender.enabled = true;


            currentRender.sharedMaterial.SetColor("_Color", Color.yellow);
        }
        if (CubeHealthBD <= 60 && extraParts[0] != null)
        {
            //    Parucl[0].gameObject.SetActive(true);
            currentRender.sharedMaterial.SetColor("_Color", Color.yellow);
        }
        if (CubeHealthBD <= 25 && extraParts[0] != null)
        {
            //Parucl[0].gameObject.SetActive(false);
            //Parucl[1].gameObject.SetActive(true);
            currentRender.sharedMaterial.SetColor("_Color", Color.red);
        }



    }

 
    private void OnCollisionEnter(Collision collision)
    {
        
        float power = collision.relativeVelocity.magnitude;
        //rgb.freezeRotation = true;
        //rgb.constraints = RigidbodyConstraints.FreezeRotationX;
        if (power > 10f&& CubeHealthBD > 10)
        {
            rgb.detectCollisions = false;

            CubeHealthBD -= 45f;
            DamageManager.instance.DamagePoints -= 10;

            StartCoroutine("TurnOn");
        }
        if (CubeHealthBD <= 15)
        {

            DamageManager.instance.DamagePoints -= 5;
            if (extraParts[0]!=null)
            { DetachPart(); }
        }

    }
    IEnumerator TurnOn()
    {
        yield return new WaitForSeconds(1);
        rgb.detectCollisions = true;
    }

    private void Restore()
    {

        extraParts.Clear();
        ExtraPart();
        extraParts[0].GetComponent<Renderer>().enabled = true;
        rendB = normalParts[0].GetComponent<Renderer>();
        rendB.sharedMaterial.SetColor("_Color", Color.green);

        //damageParts[0] = extraParts[0];
        //damageParts[0].enabled = false;
        CubeHealthBD = 100f;
    }
    private void OnCollisionStay(Collision collision)
    {
        if (Player.GetComponent<Rigidbody>().velocity.magnitude > 2f && collision.gameObject.tag != "Ground")
            //rgb.constraints = RigidbodyConstraints.FreezeRotationX & RigidbodyConstraints.FreezeRotationZ;
            CubeHealthBD -= Time.deltaTime * 1;
    }
    private void OnCollisionExit(Collision collision)
    {
        //rgb.WakeUp();
        //rgb.freezeRotation = false;
        //rgb.constraints = RigidbodyConstraints.None;


    }

    internal override void ExtraPart()
    {
        extraParts.Add(Instantiate(DamagePartsB[0], new Vector3(transform.position.x, transform.position.y, transform.position.z),
            Quaternion.Euler(gameObject.transform.rotation.x, gameObject.transform.rotation.y - 90, gameObject.transform.rotation.z)));
        extraParts[0].transform.SetParent(Player.transform);
        extraParts[0].transform.localRotation = Quaternion.identity;
        extraParts[0].transform.Rotate(0, -90, 0);
        extraParts[0].SetActive(false);
    }

    //void ExtraP()
    //{
    //    extraParts.Add(Instantiate(damageParts[0], new Vector3(transform.position.x, transform.position.y, transform.position.z),
    //        Quaternion.Euler(0, 90, 0)));
    //    extraParts[0].transform.SetParent(Player.transform);

    //    extraParts[0].SetActive(false);
    //}
    internal override void DetachPart()
    {



        //damageParts[0].SetActive(false);
        extraParts[0].gameObject.AddComponent<BoxCollider>();
        //damageParts[0].gameObject.AddComponent<Rigidbody>();
        StartCoroutine("RgbB");
        currentRender = null;
        Destroy(extraParts[0].gameObject, 3f);

    }
    public IEnumerator RgbB()
    {
        yield return new WaitForSeconds(0.1f);
        extraParts[0].gameObject.AddComponent<Rigidbody>();

    }
}
