//
//  Example.cs
//  PlayInstallReferrer
//
//  Created by Uglješa Erceg (@ugi) on 12th April 2020.
//  Copyright © 2020 Uglješa Erceg. All rights reserved.
//

using System;
using System.Text;
using System.Collections;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.UI;
using Ugi.PlayInstallReferrerPlugin;

public class Example : MonoBehaviour
{
    private string txtInstallReferrer;
    private string txtReferrerClickTimestamp;
    private string txtInstallBeginTimestamp;
    private string txtReferrerClickTimestampServer;
    private string txtInstallBeginTimestampServer;
    private string txtInstallVersion;
    private string txtGooglePlayInstant;

    private string txtInstallReferrerFromCallback;
    private string txtReferrerClickTimestampFromCallback;
    private string txtInstallBeginTimestampFromCallback;
    private string txtReferrerClickTimestampServerFromCallback;
    private string txtInstallBeginTimestampServerFromCallback;
    private string txtInstallVersionFromCallback;
    private string txtGooglePlayInstantFromCallback;

    void Awake()
    {
        AndroidJNIHelper.debug = true;

        txtInstallReferrer = "Install referrer: ";
        txtReferrerClickTimestamp = "Referrer click timestamp: ";
        txtInstallBeginTimestamp = "Install begin timestamp: ";
        txtReferrerClickTimestampServer = "Referrer click server timestamp: ";
        txtInstallBeginTimestampServer = "Install begin server timestamp: ";
        txtInstallVersion = "Install version: ";
        txtGooglePlayInstant = "Google Play instant: ";
    }

    void OnGUI()
    {
        var styleLabel = GUI.skin.GetStyle("Label");
        styleLabel.alignment = TextAnchor.MiddleCenter;
        styleLabel.fontSize = 50;

        GUI.Label(new Rect(0, Screen.height / 2 + 600, Screen.width, 100), txtGooglePlayInstantFromCallback, styleLabel);
        GUI.Label(new Rect(0, Screen.height / 2 + 500, Screen.width, 100), txtGooglePlayInstant, styleLabel);
        GUI.Label(new Rect(0, Screen.height / 2 + 400, Screen.width, 100), txtInstallVersionFromCallback, styleLabel);
        GUI.Label(new Rect(0, Screen.height / 2 + 300, Screen.width, 100), txtInstallVersion, styleLabel);
        GUI.Label(new Rect(0, Screen.height / 2 + 200, Screen.width, 100), txtInstallBeginTimestampServerFromCallback, styleLabel);
        GUI.Label(new Rect(0, Screen.height / 2 + 100, Screen.width, 100), txtInstallBeginTimestampServer, styleLabel);
        GUI.Label(new Rect(0, Screen.height / 2 + 0, Screen.width, 100), txtReferrerClickTimestampServerFromCallback, styleLabel);
        GUI.Label(new Rect(0, Screen.height / 2 - 100, Screen.width, 100), txtReferrerClickTimestampServer, styleLabel);
        GUI.Label(new Rect(0, Screen.height / 2 - 200, Screen.width, 100), txtInstallBeginTimestampFromCallback, styleLabel);
        GUI.Label(new Rect(0, Screen.height / 2 - 300, Screen.width, 100), txtInstallBeginTimestamp, styleLabel);
        GUI.Label(new Rect(0, Screen.height / 2 - 400, Screen.width, 100), txtReferrerClickTimestampFromCallback, styleLabel);
        GUI.Label(new Rect(0, Screen.height / 2 - 500, Screen.width, 100), txtReferrerClickTimestamp, styleLabel);
        GUI.Label(new Rect(0, Screen.height / 2 - 600, Screen.width, 100), txtInstallReferrerFromCallback, styleLabel);
        GUI.Label(new Rect(0, Screen.height / 2 - 700, Screen.width, 100), txtInstallReferrer, styleLabel);

        var styleButton = GUI.skin.GetStyle("Button");
        styleButton.alignment = TextAnchor.MiddleCenter;
        styleButton.fontSize = 50;

        if (GUI.Button(new Rect(100, Screen.height / 2 + 700, Screen.width - 200, 160), "Get install referrer details", styleButton))
        {
            
        }
    }
}
