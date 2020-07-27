using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Threading;

public class UnitPool
{
    public static UnitPool GetInstance
    {
        get
        { return Singleton<UnitPool>.GetInstance; }
    }

    public static implicit operator bool(UnitPool _u)
    {
        return _u != null;
    }

    public int Register(UnitBase newUnit)
    {
        int newID = Interlocked.Increment(ref next_id);

        lock (lockThis)
        {
            _pool[newID] = newUnit;
        }

        return newID;
    }

    public void Unregister(int _id)
    {
        lock (lockThis)
        {
            _pool.Remove(_id);
        }
    }

    public UnitBase this[int _id]
    {
        get
        {
            UnitBase unit = null;
            lock (lockThis) _pool.TryGetValue(_id, out unit);
            return unit;
        }
    }

    public int GetUnitCount()
    {
        return _pool.Count;
    }

    public void SetGameSpeed(float gameSpeed)
    {
        UnitBase unit = null;
        foreach (KeyValuePair<int, UnitBase> data in _pool)
        {
            unit = data.Value;
            unit.SetAnimationSpeed(gameSpeed);
        }
    }

    UnitPool()
    {
    }

    ~UnitPool()
    {
    }

    object lockThis = new object();

    SortedDictionary<int, UnitBase> _pool = new SortedDictionary<int, UnitBase>();

    public int next_id = 0;

    public void TruncateAll()
    {
        ICollection<UnitBase> collectionUnitbase = _pool.Values;
        if (collectionUnitbase.Count < 1)
        {
            return;
        }

        foreach (UnitBase unitbase in collectionUnitbase)
        {
            GameObject.Destroy(unitbase.transform.root.gameObject);
        }
    }
}