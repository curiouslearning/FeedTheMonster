using UnityEngine;
using System.Collections;

public class UIScreenTransitionEffect : MonoBehaviour {


    public bool mapSwap;

	// Use this for initialization
	void Start () {
        mapSwap = false;
	}

	// function start() { }
	// Update is called once per frame
	void Update () {
	}

    public void mapswapme()
    {
        mapSwap = true;
    }

	public void ChangeScreen(){
        if (!mapSwap)
        {
            UIController.Instance.Transition();
        }
        else
        {
            mapSwap = false;
        }
       

	}

	public void End(){
		gameObject.SetActive (false);
	}
}
