using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase.Messaging;

public class CloudMessageReceiver : MonoBehaviour
{
    public static CloudMessageReceiver Instance;
    private bool initialized = false;
    void Awake()
    {
        Instance = this;
    }
    void Start()
    {
       
    }

    // Update is called once per frame
    void Update()
    {
        if (Analitics.Instance.isReady == true && initialized == false)
        {
            FirebaseMessaging.TokenReceived += OnTokenReceived;
            FirebaseMessaging.MessageReceived += OnMessageReceived;
            initialized = true;
        }
    }


    public void OnTokenReceived(object sender, TokenReceivedEventArgs token)
    {
        Debug.Log("Received Registration Token: " + token.Token);
    }

    public void OnMessageReceived(object sender, MessageReceivedEventArgs e)
    {
        Debug.Log("Received a new message from: " + e.Message.From);
    }

}
