using System;
using UnityEngine;

[Serializable]
public struct BunnyReference<T>
{
    [SerializeField] private string guid;

    public readonly string Guid => guid;

    public readonly T LoadAsset()
    {
        return default;
    }
}
