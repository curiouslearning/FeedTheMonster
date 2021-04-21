using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Firebase;
using Firebase.Messaging;
using Firebase.Analytics;

public class Analitics : MonoBehaviour
{
	public static Analitics Instance;
	private delegate void DeferredEvent();
	private DeferredEvent deferred;
	public UINotificationPopup popup;
	public bool isReady = false;
	private const int MAXUSERPROPERTIES = 25;

	void Awake()
    {
        Instance = this;
		// we need to explicitly exclude the editor to prevent Player crashes
	}

	// Use this for initialization
	void Start ()
	{

		#if UNITY_ANDROID && !UNITY_EDITOR
		FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(task =>
		{
			var dependencyStatus = task.Result;
			if (dependencyStatus == Firebase.DependencyStatus.Available)
			{
				var app = FirebaseApp.DefaultInstance;
				Instance.isReady = true;
				deferred.Invoke(); // log events that were captured before initialization
				Debug.Log("successfully initialized firebase");
			}
			else
			{
				Debug.LogError(System.String.Format(
					"Could not resolve all Firebase dependencies: {0}", dependencyStatus));
			}
		});

		#endif
		/*if (Firebase.Analytics.FirebaseAnalytics != null) {
			Firebase.Analytics.FirebaseAnalytics.StartSession ();
		}*/
	}

    private void OnApplicationPause(bool pauseStatus)
    {
        if (!pauseStatus)
        {

        }
    }

    void OnDisable() {
		/*if (Firebase.Analytics.FirebaseAnalytics != null) {
			Firebase.Analytics.FirebaseAnalytics.StopSession ();
		}*/
	}


	public void treckScreen (string screenName)
	{
		#if  UNITY_ANDROID && !UNITY_EDITOR
			FirebaseAnalytics.SetCurrentScreen (screenName, null);
		#endif
	}


	public void treckEvent (AnaliticsCategory category, AnaliticsAction action, string label, long value = 0)
	{
		treckEvent (category, action.ToString (), label, value);
	}

	public void treckEvent (AnaliticsCategory category, string action, string label, long value = 0)
	{
		#if UNITY_ANDROID
		if(!isReady) //defer events that fire before Firebase is initialized
        {
			deferred += () =>
			{
				treckEvent(category, action, label, value);
			};
			return;
        }
		FirebaseAnalytics.LogEvent (category.ToString (), new Firebase.Analytics.Parameter[] {
			new Firebase.Analytics.Parameter (
				"action", action
			),
			new Firebase.Analytics.Parameter (
				"label", label
			),
			new Firebase.Analytics.Parameter (
				"value", value
			)
		});
		#endif
	}

	public void logNotification(FirebaseMessage message)
    {
		if(!isReady)
        {
			deferred += () =>
			{
				logNotification(message);
			};
			return;
        }
		FirebaseAnalytics.LogEvent("notification_received", new Parameter[]
		{
			new Parameter(
				"message_id", message.MessageId
				),
			new Parameter(
				"message_type", message.MessageType
				),
			new Parameter (
				"notif_opened", message.NotificationOpened.ToString()
				),
		});
    }

	public void setUserProperty(string prop, string val)
    {
		FirebaseAnalytics.SetUserProperty(prop, val);
    }

    List<string[]> parseDeepLink(string url)
    {
		string prefix = "feedthemonster://";
		char paramParseChar = '/';
		char valParseChar = '=';
		string cleanUrl = url.Replace(prefix, "").TrimStart().TrimEnd();
		string[] split_url = cleanUrl.Split(paramParseChar);
		List<string[]> paramList = new List<string[]>();
		for (int i=0; i < split_url.Length; i++)
        {
			string[] vals = split_url[i].Split(valParseChar);
			if(vals[0] != null && vals[1] != null)
            {
				paramList.Add(vals);
            }
        }
		return paramList;

    }

}
