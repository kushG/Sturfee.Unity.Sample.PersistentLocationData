Copyright (c) 2018, Sturfee Inc.

Sturfee StreetAR SDK
version 0.9.2.3 BETA

Scene : Default Android

This scene represents how XRSession can be initialized for any Android device. Notice the ProviderSet in SturfeeXRSession inspector is set to `Default/Android Provider Set`.


SturfeeXrSession : Initializes XRSession using the the selected provider set in ProviderSet dropdown. 

AlignmentManager : Provides UI for localization calls and handles session events 

SturfeeXRCamera : Applies Pose obtained from XRSession to XRCamera. XRCamera GameObject should have the tag `XRCamera`

SturfeeXRLight : A GameObject with Light component which gets its intensity and color values from XRSession. SturfeeXRLight Gameobject should have the tag `XRLight`

Helpers :

    AccessHelper : Determines what is current access(tier) level for the current session
    ToggleBuildings : A Toggle to display/hide debug buildings
    ScreenOrientationHelper : Dynamically checks if the scrren orientation is changed or not
    ToastManager : Singleton helper class to show toast messages

        
