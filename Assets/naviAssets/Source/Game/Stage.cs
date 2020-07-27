using UnityEngine;
using System.Collections;

public class Stage
{
    public virtual void Update(float delta)
    {
    }

    public static implicit operator bool(Stage s)
    {
        return s != null;
    }
}