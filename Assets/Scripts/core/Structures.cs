using System;
using Random = UnityEngine.Random;

[Serializable]
public struct RangeF
{
    public float min;
    public float max;

    public float randomValue
    {
        get { return Random.Range(min, max); }
    }
}

[Serializable]
public struct Size
{
    public int x;
    public int y;
}