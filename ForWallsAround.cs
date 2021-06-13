using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ForWallsAround : MonoBehaviour
{
    private GameObject gObj;
    private float accel = 1f;
    private float accelForCircle = 25f;
    void Start(){
        gObj = GameObject.Find("CircleWall");
    }

    private void OnTriggerExit2D(Collider2D collision) {
        Rigidbody2D rigid2dexit2d = collision.gameObject.GetComponent<Rigidbody2D>();
        if (gameObject.name == "CircleWall") {
            rigid2dexit2d.AddForce((gObj.transform.position - collision.transform.position).normalized * accelForCircle, ForceMode2D.Impulse);
        }
    }
    private void OnTriggerStay2D(Collider2D collision) {
        Rigidbody2D rigid2d = collision.gameObject.GetComponent<Rigidbody2D>();

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
    }
}
