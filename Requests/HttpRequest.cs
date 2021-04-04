using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;


public class HttpRequest : MonoBehaviour
{
    public delegate void DelegateAnswer(string j);
    public event DelegateAnswer Emit_answer;


    private string url = "http://localhost";
    private string port = ":5555";

    public void GET(string path = "") {
        StartCoroutine(GetRequest(path));
    }

    public void POST(string dataForSending) {
        StartCoroutine(PostRequest(dataForSending));
    }

    protected IEnumerator GetRequest(string path = "") {
        UnityWebRequest req = UnityWebRequest.Get(url + port + path);
        yield return req.SendWebRequest();

        if (!req.isNetworkError) {
            string json = req.downloadHandler.text;
            Emit_answer(json);
        }
    }

    protected IEnumerator PostRequest(string json) {
        UnityWebRequest req = new UnityWebRequest(url, "POST");
        byte[] jsonToSend = new System.Text.UTF8Encoding().GetBytes(json);
        req.uploadHandler = (UploadHandler)new UploadHandlerRaw(jsonToSend);
        req.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
        req.SetRequestHeader("Content-Type", "application/json");

        yield return req.SendWebRequest();

        if (!req.isNetworkError) {
            json = req.downloadHandler.text;
            Emit_answer(json);
        }
    }
}