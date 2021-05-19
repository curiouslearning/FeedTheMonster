using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase.Messaging;

public class CloudMessageReceiver : MonoBehaviour
{
    public static CloudMessageReceiver Instance;
    private bool initialized = false;
    private string token;
    public UINotificationPopup popupTray;
    void Awake()
    {
        Instance = this;
    }
    void Start()
    {
        
    }

    public void displayTestNotification()
    {
        displayMessageToGUI("Test Title", "This is the test notification body");
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


    private void OnTokenReceived(object sender, TokenReceivedEventArgs token)
    {
        this.token = token.Token;
        Debug.Log("Received Registration Token: " + token.Token);
    }

    private void OnMessageReceived(object sender, MessageReceivedEventArgs e)
    {
        Debug.Log("Received a new message from: " + e.Message.From);
        FirebaseNotification notif = e.Message.Notification;
        if (notif == null)
        {
            return;
        }
        if(notif.Title != null)
        {
            Debug.Log("Title: " + notif.Title);

        }
        if (notif.Body != null)
        {
            Debug.Log("Body: " + notif.Body);
        }
        Analitics.Instance.setUserProperty("received_notification", "true");
        Analitics.Instance.logNotification(e.Message);
    }

    public void subscribeToTopicList(List<string> topicList)
    {
        foreach (string topic in topicList)
        {
            FirebaseMessaging.SubscribeAsync(topic);
        }
    }

    public void unsubscribeFromTopic(string topic)
    {
        FirebaseMessaging.UnsubscribeAsync(topic);
    }

    private void displayMessageToGUI(string title, string body)
    {
        popupTray.updateText(title, body);
        UIController.Instance.ShowPopup(popupTray.gameObject);
    }

    public void OnDestroy()
    {
        FirebaseMessaging.MessageReceived -= OnMessageReceived;
        FirebaseMessaging.TokenReceived -= OnTokenReceived;
    }
}
