using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.iOS;
using Sturfee.Unity.XR.Core.Components.Interfaces;
using Sturfee.Unity.XR.Providers.Components.Imu;

public class ArKitImuProvider: ImuProviderBase
{

    private Vector3 _position;
    private Quaternion _orientation;

    private Vector3 _lastPosition = Vector3.zero;
    private Quaternion _lastOrientation = Quaternion.identity;
    private Quaternion _resetOffset = Quaternion.identity;

    private void Update()
    {
        Matrix4x4 matrix =  UnityARSessionNativeInterface.GetARSessionNativeInterface().GetCameraPose();

        _position = _lastPosition + UnityARMatrixOps.GetPosition(matrix);
        _orientation = _resetOffset * UnityARMatrixOps.GetRotation(matrix);
    }

    public override  Vector3 GetOffsetPosition()
    {
        return _position;
    }

    public override  Quaternion GetOrientation()
    {
        return _orientation;
    }

    public override  bool IsSupported()
    {
        return ProviderHelper.ArKitSupported;
    }

	public override void Destroy ()
	{
		UnityEngine.Object.Destroy(GameObject.Find("_OnAppFocusOrientationHelper"));
	}


    private void OnApplicationFocus(bool hasFocus)
    {
        if (hasFocus)
        {
            StartCoroutine(ResetOrientation());
        }
        else
        {
            _lastPosition = _position;
            _lastOrientation = _orientation;
        }
    }
        
    private IEnumerator ResetOrientation()
    {        

        Matrix4x4 matrix =  UnityARSessionNativeInterface.GetARSessionNativeInterface().GetCameraPose();

        var orientation = UnityARMatrixOps.GetRotation(matrix);
        while 
            (
                orientation == Quaternion.identity ||
                (
                   orientation.x == 0 && 
                   orientation.y == 0 && 
                   orientation.z == 0 && 
                   orientation.w == -1
                )
            )
        {            
            yield return null;
        }

        yield return new WaitForEndOfFrame();

        _orientation = _resetOffset * orientation;

        var orientationHelper = new GameObject();
        orientationHelper.name = "_OnAppFocusOrientationHelper";

        orientationHelper.transform.rotation = _lastOrientation;
        var oldForward = orientationHelper.transform.forward;

        orientationHelper.transform.rotation = _orientation;
        var newForward = orientationHelper.transform.forward;  

        var angle = Quaternion.Angle(_lastOrientation, _orientation);
        //        Debug.Log(" [IOSTrackingProvider] : Angle is : " + angle);

        if (Mathf.Abs(angle) <= 30)
        {
            _resetOffset = Quaternion.FromToRotation(oldForward, newForward);
        }
    }
}
