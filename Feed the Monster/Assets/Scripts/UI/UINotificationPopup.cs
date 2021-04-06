using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UINotificationPopup : MonoBehaviour
{
    public delegate void DismissDelegate();
    public DismissDelegate onDismiss;
    public float timeout;
    private float startTime = 0f;
    public TextMeshProUGUI titleText;
    public TextMeshProUGUI bodyText;

    private void OnEnable()
    {
        startTime = Time.time;
    }
    public void updateText (string title, string body)
    {
        titleText.text = title;
        bodyText.text = body;
        Debug.Log("updated title text to " + titleText.text);
        Debug.Log("updated body text to " + bodyText.text);
    }
    public void Dismiss()
    {
        startTime = 0;
        UIController.Instance.ClosePopup(this.gameObject);
    }
    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            Dismiss();
        }
        if (startTime > 0)
        {
            if(Time.time - startTime >= timeout)
            {
                startTime = 0;
                Dismiss();
            }
        }
    }
}
