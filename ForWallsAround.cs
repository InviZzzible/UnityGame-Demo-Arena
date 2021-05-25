using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ForWallsAround : MonoBehaviour
{
    void Start(){}

    private void OnCollisionEnter2D(Collision2D collision) {
        // LeftWall || RightWall || Ground || Roof || CircleAroundWall

        // Здесь можно будет обработать возможность взаимодействия стен с объектами...
        // Пока что поле оставляю без реализации ...
    }
}
