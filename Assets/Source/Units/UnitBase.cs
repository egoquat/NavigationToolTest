using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class UnitBase : MonoBehaviour
{
    public int id
    {
        get
        {
            return _id;
        }
    }

    public void SetSecondState(SECOND_STATE state, float value, float time)
    {
        //lock (_lock) m_SecondState[(int)state].SetState(value, time);
    }

    public int GetUnitType()
    {
        return m_UnitTemplate.m_ModelCode / 1000;
    }

    public bool IsUnitDie()
    {
        if (m_UnitState == UNIT_STATE.DIE)
            return true;
        return false;
    }

    public bool IsUnitAttack()
    {
        if (m_UnitState == UNIT_STATE.ATTACK)
            return true;
        return false;
    }

    public void AddDestroyTarget(int coreID)
    {
        lock (_lock)
        {
            m_DestroyTargetList.Add(coreID);
        }
    }

    public void RemoveDestroyTarget(int coreID)
    {
        lock (_lock)
        {
            m_DestroyTargetList.Remove(coreID);
        }
    }

    public void SetAnimationSpeed(float speed)
    {
        lock (_lock)
        {
            foreach (AnimationState s in ani)
            {
                s.speed = speed;
            }
        }
    }

    public void InitUnitProperty(int unitID, float addShild, float addEnegy, int addAward, int seqspline)
    {
        SetUnitTempate(unitID);
        UnitRange range = transform.root.GetComponentInChildren<UnitRange>();
        range.SetAttackRange(m_UnitTemplate.m_Range);
        SetUnitAddProperty(addShild, addEnegy, addAward);
        seqSplineGuide = seqspline;

        InitUnitMovement();
    }

    public float GetUnitSize()
    {
        Bounds colliderBounds = GetComponent<Collider>().bounds;
        return colliderBounds.size.x * colliderBounds.size.z;
    }

    // -------------------------------------------------
    #region MonoBehaviour

    protected virtual void Awake()
    {
        _id = UnitPool.GetInstance.Register(this);
    }

    protected virtual void Start()
    {
        //if (animation != null)
        //    if (animation["spawn"])
        //        animation.Play("spawn");
        ////SetAnimationSpeed(GameContext.GetInstance.GameSpeed);
        m_UnitCenterPosTransform = transform.root.Find("UnitCenter");

        if (null != m_UnitTemplate)
        {
            m_AttackTime = m_UnitTemplate.m_AttackTerm;
        }
    }

    protected virtual void Update()
    {
        //float fTimeDeltaProcess = Time.deltaTime * GameContext.GetInstance.GameSpeed;
        float fTimeDeltaProcess = Time.deltaTime;
        SetUnitStateAndTarget();
        DoAction(fTimeDeltaProcess);
        SetUnitAnimation();
        UpdateSeconState(fTimeDeltaProcess);
    }

    protected virtual void OnDestroy()
    {
        UnitPool.GetInstance.Unregister(id);
    }

    protected virtual void OnTriggerEnter(Collider other)
    {

    }

    protected virtual void OnTriggerExit(Collider other)
    {

    }

    void OnGUI()
    {
        if (!GameContext.GetInstance.ShowDebugInfo)
        {
            return;
        }

        Vector3 sp = Camera.main.WorldToScreenPoint(transform.position);
        Rect hprc = new Rect(sp.x - 10, Screen.height - sp.y - 20, 100, 20);
        GUI.Label(hprc, m_UnitEnegy + "/" + m_MaxUnitEnegy);
        Rect shildrc = new Rect(sp.x - 10, Screen.height - sp.y - 30, 100, 20);
        GUI.Label(shildrc, m_UnitShild + "/" + m_MaxUnitShild);
    }
    #endregion
    // -------------------------------------------------

    // -------------------------------------------------
    #region Movement_Navigation

    protected int m_iSplineGuide = -1;

    //@ Dynamic Object
    protected static readonly float dflt_speedmove_unit = 1.0f;             //@ 
    protected static readonly float dflt_offsetratio_movement = 0.2f;       //@ 오프셋 - 가는 길위에 가로 위치점 비율
    protected static readonly float dflt_velocityAngle_deg = 10.0f;         //@ 초당 각속도 degree
    protected static readonly Vector3 null_forward = Vector3.zero;

    //@ Unit movement 
    // -------------------------------------------------

    //@ Unit movement - position move
    private float m_fSpeedMove;                                             //@ Velocity
    private float m_fSpeedCollisionMove;                                    //@ Velocity Reflect Collision 
    protected float m_foffsetratioMove;                                     //@ offset distance-rate on way movement
    private Vector3 m_forwardforMovement;                                   //@ forward for movement
    private Vector3 m_forwardAngle;                                         //@ forward for angle

    //@ Unit movement - angle
    private float m_fvelocityAngleStandard = dflt_velocityAngle_deg;
    private float m_fvelocityAngle = dflt_velocityAngle_deg;                  //@ Velocity angle movement per second // torque
    private Vector3 m_forwardAngleTarget = null_forward;                      //@ target angle to interpolation(lerp)

    //@Get/Set
    protected float velocityAngleStandard
    {
        get { return m_fvelocityAngleStandard; }
        set { m_fvelocityAngleStandard = value; }
    }

    protected float velocityangle
    {
        get { return m_fvelocityAngle; }
        set { m_fvelocityAngle = value; }
    }

    public float speedmove
    {
        get { return m_fSpeedMove; }
        set
        {
            m_fSpeedMove = value;
            m_fvelocityAngle = m_fvelocityAngleStandard * (m_fSpeedMove);
        }
    }

    public float offsetratioMove
    {
        get { return m_foffsetratioMove; }
        set { m_foffsetratioMove = value; }
    }

    public int seqSplineGuide
    {
        get { return m_iSplineGuide; }
        set { m_iSplineGuide = value; }
    }

    protected Vector3 positionUnit
    {
        get { return transform.root.position; }
        set { transform.root.position = value; }
    }

    protected Vector3 forwardforMovement
    {
        get { return m_forwardforMovement; }
        set { m_forwardforMovement = value; }
    }

    protected Vector3 forwardforAngle
    {
        get { return transform.root.forward; }
        set
        {
            if (Vector3.zero != value)
            {
                transform.root.forward = value;
            }
        }
    }

    protected Vector3 forwardforAngleTarget
    {
        get { return m_forwardAngleTarget; }
        set { m_forwardAngleTarget = value; }
    }

    protected Animation ani
    {
        get
        {
            if (_ani == null)
            {
                lock (_lock) _ani = transform.root.GetComponent<Animation>();
            }

            return _ani;
        }

    }
    Animation _ani;

    public Vector3 getCenterWeight()
    {
        return m_UnitCenterPosTransform.position;
    }

    public virtual void InitUnitMovement()
    {
    }

    protected void Update_angle_unitDyn(float fTimeDelta)
    {
        Vector3 forwardNext = forwardforAngle;
        Vector3 forwardCurrent = forwardforAngle;

        if (null_forward != forwardforAngleTarget)
        {
            Vector3 dirFrom = forwardCurrent;
            Vector3 dirTo = m_forwardAngleTarget;

            float fTimeslice = velocityangle * fTimeDelta;
            fTimeslice = Mathf.Clamp(fTimeslice, 0.0f, 1.0f);

            if (1.0f > fTimeslice)
            {
                Quaternion quatFrom = Quaternion.LookRotation(dirFrom);
                Quaternion quatTo = Quaternion.LookRotation(dirTo);
                Quaternion quatLerp = Quaternion.Lerp(quatFrom, quatTo, fTimeslice);

                forwardNext = forwardforAngle + (quatLerp * Vector3.forward);
            }
            else
            {
                forwardNext = m_forwardAngleTarget;
            }
        }

        forwardforAngle = forwardNext;
    }

    protected void Update_movement_unitDyn(float fTimeDelta)
    {
        Vector3 v3PosCurrent = positionUnit + (m_forwardforMovement * (m_fSpeedMove * fTimeDelta));
        positionUnit = v3PosCurrent;
    }

    protected virtual void Update_unitDyn(float fTimeDelta)
    {
        Update_movement_unitDyn(fTimeDelta);
        Update_angle_unitDyn(fTimeDelta);
    }
    #endregion // #region Movement_Navigation
    // -------------------------------------------------

    protected void SetUnitStateAndTarget()
    {
        switch (m_UnitState)
        {
            case UNIT_STATE.SPAWN:
                {
                    if (!ani.isPlaying)
                    {
                        lock (_lock) m_UnitState = UNIT_STATE.MOVE;
                    }
                    break;
                }
            case UNIT_STATE.DIE:
                {
                    if (!ani.isPlaying)
                        GameObject.Destroy(transform.root.gameObject);
                    break;
                }
            case UNIT_STATE.MOVE:
            case UNIT_STATE.ATTACK:
                {
                    AttackPreUpdate();
                    break;
                }

            default:
                break;
        }
    }

    protected virtual void AttackPreUpdate()
    {
    }

    protected virtual bool IsInvalidTarget(int coreID)
    {
        return false;
    }

    protected bool IsVisblityCore(CoreBase core)
    {
        int layerMask = 1 << LayerMask.NameToLayer("ArtMapModel");
        Vector3 unitPos = transform.position;
        Vector3 corePos = core.transform.position;
        unitPos.y = unitPos.y + (transform.GetComponent<Collider>().bounds.extents.y / 2);
        corePos.y = corePos.y + (core.GetComponent<Collider>().bounds.extents.y / 2);
        //Debug.DrawLine(unitPos, corePos);

        if (Physics.Linecast(unitPos, corePos, layerMask))
            return false;

        return true;
    }

    protected int SetTargetCore()
    {
        if (m_DestroyTargetList.Count == 0)
        {
            lock (_lock)
            {
                m_UnitState = UNIT_STATE.MOVE;
            }
            return 0;
        }

        foreach (int coreID in m_DestroyTargetList)
        {
            if (!IsInvalidTarget(coreID))
            {
                lock (_lock)
                {
                    m_UnitState = UNIT_STATE.ATTACK;
                }
                return coreID;
            }
        }

        lock (_lock)
        {
            m_UnitState = UNIT_STATE.MOVE;
        }
        return 0;
    }

    protected virtual void DoAction(float deltaTime)
    {
        switch (m_UnitState)
        {
            case UNIT_STATE.SPAWN:
                break;
            case UNIT_STATE.MOVE:
                break;
            case UNIT_STATE.ATTACK:
                break;
            case UNIT_STATE.DIE:
                break;
            default:
                break;
        }
    }

    protected virtual void SetUnitAnimation()
    {

    }

    protected void UpdateSeconState(float deltaTime)
    {
        //foreach (SecondState ss in m_SecondState)
        //{
        //    ss.Update(this, deltaTime);
        //}
    }


    protected void SetUnitTempate(int unitID)
    {
        if (m_UnitTemplate == null)
            m_UnitTemplate = new CUnitTemplate();
        m_UnitTemplate.m_Filename = "Enemy";
        m_UnitTemplate.SetUnit(unitID);

        transform.root.name = "u" + id + "(" + unitID + ")";
    }

    protected void SetUnitAddProperty(float addShild, float addEnegy, int addAward)
    {
        m_UnitShild = m_MaxUnitShild = m_UnitTemplate.m_Shild + addShild;
        m_UnitEnegy = m_MaxUnitEnegy = m_UnitTemplate.m_Enegy + addEnegy;
        m_AddUnitAward = addAward;
    }

    protected CUnitTemplate m_UnitTemplate = null;
    protected UNIT_STATE m_UnitState = UNIT_STATE.MOVE;
    protected List<int> m_DestroyTargetList = new List<int>();
    protected Transform m_UnitCenterPosTransform;

    protected float m_MaxUnitEnegy;
    protected float m_UnitEnegy = 0.0f;
    protected float m_MaxUnitShild;
    protected float m_UnitShild = 0.0f;
    protected int m_AddUnitAward;
    protected int m_TargetCoreID = 0;
    protected float m_AttackTime = 0.0f;

    protected object _lock = new object();

    volatile int _id = 0;

    protected enum UNIT_STATE
    {
        SPAWN = 0,
        MOVE = 1,
        ATTACK = 2,
        DIE = 3,
    }

    public enum SECOND_STATE
    {
        BURN = 0,
        DOWNSPEED = 1,
        INVALITYSHILD = 2,
        TOTAL = 3,
    }

}