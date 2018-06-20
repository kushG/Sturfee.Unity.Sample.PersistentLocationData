using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sturfee.Unity.XR.Providers.Components.Lighting;

public class DebugLightingProvider : LightingProviderBase {

    public override bool IsSupported()
    {
        return true;
    }

	public override Quaternion GetDirection ()
	{
		return Quaternion.LookRotation(Vector3.down);
	}

	public override float GetIntensity ()
	{
		return 1;
	}

	public override void Destroy ()
	{
		
	}
}
