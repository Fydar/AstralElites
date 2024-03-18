using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class BunnyGroup : ScriptableObject
{
    [SerializeField] private List<string> inclusionLabels = new();

    public List<string> InclusionLabels => inclusionLabels;
}
