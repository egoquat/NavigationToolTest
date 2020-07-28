using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UnitWalking : UnitBase
{
    //@ Process Movement 
    #region movement_unitwalking

    //@ STATUS OBJECT WALKING
    enum e_navistate
    {
        navistate_null = -1,
        navistate_stop,
        navistate_celltocell,
        navistate_celltosight,
        navistate_end,
    };

    //@ Ratio on vertical road.
    class CRatioHorizontal
    {
        //Constant value
        #region constparameter
        float velocityratio = 0.2f;                         //@비율로 이동시 초당 속도 
        float epsilon_targetweight = CMATH.FEPSILON_F2;     //@편차 허용 범위 0.01f
        #endregion // constparameter

        enum e_decision_ratio
        {
            decision_out_minus = -1,
            decision_in = 0,
            decision_out_plus = 1,
        }

        public float _ratioPerpenRecord = 0.0f;
        public float _distEdgePerspect = 0.0f;


        public float _distRecord = 0.0f;

        public Vector3 _posEdgePerspect0 = new Vector3();
        public Vector3 _posEdgePerspect1 = new Vector3();

        public Vector3 _dirForward = new Vector3();
        public Vector3 _dirRight = new Vector3();
        public Vector3 _dirLeft = new Vector3();

        e_decision_ratio checkterritoryratio(float fratioRequest)
        {
            if ((1.0f - epsilon_targetweight) < fratioRequest)
            {
                return e_decision_ratio.decision_out_plus;
            }

            if ((0.0f + epsilon_targetweight) > fratioRequest)
            {
                return e_decision_ratio.decision_out_minus;
            }

            return e_decision_ratio.decision_in;
        }

        float calRatioProjection(Vector3 posPnt)
        {
            Vector3 posProjection = Vector3.Project((posPnt - _posEdgePerspect0), _dirRight);
            return (posProjection.magnitude / _distEdgePerspect);
        }

        public void renewRatioPerpend(Vector3 dirForwardUnit,
                                        Vector3 posEdge0,
                                        Vector3 posEdge1,
                                        float fRatioPerpen)
        {
            _dirRight = Vector3.Cross(Vector3.up, dirForwardUnit).normalized;
            _dirLeft = -_dirRight;
            _dirForward = dirForwardUnit;

            _posEdgePerspect0 = posEdge0;
            _posEdgePerspect1 = posEdge0 + Vector3.Project((posEdge1 - posEdge0), _dirRight);

            _distEdgePerspect = (_posEdgePerspect1 - _posEdgePerspect0).magnitude;

            _ratioPerpenRecord = fRatioPerpen;
            _distRecord = _distEdgePerspect * fRatioPerpen;
        }

        public Vector3 processPosPerpend_collision(Vector3 posCurrent,
                                                Vector3 vectorColli)
        {
            if (CMATH.FEPSILON_F2 >= _distEdgePerspect)
            {
                return posCurrent;
            }

            if (e_decision_ratio.decision_in != checkterritoryratio(_ratioPerpenRecord))
            {
                return posCurrent;
            }

            bool bRightColli = true;
            Vector3 dirColli = _dirRight;


            float projection = Vector3.Dot(vectorColli, _dirRight);
            if (0.0f == projection)
            {
                return posCurrent;
            }

            if (0.0f > projection)
            {
                bRightColli = false;
                dirColli = _dirLeft;
            }

            Vector3 vectorColliProjected = Vector3.Project(vectorColli, dirColli);
            float ratioPerpenEst = _ratioPerpenRecord, distPerpenEst;
            float distColli = vectorColliProjected.magnitude;
            float ratioColli = distColli / _distEdgePerspect;

            if (true == bRightColli)
            {
                ratioPerpenEst = ratioPerpenEst + ratioColli;
                distPerpenEst = _distRecord + distColli;
            }
            else
            {
                ratioPerpenEst = ratioPerpenEst - ratioColli;
                distPerpenEst = _distRecord - distColli;
            }

            if (checkterritoryratio(ratioPerpenEst) != e_decision_ratio.decision_in)
            {
                return posCurrent;
            }

            _ratioPerpenRecord = ratioPerpenEst;
            _distRecord = distPerpenEst;

            return posCurrent + vectorColliProjected;
        }

        public void processPosPerpend(float fTimeDelta,
                                    float fratioTarget,
                                    Vector3 posCurrent,
                                    ref Vector3 posRatioAdjusted)
        {
            if (CMATH.FEPSILON_F2 >= _distEdgePerspect)
            {
                posRatioAdjusted = posCurrent;
                return;
            }

            Vector3 dirPerpen = _dirRight;
            bool bRight = true;
            if (_ratioPerpenRecord > fratioTarget)
            {
                dirPerpen = _dirLeft;
                bRight = false;
            }

            float fRatioOffset = Mathf.Abs(fratioTarget - _ratioPerpenRecord);

            if (fRatioOffset < epsilon_targetweight)
            {
                posRatioAdjusted = posCurrent;
            }
            else
            {
                float fRatioAdjust = velocityratio * fTimeDelta;
                if (fRatioAdjust > fRatioOffset)
                {
                    fRatioAdjust = fRatioOffset;
                }

                float fDistCurrent = _distEdgePerspect * fRatioAdjust;
                float ratioPerpenRecordEst = _ratioPerpenRecord;
                float distRecordEst = _distRecord;

                if (true == bRight)
                {
                    ratioPerpenRecordEst += fRatioAdjust;
                    distRecordEst += fDistCurrent;
                }
                else
                {
                    ratioPerpenRecordEst -= fRatioAdjust;
                    distRecordEst -= fDistCurrent;
                }

                //@ check more far.
                e_decision_ratio decisionRatio = checkterritoryratio(ratioPerpenRecordEst);

                if ((e_decision_ratio.decision_out_minus == decisionRatio && ratioPerpenRecordEst < _ratioPerpenRecord) ||
                    (e_decision_ratio.decision_out_plus == decisionRatio && ratioPerpenRecordEst > _ratioPerpenRecord))
                {
                    posRatioAdjusted = posCurrent;
                    return;
                }

                _ratioPerpenRecord = ratioPerpenRecordEst;
                _distRecord = distRecordEst;

                dirPerpen = dirPerpen * fDistCurrent;
                posRatioAdjusted = posCurrent + dirPerpen;
            }
        }
    }; // class CRatioHorizontal

    #region constparameter
    const float dist_adjustpos_up = 0.1f;
    const float dist_raycast_adjust = 10.0f;
    const float spdcollision_unit = 1.0f;
    const float dflt_ratio_horizononroad = 0.5f;         //@ 가는 경로상에 가로 위치점 비율
    public const float default_speedmove_unitWalking = 1.0f;
    const float default_velocityAngle_unitWalking_deg = 10.0f;         //@각속도-radian
    #endregion // #region constparameter

    #region staticparam_unitwalking
    public static bool _bProcessWalking_Static = true;
    public static bool _bMoveShortestSight = false;

    //@ STATE
    static e_navistate _eStateNaviUnitWalking = e_navistate.navistate_null;
    #endregion // staticparam_unitwalking

    //@ ex-paramenters
    public bool m_bVerticalInclination = true;

    //@ Navigation Property 
    bool m_bReachtheGoal = false;
    bool m_bLanding = false;
    int m_iTriLandingCurrent = -1;

    //@ Boundary
    Vector2 m_v2Extents = new Vector2();
    Vector3 m_v3Extents = new Vector3();
    float m_fDist_aabb = -1.0f;
    float m_fRadius_aabb = -1.0f;

    //@ Movement
    float m_speedCollision = 0.8f;
    float m_fratiohorizonRoad;                                 //@ random offset-ratio on way to go for natural movement.

    bool _bRequestLandDirectly = false;

    //@ All collision objects.
    public SortedDictionary<int, float> m_listCollides
                                            = new SortedDictionary<int, float>();

    //@ Movement : Target to Target
    CNavigation.CCellWayto m_pntWayTo = new CNavigation.CCellWayto();
    CRatioHorizontal m_wayHorizon = new CRatioHorizontal();

    //@get/set
    public bool IsMoveStateCellToCell()
    {
        return (_eStateNaviUnitWalking == e_navistate.navistate_celltocell);
    }

    public bool IsMoveStateCellToSight()
    {
        return (_eStateNaviUnitWalking == e_navistate.navistate_celltosight);
    }

    public bool IsMoveStateStop()
    {
        return (_eStateNaviUnitWalking == e_navistate.navistate_stop);
    }

    CNaviMesh getNavimeshcurr()
    {
        CNaviMesh navimeshcurrent;
        navimeshcurrent = processCycle.GetInstance.m_meshNavigation_global;

        return navimeshcurrent;
    }

    public void requestSetLandingDirectly()
    {
        _bRequestLandDirectly = true;
    }

    //@ reach the Goal?
    protected bool reachtheGoal
    {
        get { return m_bReachtheGoal; }
        set { m_bReachtheGoal = value; }
    }

    public void setLandingTri(int iTriLanding) { m_iTriLandingCurrent = iTriLanding; }
    public int getLandingTri() { return m_iTriLandingCurrent; }

    bool isLanding() { return m_bLanding; }


    Vector2[] boundaryUnit2D()
    {
        Vector2[] arrv2AABB = new Vector2[4];
        Vector3 aabbmax = gameObject.GetComponent<Collider>().bounds.max;
        Vector3 aabbmin = gameObject.GetComponent<Collider>().bounds.min;

        arrv2AABB[0].x = aabbmax.x;
        arrv2AABB[0].y = aabbmax.z;

        arrv2AABB[1].x = aabbmax.x;
        arrv2AABB[1].y = aabbmin.z;

        arrv2AABB[2].x = aabbmin.x;
        arrv2AABB[2].y = aabbmax.z;

        arrv2AABB[3].x = aabbmin.x;
        arrv2AABB[3].y = aabbmin.z;

        return arrv2AABB;
    }

    static public string getStringStatus()
    {
        return _eStateNaviUnitWalking.ToString();
    }

    public static void SetVisibilityMove(bool bMoveObjVisibility)
    {
        _bMoveShortestSight = bMoveObjVisibility;
    }

    //@ Switching STATUS OBJECT  navistate_stop to OBJ_STATE_NAVI.
    public static bool SetStatus_UnitWalking_switching()
    {
        e_navistate eStatusObjCur = (e_navistate)((((int)_eStateNaviUnitWalking) + 1)
                    % ((int)e_navistate.navistate_end));

        if (e_navistate.navistate_celltocell == eStatusObjCur)
        {
            eStatusObjCur = (e_navistate)(((int)_eStateNaviUnitWalking + 1) % (int)(e_navistate.navistate_end));
        }

        _eStateNaviUnitWalking = eStatusObjCur;

        return true;
    }

    public static void SetStatusCellToCell()
    {
        SetStatus_UnitWalking(UnitWalking.e_navistate.navistate_celltocell);
    }

    public static void SetStatusCellToSight()
    {
        SetStatus_UnitWalking(UnitWalking.e_navistate.navistate_celltosight);
    }

    public static void SetStatusStop()
    {
        SetStatus_UnitWalking(UnitWalking.e_navistate.navistate_stop);
    }

    //@ All walking unit states are 100% synchronized.
    static void SetStatus_UnitWalking(e_navistate eStateObj)
    {
        _eStateNaviUnitWalking = eStateObj;

        switch (eStateObj)
        {
            case e_navistate.navistate_celltocell:
                {
                    SetVisibilityMove(false);
                    _bProcessWalking_Static = true;
                }
                break;
            case e_navistate.navistate_celltosight:
                {
                    SetVisibilityMove(true);
                    UnitWalking._bProcessWalking_Static = true;
                }
                break;
            case e_navistate.navistate_stop:
                {
                    UnitWalking._bProcessWalking_Static = true;
                }
                break;
            default:
                {

                }
                break;
        }
    }

    public float ratioHorizononRoad
    {
        get { return m_fratiohorizonRoad; }
        set { m_fratiohorizonRoad = value; }
    }

    #region collisionupdateprocess

    bool intersectNavimesh(CNaviMesh navimesh, Vector3 v3PosUnit, ref Vector3 v3PosIntersected)
    {
        Vector3 v3PosSimulate = v3PosUnit;
        v3PosSimulate.y = v3PosSimulate.y + dist_adjustpos_up;

        Ray rayTestsimulate = new Ray(v3PosSimulate, Vector3.down);
        RaycastHit casthitted = new RaycastHit();

        bool bIntersected = navimesh.GetComponent<Collider>().Raycast(rayTestsimulate, out casthitted, dist_raycast_adjust);
        if (true == bIntersected)
        {
            v3PosIntersected = casthitted.point;
        }
        return bIntersected;
    }

    void calculatePosCollision(Vector3 v3DirForceCollide, float fTimeDelta, Vector3 v3PosObject, CNaviMesh meshNavi)
    {
        float speedC_Curr = m_speedCollision * fTimeDelta;
        if (speedC_Curr > m_fRadius_aabb)
        {
            speedC_Curr = m_fRadius_aabb;
        }

        Vector3 dirForceCollide = v3DirForceCollide * (speedC_Curr);
        Vector3 v3PosSimulate = v3PosObject + dirForceCollide;
        Vector3 v3PosUnitIntersected = new Vector3();
        bool naviintersected = intersectNavimesh(meshNavi, v3PosSimulate, ref v3PosUnitIntersected);

        if (true == naviintersected)
        {
            if (false == reachtheGoal)
            {
                positionUnit = m_wayHorizon.processPosPerpend_collision(v3PosObject, dirForceCollide);
            }
            else
            {
                //WORKING
                positionUnit = v3PosUnitIntersected;
            }
        }

    } // void calculatePosCollision(Vector3 v3DirForceCollide, float fTimeDelta, Vector3 v3PosObject, CNaviMesh meshNavi)

    bool Update_Collision_sameforce(float fTimeDelta, CNaviMesh meshNavi, ref SortedDictionary<int, float> listCollider)
    {
        //@1.Forced setting, Cell wasn't prepared go to way to goal.
        if (false == _bProcessWalking_Static)
        {
            return false;
        }
        //@2.No landing mesh.
        if (null == meshNavi)
        {
            return false;
        }

        UnitPool unitpool = UnitPool.GetInstance;

        //@ 1 more colliders 
        if (listCollider.Count > 0)
        {
            //@ If unit is null, Delete unit from collideList.
            List<UnitWalking> listCollideCollect = new List<UnitWalking>();
            ICollection<int> collectionID = listCollider.Keys;
            List<int> listcollideremove = new List<int>();

            foreach (int idunit in collectionID)
            {
                UnitWalking unitWalking = unitpool[idunit] as UnitWalking;
                if (null == unitWalking)
                {
                    listcollideremove.Add(idunit);
                }
                else
                {
                    if (true == unitWalking.IsUnitDie())
                    {
                        listcollideremove.Add(idunit);
                    }
                    else
                    {
                        float fdistInterEstimate = (unitWalking.m_fRadius_aabb * unitWalking.m_fRadius_aabb + m_fRadius_aabb * m_fRadius_aabb);
                        float fdistInterCurr = (positionUnit - unitWalking.positionUnit).sqrMagnitude;

                        if (fdistInterEstimate > fdistInterCurr)
                        {
                            listCollideCollect.Add(unitWalking);
                        }
                    }
                }
            } // foreach (int idunit in collectionID)

            //@ for safety elimination
            foreach (int idunitremove in listcollideremove)
            {
                listCollider.Remove(idunitremove);
            }

            if (listCollideCollect.Count == 0)
            {
                return false;
            }

            Vector3 v3DirForceCollide = Vector3.zero;
            Vector3 v3PosCurrent = positionUnit, v3PosOther, v3DistanceOther;

            int iCntActualCollide = 0;
            foreach (UnitWalking colliderUnitWalking in listCollideCollect)
            {
                v3PosOther = colliderUnitWalking.positionUnit;
                v3DistanceOther = (positionUnit - v3PosOther).normalized;

                if (colliderUnitWalking.IsUnitAttack())
                {
                    v3DistanceOther = v3DistanceOther.normalized * spdcollision_unit;
                }

                v3DirForceCollide = v3DirForceCollide + v3DistanceOther;
                iCntActualCollide++;
            }

            v3DirForceCollide = v3DirForceCollide.normalized;

            if (Vector3.zero != v3DirForceCollide)
            {
                calculatePosCollision(v3DirForceCollide, fTimeDelta, v3PosCurrent, meshNavi);
            }
        } // if (listCollider.Count > 0)

        return true;
    } // void Update_Collision_sameforce()

    #endregion // collisionupdateprocess

    //@ Update movement process
    bool Update_unitWalking_Movement(float fTimeDelta, CNaviMesh meshNavi)
    {
        if (true == _bRequestLandDirectly)
        {
            SetPositionLandDirectly(meshNavi);
            _bRequestLandDirectly = false;
        }

        //@1.Forced setting, Cell wasn't prepared go to way to goal.
        if (false == _bProcessWalking_Static)
        {
            return true;
        }

        //@2.No landing mesh.
        if (null == meshNavi)
        {
            Debug.Log("(ERROR. null == meshNavi//Update_objDyn//");

            return false;
        }

        //@3.Current landing triangle is NULL. (== No landing mesh)
        if (CTRI.NULL_TRI == getLandingTri())
        {
            //Vector3 v3PosObj = positionUnit;
            //Debug.Log("(ERROR. NULL_TRI == getLandingTri()//Position(x="
            //    + v3PosObj.x + ")(y="
            //    + v3PosObj.y + ")(z="
            //    + v3PosObj.z + ")//Update_objDyn//");

            return false;
        }

        //@4.Reach GOAL
        if (true == reachtheGoal)
        {
            return true;
        }

        Vector3 v3PosCurrent = positionUnit;
        Vector3 v3PosCurrAdjusted = v3PosCurrent;
        Vector3 v3PosNextAdjusted = v3PosCurrent + (m_pntWayTo._dirEntrytoNext * fTimeDelta * speedmove);

        bool bCrossoverAdjustedWayto = false;

        //@ crossover == true than adjustment positioning.
        {
            int iCountofLevel = m_pntWayTo._ileveltoGoal;
            Vector3 v3PosIntersected = new Vector3();
            CNavigation.CCellWayto pntWayTo_ = new CNavigation.CCellWayto(m_pntWayTo);
            CNAVICELL.E_CELL_STATUS ecellStatus = CNAVICELL.E_CELL_STATUS.CELL_STATUS_NULL;

            //@ penetrate n-cell.
            for (int iterlevel = 0; iterlevel <= iCountofLevel; ++iterlevel)
            {
                if (true == pntWayTo_.crossoverTest(v3PosCurrAdjusted, v3PosNextAdjusted))
                {
                    setLandingTri(pntWayTo_._idxnavicell);
                    if (true == pntWayTo_.IntersectedPosToEdge(v3PosCurrAdjusted, v3PosNextAdjusted, ref v3PosIntersected))
                    {
                        ecellStatus = meshNavi.navigation.getPosNextWayTo(pntWayTo_._idxnavicell, ref pntWayTo_);

                        if (CNAVICELL.E_CELL_STATUS.CELL_STATUS_GOAL == ecellStatus)
                        {
                            v3PosNextAdjusted = v3PosIntersected;
                        }
                        float fRemainder = (v3PosNextAdjusted - v3PosIntersected).magnitude;

                        v3PosNextAdjusted = v3PosIntersected + (pntWayTo_._dirEntrytoNext * fRemainder);

                        v3PosCurrAdjusted = v3PosIntersected;
                        bCrossoverAdjustedWayto = true;
                    }
                    else
                    {
                        ecellStatus = meshNavi.navigation.getPosNextWayTo(pntWayTo_._idxnavicell, ref pntWayTo_);
                    }
                }
                else
                {
                    break;
                }
            } // for (int iterlevel = 0; iterlevel <= iCountofLevel; ++iterlevel)

            if (true == bCrossoverAdjustedWayto)
            {
                m_wayHorizon.renewRatioPerpend(pntWayTo_._dirEntrytoNext,
                                   pntWayTo_._posEdgeFrom0,
                                   pntWayTo_._posEdgeFrom1,
                                   pntWayTo_._ratioPerpen);

                m_pntWayTo = pntWayTo_;
            }

            switch (ecellStatus)
            {
                case CNAVICELL.E_CELL_STATUS.CELL_STATUS_GOAL:
                    {
                        reachtheGoal = true;
                        bCrossoverAdjustedWayto = true;
                    }
                    break;
                case CNAVICELL.E_CELL_STATUS.CELL_STATUS_BLOCK:
                case CNAVICELL.E_CELL_STATUS.CELL_STATUS_ROAD:
                    {
                        bCrossoverAdjustedWayto = true;
                    }
                    break;
            }
        }

        //@ 목표각도 설정.
        if (true == bCrossoverAdjustedWayto)
        {
            Vector3 v3DirAngleTarget = m_pntWayTo._dirEntrytoNext;

            if (true == m_bVerticalInclination)
            {
                forwardforAngleTarget = new Vector3(v3DirAngleTarget.x, 0.0f, v3DirAngleTarget.z);
            }
            else
            {
                forwardforAngleTarget = v3DirAngleTarget;
            }
            forwardforMovement = m_pntWayTo._dirEntrytoNext;

            Vector3 v3PosRatioAdjusted = new Vector3();
            m_wayHorizon.processPosPerpend(fTimeDelta,
                                            m_fratiohorizonRoad,
                                            v3PosNextAdjusted,
                                            ref v3PosRatioAdjusted);


            positionUnit = v3PosRatioAdjusted;
        }
        else
        { //@ position adjusted position, calculaed ratio on way perpendicular
            Vector3 v3PosRatioAdjusted = new Vector3();
            m_wayHorizon.processPosPerpend(fTimeDelta,
                                            m_fratiohorizonRoad,
                                            v3PosCurrent,
                                            ref v3PosRatioAdjusted);

            positionUnit = v3PosRatioAdjusted;
            base.Update_unitDyn(fTimeDelta);
        }

        #region inclination_teritory
        //bool bWayInclination = false;
        //if(false == CMATH.similarEst_float(m_pntWayTo._dirEntrytoNext.y, 0.0f, CMATH.FEPSILON_F2))
        //{
        //    bWayInclination = true;
        //}
        #endregion



        return true;
    } // public void Update_objWalk(float fTimeDelta)


    //@ collect collide objects to sorteddictionary
    protected override void OnTriggerEnter(Collider colliderOther)
    {
        UnitWalking unitwalkingColliderOther = colliderOther.gameObject.GetComponentInChildren<UnitWalking>();
        if (null == unitwalkingColliderOther)
        {
            return;
        }

        lock (_lock)
        {
            m_listCollides.Add(unitwalkingColliderOther.id, Time.time);
        }
    }

    protected override void OnTriggerExit(Collider colliderOther)
    {
        base.OnTriggerExit(colliderOther);
        UnitWalking unitwalkingColliderOther = colliderOther.gameObject.GetComponentInChildren<UnitWalking>();
        if (null == unitwalkingColliderOther)
        {
            return;
        }

        lock (_lock)
        {
            m_listCollides.Remove(unitwalkingColliderOther.id);
        }
    }

    //@ Intersect ray
    public bool intersectRay_obj(Ray rayRequest)
    {
        return transform.GetComponent<Collider>().bounds.IntersectRay(rayRequest);
    }

    //@ Process // Initialize
    public override void InitUnitMovement()
    {
        //@ Initialize Geometry
        if (true == processCycle.GetInstance._modeTool)
        {
            speedmove = processCycle.GetInstance.m_speedmove_unitwalking;

            offsetratioMove = UnityEngine.Random.Range(0.0f, UnitBase.dflt_offsetratio_movement);
            ratioHorizononRoad = dflt_ratio_horizononroad + (offsetratioMove - (UnitBase.dflt_offsetratio_movement * 0.5f));
        }
        else
        {
            speedmove = m_UnitTemplate.m_MoveSpeed;

            offsetratioMove = UnityEngine.Random.Range(0.0f, UnitBase.dflt_offsetratio_movement);
            ratioHorizononRoad = dflt_ratio_horizononroad + (offsetratioMove - (UnitBase.dflt_offsetratio_movement * 0.5f));
        }

        BoxCollider boxcollider = gameObject.GetComponent<BoxCollider>();
        if (boxcollider)
        {
            m_v3Extents.x = boxcollider.extents.x * transform.localScale.x;
            m_v3Extents.y = boxcollider.extents.y * transform.localScale.y;
            m_v3Extents.z = boxcollider.extents.z * transform.localScale.z;

            m_v2Extents.x = m_v3Extents.x;
            m_v2Extents.y = m_v3Extents.z;

            m_fDist_aabb = m_v2Extents.magnitude;
            m_fRadius_aabb = m_fDist_aabb * 0.8f;
        }
        else
        {
            SphereCollider spherecollider = gameObject.GetComponent<SphereCollider>();
            if (spherecollider)
            {
                m_v3Extents.x = (spherecollider.radius * 2) * transform.localScale.x;
                m_v3Extents.y = (spherecollider.radius * 2) * transform.localScale.y;
                m_v3Extents.z = (spherecollider.radius * 2) * transform.localScale.z;

                m_v2Extents.x = (spherecollider.radius * 2) * transform.localScale.x;
                m_v2Extents.y = (spherecollider.radius * 2) * transform.localScale.z;
                m_fDist_aabb = m_v3Extents.magnitude;
            }
        } //if(boxcollider)

        bool bLanding = SetPositionLandDirectly(getNavimeshcurr());
        if (false == bLanding)
        {
            Debug.Log("UnitWalking landing false!");
        }

        velocityAngleStandard = default_velocityAngle_unitWalking_deg;
    }
    //@ Process
    //@ 1.landing position 2.set target dir to object
    public bool SetPositionLandDirectly(CNaviMesh meshNavi)
    {
        Vector3 v3DirDown = Vector3.down;
        Vector3 v3PosCurrent;
        Vector3 v3PosVertical;
        bool bIntersectedWithLanding = false;
        float fAdjust = dist_adjustpos_up;

        v3PosCurrent = positionUnit;
        v3PosVertical = v3PosCurrent + (Vector3.up * fAdjust);

        Ray rayObj = new Ray(v3PosVertical, v3DirDown);

        int iTriLandingCurr = -1;
        RaycastHit raycastHit_info = new RaycastHit();
        bool bCrossoverAdjustedWayto = false;

        bIntersectedWithLanding = meshNavi.GetComponent<Collider>().Raycast(rayObj, out raycastHit_info, dist_raycast_adjust);
        if (false == bIntersectedWithLanding)
        {
            //Debug.Log("ERROR. SetPositionLandDirectly()//not Landing.");
            return false;
        }

        iTriLandingCurr = raycastHit_info.triangleIndex;

        setLandingTri(iTriLandingCurr);
        positionUnit = raycastHit_info.point;

        {
            Vector3 v3PosIntersected = raycastHit_info.point;
            Vector2[] arrv2PntOBJBoundary = boundaryUnit2D();

            if (true == _bMoveShortestSight)
            {
                bCrossoverAdjustedWayto = meshNavi.navigation.getPosWayTo_Portal(iTriLandingCurr, arrv2PntOBJBoundary, ref m_pntWayTo);
            }
            else
            {
                CNAVICELL.E_CELL_STATUS ecellStatus = meshNavi.navigation.getPosCurrentWayToGoal(iTriLandingCurr, ref m_pntWayTo);

                switch (ecellStatus)
                {
                    case CNAVICELL.E_CELL_STATUS.CELL_STATUS_GOAL:
                        {
                            bCrossoverAdjustedWayto = true;

                            reachtheGoal = true;
                        }
                        break;
                    case CNAVICELL.E_CELL_STATUS.CELL_STATUS_BLOCK:
                    case CNAVICELL.E_CELL_STATUS.CELL_STATUS_ROAD:
                        {
                            bCrossoverAdjustedWayto = true;
                        }
                        break;
                    case CNAVICELL.E_CELL_STATUS.CELL_STATUS_NULL:
                    default:
                        {
                            bCrossoverAdjustedWayto = false;
                        }
                        break;
                } // switch (ecellStatus)
            } // if (true == _bMoveShortestSight)

            if (false == bCrossoverAdjustedWayto || (int)CNAVICELL.E_CELL_STATUS.CELL_STATUS_NULL == m_pntWayTo._idxnavicell)
            {
                Debug.Log("Error(critical) set Unit landing.="
                    + (CNAVICELL.E_CELL_STATUS)m_pntWayTo._idxnavicell + "//" + UnityEngine.Random.Range(0.0f, 1000.0f));
                return false;
            }

            //@ Set dir standard
            forwardforMovement = m_pntWayTo._dirEntrytoNext;
            forwardforAngle = m_pntWayTo._dirEntrytoNext;
            positionUnit = v3PosIntersected;
        } // if (false == meshNavi.getCell(iTriLandingCurr).IsBlockCell())

        //@ To slicely adjustment position for rigidbody height.
        //if (rigidbody)
        //{
        //    Vector3 v3PositionAdjust = new Vector3();
        //    v3PositionAdjust = positionUnit;
        //    v3PositionAdjust.y = v3PositionAdjust.y + CMATH.FEPSILON_F3;

        //    positionUnit = v3PositionAdjust;
        //    setPosHeadObj(v3PositionAdjust, Vector3.zero);
        //}


        return bCrossoverAdjustedWayto;
    } // public bool SetPositionLandDirectly()

    #endregion // #region movement_unitwalking

    #region animationstatus
    protected override void DoAction(float deltaTime)
    {
        if (e_navistate.navistate_stop == _eStateNaviUnitWalking)
        {
            return;
        }

        CNaviMesh meshNavi = getNavimeshcurr();
        if (null == meshNavi)
        {
            //Debug.Log("ERROR! null == meshNavi //" + UnityEngine.Random.Range(0.0f, 1000.0f));
            return;
        }

        switch (m_UnitState)
        {
            case UNIT_STATE.SPAWN:
                break;
            case UNIT_STATE.MOVE:
                {
                    if (true == Update_unitWalking_Movement(deltaTime, meshNavi))
                    {
                        Update_Collision_sameforce(deltaTime, meshNavi, ref m_listCollides);
                    }
                }
                break;
            case UNIT_STATE.ATTACK:
                {
                    //Update_Collision_sameforce(fTimeDeltaProcess, ref m_listCollides);
                    m_AttackTime += deltaTime;
                    //Debug.DrawLine(transform.position, field.FindCoreMocel(m_TargetCoreID).transform.position, new Color(255, 0, 0));
                }
                break;
            case UNIT_STATE.DIE:

                break;
            default:
                break;
        }
    } // protected override void DoAction(float deltaTime)

    protected override void SetUnitAnimation()
    {
        switch (m_UnitState)
        {
            case UNIT_STATE.SPAWN:
                break;
            case UNIT_STATE.MOVE:
                {
                    if (!ani.isPlaying)
                    {
                        ani.Rewind("move");
                        ani.CrossFade("move");
                    }

                    break;
                }
            case UNIT_STATE.ATTACK:
                {
                    if (ani.IsPlaying("move"))
                    {
                        ani.Rewind("attack");
                        ani.CrossFade("attack");
                    }
                    else if (!ani.IsPlaying("attack"))
                    {
                        ani.Rewind("attack");
                        ani.CrossFade("attack");
                    }
                    break;
                }
            case UNIT_STATE.DIE:
                {
                    //if (!ani.IsPlaying("die"))
                    //{
                    //    if (ani.isPlaying)
                    //    {
                    //        ani.Rewind("die");
                    //        ani.CrossFade("die");
                    //    }
                    //    else
                    //    {
                    //        Destroy(transform.root.gameObject);
                    //    }
                    //}

                    break;
                }
            default:
                break;

        }
    } // override void SetUnitAnimation()
    #endregion // animationstatus

} // public class UnitWalking : UnitBase