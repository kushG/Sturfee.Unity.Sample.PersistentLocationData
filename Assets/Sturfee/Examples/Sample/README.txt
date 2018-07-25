Copyright (c) 2018, Sturfee Inc.

Sturfee StreetAR SDK
version 1.0.0

Scene : Sample

This scene is a representation of how an actual testing would look like without having to make a build on a device. Notice the ProviderSet in SturfeeXRSession inspector is set to Custom/Sample Provider Set.

Note : This scene is useful in understanding the localization process. It removes the dependency on using device's sensors for IMU and GPS data and also the camera.

"Sample Provider set" is the provider set that provides with the providers that are used to simulate a real test field.

SampleVideoProvider : Simulates device's back camera by displaying a set of frames stored in an array. 
SampleImuProvider : Provides IMU orientation to XRCamera by reading orientation from data stored in a text file. This file consists of orientation mapped for each frame that is used by SampleVideoProvider 
DebugGpsProvider : Allows to set GPS values in inspector which will be provided to XRSession

SampleManager : Keeps frame array and the data(text) file. Also does the mapping between frames and their respective orientation in Data(text) file.

GameObjects :

SturfeeXrSession : Initializes XRSession using the the selected provider set in ProviderSet dropdown. AlignmentManager : Provides UI for localization calls and handles session events.
 Also provides a way to toggle between the type of scan, i.e., Singleframe scan or Multiframe scan. Check `UseMultiframeLocalization` to use the Multiframe scan.

SturfeeXRCamera : Applies Pose obtained from XRSession to XRCamera. XRCamera GameObject should have the tag XRCamera.

SturfeeXRLight : A GameObject with Light component which gets its intensity and color values from XRSession. SturfeeXRLight Gameobject should have the tag XRLight

Helpers :
AccessHelper : Determines what is current access(tier) level for the current session.
ToggleBuildings : A Toggle to display/hide debug buildings.
ScreenOrientationHelper : Dynamically checks if the scrren orientation is changed or not.
ToastManager : Singleton helper class to show toast messages.