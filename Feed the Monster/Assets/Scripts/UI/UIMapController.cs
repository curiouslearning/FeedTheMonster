using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class UIMapController : MonoBehaviour {
//	public Text ScoreText;
	public Button btnCollection;
	public ScrollRect mapScroll;
    public Transform[] mapPages;
    public Button pageNext, pagePrevious;
    public int currentlyActivePage;

    int pagetoswap;
	bool isStatusPopupShow = false;

	void Awake() {

	}

	// Use this for initialization
	/*void Start () {
	
	}*/
	
	// Update is called once per frame
	void Update () {
        
	}

	void OnEnable()
	{
		GameplayController.Instance.ReaplaceBackground_SelectMonster = true;
      
		updatePosition ();

		//Invoke("ShowStatusPopup", 2f);

		Analitics.Instance.treckScreen ("Map");
	}


	void OnDisable() {
		TutorialController.Instance.EndTutorial ();
		CancelInvoke ();
	}

    public void SwapPageFwd()
    {
        SwapMapPage(currentlyActivePage + 1);
    }

    public void SwapPageBack()
    {
        SwapMapPage(currentlyActivePage - 1);
    }

    public void SwapMapPage()
    {
        SwapMapPage(1 - currentlyActivePage);
    }


    public void SwapMapPage(int pageNum)
    {

        UIController.Instance.ScreenTransitionEffect.SetActive(true);
        UIController.Instance.ScreenTransitionEffect.SendMessage("mapswapme");

        pageNum = Mathf.Max (0, pageNum);
        pageNum = Mathf.Min (mapPages.Length - 1, pageNum);
        pagetoswap = pageNum;

        Invoke("swapInternal", 1f);
        pagePrevious.interactable = false;
        pagePrevious.gameObject.SetActive(false);
        pageNext.interactable = false;
        pageNext.gameObject.SetActive(false);

    }

    void swapInternal()
    {
        mapPages[currentlyActivePage].gameObject.SetActive(false);
        currentlyActivePage = pagetoswap;
        mapPages[currentlyActivePage].gameObject.SetActive(true);

        if (currentlyActivePage == 0)
        {
            pagePrevious.interactable = false;
            pagePrevious.gameObject.SetActive(false);
        }
        else
        {
            pagePrevious.interactable = true;
            pagePrevious.gameObject.SetActive(true);
        }

        if (currentlyActivePage == mapPages.Length - 1)
        {
            pageNext.interactable = false;
            pageNext.gameObject.SetActive(false);
        }
        else
        {
            pageNext.interactable = true;
            pageNext.gameObject.SetActive(true);
        }
    }

	public void updatePosition()
	{
//		int sumAllLevelsScore = 0;
		int highestLevelOpenIndex;
		int lastPlayingLevelIndex;

		highestLevelOpenIndex = UserInfo.Instance.GetHighestOpenLevel ();
		lastPlayingLevelIndex = UserInfo.Instance.GetLastPlayingLevel ();
		TutorialController.Instance.EndTutorial (); //Jonathan??

        int snaptolevel = lastPlayingLevelIndex;

        pagetoswap = lastPlayingLevelIndex / 77;
        swapInternal();

		SnapTo (GameObject.Find ("Pin - Level " + lastPlayingLevelIndex).transform);

//		for (int i = highestLevelOpenIndex; i >= 0; i--) {
//			sumAllLevelsScore += UserInfo.Instance.GetLevelScore (i);
//		}

		if (highestLevelOpenIndex == 0) {
			var curPin = GameObject.Find ("Pin - Level 0").transform;
			TutorialController.Instance.PointAt (curPin.localPosition + new Vector3 (0, 30, 0), curPin.parent, true);
		}

//		ScoreText.text = sumAllLevelsScore.ToString ();
//		btnCollection.interactable = UserInfo.Instance.CollectionLength > 0;
	}


	private void SnapTo(Transform target)
	{
		Canvas.ForceUpdateCanvases();
		RectTransform contentPanel = mapScroll.content;

		var d = (target.localPosition.y - 200) / (contentPanel.sizeDelta.y - 400);
		d = Mathf.Min (d, 1);
		d = Mathf.Max (d, 0);
		mapScroll.normalizedPosition = new Vector2(0, d);

		/*contentPanel.anchoredPosition =
			(Vector2)mapScroll.transform.InverseTransformPoint(contentPanel.position)
			- (Vector2)mapScroll.transform.InverseTransformPoint(target.position);*/
	}

	void ShowStatusPopup()
	{
		if(!isStatusPopupShow)
		{
			Monster monster = MiniGameController.Instance.getEmotionMonster ();
			if (monster != null) {
				UIController.Instance.ShowPopup (UIController.Instance.MonsterStatusPopup);
				UIStatusController StatusController = UIController.Instance.MonsterStatusPopup.GetComponent<UIStatusController> ();

				StatusController.init (monster);

				isStatusPopupShow = true;
			}
		}
	}


	public void OnSELClick()
	{
		Analitics.Instance.treckEvent (AnaliticsCategory.Sel, AnaliticsAction.Open, "Map");

		UIController.Instance.ShowPanel (UIController.Instance.MiniGamePopup);
	}
}
