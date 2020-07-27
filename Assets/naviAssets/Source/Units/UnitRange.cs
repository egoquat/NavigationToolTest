using UnityEngine;
using System.Collections;

public class UnitRange : MonoBehaviour
{
    void Awake()
    {

    }

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }


    public void AddCore(int coreID)
    {

    }

    public void RemoveCore(int coreID)
    {

    }

    public void SetAttackRange(float raidus)
    {
        if (raidus < 0)
            return;

        SphereCollider col = gameObject.GetComponent<SphereCollider>();
        col.radius = raidus;
    }
}