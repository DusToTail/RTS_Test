using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Vision : MonoBehaviour, IVision
{
    [SerializeField]
    private float visionRange;

    private float cellRadius;

    private void Start()
    {
        cellRadius = FindObjectOfType<GridController>().cellRadius;
    }
    public float GetVisionRange()
    {
        if(visionRange < 0) { visionRange = 0;}
        return visionRange * cellRadius * 2;
    }
}
