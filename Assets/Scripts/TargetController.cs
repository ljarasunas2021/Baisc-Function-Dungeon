using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetController : MonoBehaviour
{
    public float headJointBreakForce;
    private int targets;

    private void Awake() { targets = GameObject.FindGameObjectsWithTag("Target").Length; }

    public void RemoveTarget() { targets -= 1; }

    public int ReturnTargets() { return targets; }
}
