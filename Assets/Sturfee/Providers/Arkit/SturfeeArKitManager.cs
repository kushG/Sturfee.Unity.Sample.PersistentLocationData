using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.iOS;

public class SturfeeArKitManager : MonoBehaviour {
		
    private UnityARSessionNativeInterface _arKitSession;
    private ARKitWorldTrackingSessionConfiguration _config;


	private void Awake()
	{
		//Configure ArKit
        _arKitSession = UnityARSessionNativeInterface.GetARSessionNativeInterface();


        #if !UNITY_EDITOR
        // config
        Application.targetFrameRate = 60;

        _config = new ARKitWorldTrackingSessionConfiguration();

        _config.planeDetection = UnityARPlaneDetection.Horizontal;
        _config.alignment = UnityARAlignment.UnityARAlignmentGravity;
        _config.getPointCloudData = true;
        _config.enableLightEstimation = true;
        _config.enableAutoFocus = true;
        UnityARSessionRunOption tracking = UnityARSessionRunOption.ARSessionRunOptionResetTracking;
        _arKitSession.RunWithConfigAndOptions(_config, tracking);

        if(!_config.IsSupported)            
        {
            Debug.LogError("Tracking is not supported on this device");
            //Delete the VideoCamera. Default providers will create its own video camera
            Destroy(GetComponent<Camera>());        
        }
        else
        {
            ProviderHelper.ArKitSupported = true;
        }


        #else
        UnityARCamera scamera = new UnityARCamera ();
        scamera.worldTransform = new UnityARMatrix4x4 (new Vector4 (1, 0, 0, 0), new Vector4 (0, 1, 0, 0), new Vector4 (0, 0, 1, 0), new Vector4 (0, 0, 0, 1));
        Matrix4x4 projMat = Matrix4x4.Perspective (60.0f, 1.33f, 0.1f, 30.0f);
        scamera.projectionMatrix = new UnityARMatrix4x4 (projMat.GetColumn(0),projMat.GetColumn(1),projMat.GetColumn(2),projMat.GetColumn(3));

        UnityARSessionNativeInterface.SetStaticCamera (scamera);

        #endif



	}

}
