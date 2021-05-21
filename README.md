# Feed The Monster Core


This repository contains the code for the core Feed the Monster Unity app, including all language-independent gameplay code and assets. The following documentation will cover how to create and build your own localization of Feed The Monster.

For pedagogical tips on getting started or registering interest in localizing to a specific language, please [contact us](https://www.curiouslearning.org/contact).

**Development environment:**
Unity 2019.4.8f1

**Supplementary plug-ins:**
Unity Firebase SDK version 6.3.0

**Related repositories:**
1. [Language packs](https://github.com/curiouslearning/ftm-languagepacks)
1. [Automation scripts](https://github.com/curiouslearning/ftm-automation)


# Generating levels from a levelgen.csv file
The [Level Generation code](https://github.com/curiouslearning/ftm-automation/tree/master/level_generation) section of the automation scripts repository contains instructions on how to format a comma-separated value file defining the content to be taught on each level, and how to use the python script that generates the individual level xml files from this levelgen.

# Applying a Language Pack

Language Pack data files, which contain the language-specific content, as well as a description of the structure and instructions for creating your own language pack, can be found at
https://github.com/curiouslearning/ftm-languagepacks/tree/langpacks

1. Download the folder for the language you wish to build to a directory on your computer outside the Feed The Monster core project.
1. Add your google-services.json file with your Firebase project information to the Assets directory of the Feed The Monster core project. If you have not already created a google-services.json file for your group of languages, see the [following section](#data-flow-from-app-events-to-storage).
1. Open the Feed the Monster core project in a supported Unity version, with a selected build target of Android.
1. From the Unity menu toolbar item LangPacks, click Parse LangPack. This will open up a folder selection dialog window.
1. Select the location of the langpack folder you downloaded in step 1. This step might take a few minutes as the Unity asset database updates with the language-specific content.
1. Run the game in Unity, and verify that your language's audio and titles show up.
1. Build the app through the standard Unity build dialog.


# Unity TextMeshPro bug workaround
Depending on the version of TextMeshPro included with Unity, you may get the following error when opening the project for the first time:

`Library\PackageCache\com.unity.textmeshpro@2.0.1\Scripts\Editor\TMP_PackageUtilities.cs(310,17): error CS0433: The type 'Task' exists in both 'Unity.Tasks, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null' and 'mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089'`
  
If this bug occurs, edit the *Unity.TextMeshPro.Editor.asmdef* file found in the directory `FeedTheMonster\Library\PackageCache\com.unity.textmeshpro@2.0.1\Scripts\Editor\` to include the line 

`"overrideReferences": true,`

A more detailed explanation of the bug and workaround can be found [here](https://stackoverflow.com/questions/56422962/unity3d-the-type-task-exist-in-both-unity-tasks-and-mscorelib/56430847).

# Data flow from app events to storage

To improve the Feed The Monster experience over time, the app has been outfitted with the Google Firebase SDK. With this SDK, we collect the following out-of-the-box [demographic and device data](https://support.google.com/firebase/answer/6317486?hl=en) and [event information](https://support.google.com/firebase/answer/6317485?hl=en).

In addition to this standard Firebase collected data, there is also code written to collect “Gameplay” event data. This “Gameplay” event data will give information on things like: whether a learner is correct or not in answering a segment of a level, what letter or letter combinations were presented for matching for a particular segment, whether a learner completes a level or not and how many stars they achieved, etc. This data on aggregate and over time, should be utilized in tweaking the levelgen file (are there levels that create bottlenecks where users are getting stuck because they need more practice? Are there areas that are too easy? Etc.)

If additional data collection would be helpful for further study or improvement of Feed The Monster, feel free to supplement with additional Firebase event collection functions.

To create your own long-term storage for Feed The Monster data, first create a new [Firebase project](https://firebase.google.com/). When creating the first app, select that it is a Unity app. Fill out all fields including what the app package name will be when posted to the Google Play Store.

It is highly recommended if there will be multiple Feed The Monster apps created that they all funnel to the same long-term storage. Each Firebase project can hold up to 25 apps.

After the Firebase project is created, make sure to download the google-services.json file and add it to the Assets directory in Unity per the steps outlined above.

Finally, in the Firebase project you have just created, click on the "Events" section and click on the link at the bottom to link your Firebase project to Google BigQuery. This allows all Firebase out-of-the-box events and Gameplay events to be stored long-term in a BigQuery datalake for further data analysis.

Before launching the app to the Play Store, also make sure to lockdown your Firebase key.

# Updating the Firebase SDK

// To Do -- tips on updating the Firebase SDK


# Posting to the Google Play Store

If you would like to: receive past data on other Feed The Monster localizations, receive a template Data Studio visualization for easily analyzing Feed The Monster data, receive helpful distribution tips in getting Feed The Monster out to those most in need, contribute your localization's data to existing Feed The Monster data, or contribute your localization for inclusion in a network of literacy learning applications, please [contact us](https://www.curiouslearning.org/contact).


# Eduapp4syria

Almost 2.5 million Syrian children are out of school because of conflict. Many have to cope with traumas and high levels of stress, which also affects their learning ability. High availability of smartphones among war-affected Syrian families can be a means for reaching children with engaging and fun learning supplements.

This can help facilitate their continued learning and future reintegration into school. Norway and several partners have therefore since January 2016 been conducting an international innovation competition to develop an open source smartphone application that can help Syrian children learn how to read in Arabic and improve their psychosocial wellbeing. The winning games will be released on Google Play and App Store in March 2017.([www.norad.no/eduapp4syria](https://www.norad.no/eduapp4syria)).
