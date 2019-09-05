using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{

    void OnTriggerEnter(Collider collision)
    {
        if (collision.gameObject.tag == "Target") Destroy(collision.gameObject);
        if (collision.gameObject.tag == "Boundary") Destroy(gameObject);
    }
}
