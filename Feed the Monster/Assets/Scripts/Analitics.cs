using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;
using Firebase;
using Firebase.Messaging;
using Firebase.Analytics;

using Facebook.Unity;
using System;

public class Analitics : MonoBehaviour
{
	public static Analitics Instance;
	private delegate void DeferredEvent();
	private DeferredEvent deferred;
	public UINotificationPopup popup;
	public bool isReady = false;
	private const int MAXUSERPROPERTIES = 25;


        public List<string> ListOfSubskills;

    public int numsessions;
    public float sessionstart, avgsession, totalplaytime;
    public double sessionend;
    public System.DateTime m_StartTime;
    public System.DateTime lastplayed;

    public void TryAddNewSubskill(string SSN)
    {
        string subskillname = SSN.ToLower();
        if (!ListOfSubskills.Contains(subskillname))
        {
            ListOfSubskills.Add(subskillname);
        }
    }


    public void TryImproveSubskill(int levelnum, string SSN, float amt)
    {
        if (!UserInfo.Instance.HasEarnedSubskill(levelnum))
        {
            UserInfo.Instance.SetEarnedSubskill(levelnum);
            ImproveSubskill(SSN, amt);

        }
    }

    public void ImproveSubskill(string SSN, float amt)
    {
        float value = UserInfo.Instance.GetSubskillValue(SSN);
        value = value + amt;
        value = Mathf.Min(value, 100f);
        UserInfo.Instance.SetSubskillValue(SSN, value);
        ReportSubskillIncrease(SSN);
    }


    public void ReportSubskillIncrease(string SSN)
    {
        float value = UserInfo.Instance.GetSubskillValue(SSN);
        Debug.Log("improving " + SSN + " to " + value);
        treckEvent(AnaliticsCategory.SubSkills, AnaliticsAction.SubskillIncrease,SSN,value);

    }



    public void startTimeTracking(int uid)
    {
        if (PlayerPrefs.HasKey(uid + "_numSessions"))
        {

            numsessions = PlayerPrefs.GetInt(uid + "_numSessions");
            totalplaytime = PlayerPrefs.GetFloat(uid + "_totalPlayTime");

        }
        else
        {
            numsessions = 0;
            totalplaytime = 0f;

        }

        m_StartTime = System.DateTime.Now;
        Debug.Log(m_StartTime);

    }

		void Awake()
	    {
	        Instance = this;
			// we need to explicitly exclude the editor to prevent Player crashes
	#if UNITY_ANDROID && !UNITY_EDITOR
			initFacebook();

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

   


    public void requestuuid()
    {
        var uid = UsersController.Instance.CurrentProfileId;

        if (PlayerPrefs.HasKey("uuid" + uid ))
        {
            Debug.Log(PlayerPrefs.GetString("uuid" + uid));
       }
        else
        {
            // string adid = reportAdId();

            string adid = SystemInfo.deviceUniqueIdentifier;
           // PlayerPrefs.SetString("uuid" + uid, "noAdidCollected");
          
                
            string rurl = "https://data.curiouslearning.org/generate_uuid?id=";
            rurl += "" + adid + "" + uid;
            rurl += "&organization=CuriousLearning&idType=unityDeviceUniqueIdentifier+profileid";
            Debug.Log(rurl);
            StartCoroutine(GetRequest(rurl));
              
        }
    }

    IEnumerator GetRequest(string uri)
    {
        Debug.Log("trying to get " + uri);
        using (UnityWebRequest webRequest = UnityWebRequest.Get(uri))
        {
            // Request and wait for the desired page.
            yield return webRequest.SendWebRequest();

            string[] pages = uri.Split('/');
            int page = pages.Length - 1;

            if (webRequest.isNetworkError)
            {
                Debug.Log(pages[page] + ": Error: " + webRequest.error);
            }
            else
            {
                Debug.Log(pages[page] + ":\nReceived: " + webRequest.downloadHandler.text);

                UuidReturn json = JsonUtility.FromJson<UuidReturn>(webRequest.downloadHandler.text);
                //Debug.Log(json);
                Debug.Log(json.uuid);
                //Debug.Log(JsonUtility.ToJson(json));
                var uid = UsersController.Instance.CurrentProfileId;
                PlayerPrefs.SetString("uuid" + uid, json.uuid);
                FirebaseAnalytics.SetUserProperty("uuid" + uid, json.uuid);

                Parameter[] paramz = {
                   new Parameter("uuid",json.uuid)
                };
                FirebaseAnalytics.LogEvent("UUIDGenerated", paramz);

                // AssessmentHandler.instance.tryAssessment();


            }
        }
    }


    void OnApplicationFocus(bool hasFocus)
    {
        #if UNITY_ANDROID && !UNITY_EDITOR
        if (hasFocus)
        {
            startTimeTracking(UsersController.Instance.CurrentProfileId);
            initFacebook();
        }
        else
        {
            dotimetracking();
        }
#endif

    }

    private void OnApplicationQuit()
    {
        #if UNITY_ANDROID && !UNITY_EDITOR
        dotimetracking();
#endif
    }

    private void OnApplicationPause(bool pauseStatus)
	    {
	        if (!pauseStatus)
	        {
				initFacebook();

	        }

	    }

	    void OnDisable() {
			/*if (Firebase.Analytics.FirebaseAnalytics != null) {
				Firebase.Analytics.FirebaseAnalytics.StopSession ();
			}*/
		}



    public void dotimetracking()
    {
#if UNITY_ANDROID && !UNITY_EDITOR
        var uid = UsersController.Instance.CurrentProfileId;
        var timeSpan = System.DateTime.Now.Subtract(m_StartTime);
         Debug.Log("current session (" + numsessions + ")" + (float)timeSpan.TotalSeconds);

        totalplaytime += (float)timeSpan.TotalSeconds;
        numsessions += 1;
         Debug.Log("total playtime: " + totalplaytime);
        avgsession = totalplaytime / numsessions;
          Debug.Log("Average: " + avgsession);
        PlayerPrefs.SetInt(uid + "_numSessions", numsessions);
        PlayerPrefs.SetFloat(uid + "_totalPlayTime", totalplaytime);

        float sincelastdays = 0f;

        if (PlayerPrefs.HasKey(uid + "_LastPlayed"))
        {
            lastplayed = System.DateTime.Parse(PlayerPrefs.GetString(uid + "_LastPlayed"));
            var sincelast = System.DateTime.Now.Subtract(lastplayed);
            sincelastdays = (float)sincelast.TotalDays;
              Debug.Log("since last play:" + sincelastdays + "days");
        }


        PlayerPrefs.SetString(uid + "_LastPlayed", System.DateTime.Now.ToString());

        ReportTimeTracking(uid, avgsession, totalplaytime, sincelastdays);
#endif
    }

    public void ReportTimeTracking(int uid, float avgplaytime, float totalplaytime, float dayssincelast)
    {


        treckEvent(AnaliticsCategory.TimeTracking, AnaliticsAction.AvgSession,"average_session" ,avgsession);
        treckEvent(AnaliticsCategory.TimeTracking, AnaliticsAction.TotalPlaytime, "total_playtime" , totalplaytime);
        treckEvent(AnaliticsCategory.TimeTracking, AnaliticsAction.DaysSinceLast, "days_since_last", dayssincelast);
    }


    public void treckScreen (string screenName)
	{
#if UNITY_ANDROID && !UNITY_EDITOR
			FirebaseAnalytics.SetCurrentScreen (screenName, null);
#endif
	}

    public void trackwithuserids(AnaliticsCategory category, AnaliticsAction action, string label, float value, int userid)
    {
        Firebase.Analytics.FirebaseAnalytics.LogEvent(category.ToString(), new Firebase.Analytics.Parameter[] {
            new Firebase.Analytics.Parameter (
                "action", action.ToString()
            ),
            new Firebase.Analytics.Parameter (
                "label", label
            ),
            new Firebase.Analytics.Parameter (
                "value", value
            ),
            new Firebase.Analytics.Parameter(
                "userid", userid
            )

        });
    }


	public void treckEvent (AnaliticsCategory category, AnaliticsAction action, string label, long value = 0)
	{
		treckEvent (category, action.ToString (), label, value);
	}
    public void treckEvent(AnaliticsCategory category, AnaliticsAction action, string label, float value)
    {
        treckEvent(category, action.ToString(), label, value);
    }

    public void treckEvent(AnaliticsCategory category, string action, string label, float value)
    {

        Firebase.Analytics.FirebaseAnalytics.LogEvent(category.ToString(), new Firebase.Analytics.Parameter[] {
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

	private void initFacebook()
	{
        Debug.Log("initing facebook");
#if UNITY_ANDROID && !UNITY_EDITOR
		if (FB.IsInitialized)
		{
			activateFacebook();
		}
		else
		{
			FB.Init(() =>
			{
				activateFacebook();
			});
		}
#endif
	}
    private void activateFacebook()
    {
        Debug.Log("activating facebook");
#if UNITY_ANDROID && !UNITY_EDITOR
       
        FB.ActivateApp();
        FB.Mobile.SetAdvertiserIDCollectionEnabled (false);
        FB.Mobile.FetchDeferredAppLinkData(FbDeepLinkCallback);
        FB.GetAppLink(FbDeepLinkCallback);
        Debug.Log("facebook activations complete");



#endif
      

    }

    private string reportAdId()
    {
        string advertisingID = "";
        bool limitAdvertising = false;

        AndroidJavaClass up = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
        AndroidJavaObject currentActivity = up.GetStatic<AndroidJavaObject>("currentActivity");
        AndroidJavaClass client = new AndroidJavaClass("com.google.android.gms.ads.identifier.AdvertisingIdClient");
        AndroidJavaObject adInfo = client.CallStatic<AndroidJavaObject>("getAdvertisingIdInfo", currentActivity);

        advertisingID = adInfo.Call<string>("getId").ToString();
        Debug.Log("Device ad ID: " + advertisingID);
        limitAdvertising = (adInfo.Call<bool>("isLimitAdTrackingEnabled"));
        setUserProperty("device_ad_id", advertisingID);
        return advertisingID;
    }

    void FbDeepLinkCallback(IAppLinkResult result)
    {
		Debug.Log("received deeplink callback");
        Debug.Log("result URL: " + result.Url);
        Debug.Log("Target URL: " + result.TargetUrl);
        createParamList(parseDeepLink(result.Url));
        createParamList(parseDeepLink(result.TargetUrl));
        setDeepLinkUserProperty(result);
        if (PlayerPrefs.GetInt("isFirst") == 0)
        {
            setDeepLinkUserProperty(result);
            PlayerPrefs.SetInt("isFirst", 1);
        }
        if (!string.IsNullOrEmpty(result.TargetUrl))
        {
            Debug.Log("received Deep link URL: ");
            Debug.Log(result.TargetUrl);
            sendInitEvent(result.TargetUrl);
        } else if (!string.IsNullOrEmpty(result.Url)) {
            Debug.Log("received Deep Link URL: ");
            Debug.Log(result.Url);
            sendInitEvent(result.Url);

        } else {
            sendInitEvent("");
        }
    }
    void sendInitEvent(string url)
    {
        Debug.Log("getting ready to send init events for " + url);
        Parameter[] @params = !string.IsNullOrEmpty(url) ? createParamList(parseDeepLink(url)) : null;
        if (@params != null) {
            Debug.Log("params found");
            FirebaseAnalytics.LogEvent("app_initialized", @params);
            FirebaseAnalytics.LogEvent("DeepLinkParams", @params);
        } else {
            Debug.Log("no params found");
            FirebaseAnalytics.LogEvent("app_initialized");
        }
        Debug.Log("init events sent");

    }

   

	private void setDeepLinkUserProperty(IAppLinkResult result)
    {
        Debug.Log("trying to parse seep link user properties");
        List<string[]> parameters = new List<string[]>();
        if (!string.IsNullOrEmpty(result.TargetUrl)) {
            parameters = parseDeepLink(result.TargetUrl);
        } else if (!string.IsNullOrEmpty(result.Url)) {
            parameters = parseDeepLink(result.Url);
        }
		for (int i=0; i < parameters.Count; i++)
        {
            string[] vals = parameters[i];

            Parameter[] paramz = {
           new Parameter(vals[0], vals[1])
            };
            FirebaseAnalytics.LogEvent("DeepLinkParams", paramz);
            if (i > MAXUSERPROPERTIES) break; //Firebase will not accept more properties
			
           
            PlayerPrefs.SetString("dl_" + vals[0], vals[1]);
			Debug.Log(string.Format("User Property \"{0}\" set to \"{1}\"", vals[0], vals[1]));
            FirebaseAnalytics.SetUserProperty(vals[0], vals[1]);
        }
    }

	public void setUserProperty(string prop, string val)
    {
		FirebaseAnalytics.SetUserProperty(prop, val);
    }

    List<string[]> parseDeepLink(string url)
    {
        Debug.Log(url);
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

    Parameter[] createParamList(List<string[]> paramList)
    {
        List<Parameter> @params = new List<Parameter>();
        List<Parameter> rparts = new List<Parameter>();
        foreach(string[] kvp in paramList)
        {
            @params.Add(new Parameter(kvp[0], kvp[1]));

            if (kvp[0]=="utm_campaign")
             FirebaseAnalytics.SetUserProperty(kvp[0], kvp[1]);

            if (kvp[0] == "ref")
            {
                string[] refparts = kvp[1].Split('.');
               
                for (int i = 0; i < refparts.Length-1; i+=2)
                {
                    string val = refparts[i];
                    string val2 = refparts[i + 1];
                    rparts.Add(new Parameter(val, val2));

                }

            }


        }
        FirebaseAnalytics.LogEvent("DeepLinkParams", rparts.ToArray());
        return @params.ToArray();
    }

}
