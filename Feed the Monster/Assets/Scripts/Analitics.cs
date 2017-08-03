using UnityEngine;
using System.Collections;

public class Analitics : MonoBehaviour
{
	public static Analitics Instance;


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
		Firebase.Analytics.FirebaseAnalytics.SetCurrentScreen (screenName, null);
	}


	public void treckEvent (AnaliticsCategory category, AnaliticsAction action, string label, long value = 0)
	{
		treckEvent (category, action.ToString (), label, value);
	}

	public void treckEvent (AnaliticsCategory category, string action, string label, long value = 0)
	{
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
	}



}
