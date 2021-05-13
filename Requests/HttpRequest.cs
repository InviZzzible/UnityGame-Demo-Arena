using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;


public class HttpRequest : MonoBehaviour {
    public delegate void DelegateAnswer(string j);
    public event DelegateAnswer Emit_answer;

    private string url = "http://localhost";
    private string port = ":5555";

    public void GET(string path = "") {
        StartCoroutine(GETRequest(path));
    }

    public void POST(string dataForSending = "") {
        StartCoroutine(POSTRequest(dataForSending));
    }

    private IEnumerator GETRequest(string path) {
        UnityWebRequest req = UnityWebRequest.Get(url + port + path);
        yield return req.SendWebRequest();

        if (!req.isNetworkError && !req.isHttpError) {
            string json = req.downloadHandler.text;
            Emit_answer(json);
        }
    }

    private IEnumerator POSTRequest(string dataForSending) {
        dataForSending = hidden.SH.Encrypt(dataForSending);

        UnityWebRequest req = new UnityWebRequest(url + port, "POST");
        req.uploadHandler = (UploadHandler)new UploadHandlerRaw(Encoding.UTF8.GetBytes(dataForSending));
        req.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
        req.SetRequestHeader("Content-Type", "application/json");
        yield return req.SendWebRequest();
        
        if (!req.isNetworkError && !req.isHttpError) {
            string json = hidden.SH.Decipher(req.downloadHandler.text);
            Emit_answer(json);
        }
    }
}