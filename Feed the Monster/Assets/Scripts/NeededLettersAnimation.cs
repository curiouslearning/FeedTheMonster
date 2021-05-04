using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Text;

public class NeededLettersAnimation  {

    Image imTarget;
    Text mTarget;
	int mDefaultSize;
	int mPopSize;
	float mTimeStarted;
	float mLength;
	bool mRunning;
	int mLastIndex;
	float mLastTimeSine;

	MonsterCalloutController mController;

    public NeededLettersAnimation(Image target, MonsterCalloutController controller)
    {
        imTarget = target;
        mController = controller;
        //mDefaultSize = mTarget.fontSize;
        mPopSize = (int)(mDefaultSize * 1.25f);
    }

    public NeededLettersAnimation(Text target, MonsterCalloutController controller)
	{
		mTarget = target;
		mController = controller;
		mDefaultSize = mTarget.fontSize;
		mPopSize = (int)(mDefaultSize * 1.25f);
	}

	public void Show(float length)
	{
		mTimeStarted = Time.time;
		mLastTimeSine = Time.time;
		mLength = length;
		Timer.Instance.Create (new Timer.TimerCommand (Time.time, 0.01f, Update));
		mLastIndex = 0;
		mRunning = true;
	}

    void loadCharImage(string CharName)
    {
        string url;
        Sprite res;
        //CharName = CharName.Normalize(NormalizationForm.FormD);
        url = "charimg/" + CharName ;
        res = Resources.Load<Sprite>(url);
        imTarget.sprite = res;
        imTarget.enabled = true;
        if (res == null)
        {
            Debug.Log("Can't load character image: " + url);
        }
    }

    void Update()
	{
		float lerpRate = Mathf.Abs(Mathf.Sin((Time.time - mLastTimeSine) * 3f));

		if (mRunning) {
			SetSize ((int)((float)mDefaultSize * (1f - lerpRate) + (float)mPopSize * lerpRate));
			if (Time.time >= mTimeStarted + mLength) {
				SetSize (mDefaultSize);
				mRunning = false;
			}
		}
	}

	void SetSize(int sizeForNeededLetters)
	{
		string richTextForUI = "";
		string letter;
		int currentIndex;
		switch (GameplayController.Instance.CurrentLevel.monsterInputType) {
		case MonsterInputType.Letter:
//			letter = GameplayController.Instance.CurrentSegment.MonsterRequiredLetters [0];
			letter = GameplayController.Instance.CurrentSegment.GetFixRequiredLetters(0);
                //			letter = ArabicSupport.ArabicFixer.Fix(letter, true, true);
                //			letter = RTL.Fix(letter);
                if (!LangPackParser.IsImgRenderer)
                {
                    richTextForUI += StringWithColorTags(StringWithBoldTags(StringWithSizeTags(letter, sizeForNeededLetters)), mController.FontColorLetter);
                }
                else
                {
                    richTextForUI = letter;
                }
                    break;
		case MonsterInputType.LetterInWord:
			for (int i=0; i< GameplayController.Instance.CurrentSegment.MonsterAllLetters.Length; i++) { //THIS ARRAY RESPONSIBLE FOR RIGHT-TO-LEFT VS. LEFT-TO-RIGHT TEXT
				//letter = GameplayController.Instance.CurrentSegment.MonsterAllLetters [i];
				letter = GameplayController.Instance.CurrentSegment.GetFixAllLetters(i);

				if (letter == "X") {
					//letter = GameplayController.Instance.CurrentSegment.MonsterRequiredLetters [0];
					letter = GameplayController.Instance.CurrentSegment.GetFixRequiredLetters(0);
				}

//				letter = ArabicSupport.ArabicFixer.Fix(letter, true, true);
//				letter = RTL.Fix(letter);
				if (GameplayController.Instance.CurrentSegment.MonsterAllLetters [i] == "X") {
					richTextForUI += StringWithColorTags (StringWithBoldTags (StringWithSizeTags (letter, sizeForNeededLetters)), 							   mController.FontColorWordBold);
				} else {
					richTextForUI += StringWithColorTags (StringWithBoldTags (StringWithSizeTags (letter, mDefaultSize)), 									   mController.FontColorWordDefault);
				}
			}
			break;

		case MonsterInputType.Word:
			currentIndex = (int)(((Time.time - mTimeStarted) / mLength) * (float)GameplayController.Instance.CurrentSegment.MonsterRequiredLetters.Length);

			if (currentIndex > mLastIndex) {
				mLastTimeSine = Time.time;
			}

			for (int i = 0; i< GameplayController.Instance.CurrentSegment.MonsterRequiredLetters.Length; i++) { //THIS ARRAY RESPONSIBLE FOR RIGHT-TO-LEFT VS. LEFT-TO-RIGHT TEXT
//				letter = ArabicSupport.ArabicFixer.Fix(GameplayController.Instance.CurrentSegment.MonsterRequiredLetters [i], true, true);
				letter = GameplayController.Instance.CurrentSegment.GetFixRequiredLetters(i);
				//letter = RTL.Fix(GameplayController.Instance.CurrentSegment.MonsterRequiredLetters [i]);

				if (i == currentIndex) {
					richTextForUI += StringWithColorTags (StringWithBoldTags (StringWithSizeTags (letter, sizeForNeededLetters)), mController.FontColorWordBold);
				} else if (i <= mLastIndex) {
					richTextForUI += StringWithColorTags (StringWithBoldTags (StringWithSizeTags (letter, mDefaultSize)), mController.FontColorWordBold);
				} else if (i > mLastIndex) {
					richTextForUI += StringWithColorTags (StringWithSizeTags (letter, mDefaultSize), mController.FontColorWordDefault);
				}
			}
			mLastIndex = currentIndex;
			break;
		}
        if (LangPackParser.IsImgRenderer)
        {
            if (imTarget != null)
                loadCharImage(richTextForUI);
        }
        else
        {
            if (mTarget != null)
                mTarget.text = richTextForUI;
        }
		
	}

	string StringWithSizeTags(string str, int size)
	{
		return "<size="+size+">" + str + "</size>";
	}

	string StringWithBoldTags(string str)
	{
		return str;
//		return "<b>" + str + "</b>";
	}

	string StringWithColorTags(string str, Color color)
	{
		return "<color="+ColorToHexStr(color)+">" + str + "</color>";
	}

	string ColorToHexStr(Color color)
	{
		var _R = (byte)(color.r * 255f);
		var _G = (byte)(color.g * 255f);
		var _B = (byte)(color.b * 255f);

		return string.Format("#{0:X2}{1:X2}{2:X2}", _R, _G, _B);
	}


	int lastIndex = 0;
	public void UnMarkFirstLetter()
	{
		string richTextForUI = "";
		string letter;

		if(GameplayController.Instance.CurrentLevel.monsterInputType == MonsterInputType.Word) {
			for (int i = 0; i< GameplayController.Instance.CurrentSegment.MonsterRequiredLetters.Length; i++) { //THIS ARRAY RESPONSIBLE FOR RIGHT-TO-LEFT VS. LEFT-TO-RIGHT TEXT
				//letter = ArabicSupport.ArabicFixer.Fix (GameplayController.Instance.CurrentSegment.MonsterRequiredLetters [i], true, true);
				letter = RTL.Fix (GameplayController.Instance.CurrentSegment.MonsterRequiredLetters [i]);
				if (i <= lastIndex) {
					richTextForUI += StringWithColorTags (StringWithBoldTags (StringWithSizeTags (letter, mDefaultSize)), mController.FontColorWordDefault);
				} else {
					richTextForUI += StringWithColorTags (StringWithBoldTags (StringWithSizeTags (letter, mDefaultSize)), mController.FontColorWordBold);
				}
			}
			if (mTarget != null && !LangPackParser.IsImgRenderer) {
				mTarget.text = richTextForUI;
			}
			lastIndex++;
		}
	}
}
