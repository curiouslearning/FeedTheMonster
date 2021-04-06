using UnityEngine;
using System.Collections;
using Firebase;
using Firebase.Messaging;
using Firebase.Analytics;
using Facebook.Unity;

public class Analitics : MonoBehaviour
{
	public static Analitics Instance;
	public UINotificationPopup popup;
	public bool isReady = false;
	int isFirstOpen = 1;

	void Awake()
    {
        Instance = this;
		// we need to explicitly exclude the editor to prevent Player crashes
#if UNITY_ANDROID && !UNITY_EDITOR
		activateFacebook();
		
#endif
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
			activateFacebook();
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

	private void activateFacebook()
	{
		#if UNITY_ANDROID && !UNITY_EDITOR
		if (FB.IsInitialized)
		{
			FB.ActivateApp();
		}
		else
		{
			FB.Init(() =>
			{
				FB.ActivateApp();
				isFirstOpen = PlayerPrefs.GetInt("isFirst");
				if (isFirstOpen == 0)
				{
					Debug.Log("first open");
					FB.Mobile.FetchDeferredAppLinkData(FbDeepLinkCallback);
					PlayerPrefs.SetInt("isFirst", 1);
				}
				else
				{
					FB.GetAppLink(FbDeepLinkCallback);
				}


			});
		}
		#endif
	}

	void FbDeepLinkCallback(IAppLinkResult result)
    {
		Debug.Log("received result");
		if(!string.IsNullOrEmpty(result.TargetUrl))
        {
			popup.updateText("Deep Link: ", result.TargetUrl);
			Debug.Log("received Deep link URL: ");
			Debug.Log(result.TargetUrl);
        }
    }

}
