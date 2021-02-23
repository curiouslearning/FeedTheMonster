using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Analitics : MonoBehaviour
{
	public static Analitics Instance;

        public List<string> ListOfSubskills;




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
