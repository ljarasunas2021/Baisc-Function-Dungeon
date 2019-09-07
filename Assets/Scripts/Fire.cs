using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fire : MonoBehaviour
{
    public void FireFromPlayer()
    {
        GameObject.FindGameObjectWithTag("Player").GetComponent<Shooter>().Shoot();
    }
}
