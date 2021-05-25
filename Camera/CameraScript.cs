using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraScript : MonoBehaviour
{
    private GameObject body;
    void Start()
    {
        body = GameObject.Find("Body");
    }

    void LateUpdate()
    {
        Vector3 newCamPos = new Vector3(body.transform.position.x, body.transform.position.y, -1f);
        transform.position = newCamPos;
    }
}
