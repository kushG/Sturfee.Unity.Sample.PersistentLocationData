using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sturfee.Unity.XR.Providers.Components.ArCamera;
using Sturfee.Unity.XR.Providers.Components.Imu;
using Sturfee.Unity.XR.Providers.Components.Gps;
using Sturfee.Unity.XR.Providers.Components.Video;
using Sturfee.Unity.XR.Providers.Components.Lighting;

public class ProviderSet : MonoBehaviour 
{
    public ImuProviderBase ImuProvider;
    public GpsProviderBase GpsProvider;
    public CameraProviderBase CameraProvider;
    public VideoProviderBase VideoProvider;
    public LightingProviderBase LightingProvider;
	
}
