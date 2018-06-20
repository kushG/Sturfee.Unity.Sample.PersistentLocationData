
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SpatialTracking;
using Sturfee.Unity.XR.Core.Components.Interfaces;
using Sturfee.Unity.XR.Providers.Components.Imu;
#if UNITY_ANDROID
using GoogleARCore;
#endif

#if UNITY_ANDROID
[RequireComponent(typeof(TrackedPoseDriver))]
[RequireComponent(typeof(ARCoreSession))]
#endif
public class ArCoreImuProvider :  ImuProviderBase
{
    private Quaternion _headingOffset = Quaternion.identity;
    private Quaternion _orientation = Quaternion.identity;
    private Vector3 _position = Vector3.zero;

    private float _startTheta;
    private float _theta = 0;

    private float _trueHeading;

    private void Awake()
    {
        transform.position = Vector3.zero;
        transform.rotation = new Quaternion();
    }

    private void Start()
    {
        Input.compass.enabled = true;

        StartCoroutine(InitialPositionRotation());
    }

    private void Update()
    {
        _position = GetWorldPosition(transform.position);
        _orientation = GetWorldRotation(transform.rotation);
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
        return ProviderHelper.ArCoreSupported;
    }

	public override void Destroy ()
	{
		UnityEngine.Object.Destroy(GameObject.Find("_rotationHelper"));
	}

    private IEnumerator InitialPositionRotation()
    {
        while (transform.rotation == Quaternion.identity)
        {
            yield return null;
        }

        var helperObj = new GameObject();
        helperObj.gameObject.name = "_rotationHelper";

        helperObj.transform.rotation = transform.rotation;
        Debug.Log("HelperObj Rot : " + helperObj.transform.rotation.ToString("f6"));                                                                                                           
        var mappedOrientation = Quaternion.LookRotation(helperObj.transform.forward, helperObj.transform.up);

        Input.compensateSensors = true;
        var yaw = (Quaternion.AngleAxis(Input.compass.trueHeading, Vector3.up) * Vector3.forward);
        _headingOffset = Quaternion.Inverse(mappedOrientation) * Quaternion.LookRotation(yaw, Vector3.up);

        _startTheta = _headingOffset.eulerAngles.y;
        Debug.Log("Theta in start : " + _startTheta);
        Debug.Log("true Heading : " + Input.compass.trueHeading);
        _startTheta = -((Mathf.PI * _startTheta) / 180);

        _trueHeading = Input.compass.trueHeading;


    }

    private Vector3 GetWorldPosition(Vector3 relativePos)
    {
        Vector3 worldPos = new Vector3();

        worldPos.x = (relativePos.x * Mathf.Cos(_startTheta)) - (relativePos.z * Mathf.Sin(_startTheta));
        worldPos.y = relativePos.y;
        worldPos.z = (relativePos.z * Mathf.Cos(_startTheta)) + (relativePos.x * Mathf.Sin(_startTheta));

        return worldPos;
    }

    private Quaternion GetWorldRotation(Quaternion relative)
    {
        var euler = relative.eulerAngles;
        euler.y += _headingOffset.eulerAngles.y;

        return Quaternion.Euler(euler);
    }


    private void OnGUI()
    {

//        string guiText = "Position : " + transform.position+ "\n" +
//            "ROtation : " + transform.rotation + "\n" + 
//            "World Pos : " + _position + "\n" + 
//            "World Rot : " + _orientation + "\n" +
//            "True Heading : " + _trueHeading;
//
//        GUIStyle style = new GUIStyle();
//        style.fontSize = 40;
//
//        GUI.Label(new Rect(Screen.width - 500, Screen.height -  500, 400, 400), guiText, style);
    }
}
