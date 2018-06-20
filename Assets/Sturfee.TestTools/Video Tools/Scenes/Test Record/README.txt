Copyright (c) 2018, Sturfee Inc.

Sturfee StreetAR SDK
version 0.9.2.3 BETA

Scene : Test Record

This scene is used to record a footage while you are on test location. Once recorded this footage can be played in `Test Playback` scene for better analysis or to make localization requests from Unity editor on this footage instead of physically going to that location and testing

TestRecordManager : Records video provided by `VideoProvider`, GPS from `GPSProvider` and IMU from `ImuProvider` of SturfeeXRSession. 
                    Saves Video file at Application.PersistentDataPath>/files/<unique-folder-name>/<video-name>.mp4
                    Saves PoseData(GPS + IMU data) at ,Application.PersistentDataPath>/files/<unique-file-name>.txt     //NOte : <unique-file-name> here is same as <unique-folder-name> of video file

                    Example : Video : /storage/emulated/0/Android/data/com.sturfee.sdk/files/636640719621339310/session1.mp4
                              Pose Data : /storage/emulated/0/Android/data/com.sturfee.sdk/files/636640719621339310.txt

SturfeeXrSession : Initializes XRSession using the the selected provider set in ProviderSet dropdown. 

AlignmentManager : Provides UI for localization calls and handles session events 

SturfeeXRCamera : Applies Pose obtained from XRSession to XRCamera. XRCamera GameObject should have the tag `XRCamera`

SturfeeXRLight : A GameObject with Light component which gets its intensity and color values from XRSession. SturfeeXRLight Gameobject should have the tag `XRLight`

Helpers :

    AccessHelper : Determines what is current access(tier) level for the current session
    ToggleBuildings : A Toggle to display/hide debug buildings
    ScreenOrientationHelper : Dynamically checks if the scrren orientation is changed or not
    ToastManager : Singleton helper class to show toast messages


