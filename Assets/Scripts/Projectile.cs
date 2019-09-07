using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    private Vector3 oldpos, newpos, velocity;

    void Start() { oldpos = transform.position; }

    void Update()
    {
        newpos = transform.position;
        var media = (newpos - oldpos);
        velocity = media / Time.deltaTime;
        oldpos = newpos;
        newpos = transform.position;
    }

    // destroy any targets and self destruct if hits boundary
    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Boundary") Destroy(gameObject);
    }

    public Vector2 ReturnVelocity()
    {
        return velocity;
    }
}
