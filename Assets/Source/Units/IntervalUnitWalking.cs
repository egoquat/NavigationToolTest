using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class IntervalUnitWalking
{
    class cintervalinfo
    {
        public float _fTimeIntervalSource = 1.0f;
        public float _fTimeInterval = 1.0f;
        public Vector3 _v3Position = new Vector3();
        public float _fTimeElapsedSum = 0.0f;
        public int _iIdxTriInterval = -1;
        public int _icountRespawn = 0;
        GameContext _gc = GameContext.GetInstance;

        public bool IntervalTime(float fElapsedTime_)
        {
            _fTimeInterval = _fTimeIntervalSource / _gc.GameSpeed;
            _fTimeElapsedSum = _fTimeElapsedSum + fElapsedTime_;

            if (_fTimeElapsedSum > _fTimeInterval)
            {
                _fTimeElapsedSum = 0.0f;
                return true;
            }

            return false;
        }

        public cintervalinfo(int iIdxTriInterval, int icountRespawn, float fTimeInterval_, Vector3 v3Position_)
        {
            _fTimeIntervalSource = _fTimeInterval = fTimeInterval_;
            _v3Position = v3Position_;
            _fTimeElapsedSum = 0.0f;
            _iIdxTriInterval = iIdxTriInterval;
            _icountRespawn = icountRespawn;
        }
    }

    List<cintervalinfo> m_listInterval = new List<cintervalinfo>();
    CTriCollector m_tricollector = null;

    public bool randomUnitWalkingVisual = false;

    bool m_bWorkprocessInterval = false;
    public bool workprocessInterval
    {
        get { return m_bWorkprocessInterval; }
        set { m_bWorkprocessInterval = value;}
    }

    Vector3 GetStartTriangleRandomPosition(CTriCollector tricollector, int idxTriangle)
    {
        CTRI tri = tricollector.getTri(idxTriangle);

        float r1 = Random.Range(0.1f, 0.8f);
        float r2 = Random.Range(0.1f, (1.0f - r1 - 0.1f));
        float r3 = 1.0f - r1 - r2;

        float positionX = tri._arrv3PT[0].x * r1 + tri._arrv3PT[1].x * r2 + tri._arrv3PT[2].x * r3;
        float positionZ = tri._arrv3PT[0].z * r1 + tri._arrv3PT[1].z * r2 + tri._arrv3PT[2].z * r3;

        return new Vector3(positionX, tri._arrv3PT[0].y, positionZ);
    }


    //@ Process
    public void InitIntervalUnitWalking(CTriCollector tricollector)
    {
        m_tricollector = tricollector;
    }

    public void newIntervalConstantly(int iTriIdxInterval, int icountRespawn, float fTimeInterval, Vector3 v3Position)
    {
        cintervalinfo intervalInfo = new cintervalinfo(iTriIdxInterval, icountRespawn, fTimeInterval, v3Position);
        m_listInterval.Add(intervalInfo);
    }

    public static void newUnitWalkingModel(Vector3 posUnitWalking, int idUnitModelType, bool RepresentRandomUnit)
    {
        UnitWalking unitWalking_New;
        if (CProcessInput.NULLMODELTYPE_UNITWALKING == idUnitModelType)
        {
            unitWalking_New = (UnitWalking)GameObject.Instantiate(
                                processCycle.GetInstance.m_unitWalking_src, posUnitWalking, Quaternion.identity);
            unitWalking_New.InitUnitMovement();
        }
        else
        {   // represent unitmodel.
            GameObject uobject = processCycle.GetInstance.m_unitFactory_.NewUnitInstantiate(
                                idUnitModelType, posUnitWalking, Quaternion.identity);

            unitWalking_New = uobject.transform.root.GetComponentInChildren<UnitWalking>();
            unitWalking_New.InitUnitProperty(idUnitModelType, 1000,1000, 10, -1);
        }

        if (true == RepresentRandomUnit)
        {
            float fSpeedLimitMax = 1.5f, fSpeedLimitMin = 0.4f;
            float fSpeedrandom = UnityEngine.Random.Range(fSpeedLimitMin, fSpeedLimitMax);
            float fScalerandom = UnityEngine.Random.Range(0.4f, 1.2f);

            float fcolorlerp = (fSpeedrandom - fSpeedLimitMin) * (1.0f / (fSpeedLimitMax - fSpeedLimitMin));
            Color colorlerp = Color.Lerp(Color.green, Color.red, fcolorlerp);

            unitWalking_New.speedmove = unitWalking_New.speedmove * (fSpeedrandom);
            unitWalking_New.transform.root.localScale = unitWalking_New.transform.root.localScale * (fScalerandom);

            unitWalking_New.GetComponent<Renderer>().material.color = colorlerp;
        }
    }

    public void deleteInterval(int iTriIdxInterval)
    {
        List<cintervalinfo> listDeleteInterval = new List<cintervalinfo>();
        foreach (cintervalinfo intervalInfo in m_listInterval)
        {
            if (iTriIdxInterval == intervalInfo._iIdxTriInterval)
            {
                listDeleteInterval.Add(intervalInfo);
            }
        }

        foreach (cintervalinfo intervalInfoDel in listDeleteInterval)
        {
            m_listInterval.Remove(intervalInfoDel);
        }

        listDeleteInterval.Clear();
    }

    public void UpdateIntervalProcess(float fElapsedTime, int iIDModelUnit)
    {
        if (false == m_bWorkprocessInterval)
        {
            return;
        }

        Vector3 v3respawn;

        for (int iIt = 0; iIt < m_listInterval.Count; ++iIt)
        {
            cintervalinfo intervalInfoCurr = m_listInterval[iIt];
            
            if(intervalInfoCurr.IntervalTime(fElapsedTime) == true)
            {
                for (int irespawn = 0; irespawn < intervalInfoCurr._icountRespawn; ++irespawn)
                {
                    v3respawn = GetStartTriangleRandomPosition(m_tricollector, intervalInfoCurr._iIdxTriInterval);
                    newUnitWalkingModel(v3respawn, iIDModelUnit, randomUnitWalkingVisual);
                }
            }
        }
    }

    //@ DestructIntervalsAll
    public void DestructIntervalsAll()
    {
        m_listInterval.Clear();
    }

} // public class IntervalUnitWalking : MonoBehaviour
