using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Target : MonoBehaviour
{
    private TargetController targetController;
    private bool dead = false;
    private GameObject body, head;
    private TargetChild bodyTarChild, headTarChild;
    private FixedJoint headFixedJoint;
    private ControllerController controller;

    private void Awake()
    {
        controller = GameObject.FindGameObjectWithTag("Controller").GetComponent<ControllerController>();
        targetController = controller.targetController;
        headFixedJoint = transform.GetChild(1).gameObject.GetComponent<FixedJoint>();
        headFixedJoint.breakForce = targetController.headJointBreakForce;
    }

    public void Collision()
    {
        if (!dead)
        {
            dead = true;
            targetController.RemoveTarget();
        }

    }
}
