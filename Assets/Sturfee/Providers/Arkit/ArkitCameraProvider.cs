using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.iOS;
using Sturfee.Unity.XR.Core.Constants;
using Sturfee.Unity.XR.Core.Components.Interfaces;
using Sturfee.Unity.XR.Providers.Components.ArCamera;


public class ArkitCameraProvider : CameraProviderBase
{
    public Camera SturfeeXRCamera;

    private void Awake()
    {
        if (SturfeeXRCamera == null)
        {
            SturfeeXRCamera = GameObject.FindGameObjectWithTag("XRCamera").GetComponent<Camera>();

            if (SturfeeXRCamera == null)
            {
                SturfeeXRCamera = GameObject.Find("SturfeeXRCamera").GetComponent<Camera>();

                if (SturfeeXRCamera == null)
                {                       
                    Debug.LogError ("SturfeeXRCamera of SturfeeCameraProvider is Null. Camera from SturfeeXRCamera prefab is recommended");
                }
            }
        }

        if (SturfeeXRCamera.gameObject.GetComponent <UnityARCameraNearFar>() == null)
        {
            SturfeeXRCamera.gameObject.AddComponent<UnityARCameraNearFar>();
        }
    }

    private void Update()
    {
        SturfeeXRCamera.projectionMatrix = UnityARSessionNativeInterface.GetARSessionNativeInterface().GetCameraProjection();
    }

    public override  float GetDepth ()
    {
        return SturfeeXRCamera.depth;
    }

    public override  float GetFarClippingPlane ()
    {
        return SturfeeXRCamera.farClipPlane;
    }

    public override  float GetFOV ()
    {
        return SturfeeXRCamera.fieldOfView;
    }

    public override  float GetNearClippingPlane ()
    {
        return SturfeeXRCamera.nearClipPlane;
    }

    public override  Matrix4x4 GetProjectionMatrix ()
    {
        return SturfeeXRCamera.projectionMatrix;
    }

    public override  bool IsSupported()
    {
        return ProviderHelper.ArKitSupported;
    }

	public override void Destroy ()
	{
		
	}

}
