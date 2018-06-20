using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sturfee.Unity.XR.Core.Components.Interfaces;
using Sturfee.Unity.XR.Core.Events;
using Sturfee.Unity.XR.Providers.Components.Imu;

public class DebugImuProvider : ImuProviderBase {

    public Quaternion TestQuaternion = new Quaternion(-0.01262033f, 0.9623741f, 0.04538436f, 0.2676137f);
    public Vector3 TestPosition;

	private bool _moveOrientation = true;

	private void Awake()
	{
		SturfeeEventManager.Instance.OnLocalizationLoading += () => {_moveOrientation = false;};
	}

    private void Update()
    {
		if (_moveOrientation) 
		{

			float y = Mathf.PerlinNoise (Time.time * TestQuaternion.y, 0.0f);
			y *= Mathf.Rad2Deg;
			var newQuat = TestQuaternion;
			newQuat.eulerAngles = new Vector3 (TestQuaternion.eulerAngles.x, y, TestQuaternion.eulerAngles.z);
//        newQuat.y = y;

			TestQuaternion = Quaternion.Lerp (TestQuaternion, newQuat, Time.deltaTime);
		}
    }


    public override  Quaternion GetOrientation ()
    {
        return TestQuaternion;
    }

    public override  Vector3 GetOffsetPosition ()
    {
        return TestPosition;
    }

    public override  bool IsSupported()
    {
        return true;
    }

	public override void Destroy ()
	{
		
	}

}
