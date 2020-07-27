using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UnitFlying : UnitBase
{
    #region constparameter
    public const float default_speedmove_unitFlying = 3.0f;
    const int default_section_start = 0;
    const float default_velocityAngle_unitflying_rad = 15.0f;           //@각속도
    #endregion // constparameter

    #region staticparam_movement
    public static bool _MovementUnitFlying = true;
    #endregion // staticparam_movement

    #region MOVEMENT_UNITFLY
    //@ Process Property
    protected int m_iSection_spline = -1;
    protected bool m_bReachTheDestination = false;
    protected Vector3 m_v3PosOffset = new Vector3();

    //@ Guide line
    protected CSplineManufacturer.curvesection[] m_arrLineGuide = null;

    protected int sectionSP
    {
        get { return m_iSection_spline; }
        set { m_iSection_spline = value; }
    }

    public bool reachTheDestination
    {
        get { return m_bReachTheDestination; }
        set { m_bReachTheDestination = value; }
    }

    protected void setSplineGuide(int isplineguide, ref CSplineManufacturer.curvesection[] arrLineGuide)
    {
        CNaviMesh navimeshcurr = getNavimeshcurr();
        CSplineManufacturer splinecurveManufacturer = navimeshcurr.splinecurve;
        arrLineGuide = splinecurveManufacturer.getLineGuide(isplineguide);
    }

    public int getSectionSP() { return m_iSection_spline; }
    public void setSectionSP(int iSectionSP)
    { m_iSection_spline = iSectionSP; }

    protected CNaviMesh getNavimeshcurr()
    {
        CNaviMesh navimeshcurrent;
        navimeshcurrent = processCycle.GetInstance.m_meshNavigation_global;       

        return navimeshcurrent;
    }

    //@ Compute 
    protected Vector3 getDirBetween(Vector3 v3Pos_From, Vector3 v3Pos_To)
    {
        return Vector3.Normalize(v3Pos_To - v3Pos_From);
    }

    //@ Process
    public override void InitUnitMovement()
    {
        setSplineGuide(seqSplineGuide, ref m_arrLineGuide);
        if (null == m_arrLineGuide)
        {
            Debug.Log("Error(critical). UnitFlying got no spline guide.");

            return;
        }

        CSplineManufacturer.curvesection pos_pathFirst = m_arrLineGuide[default_section_start];

        //@ TM 설정
        positionUnit = pos_pathFirst._posSectionStart;
        forwardforAngle = forwardforMovement = pos_pathFirst._dirtoNextP;
        velocityAngleStandard = default_velocityAngle_unitflying_rad;

        if (true == processCycle.GetInstance._modeTool)
        {
            speedmove = processCycle.GetInstance.m_speedmove_unitflying;
        }
        else
        {
            speedmove = m_UnitTemplate.m_MoveSpeed;
        }

        setSectionSP(default_section_start);
    }

    protected Vector3 getLastDir(CSplineManufacturer.curvesection[] arrPosFlyPath__)
    {
        int iSeqLast = arrPosFlyPath__.Length - 1;

        return Vector3.Normalize(arrPosFlyPath__[iSeqLast]._posSectionDest - arrPosFlyPath__[iSeqLast - 1]._posSectionDest);
    }

    protected int getSectionNextAdjust(int iSectionCurrent,
                                        Vector3 posCurrent,
                                        Vector3 posNextestimate,
                                        CSplineManufacturer.curvesection[] arrpossection,
                                        ref Vector3 posnextCalculated)
    {
        int iSectionLast = arrpossection.Length - 1;
        int iSectionnew = iSectionCurrent, iSectionCurr = iSectionCurrent;

        if (iSectionLast <= iSectionCurr)
        {
            posnextCalculated = posNextestimate;
            return iSectionLast;
        }

        Vector3 posnextestIter = posNextestimate;
        bool bAdjustment = false;
        float distCrossover = 0.0f;
        for (int iSec = iSectionCurr; iSec <= iSectionLast; ++iSec)
        {
            CSplineManufacturer.curvesection curveSec = arrpossection[iSec];
            if (true == curveSec.crossoverCurr(posnextestIter))
            {
                if (iSectionLast == iSec)
                {
                    iSectionnew = iSec;
                    break;
                }

                CSplineManufacturer.curvesection curveSecNext = arrpossection[iSec + 1];

                distCrossover = (posnextestIter - curveSec._posSectionDest).magnitude;
                posnextestIter = curveSecNext._posSectionStart + (curveSecNext._dirtoNextP * distCrossover);
                bAdjustment = true;
            }
            else
            {
                iSectionnew = iSec;
                break;
            }
        }

        if (true == bAdjustment)
        {
            forwardforAngleTarget = forwardforMovement = m_arrLineGuide[iSectionnew]._dirtoNextP;
        }

        posnextCalculated = posnextestIter;

        return iSectionnew;
    }

    protected Vector3 getFindNextDir(Vector3 v3Pos, int iSectionPath, CSplineManufacturer.curvesection[] arrPosFlyPath, ref int iseqSPnew)
    {
        int iSeqSelect = -1;
        int iSeqLast = arrPosFlyPath.Length - 1;

        if (iSeqLast <= iSectionPath)
        {
            iSeqSelect = iSeqLast - 1;
        }
        else
        {
            iSeqSelect = iSectionPath;
        }

        int iSeqPathTarget = -1;

        for (int iSeq_ = iSeqSelect + 1; iSeq_ < iSeqLast + 1; ++iSeq_)
        {
            if (false == arrPosFlyPath[iSeq_].crossoverCurr(v3Pos))
            {
                iSeqPathTarget = iSeq_;
                break;
            }
        }

        if (-1 == iSeqPathTarget || iSeqLast == iSeqPathTarget)
        {
            iseqSPnew = iSectionPath;
            return getLastDir(arrPosFlyPath);
        }

        iSectionPath = iSeqPathTarget;
        Vector3 v3TargetPath = arrPosFlyPath[iSeqPathTarget]._posSectionDest;

        return (v3TargetPath - v3Pos).normalized;
    }

    protected void UpdateAllFlyObj_Movements(float fDeltaTime)
    {
        if (null == m_arrLineGuide)
        {
            return;
        }

        int iCurrent = sectionSP;

        bool bReachtheLast = false;

        if (iCurrent <= m_arrLineGuide.Length - 1)
        {
            Vector3 v3PosObj = positionUnit;
            Vector3 v3PosEstimateNext = v3PosObj + (forwardforMovement * (speedmove * fDeltaTime));
            Vector3 v3PosNextAdjustment = new Vector3();

            sectionSP = getSectionNextAdjust(sectionSP, v3PosObj, v3PosEstimateNext, m_arrLineGuide, ref v3PosNextAdjustment);

            positionUnit = v3PosNextAdjustment;
        }
        else
        {
            bReachtheLast = true;
        }

        //@ Reach the goal
        if (true == bReachtheLast)
        {
            //@ !!!Reach the goal
            reachTheDestination = true;
        }

        base.Update_angle_unitDyn(fDeltaTime);
    }

    protected override void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("ArtMapModel"))
        {
            isCollisionArtMap = true;
            aroundUnits.Clear();
            adjustAttackPosition = -adjustAttackPosition;
            return;
        }

        UnitFlying unit = other.GetComponent<UnitFlying>();
        if (unit != null)
        {
            if (aroundUnits.BinarySearch(unit.id) < 0)
            {
                aroundUnits.Add(unit.id);
                aroundUnits.Sort();
            }
        }
    }

    protected override void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("ArtMapModel"))
        {
            isCollisionArtMap = false;
        }

        UnitFlying unit = other.GetComponent<UnitFlying>();
        if (unit != null)
        {
            aroundUnits.Remove(unit.id);
        }
    }

    #endregion // MOVEMENT_UNITFLY

    protected override void DoAction(float deltaTime)
    {
        if (false == _MovementUnitFlying)
        {
            return;
        }

        switch (m_UnitState)
        {
            case UNIT_STATE.SPAWN:
                break;
            case UNIT_STATE.MOVE:
                {
                    UpdateAllFlyObj_Movements(deltaTime);
                }
                break;
            case UNIT_STATE.ATTACK:
                {
                    m_AttackTime += deltaTime;
                    FlyAttackMove(deltaTime);
                }
                break;
            case UNIT_STATE.DIE:

                break;
            default:
                break;
        }
    }

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
                    if (ani.IsPlaying("attack"))
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
    }

    //protected void FlyAttackMove(float deltaTime)
    //{
    //    if (MoveCore == 0)      //  타겟으로 이동
    //    {
    //        //transform.root.Translate(targetDirection);
    //        positionUnit = transform.root.position + targetDirection * deltaTime;
    //        if (Vector3.Distance(transform.root.position, targetPostion) <= 0.1f)
    //            MoveCore = 1;

    //    }
    //    else if (MoveCore == 1)     //  타겟주위에서 비행
    //    {
    //        if (bezierElapsedTime == 0.0f)
    //        {
    //            bezierPoint[0] = targetPostion;
    //            Quaternion q = Quaternion.Euler(0, 90, 0);
    //            bezierPoint[1] = targetPostion + bezierDirection * bezierTurningRadius + q * bezierDirection * bezierTurningRadius * bezierRotationDir;
    //            bezierPoint[2] = targetPostion + bezierDirection * bezierTurningRadius - q * bezierDirection * bezierTurningRadius * bezierRotationDir;
    //        }
    //        bezierElapsedTime += deltaTime;
    //        positionUnit = CMATH.Bezier(bezierPoint[0], bezierPoint[1], bezierPoint[2], bezierPoint[0],
    //            (bezierElapsedTime / bezierTimePerTurning > 1.0f ? 1.0f : bezierElapsedTime / bezierTimePerTurning));
    //        if (bezierElapsedTime >= bezierTimePerTurning)
    //        {
    //            bezierElapsedTime = 0.0f;
    //            bezierDirection = -bezierDirection;
    //            if (bezierRotationDir == 1)
    //                bezierRotationDir = -1;
    //            else
    //                bezierRotationDir = 1;

    //        }
    //    }
    //}

    protected void FlyAttackMove(float deltaTime)
    {
        if (aroundUnits.Count > 0)
        {
            UnitPool unitPool = UnitPool.GetInstance;
            UnitBase unit = null;
            int count = 0;

            //if (m_UnitTemplate.m_AttackTerm <= m_AttackTime)
            {
                foreach (int unitID in aroundUnits)
                {
                    unit = unitPool[unitID];
                    if (unit != null && !unit.IsUnitDie())
                    {
                        adjustAttackPosition += transform.position - unit.transform.position;
                        count++;
                    }
                }
            }

            if (count != 0)
            {
                adjustAttackPosition = Vector3.Normalize(adjustAttackPosition) * deltaTime * speedmove;
                positionUnit += adjustAttackPosition;
            }
        }
        else
        {
            if (isCollisionArtMap)
            {
                adjustAttackPosition = Vector3.Normalize(adjustAttackPosition) * deltaTime * speedmove;
                positionUnit += adjustAttackPosition;
            }
        }
    }

    protected override void AttackPreUpdate()
    {
    }

    protected override bool IsInvalidTarget(int coreID)
    {
        return false;
    }

    protected void SetFlyAttackMove(Vector3 targetPos, float turningRadius, float turningTime)
    {
        bezierDirection = new Vector3(1, 0, 0);
        bezierRotationDir = 1;
        bezierElapsedTime = 0;
        Quaternion q = Quaternion.Euler(0, Random.value * 360, Random.value * 15);
        bezierDirection = q * bezierDirection;
        bezierTurningRadius = turningRadius;
        bezierTimePerTurning = turningTime;

        targetPostion = targetPos;
        targetDirection = (targetPostion - transform.root.position);
        targetDirection = Vector3.Normalize(targetDirection);
    }

    protected float bezierTimePerTurning = 2;
    protected float bezierTurningRadius = 2;
    protected float bezierElapsedTime = 0;
    protected int bezierRotationDir = 1;
    protected Vector3[] bezierPoint = new Vector3[3];
    protected Vector3 bezierDirection;

    protected int MoveCore = 0;
    protected Vector3 targetPostion;
    protected Vector3 targetDirection;

    List<int> aroundUnits = new List<int>();
    Vector3 adjustAttackPosition = new Vector3();
    bool isCollisionArtMap = false;

} // public class UnitFlying : UnitBase