
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sturfee.Unity.XR.Core.Components.Interfaces;
using Sturfee.Unity.XR.Providers.Components.ArCamera;

public class ArCoreCameraProvider : CameraProviderBase
{

    public Camera SturfeeXRCamera;

	private void Start()
	{
		if (SturfeeXRCamera == null) 
		{
			SturfeeXRCamera = GameObject.FindGameObjectWithTag ("XRCamera").GetComponent<Camera>();
		}
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
        return ProviderHelper.ArCoreSupported;
    }
        
	public override void Destroy ()
	{
		
	}
}
