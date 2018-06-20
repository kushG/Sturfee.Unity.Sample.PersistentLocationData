using System.Collections;
using UnityEngine;
using Sturfee.Unity.XR.Core.Events;
using Sturfee.Unity.XR.Core.Session;
using Sturfee.Unity.XR.Core.Config.Session;
using Sturfee.Unity.XR.Providers;
using Sturfee.Unity.XR.Providers.Components.ArCamera;
using Sturfee.Unity.XR.Providers.Components.Imu;
using Sturfee.Unity.XR.Providers.Components.Gps;
using Sturfee.Unity.XR.Providers.Components.Video;
using Sturfee.Unity.XR.Providers.Components.Lighting;

[RequireComponent(typeof(ProviderHandler))]
public class SturfeeXRSession: MonoBehaviour
{
    [HideInInspector]
    public ProviderSet ProviderSet;
    [HideInInspector]
    public ImuProviderBase ImuProvider;
    [HideInInspector]
    public GpsProviderBase GpsProvider;
    [HideInInspector]
    public CameraProviderBase CameraProvider;
    [HideInInspector]
    public VideoProviderBase VideoProvider;
    [HideInInspector]
    public LightingProviderBase LightingProvider;

    [SerializeField][HideInInspector]
    private int _provTypeInt;
    [SerializeField][HideInInspector]
    public string ProviderSetName;

    public bool PlayOnStart = true;

	private bool _onSessionReady;

    private IEnumerator Start()
    {    
        yield return new WaitForEndOfFrame();

        #if STURFEE_ARCORE
        yield return new WaitUntil (()=> GoogleARCore.AndroidPermissionsManager.IsPermissionGranted("android.permission.CAMERA"));

        //Note : If ArCore Support is checked in Player settings, .apk build won't ask for any permissions
        yield return new WaitUntil (() => GoogleARCore.AndroidPermissionsManager.RequestPermission ("android.permission.ACCESS_FINE_LOCATION").Result.IsAllGranted);
        yield return new WaitUntil (() => GoogleARCore.AndroidPermissionsManager.RequestPermission ("android.permission.WRITE_EXTERNAL_STORAGE").Result.IsAllGranted);
        #endif

        if (PlayOnStart)
        {
            StartSession();
        }			      
    }

	private void OnDestroy()
	{
		XRSessionManager.GetSession ().DestroySession ();
	}

    public void StartSession()
    {        
        SetupDefaultProivders();

        var xrConfig = new XRSessionConfig();
		xrConfig.ImuProvider = ImuProvider;
		xrConfig.GpsProvider = GpsProvider;
		xrConfig.CameraProvider = CameraProvider;
		xrConfig.VideoProvider = VideoProvider;
		xrConfig.LightingProvider = LightingProvider;

        SturfeeEventManager.Instance.OnSessionReady += OnSessionReady;
		XRSessionManager.CreateSessionWithConfig (xrConfig);
    }

    private void OnSessionReady()
    {	
        Debug.Log("Session is ready");
		_onSessionReady = true;
	}
    		
	//NOTE: This needs Sturfee.Unity.XR>Providers.dll to work
    private void SetupDefaultProivders()
    {
        if (ImuProvider == null || !ImuProvider.IsSupported())
        {
            Debug.Log("ImuProvider is Null or not supported. Using Sturfee default ImuProvider");
			#if UNITY_ANDROID
            ImuProvider = gameObject.AddComponent<SturfeeAndroidImuProvider>();
			#elif UNITY_IOS
			ImuProvider = gameObject.AddComponent<SturfeeIOSImuProvider>();
			#endif
        }
        if (GpsProvider == null|| !GpsProvider.IsSupported())
        {
            Debug.Log("GpsProvider is Null or not supported. Using Sturfee default GpsProvider");
            GpsProvider = gameObject.AddComponent<SturfeeGpsProvider>();
        }
        if (VideoProvider == null|| !VideoProvider.IsSupported())
        {
            Debug.Log("VideoProvider is Null or not supported. Using Sturfee default VideoProvider");
            VideoProvider = gameObject.AddComponent<SturfeeVideoProvider>();
        }
        if (CameraProvider == null|| !CameraProvider.IsSupported())
        {
            Debug.Log("CameraProvider is Null or not supported. Using Sturfee default CameraProvider");
            CameraProvider = gameObject.AddComponent<SturfeeCameraProvider>();
        }
        if (LightingProvider == null|| !LightingProvider.IsSupported())
        {
            Debug.Log("LightingProvider is Null or not supported. Using Sturfee default LightingProvider");
            LightingProvider = gameObject.AddComponent<SturfeeLightingProvider>();
        }
    }
}
