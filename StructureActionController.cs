using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Structure))]
public class StructureActionController : MonoBehaviour
{
    /// <summary>
    /// DESCRIPTION: An Action Controller is a component attached to a Structure gameObject.
    /// Used to QUEUE and EXECUTE actions
    /// </summary>
    /// 

    public Queue<UnitAction> actionQueue { get; private set; }
    private UnitAction curAction;

    // Components
    private Structure structureInfo;

    private void Awake()
    {
        structureInfo = GetComponent<Structure>();
    }

    private void Start()
    {
        
    }

    private void Update()
    {
        
    }
}
