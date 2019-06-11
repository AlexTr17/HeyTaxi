using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoliceDepartmentManager : MonoBehaviour
{
    [SerializeField] private List<GameObject> officers;
    public static PoliceDepartmentManager instance;
    EnemySight police;
    // Start is called before the first frame update
    private void Start()
    {
        if(instance==null)
        {
            instance = this;
        }
        officers = new List<GameObject>();
        for(int i =0;i<transform.childCount;i++)
        {
            officers.Add(transform.GetChild(i).gameObject);
        }
    }

    public void StartSearch()
    {
        for(int i=0;i<officers.Count;i++)
        {
            gameObject.GetComponent<EnemySight>().state = State.suspicion;
        }
    }
}
