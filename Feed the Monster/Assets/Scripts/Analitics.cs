using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Analitics : MonoBehaviour
{
	public static Analitics Instance;

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
        treckEvent(AnaliticsCategory.SubSkills, UsersController.Instance.CurrentProfileId + "_IncreaseSubskill_" + SSN,SSN,value);

    }

    public void ReportTimeTracking()
    {
        string pt = UsersController.Instance.CurrentProfileId + "_totalPlayTime";

        treckEvent(AnaliticsCategory.TimeTracking, pt, pt, totalplaytime);
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

       


    }


	// Use this for initialization
	void Start ()
	{
		/*if (Firebase.Analytics.FirebaseAnalytics != null) {
			Firebase.Analytics.FirebaseAnalytics.StartSession ();
		}*/
	}

	void OnDisable() {
        /*if (Firebase.Analytics.FirebaseAnalytics != null) {
			Firebase.Analytics.FirebaseAnalytics.StopSession ();
		}*/


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

        
       
    }


	public void treckScreen (string screenName)
	{
		#if  UNITY_ANDROID 
			Firebase.Analytics.FirebaseAnalytics.SetCurrentScreen (screenName, null);
		#endif
	}


	public void treckEvent (AnaliticsCategory category, AnaliticsAction action, string label, long value = 0)
	{
		treckEvent (category, action.ToString (), label, value);
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
		Firebase.Analytics.FirebaseAnalytics.LogEvent (category.ToString (), new Firebase.Analytics.Parameter[] {
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



}
