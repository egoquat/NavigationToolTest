using UnityEngine;
using System.Collections;


public class CoreBase : MonoBehaviour
{
    public int id
    {
        get { return m_CoreID; }
    }

    public float hp
    {
        get { return m_CoreHp; }
    }

    public float MaxHP
    {
        get { return m_MaxCoreHp; }
    }

    public void InitCore(int coreType, float coreHP, int coreID)
    {
        m_CoreType = coreType;
        m_MaxCoreHp = m_CoreHp = coreHP;
        //m_CoreID = coreID;
        m_CoreID = (coreType << 24) | (0x00ffffff & coreID);
    }

    #region MonoBehaviour
    void Awake()
    {

    }

    // Use this for initialization
    void Start()
    {

    }

    void OnTriggerEnter(Collider other)
    {
        UnitRange range = other.GetComponent<UnitRange>();
        if (range != null)
        {
            if (m_CoreType == 1)    //  maincore는 모든유닛들이 공격
            {
                UnitBase unit = range.transform.root.GetComponentInChildren<UnitBase>();
                unit.AddDestroyTarget(m_CoreID);
            }
            else                    // subcore는 지상유닛만 공격함
            {
                UnitWalking unit = range.transform.root.GetComponentInChildren<UnitWalking>();
                if (unit != null)
                    unit.AddDestroyTarget(m_CoreID);
            }

        }
    }


    // Update is called once per frame
    protected virtual void Update()
    {
        if (m_CoreHp <= 0)
        {
            //if (!animation.isPlaying)
            //    Destroy(gameObject);
        }
    }

    void OnGUI()
    {
        if (!GameContext.GetInstance.ShowDebugInfo)
        {
            return;
        }

        Vector3 sp = Camera.main.WorldToScreenPoint(transform.position);
        Rect rc = new Rect(sp.x - 10, Screen.height - sp.y - 20, 100, 20);
        GUI.Label(rc, m_CoreHp + "/" + m_MaxCoreHp);
    }
    #endregion


    protected int m_CoreType;       //  1: main, 2: sub
    protected int m_CoreID;
    protected float m_CoreHp;
    protected float m_MaxCoreHp;

    protected object _lock = new object();

}