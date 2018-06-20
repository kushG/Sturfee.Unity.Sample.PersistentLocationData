using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sturfee.Unity.XR.Core.Session;
using Sturfee.Unity.XR.Core.Events;
using Sturfee.Unity.XR.Core.Utilities;
using Sturfee.Unity.XR.Package.Utilities;
using Sturfee.Unity.XR.Providers.Components.Gps;
using Sturfee.Unity.XR.Providers.Components.Imu;

namespace Sturfee.Unity.XR.Package.TestTools
{
	public class SessionDataRecorder : MonoBehaviour 
	{
        private GpsProviderBase _gpsProvider;
        private ImuProviderBase _imuProvider;

        public string FileSavePath;

		private string _fileName;
        public bool IsRecording { get { return _isRecording; } }
		private bool _isRecording;
		private string _jsonData;

		private List<Data> _sessionData;

        private bool _alignmentComplete;
        private bool _alignmentFailed;
        private bool _waitingForAlignment;

		private int _count;

		private void Start()
		{

            _imuProvider = FindObjectOfType<SturfeeXRSession>().GetComponent<ImuProviderBase>();
            _gpsProvider = FindObjectOfType<SturfeeXRSession>().GetComponent<GpsProviderBase>();

            // set the target framerate to 30 and the fixed timer to 30
            //Application.targetFrameRate = 30;
            //Time.fixedDeltaTime = 1.0f / 30.0f;

            Input.location.Start ();
			Input.gyro.enabled = true;

			if (string.IsNullOrEmpty (FileSavePath)) 
			{
				FileSavePath = Application.persistentDataPath;
			}

			if (!Directory.Exists(FileSavePath))
			{ 
				Directory.CreateDirectory(FileSavePath);
			}

            //Events
			SturfeeEventManager.Instance.OnLocalizationLoading += OnLocalizationLoading;
            SturfeeEventManager.Instance.OnLocalizationComplete += OnLocalizationComplete;

		}

        private void Update()
		{

			if (!_isRecording) 
			{
				return;
			}

			Data data = new Data ();

            //Gps
            data.gpsLocation = _gpsProvider.GetGPSPosition();

            //Imu
            data.worldOrientation = OrientationUtils.UnityToWorld(_imuProvider.GetOrientation());
            data.unityOrientation = _imuProvider.GetOrientation();
            data.imuOffsetPosition = _imuProvider.GetOffsetPosition();

            //XR session
			data.xrCameraPosition = XRSessionManager.GetSession ().GetXRCameraPosition ();
			data.xrCameraOrientation = XRSessionManager.GetSession ().GetXRCameraOrientation ();
            data.xrCameraLocation = XRSessionManager.GetSession().GetXRCameraLocation();
			
            //Offset corrections
			data.aiGpsLocation = XRSessionManager.GetSession ().GetLocationCorrection ();
			data.aiOffsetOrientation = XRSessionManager.GetSession ().GetOrientationCorrection ();

			data.timestamp = System.DateTime.Now.ToString("F");

            data.sessionEvents = new SessionEvents();
            data.sessionEvents.alignmentFinished = _alignmentComplete;
            data.sessionEvents.alignmentStarted = _waitingForAlignment;
            data.sessionEvents.waitingForAlignment = _waitingForAlignment;
            data.sessionEvents.alignmentFailed = _alignmentFailed;

            data.PreRenderData = false;

			if (_sessionData == null) 
			{
				Debug.LogError ("Session Data list is null");
				return;
			}

			_sessionData.Add (data);

		}

		public void StartRecording()
		{
			
			_fileName = TestRecordManager.FileName + ".txt";

			_isRecording = true;
			_sessionData = new List<Data> ();
		}

		public void StopRecording()
		{			
			_isRecording = false;

			StartCoroutine (Save ());
		}
            
		private IEnumerator Save()
		{
			_jsonData = JsonHelper.ToJson<Data> (_sessionData.ToArray (), true);
			File.AppendAllText(Path.Combine(FileSavePath, _fileName), _jsonData);

			yield return null;
		}		

        private void OnLocalizationLoading ()
        {            
            _waitingForAlignment = true;
        }

        private void OnLocalizationComplete (Sturfee.Unity.XR.Core.Constants.Enums.AlignmentStatus status)
        {
            if (status == Sturfee.Unity.XR.Core.Constants.Enums.AlignmentStatus.Done)
            {
                _alignmentComplete = true;
                ToastManager.Instance.ShowToastTimed("Localization Complete");
            }

			if (status == Sturfee.Unity.XR.Core.Constants.Enums.AlignmentStatus.Error)
			{
				_alignmentFailed = true;
				ToastManager.Instance.ShowToastTimed("Localization failed");
			}
        }



	}
}