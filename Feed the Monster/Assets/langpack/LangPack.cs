using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New LangPack", menuName = "LangPack", order = 51)]
public class LangPack : ScriptableObject {
    public string LangCode = "FTMDefault";
    public string ApplicationName = "Feed The Monster";
    public int VersionNumber = 1;
    
    public bool ImageBasedRendering = false;
    public Font ReplacementFont = null;
    public int NumLevels = 77;
    public bool HasTracingGame = false;
    public Texture[] PositiveFeedbackTexts;


}
