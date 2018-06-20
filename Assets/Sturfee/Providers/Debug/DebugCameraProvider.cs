using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sturfee.Unity.XR.Core.Events;
using Sturfee.Unity.XR.Core.Session;
using Sturfee.Unity.XR.Core.Components.Interfaces;
using Sturfee.Unity.XR.Providers.Components.ArCamera;

public class DebugCameraProvider : CameraProviderBase 
{
    public float Depth = -1;
    public float FarClippingPlane = 2000;
    public float NearClippingPlane = 0.01f;
    public float FOV = 40;
    public Matrix4x4 ProjectionMatrix = new Matrix4x4(
        new Vector4(0.6562428f, 0, 0, 0),
        new Vector4(0, 1.732051f, 0, 0),
        new Vector4(0, 0, -1.0006f, -1),
        new Vector4(0, 0, -0.60018f, 0)
    );

    private void Awake()
    {
        SturfeeEventManager.Instance.OnSessionReady += OnSessionReady;
    }

    private void OnSessionReady ()
    {
//        XRSessionManager.GetSession().PerformAlignment();
    }

    public override  float GetDepth ()
    {
        return Depth;
    }

    public override  float GetFarClippingPlane ()
    {
        return FarClippingPlane;
    }

    public override  float GetFOV ()
    {
        return FOV;
    }

    public override  float GetNearClippingPlane ()
    {
        return NearClippingPlane;
    }


    public override  Matrix4x4 GetProjectionMatrix ()
    {
        return ProjectionMatrix;
    }


    public override  bool IsSupported()
    {
        return true;
    }

	public override void Destroy ()
	{
		
	}
}
