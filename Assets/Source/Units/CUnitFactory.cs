using UnityEngine;
using System.Collections;

public class CUnitFactory : MonoBehaviour
{

    public GameObject[] m_UnitModel;


    public GameObject NewUnitInstantiate(int unitID, Vector3 pos, Quaternion rot)
    {
        if (unitID < 0)
            return null;

        int id = 0;

        foreach (GameObject o in m_UnitModel)
        {
            int.TryParse(o.name, out id);
            if (id == unitID)
            {
                return (GameObject)GameObject.Instantiate(o, pos, rot);
            }
        }

        return null;
    }

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
}