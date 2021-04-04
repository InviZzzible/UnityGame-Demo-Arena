using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RightShoulderOfEnemy : MonoBehaviour
{
    private Rigidbody2D rbObj;

    private AudioSource Hit;
    private AudioSource Block;


    void Start() {
        this.gameObject.name = "__RightShoulder";
        rbObj = this.GetComponent<Rigidbody2D>();
        Hit = GameObject.Find("HitSound").GetComponent<AudioSource>();
        Block = GameObject.Find("BlockSound").GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update() {

    }

    // Функция для определения обратного вектора в результате столкновения двух объектов:
    private Vector2 VectorDirect(Vector2 cont) {

        float x = rbObj.transform.position.x;
        float y = rbObj.transform.position.y;

        // Длина вектора будет максимальной из сторон объекта умноженного на коэффициент:
        float max = Mathf.Max(rbObj.GetComponent<RectTransform>().rect.width, rbObj.GetComponent<RectTransform>().rect.height) * 15f;

        if (x - cont.x < 0f) {
            x -= max;
        }
        else if (x - cont.x > 0f) {
            x += max;
        }

        if (y - cont.y < 0f) {
            y -= max;
        }
        else if (y - cont.y > 0f) {
            y += max;
        }

        return new Vector2(x, y);
    }

    private void OnCollisionEnter2D(Collision2D collision) {
        GameObject oth = collision.gameObject;
        Rigidbody2D rbOther = oth.GetComponent<Rigidbody2D>();

        if (oth.name == "LeftWall" ||
            oth.name == "Roof" ||
            oth.name == "RightWall" ||
            oth.name == "Ground" ||
            oth.name == "__Body" ||
            oth.name == "__Head" ||
            oth.name == "__LeftShoulder" ||
            oth.name == "__RightShoulder" ||
            oth.name == "__LeftHand" ||
            oth.name == "__RightHand" ||
            oth.name == "__LeftLeg" ||
            oth.name == "__RightLeg" ||
            oth.name == "__LeftFoot" ||
            oth.name == "__RightFoot") {
            // -------------
        }
        else {
            if (oth.name == "Head") { // Получаем повреждение
                ContactPoint2D[] contact = collision.contacts;
                Vector2 cont = contact[0].point;
                Vector2 move = VectorDirect(cont);
                rbObj.AddForce(move, ForceMode2D.Impulse);
                Hit.Play();
                ShowEffect();
            }
            else if (oth.name == "LeftHand" ||
                oth.name == "RightHand" ||
                oth.name == "LeftFoot" ||
                oth.name == "RightFoot") { // Получаем повреждение
                ContactPoint2D[] contact = collision.contacts;
                Vector2 cont = contact[0].point;
                Vector2 move = VectorDirect(cont);
                rbObj.AddForce(move, ForceMode2D.Impulse);
                Hit.Play();
                ShowEffect();
            }
            else { // Не получаем повреждений.
                ContactPoint2D[] contact = collision.contacts;
                Vector2 cont = contact[0].point;
                Vector2 move = VectorDirect(cont);
                rbObj.AddForce(move, ForceMode2D.Impulse);
                Block.Play();
            }
        }
    }

    IEnumerator SetTimeout(float sec) {
        yield return new WaitForSeconds(sec);
        this.GetComponent<CanvasRenderer>().SetColor(new Color(255f, 255f, 255f, 255f));
    }

    private void ShowEffect() {
        this.GetComponent<CanvasRenderer>().SetColor(new Color(255f, 0f, 0f, 255f));
        StartCoroutine(SetTimeout(0.1f));
    }

}
