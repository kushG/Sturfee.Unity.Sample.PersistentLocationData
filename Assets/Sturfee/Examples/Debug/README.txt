Copyright (c) 2018, Sturfee Inc.

Sturfee StreetAR SDK
version 0.9.2.3 BETA

Scene : Debug

This scene is a representation of how an actual testing would look like without having to make a build on a Device. Notice the ProviderSet in SturfeeXRSession inspector is set to `Custom/DebugProvider Set`.

Note : This scene is useful in understanding the localization process. It removes the dependency on using device's sensors for IMU and GPS data

Device's back camera feed is replaced by a static image using `DebugVideoProvider`.
A debug GpS location is provided by `DebugGpsProvider` instead of reading GPS from a device.
Also, `DebugImuProvider` provides with a pseudo random orientation instead of depending on Device''s IMU sensors(Gyroscope).



SturfeeXrSession : Initializes XRSession using the the selected provider set in ProviderSet dropdown. 

AlignmentManager : Provides UI for localization calls and handles session events 

SturfeeXRCamera : Applies Pose obtained from XRSession to XRCamera. XRCamera GameObject should have the tag `XRCamera`

SturfeeXRLight : A GameObject with Light component which gets its intensity and color values from XRSession. SturfeeXRLight Gameobject should have the tag `XRLight`

Helpers :

    AccessHelper : Determines what is current access(tier) level for the current session
    ToggleBuildings : A Toggle to display/hide debug buildings
    ScreenOrientationHelper : Dynamically checks if the scrren orientation is changed or not
    ToastManager : Singleton helper class to show toast messages




