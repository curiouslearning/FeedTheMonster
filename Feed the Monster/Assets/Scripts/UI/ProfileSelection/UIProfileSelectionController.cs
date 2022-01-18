﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIProfileSelectionController : MonoBehaviour {
	public static UIProfileSelectionController Instance;


	public AudioClip TitleSound;

	public UIProfileSelectionButton[] profiles;

	[HideInInspector]
	public GameObject NextScreen;

	[HideInInspector]
	public bool skipConfirmationPopup;

	[HideInInspector]
	public bool allowToSelectCurrentProfile = false;


	UIProfileSelectionButton currentButton;

//	bool isFirstUse = true;

	void Awake()
	{
		Instance = this;
	}


	// Use this for initialization
	void Start () {
		
	}


	void  OnDisable()
	{
		NextScreen = null;
		allowToSelectCurrentProfile = false;
	}

	void OnEnable()
	{
		if (AudioController.Instance) {
			AudioController.Instance.PlaySound (TitleSound);
		}
		UpdateAllProfiles ();
	}	


	// Update is called once per frame
	void Update () {
		
	}

	void UpdateAllProfiles()
	{
		foreach (UIProfileSelectionButton profile in profiles) {
			if (profile != null) {
				if (profile.ProfileId.Equals (UsersController.Instance.CurrentProfileId) && allowToSelectCurrentProfile) {
					profile.MarkSelected ();
				} else {
					profile.MarkUnSelected ();
				}
			}
		}

//		isFirstUse = false;
	}


	public void OnChangeProfile(UIProfileSelectionButton button)
	{
		currentButton = button;

		if (UsersController.Instance.CurrentProfileId.Equals (button.ProfileId) || skipConfirmationPopup) {
			ChangeProfile ();
		} else {
			UIController.Instance.ConfirmationPopup.onConfirm = ChangeProfile;
			UIController.Instance.ShowPopup (UIController.Instance.ConfirmationPopup.gameObject);
		} 
		skipConfirmationPopup = false;
	}

	void ChangeProfile()
	{

        
		UserInfo.Instance.Save();

		UsersController.Instance.CurrentProfileId = currentButton.ProfileId;


		UserInfo.Instance.AddFirstFriendsToCollection ();
		UIController.Instance.OnChangeProfile (NextScreen);
		UIController.Instance.ClosePopup (gameObject);

		NextScreen = null;

        Analitics.Instance.requestuuid();
        AssessmentHandler.instance.tryAssessment();


        Analitics.Instance.treckEvent (AnaliticsCategory.Profiles, "Select Profile_" + currentButton.ProfileId.ToString(), currentButton.ProfileId.ToString ());
	}







}
