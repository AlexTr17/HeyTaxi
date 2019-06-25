using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SitInCar : MonoBehaviour
{
    private Animator animator;
    private bool incar;

    private void Start()
    {
        animator = GetComponent<Animator>();
        incar = false;
    }
    private void OnTriggerStay(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            Debug.Log("Sut");
            StartCoroutine("Sit");
            gameObject.transform.SetParent(other.transform);
        }
    }
    IEnumerator Sit()
    {
        animator.SetBool("SitInCar", true);
        yield return new WaitForSeconds(0.5f);
        animator.SetBool("SitInCar", false);
        incar = true;
        
    }
}
