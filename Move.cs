using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Move : MonoBehaviour {

    public GameObject Canvas;
    public GameObject head;
    private Rigidbody2D rbHead;
    
    void Start() {
        rbHead = head.GetComponent<Rigidbody2D>();
    }

    void Update() {
        // Реализация кнопок перемещения персонажа:
        if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow)) {
            Vector2 move = new Vector2(-head.transform.position.x * 300f * Time.deltaTime, 0);
            rbHead.AddForce(move, ForceMode2D.Force);
        }
        if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow)) {
            Vector2 move = new Vector2(head.transform.position.x * 300f * Time.deltaTime, 0);
            rbHead.AddForce(move, ForceMode2D.Force);
        }
        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow)) {
            Vector2 move = new Vector2(0, head.transform.position.y * 300f * Time.deltaTime);
            rbHead.AddForce(move, ForceMode2D.Force);
        }
        if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow)) {
            Vector2 move = new Vector2(0, -head.transform.position.y * 300f * Time.deltaTime);
            rbHead.AddForce(move, ForceMode2D.Force);
        }
    }
}
