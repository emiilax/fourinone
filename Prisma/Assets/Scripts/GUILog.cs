using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
public class GUILog : NetworkBehaviour
{
    Queue<string> logQ = new Queue<string>();
    int max = 50;
    static GUILog mInstance;
    string log;
    public static GUILog Instance
    {
        get
        {
            if (mInstance == null) {
                mInstance = (new GameObject("GUILog")).AddComponent<GUILog>();
                DontDestroyOnLoad(mInstance);
                return mInstance;
            }
            else { return mInstance; }
            //return mInstance ? (mInstance = (new GameObject("MyClassContainer")).AddComponent<GUILog>());
        }
    }
    void addMessage(string msg) {
        logQ.Enqueue(msg);
        while (logQ.Count > max) { logQ.Dequeue(); }
    }

    public static void Log(string msg) { Instance.addMessage(msg); }

    void OnGUI()
	{
        string printString = "";
        foreach (string m in logQ) {
            printString += m + "\n";
        }

        //GUI.Label(new Rect(10, 10, 200, 500), printString);
    }
}