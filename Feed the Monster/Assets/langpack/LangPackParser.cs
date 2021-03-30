using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using System;

public class LangPackParser : MonoBehaviour {

    public  LangPack curlp;
    public UIController uicontroller;
    public  GameplayController gameplaycontroller;
    string myloc, fromloc, langpackdir;
    static LangPackParser s_Instance;

    // A static property that finds or creates an instance of the manager object and returns it.
    public static LangPackParser instance
    {
        get
        {
            if (s_Instance == null)
            {
                // FindObjectOfType() returns the first AManager object in the scene.
                s_Instance = FindObjectOfType(typeof(LangPackParser)) as LangPackParser;
                s_Instance.gameplaycontroller = s_Instance.GetComponent<GameplayController>();
                s_Instance.uicontroller = s_Instance.GetComponent<UIController>();
            }

            // If it is still null, create a new instance
            if (s_Instance == null)
            {
                Debug.Log("missing langpack parser script in scene");
            }

            return s_Instance;
        }
    }



    public static bool IsImgRenderer
    {
        get
        {
            return instance.curlp.ImageBasedRendering;
        }
    }

    public static bool HasTracingGame
    {
        get
        {
            return instance.curlp.HasTracingGame;
        }
    }


#if UNITY_EDITOR
    [MenuItem("LangPacks/Parse LangPack")]
    static void ParseTest()
    {
        string path = EditorUtility.OpenFolderPanel("Select LangPack Folder", "", "");
        Debug.Log("path: " + path);
        instance.doParse(path);


    }

     void doParse(string lppath)
    {
        
        langpackdir = lppath;
        parseInternal();
       

    }

    
    void parseInternal() {

        //parse from settings file
        try
        {
            string path = langpackdir+"/settings.json";
        StreamReader reader = new StreamReader(path);
        string jsontext = reader.ReadToEnd();

        curlp = JsonUtility.FromJson<LangPack>(jsontext);

        LangPack nlp = curlp;

        UnityEditor.PlayerSettings.productName = nlp.ApplicationName;
        
        myloc = Application.dataPath;
        fromloc = langpackdir + "/";

        

        Debug.Log("parsing " + nlp.ApplicationName +  " from " + fromloc + " into " + myloc);
        gameplaycontroller.NumOfLevels = nlp.NumLevels;

        //deal with 77 levels
        if (nlp.NumLevels <= 77)
        {
            uicontroller.mapController.RealPages = uicontroller.mapController.singlepage;
        }
        //deal with larger
        else
        {
            uicontroller.mapController.RealPages = uicontroller.mapController.doublepages;
        }

        string resourceloc = myloc + "/Resources/";

        string chardir = "charimg/";
      


        
        string charimgloc = resourceloc + chardir;
        string charimgfrom = fromloc + chardir;


        //charimgs
        foreach (string sfile in System.IO.Directory.GetFiles(charimgloc))
        {
            FileUtil.DeleteFileOrDirectory(sfile);
        }

            if (nlp.ImageBasedRendering)
        {
            FileUtil.ReplaceDirectory(charimgfrom, charimgloc);
        }


        //levels
        string levelsdir = "levels/";
        string levelfrom = fromloc + levelsdir;
        string levelto = resourceloc + "Gameplay/" +  levelsdir;
        FileUtil.ReplaceDirectory(levelfrom, levelto);


        //resource sounds

        string soundsfrom = fromloc + "sounds/";

        string soundsprojdir = resourceloc + "Sounds/Voice/";

        FileUtil.ReplaceDirectory(soundsfrom + "letters/", soundsprojdir + "Letters/");
        FileUtil.ReplaceDirectory(soundsfrom + "words/", soundsprojdir + "Words/");
        FileUtil.ReplaceDirectory(soundsfrom + "feedbacks/", soundsprojdir + "Feedbacks/Positive/");


        //referenced sounds

        foreach (string sfile in System.IO.Directory.GetFiles(soundsfrom + "other/"))
        {
            FileUtil.ReplaceFile(sfile, soundsprojdir + "Instructions/" + Path.GetFileName(sfile));
           
        }

        //referenced arts   
        string fbartsfrom = fromloc + "art/feedbacks/";

        foreach (string sfile in System.IO.Directory.GetFiles(fbartsfrom))
        {
            FileUtil.ReplaceFile(sfile, myloc + "/Art/feedbacks/" + Path.GetFileName(sfile));

        }
        string titlearts = fromloc + "art/titles/";

        foreach (string sfile in System.IO.Directory.GetFiles(titlearts))
        {
            FileUtil.ReplaceFile(sfile, myloc + "/Art/titles/" + Path.GetFileName(sfile));

        }
        string memfloc = fromloc + "art/memg/";

        foreach (string sfile in System.IO.Directory.GetFiles(memfloc))
        {
            FileUtil.ReplaceFile(sfile, myloc + "/Art/MemoryGame/used/" + Path.GetFileName(sfile));

        }



     


        //deal with version number and version code

        PlayerSettings.bundleVersion = nlp.VersionNumber.ToString();
        PlayerSettings.Android.bundleVersionCode = nlp.VersionNumber;

        //edit gradle script here


        //refresh assets
        AssetDatabase.Refresh();


        }
        catch (Exception e)
        {
            // Let the user know what went wrong.
            Debug.Log("The langpack could not be read:");
            Debug.Log(e.Message);
            return;
        }

    }
#endif


}


