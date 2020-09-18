using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FeedbackInstance : MonoBehaviour
{
    public Sprite text, outline;
    public Image textRenderer, outlineRenderer;

    [SerializeField]
    AudioClip[] sounds;
    public AudioClip fsound
    {
        get
        {
            return sounds[Random.Range(0, sounds.Length)];
        }
    }
    Color transc, fullc;
    float livetime;
    bool fadingin = true;


    // Start is called before the first frame update
    void Start()
    {
        transc = new Color(1, 1, 1, 0);
        fullc = Color.white;
        textRenderer.sprite = text;
        outlineRenderer.sprite = outline;
        
    }

    // Update is called once per frame
    void Update()
    {
        if (fadingin)
        {
            livetime += Time.deltaTime;
            outlineRenderer.color = Color.Lerp(transc, fullc, livetime);
            if (livetime > 1f)
            {
                livetime = 0f;
                fadingin = false;
            }
        }
        else
        {
            livetime += Time.deltaTime;
            outlineRenderer.color = Color.Lerp(fullc, transc, livetime);
            textRenderer.color = Color.Lerp(fullc, transc, livetime);
            if (livetime > 1f)
            {
                Destroy(gameObject);
            }
        }
        
    }
}
