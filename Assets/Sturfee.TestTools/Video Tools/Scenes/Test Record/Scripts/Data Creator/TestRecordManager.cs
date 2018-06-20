using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sturfee.Unity.XR.Core.Events;
using Sturfee.Unity.XR.Package.Utilities;

using GoogleARCore;


namespace Sturfee.Unity.XR.Package.TestTools
{
	[RequireComponent(typeof(SessionVideoRecorder))]
	[RequireComponent(typeof(SessionDataRecorder))]
	public class TestRecordManager : MonoBehaviour 
	{

		public static string FileName;	//TODO: this is temporary

		public Canvas LandscapeCanvas;
		public Canvas PortraitCanvas;

		private bool _isRecording = false;

		private SessionVideoRecorder _videoRecorder;
		private SessionDataRecorder _dataRecorder;

		private bool _sessionIsReady;

		private void Awake()
		{			
			SturfeeEventManager.Instance.OnSessionReady += OnSessionReady;
		}

		private IEnumerator Start()
		{
			_videoRecorder = GetComponent<SessionVideoRecorder> ();
			_dataRecorder = GetComponent<SessionDataRecorder> ();

			#if STURFEE_ARCORE
			//Note : If ArCore Support is checked in Player settings, .apk build won't ask for any permissions
			yield return new WaitUntil (() => AndroidPermissionsManager.IsPermissionGranted ("android.permission.CAMERA"));
			yield return new WaitUntil (() => AndroidPermissionsManager.IsPermissionGranted ("android.permission.ACCESS_FINE_LOCATION"));
			yield return new WaitUntil (() => AndroidPermissionsManager.IsPermissionGranted ("android.permission.WRITE_EXTERNAL_STORAGE"));

			yield return new WaitUntil (() => AndroidPermissionsManager.RequestPermission ("android.permission.RECORD_AUDIO").IsComplete);
			#endif

			yield return null;
		}

		private void Update()
		{
			if (!_sessionIsReady)
			{
				return;
			}

			if (_isRecording) 
			{           
				Screen.orientation = ScreenOrientationHelper.Instance.LastOrientation;
			}   

			//if (ScreenOrientationHelper.Instance.OrientationChanged || (!LandscapeCanvas.isActiveAndEnabled && !PortraitCanvas.isActiveAndEnabled)) 
			//{
			//	UpdateOrientation ();
			//}
		}

		public void StartRecording()
		{
			FileName = System.DateTime.Now.Ticks.ToString ();
			_videoRecorder.StartRecording ();
			_dataRecorder.StartRecording ();
		}

		public void StopRecording()
		{
			_videoRecorder.StopRecording ();
			_dataRecorder.StopRecording ();
		}
			
        public void AlignButton()
        {
            Sturfee.Unity.XR.Core.Session.XRSessionManager.GetSession().PerformLocalization();
            ToastManager.Instance.ShowToast("Localizing scene....please wait");   
        }

		private void OnSessionReady()
		{
			_sessionIsReady = true;
            LandscapeCanvas.gameObject.SetActive(true);
			//UpdateOrientation();
		}

		private void UpdateOrientation()
		{
			if (Screen.orientation == ScreenOrientation.Landscape)
			{
				LandscapeCanvas.gameObject.SetActive(true);
				PortraitCanvas.gameObject.SetActive(false);
			}
			else if (Screen.orientation == ScreenOrientation.Portrait)
			{
				LandscapeCanvas.gameObject.SetActive(false);
				PortraitCanvas.gameObject.SetActive(true);
			}
		}
	}
}