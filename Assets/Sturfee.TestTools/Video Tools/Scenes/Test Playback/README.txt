Copyright (c) 2018, Sturfee Inc.

Sturfee StreetAR SDK
version 0.9.2.3 BETA

Scene : Test Playback

This scene is use to play back the video recorded using recording tool, i.e., using the build made out of `Test Record` scene. Notice the ProviderSet in SturfeeXRSession inspector is set to `Test Tools/Test Playback`.


TestPlaybackManager : 

    Configuration:
        Pose Data Asset         : Drag-drop the pose data recorded with Recording tool
        Video Clip              : Drag-drop the video recorded with Recording tool
        IsLooping               : Turn it ON if you want the video to be played in loop
        LocalizeWhilePlaying    ; If this is ON, you won't be able to make localization requests. This is useful if localization request was made during recording(using Recording tool) as 
                                  this will play the Video with localization results applied. If this is turned OFF, video will not have localization results applied even if localization was called 
                                  during recording.
                                  

SturfeeXrSession : Initializes XRSession using the the selected provider set in ProviderSet dropdown. 

AlignmentManager : Provides UI for localization calls and handles session events 

SturfeeXRCamera : Applies Pose obtained from XRSession to XRCamera. XRCamera GameObject should have the tag `XRCamera`


Helpers :

    AccessHelper : Determines what is current access(tier) level for the current session
    ToggleBuildings : A Toggle to display/hide debug buildings
    ScreenOrientationHelper : Dynamically checks if the scrren orientation is changed or not
    ToastManager : Singleton helper class to show toast messages


NOTE: PlayOnStart of SturfeeXRSession is not checked, i.e, it is OFF
