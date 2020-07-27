using UnityEngine;
using System.Collections;
using System.Collections.Generic;

//@ Rounded Nonuniform Spline Curve // 둥근 비균일 스플라인 ///////////////////////////////////////////////////////////////////
public class RNS
{
    protected struct SPPNT // splineData
    {
        public Vector3 _posSP;
        public Vector3 _velSP;
        public float _distSP;
    };

    #region constparameter
    public static int NODE_FIRST = 0;
    public static readonly float DFLT_MAXDISTANCE = 0.0f;
    #endregion // #region constparameter

    public float m_fmaxDistance = DFLT_MAXDISTANCE;

    protected List<SPPNT> m_listCurvePnt = new List<SPPNT>();
    protected Matrix4x4 m_matHermit = new Matrix4x4();

    // cubic curve defined by 2 positions and 2 velocities
    protected Vector3 GetPosOnCubic(Vector3 startPos, 
                                    Vector3 startVel, 
                                    Vector3 endPos, 
                                    Vector3 endVel, 
                                    float time)
    {
        Matrix4x4 matQuad = new Matrix4x4();

        matQuad.SetRow(0, startPos);
        matQuad.SetRow(1, endPos);
        matQuad.SetRow(2, startVel);
        matQuad.SetRow(3, endVel);

        Matrix4x4 matCubic = m_matHermit * matQuad;
        Vector4 timeVector = new Vector4(time * time * time, time * time, time, 1.0f);

        //Vector3 v3PosCubicResult = new Vector3( Vector4.Dot(timeVector, matCubic.GetColumn(0)), 
        //                                        Vector4.Dot(timeVector, matCubic.GetColumn(1)), 
        //                                        Vector4.Dot(timeVector, matCubic.GetColumn(2)) );
        Vector3 v3PosCubicResult = matCubic.transpose * timeVector;

        return v3PosCubicResult;
    }

    //@ Intialize Hermit Matrix, m_listCurvePnt Property...
    protected void Init()
    {
        m_matHermit.SetRow(0, new Vector4(2.0f, -2.0f, 1.0f, 1.0f));
        m_matHermit.SetRow(1, new Vector4(-3.0f, 3.0f, -2.0f, -1.0f));
        m_matHermit.SetRow(2, new Vector4(0.0f, 0.0f, 1.0f, 0.0f));
        m_matHermit.SetRow(3, new Vector4(1.0f, 0.0f, 0.0f, 0.0f));
    }

    public void ReleaseSpline()
    {
        m_listCurvePnt.Clear();
    }

    public void AddNode(Vector3 pos)
    {
        if (m_listCurvePnt.Count < 1)
            m_fmaxDistance = DFLT_MAXDISTANCE;
        else
        {
            int iNodeLast = m_listCurvePnt.Count - 1;

            SPPNT sppntLast = m_listCurvePnt[iNodeLast];

            sppntLast._distSP = Vector3.Magnitude(sppntLast._posSP - pos);
            m_fmaxDistance += sppntLast._distSP;

            m_listCurvePnt[iNodeLast] = sppntLast;
        }

        SPPNT spPntAddNode = new SPPNT();
        spPntAddNode._posSP = pos;

        m_listCurvePnt.Add(spPntAddNode);
    }

    public bool BuildSpline_rns()
    {
        if(m_listCurvePnt.Count < 2)
        {
            Debug.Log("BuildCurve needs at least 2 more points!!");
            return false;
        }

        for (int i = 1; i < m_listCurvePnt.Count - 1; i++)
        {
            // split the angle (figure 4)
            SPPNT sppntCurr = m_listCurvePnt[i];
            sppntCurr._velSP = Vector3.Normalize(m_listCurvePnt[i + 1]._posSP - m_listCurvePnt[i]._posSP) -
                               Vector3.Normalize(m_listCurvePnt[i - 1]._posSP - m_listCurvePnt[i]._posSP);

            sppntCurr._velSP.Normalize();

            m_listCurvePnt[i] = sppntCurr;
        }
        // calculate start and end velocities
        int iLastNode = m_listCurvePnt.Count - 1;

        SPPNT sppntFirst = m_listCurvePnt[NODE_FIRST];
        SPPNT sppntLast = m_listCurvePnt[iLastNode];

        sppntFirst._velSP = GetStartVelocity(0);
        sppntLast._velSP = GetEndVelocity(iLastNode);

        m_listCurvePnt[NODE_FIRST] = sppntFirst;
        m_listCurvePnt[iLastNode] = sppntLast;

        return true;
    }

    //@ spline access function. time is 0 -> 1
    public Vector3 GetPositionSpline(float time)
    {
        float distance = time * m_fmaxDistance;
        float currentDistance = 0.0f;
        int iSpot = m_listCurvePnt.Count-1;

        for (int iSeq = 0; iSeq < m_listCurvePnt.Count; ++iSeq )
        {
            if(currentDistance + m_listCurvePnt[iSeq]._distSP >= distance)
            {
                iSpot = iSeq;
                break;
            }

            currentDistance += m_listCurvePnt[iSeq]._distSP;
        }

        SPPNT spPntSpot = m_listCurvePnt[iSpot];
        SPPNT spPntSpot_next = m_listCurvePnt[iSpot + 1];

        float t = distance - currentDistance;
        t /= spPntSpot._distSP; // scale t in range 0 - 1

        Vector3 startVel = spPntSpot._velSP * spPntSpot._distSP;
        Vector3 endVel = spPntSpot_next._velSP * spPntSpot._distSP;

        Vector3 v3PosResult = GetPosOnCubic(  spPntSpot._posSP, 
                                    startVel,
                                    spPntSpot_next._posSP, 
                                    endVel, 
                                    t);

        return v3PosResult;
    }

    protected Vector3 GetStartVelocity(int index)
    {
        SPPNT spPntCurrent = m_listCurvePnt[index];
        SPPNT spPntNext = m_listCurvePnt[index + 1];

        Vector3 temp = 3.0f * (spPntNext._posSP - spPntCurrent._posSP) / spPntCurrent._distSP;
        return (temp - spPntNext._velSP) * 0.5f;
    }

    protected Vector3 GetEndVelocity(int index)
    {
        SPPNT spPntCurrent = m_listCurvePnt[index];
        SPPNT spPntPrevious = m_listCurvePnt[index - 1];

        Vector3 temp = 3.0f * (spPntCurrent._posSP - spPntPrevious._posSP) / spPntPrevious._distSP;
        return (temp - spPntPrevious._velSP) * 0.5f;
    }

    public RNS()
    {
        Init();
    }

    ~RNS()
    {
        ReleaseSpline();
    }
}; // public class RNS

//@ Smooth Nonuniform Spline Curve // 부드러운 비 균일 스플라인 ///////////////////////////////////////////////////////////////////
public class SNS : RNS
{
     public bool BuildSpline_sns() 
     { 
         if(true==BuildSpline_rns())
         {
             Smooth_sns();
             Smooth_sns();
             Smooth_sns();

             return true;
         }

         return false;
     }

     public void Smooth_sns()
     {
         Vector3 newVel;
         Vector3 oldVel = GetStartVelocity(0);
         for (int i = 1; i < m_listCurvePnt.Count - 1; i++)
         {
             //@ Equation 12
             SPPNT sppntPrevious = m_listCurvePnt[i - 1];
             newVel = GetEndVelocity(i) * m_listCurvePnt[i]._distSP +
                      GetStartVelocity(i) * m_listCurvePnt[i - 1]._distSP;
             newVel /= (m_listCurvePnt[i - 1]._distSP + m_listCurvePnt[i]._distSP);
             sppntPrevious._velSP = oldVel;
             oldVel = newVel;

             m_listCurvePnt[i - 1] = sppntPrevious;
         }

         SPPNT sppntLast = m_listCurvePnt[m_listCurvePnt.Count - 1];
         SPPNT sppntLast2 = m_listCurvePnt[m_listCurvePnt.Count - 2];

         sppntLast._velSP = GetEndVelocity(m_listCurvePnt.Count - 1);
         sppntLast2._velSP = oldVel;

         m_listCurvePnt[m_listCurvePnt.Count - 1] = sppntLast;
         m_listCurvePnt[m_listCurvePnt.Count - 2] = sppntLast2;
     } // public void Smooth_sns()
}; // public class SNS : RNS

//@ Timed Nonuniform spline curve // 시간지정 비균일 스플라인  ///////////////////////////////////////////////////////////////////
public class TNS : SNS
{
    public void AddNode(Vector3 pos, float timePeriod)
    {
        if (m_listCurvePnt.Count == 0)
            m_fmaxDistance = 0.0f;
        else
        {
            int iNodeLast = m_listCurvePnt.Count - 1;
            SPPNT sppntLast = m_listCurvePnt[iNodeLast];

            sppntLast._distSP = timePeriod;
            m_fmaxDistance += sppntLast._distSP;

            m_listCurvePnt[iNodeLast] = sppntLast;
        }

        SPPNT spPntAddNode = new SPPNT();
        spPntAddNode._posSP = pos;

        m_listCurvePnt.Add(spPntAddNode);
    } // public void AddNode(Vector3 pos, float timePeriod)

    public bool BuildSpline_tns()
    {
        if(true==BuildSpline_rns())
        {
            Smooth_tns();
            Smooth_tns();
            Smooth_tns();
            return true;
        }

        return false;
    }

    public void Smooth_tns()
    { 
        Smooth_sns(); 
        Constrain(); 
    }

    //@ Constrain
    public void Constrain()
    {
        for (int i = 1; i < m_listCurvePnt.Count - 1; i++)
        {
            //@ Equation 13
            SPPNT sppntCurr = m_listCurvePnt[i];
            float r0 = Vector3.Magnitude(m_listCurvePnt[i]._posSP - m_listCurvePnt[i-1]._posSP) / m_listCurvePnt[i-1]._distSP;
            float r1 = Vector3.Magnitude(m_listCurvePnt[i+1]._posSP - m_listCurvePnt[i]._posSP) / m_listCurvePnt[i]._distSP;
            sppntCurr._velSP *= 4.0f * r0 * r1 / ((r0 + r1) * (r0 + r1));

            m_listCurvePnt[i] = sppntCurr;
        }
    }

}; // public class TNS : SNS
