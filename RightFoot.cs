using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RightFoot : MonoBehaviour
{
    private Rigidbody2D rbObj;
    private Vector2 velocityPreCollision = new Vector2(0f, 0f); // Вектор скорости до столкновения
    private bool isCollisionEnter = false; // Флаг обработки события OnCollisionEnter2D

    private AudioSource Hit;
    private AudioSource Block;

    void Start()
    {
        rbObj = this.GetComponent<Rigidbody2D>();
        Hit = GameObject.Find("HitSound").GetComponent<AudioSource>();
        Block = GameObject.Find("BlockSound").GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!isCollisionEnter) {
            velocityPreCollision = rbObj.velocity;
        }
    }

    // Функция для определения обратного вектора в результате столкновения двух объектов:
    private Vector2 VectorDirect(Vector2 cont) {

        float x = rbObj.transform.position.x;
        float y = rbObj.transform.position.y;

        // Длина вектора будет максимальной из сторон объекта умноженного на коэффициент:
        float max = Mathf.Max(rbObj.GetComponent<RectTransform>().rect.width, rbObj.GetComponent<RectTransform>().rect.height) * 20f;

        if (rbObj.transform.position.x - cont.x < 0f) {
            x = rbObj.transform.position.x - max;
        }
        else if (rbObj.transform.position.x - cont.x > 0f) {
            x = rbObj.transform.position.x + max;
        }

        if (rbObj.transform.position.y - cont.y < 0f) {
            y = rbObj.transform.position.y - max;
        }
        else if (rbObj.transform.position.y - cont.y > 0f) {
            y = rbObj.transform.position.y + max;
        }

        return new Vector2(x, y);
    }

    private void OnCollisionEnter2D(Collision2D collision) {
        isCollisionEnter = true;
        GameObject oth = collision.gameObject;
        Rigidbody2D rbOther = oth.GetComponent<Rigidbody2D>();
        velocityPreCollision = -velocityPreCollision;

        if (oth.name == "LeftWall" ||
            oth.name == "Roof" ||
            oth.name == "RightWall" ||
            oth.name == "Ground" ||
            oth.name == "Body" ||
            oth.name == "Head" ||
            oth.name == "LeftShoulder" ||
            oth.name == "RightShoulder" ||
            oth.name == "LeftHand" ||
            oth.name == "RightHand" ||
            oth.name == "LeftLeg" ||
            oth.name == "RightLeg" ||
            oth.name == "LeftFoot" ||
            oth.name == "RightFoot") {
            // -------------
        }
        else {
            if (oth.name == "__Head") { // Не получаем урон от столкновений. Но при этом отбрасываемся.
                ContactPoint2D[] contact = collision.contacts;
                Vector2 cont = contact[0].point;
                Vector2 move = VectorDirect(cont);
                rbObj.AddForce(move, ForceMode2D.Impulse);
                Hit.Play();
            }
            else if (oth.name == "__LeftHand" ||
                oth.name == "__RightHand" ||
                oth.name == "__LeftFoot" ||
                oth.name == "__RightFoot") { // Блок. Не получаем повреждений. Незначительно отбрасываемся.
                ContactPoint2D[] contact = collision.contacts;
                Vector2 cont = contact[0].point;
                Vector2 move = VectorDirect(cont);
                rbObj.AddForce(move, ForceMode2D.Impulse);
                Block.Play();
            }
            else { // Остальное. Отбрасываемся
                ContactPoint2D[] contact = collision.contacts;
                Vector2 cont = contact[0].point;
                Vector2 move = VectorDirect(cont);
                rbObj.AddForce(move, ForceMode2D.Impulse);
                Hit.Play();
            }
        }
    }

    private void OnCollisionExit2D(Collision2D collision) {
        isCollisionEnter = false;
    }
}
