using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Move : MonoBehaviour {

    private Rigidbody2D rbHead;
    private float accel = 1000f;
    
    void Start() {
        rbHead = GetComponent<Rigidbody2D>();
    }

    void FixedUpdate() {
        // Реализация кнопок перемещения персонажа:
        if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow)) {
            Vector2 move = new Vector2(transform.localPosition.x - 100f, transform.localPosition.y);
            rbHead.AddForce(move.normalized * accel * Time.fixedDeltaTime, ForceMode2D.Force);
        }
        if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow)) {
            Vector2 move = new Vector2(transform.localPosition.x + 100f, transform.localPosition.y);
            rbHead.AddForce(move.normalized * accel * Time.fixedDeltaTime, ForceMode2D.Force);
        }
        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow)) {
            Vector2 move = new Vector2(transform.localPosition.x, transform.localPosition.y + 100f);
            rbHead.AddForce(move.normalized * accel * Time.fixedDeltaTime, ForceMode2D.Force);
        }
        if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow)) {
            Vector2 move = new Vector2(transform.localPosition.x, transform.localPosition.y - 100f);
            rbHead.AddForce(move.normalized * accel * Time.fixedDeltaTime, ForceMode2D.Force);
        }
    }
}
