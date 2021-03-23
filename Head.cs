using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Head : MonoBehaviour
{
    private Rigidbody2D rbObj; // Head

    private AudioSource Hit;

    void Start()
    {
        rbObj = this.GetComponent<Rigidbody2D>();
        Hit = GameObject.Find("HitSound").GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
		
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

        if ( oth.name == "LeftWall" ||
            oth.name == "Roof" ||
            oth.name == "RightWall" ||
            oth.name == "Ground" ||
            oth.name == "Head" ||
            oth.name == "Body" ||
            oth.name == "LeftShoulder" ||
            oth.name == "RightShoulder" ||
            oth.name == "LeftHand" ||
            oth.name == "RightHand" ||
            oth.name == "LeftLeg" ||
            oth.name == "RightLeg" ||
            oth.name == "LeftFoot" ||
            oth.name == "RightFoot") {
            // ----------------------------- Возможно надо добавить силу небольшую...
        }
        else {
            if(oth.name == "__Head") { // Столкновение головами.
                ContactPoint2D[] contact = collision.contacts;
                Vector2 cont = contact[0].point;
                Vector2 move = VectorDirect(cont);
                rbObj.AddForce(move, ForceMode2D.Impulse);
                Hit.Play();
                ShowEffect();
            }
            else if(oth.name == "__LeftHand" ||
                oth.name == "__RightHand" ||
                oth.name == "__LeftFoot" ||
                oth.name == "__RightFoot") { // Столкновение головой с вражескими атакующими частями тела, кроме головы.
                ContactPoint2D[] contact = collision.contacts;
                Vector2 cont = contact[0].point;
                Vector2 move = VectorDirect(cont);
                rbObj.AddForce(move, ForceMode2D.Impulse);
                Hit.Play();
                ShowEffect();
            }
            else { // Остальное. Столкновение головой по уязвимым частям тела врага.
                ContactPoint2D[] contact = collision.contacts;
                Vector2 cont = contact[0].point;
                Vector2 move = VectorDirect(cont);
                rbObj.AddForce(move, ForceMode2D.Impulse);
                Hit.Play();
                ShowEffect();
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
