# Feed The Monster Core

This repository contains the code for the core Feed the Monster app unity app, including all language-independent gameplay code and assets. Feed the Monster is built in Unity 2017.4.19f1, and uses the Unity Firebase plugin version 6.3.0

# Applying a Language Pack

Language Pack data files, which contain the language-specific content, as well as a description of the structure and instructions for creating your own language pack, can be found at
https://github.com/curiouslearning/ftm-languagepacks/tree/langpacks

1. Download the folder for the language you wish to build to a directory on your computer outside the Feed The Monster core project.
1. Add your google-services.json file with your Firebase project information to the Assets directory of the Feed The Monster core project.
1. Open the Feed the Monster core project in a supported Unity version, with a selected build target of Android.
1. From the Unity menu toolbar item LangPacks, click Parse LangPack. This will open up a folder selection dialog window.
1. Select the location of the langpack folder you downloaded in step 1. This step might take a few minutes as the Unity asset database updates with the language-specific content.
1. Run the game in Unity, and verify that your language's audio and titles show up.
1. Build the app through the standard Unity build dialog.


# Eduapp4syria

Almost 2.5 million Syrian children are out of school because of conflict. Many have to cope with traumas and high levels of stress, which also affects their learning ability. High availability of smartphones among war-affected Syrian families can be a means for reaching children with engaging and fun learning supplements.

This can help facilitate their continued learning and future reintegration into school. Norway and several partners have therefore since January 2016 been conducting an international innovation competition to develop an open source smartphone application that can help Syrian children learn how to read in Arabic and improve their psychosocial wellbeing. The winning games will be released on Google Play and App Store in March 2017.([www.norad.no/eduapp4syria](https://www.norad.no/eduapp4syria)).
