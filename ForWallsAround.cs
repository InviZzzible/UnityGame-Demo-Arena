using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ForWallsAround : MonoBehaviour
{
    private float accel = 25f;
    void Start(){}

    private void OnTriggerEnter2D(Collider2D collision) {
        Rigidbody2D rigid2d = collision.gameObject.GetComponent<Rigidbody2D>();
        //rigid2d.velocity = -rigid2d.velocity;
        
        if (gameObject.name == "LeftWall") {
            rigid2d.AddForce(Vector2.right * accel, ForceMode2D.Impulse);
        }
        else if (gameObject.name == "RightWall") {
            rigid2d.AddForce(-Vector2.right * accel, ForceMode2D.Impulse);
        }
        else if (gameObject.name == "Ground") {
            rigid2d.AddForce(Vector2.up * accel, ForceMode2D.Impulse);
        }
        else if (gameObject.name == "Roof") {
            rigid2d.AddForce(-Vector2.up * accel, ForceMode2D.Impulse);
        }
        else if (gameObject.name == "CircleAroundWall") {

        }
        
    }
}
