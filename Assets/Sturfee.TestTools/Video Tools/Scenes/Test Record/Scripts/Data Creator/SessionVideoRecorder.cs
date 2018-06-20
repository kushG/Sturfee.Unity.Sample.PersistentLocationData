using UnityEngine;

namespace Sturfee.Unity.XR.Package.TestTools
{
    public class SessionVideoRecorder : MonoBehaviour {

		private EveryplayVideoRecorder _everyPlayVideoRecorder;

		private void Start()
		{
			_everyPlayVideoRecorder = GetComponent<EveryplayVideoRecorder> ();
		}	

		public void StartRecording()
		{
			_everyPlayVideoRecorder.RecordFileName = TestRecordManager.FileName;
			_everyPlayVideoRecorder.StartRecording ();
		}
			
		public void StopRecording()
		{
			_everyPlayVideoRecorder.StopRecording ();
		}


	}
}
