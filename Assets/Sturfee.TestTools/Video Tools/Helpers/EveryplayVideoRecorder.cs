using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class EveryplayVideoRecorder : MonoBehaviour {
	[HideInInspector]
	public string RecordFileName;
	public string FileSavePath;

	public bool showUploadStatus = true;
	private bool isRecording = false;
	private bool isPaused = false;
	private bool isRecordingFinished = false;
	private GUIText uploadStatusLabel;

	private void Awake()
	{
		if (enabled && showUploadStatus)
		{
			CreateUploadStatusLabel();
		}

		DontDestroyOnLoad(gameObject);
	}

	private void Start()
	{
		if (uploadStatusLabel != null)
		{
			Everyplay.UploadDidStart += UploadDidStart;
			Everyplay.UploadDidProgress += UploadDidProgress;
			Everyplay.UploadDidComplete += UploadDidComplete;
		}

		Everyplay.RecordingStarted += RecordingStarted;
		Everyplay.RecordingStopped += RecordingStopped;
	}

	//  private void Update()
	//  {
	//      Debug.Log ("IS Paused : " + isPaused);
	//  }

	private void Destroy()
	{
		if (uploadStatusLabel != null)
		{
			Everyplay.UploadDidStart -= UploadDidStart;
			Everyplay.UploadDidProgress -= UploadDidProgress;
			Everyplay.UploadDidComplete -= UploadDidComplete;
		}

		Everyplay.RecordingStarted -= RecordingStarted;
		Everyplay.RecordingStopped -= RecordingStopped;
	}

	public void StartRecording()
	{
		if (!isRecording) 
		{
			Debug.Log ("[Everyplay] : Start Recording");
			Everyplay.StartRecording();
			#if UNITY_EDITOR
			Debug.Log ("The video recording is not available in the Unity editor. Please compile and run on a device.");
			#endif
		}
	}

	public void StopRecording()
	{
		if (isRecording)
		{
			Debug.Log ("[Everyplay] : Stop Recording");
			Everyplay.StopRecording();
			StartCoroutine (EveryplayLocalSaveAsync ());
			#if UNITY_EDITOR
			Debug.Log("The video recording is not available in the Unity editor. Please compile and run on a device.");
			#endif
		}
	}

	public void PauseRecording()
	{
		if (isRecording && !isPaused)
		{
			Debug.Log ("[Everyplay] : Pause Recording");
			Everyplay.PauseRecording();
			isPaused = true;
			#if UNITY_EDITOR
			Debug.Log("The video recording is not available in the Unity editor. Please compile and run on a device.");
			#endif
		}
	}

	public void ResumeRecording()
	{
		if (isRecording && isPaused)
		{
			Debug.Log ("[Everyplay] : Resume Recording");
			Everyplay.ResumeRecording();
			isPaused = false;
			#if UNITY_EDITOR
			Debug.Log("The video recording is not available in the Unity editor. Please compile and run on a device.");
			#endif
		}
	}

	public void SetTargetFPS(int fps)
	{       
		Everyplay.SetTargetFPS (fps);
	}

	private void RecordingStarted()
	{
		Debug.Log ("[EveryPlay] : Recording Started ");

		isRecording = true;
		isPaused = false;	
		isRecordingFinished = false;
	}

	private void RecordingStopped()
	{
		isRecording = false;
		isRecordingFinished = true;
	}

	private void CreateUploadStatusLabel()
	{
		GameObject uploadStatusLabelObj = new GameObject("UploadStatus", typeof(GUIText));

		if (uploadStatusLabelObj)
		{
			uploadStatusLabelObj.transform.parent = transform;
			uploadStatusLabel = uploadStatusLabelObj.GetComponent<GUIText>();

			if (uploadStatusLabel != null)
			{
				uploadStatusLabel.anchor = TextAnchor.LowerLeft;
				uploadStatusLabel.alignment = TextAlignment.Left;
				uploadStatusLabel.text = "Not uploading";
			}
		}
	}

	private void UploadDidStart(int videoId)
	{
		uploadStatusLabel.text = "Upload " + videoId + " started.";
	}

	private void UploadDidProgress(int videoId, float progress)
	{
		uploadStatusLabel.text = "Upload " + videoId + " is " + Mathf.RoundToInt((float) progress * 100) + "% completed.";
	}

	private void UploadDidComplete(int videoId)
	{
		uploadStatusLabel.text = "Upload " + videoId + " completed.";

		StartCoroutine(ResetUploadStatusAfterDelay(2.0f));
	}

	private IEnumerator ResetUploadStatusAfterDelay(float time)
	{
		yield return new WaitForSeconds(time);
		uploadStatusLabel.text = "Not uploading";
	}

	private IEnumerator EveryplayLocalSaveAsync()
	{
		// Only async on iOS, results immediately on Android
		yield return EveryplayLocalSave.SaveToAsync(Path.Combine((string.IsNullOrEmpty(FileSavePath) ? Application.persistentDataPath : FileSavePath) , RecordFileName));

		if (EveryplayLocalSave.SavedPath != null)
		{
			Debug.Log("Video saved to " + EveryplayLocalSave.SavedPath);
            Sturfee.Unity.XR.Package.Utilities.ToastManager.Instance.ShowToastTimed("Video Saved");
		}            
		else
		{
			Debug.LogWarning("Could not save the video; check logs!");
		}            
	}

	//  void OnGUI()
	//  {
	//      if (GUI.Button(new Rect(10, 10, 138, 48), "Everyplay"))
	//      {
	//          Everyplay.Show();
	//          #if UNITY_EDITOR
	//          Debug.Log("Everyplay view is not available in the Unity editor. Please compile and run on a device.");
	//          #endif
	//      }
	//
	//      if (isRecording && GUI.Button(new Rect(10, 64, 138, 48), "Stop Recording"))
	//      {
	//          Everyplay.StopRecording();
	//          #if UNITY_EDITOR
	//          Debug.Log("The video recording is not available in the Unity editor. Please compile and run on a device.");
	//          #endif
	//      }
	//      else if (!isRecording && GUI.Button(new Rect(10, 64, 138, 48), "Start Recording"))
	//      {
	//          Everyplay.StartRecording();
	//          #if UNITY_EDITOR
	//          Debug.Log("The video recording is not available in the Unity editor. Please compile and run on a device.");
	//          #endif
	//      }
	//
	//      if (isRecording)
	//      {
	//          if (!isPaused && GUI.Button(new Rect(10 + 150, 64, 138, 48), "Pause Recording"))
	//          {
	//              Everyplay.PauseRecording();
	//              isPaused = true;
	//              #if UNITY_EDITOR
	//              Debug.Log("The video recording is not available in the Unity editor. Please compile and run on a device.");
	//              #endif
	//          }
	//          else if (isPaused && GUI.Button(new Rect(10 + 150, 64, 138, 48), "Resume Recording"))
	//          {
	//              Everyplay.ResumeRecording();
	//              isPaused = false;
	//              #if UNITY_EDITOR
	//              Debug.Log("The video recording is not available in the Unity editor. Please compile and run on a device.");
	//              #endif
	//          }
	//      }
	//
	//      if (isRecordingFinished && GUI.Button(new Rect(10, 118, 138, 48), "Play Last Recording"))
	//      {
	//          Everyplay.PlayLastRecording();
	//          #if UNITY_EDITOR
	//          Debug.Log("The video playback is not available in the Unity editor. Please compile and run on a device.");
	//          #endif
	//      }
	//
	//      if (isRecording && GUI.Button(new Rect(10, 118, 138, 48), "Take Thumbnail"))
	//      {
	//          Everyplay.TakeThumbnail();
	//          #if UNITY_EDITOR
	//          Debug.Log("Everyplay take thumbnail is not available in the Unity editor. Please compile and run on a device.");
	//          #endif
	//      }
	//
	//      if (isRecordingFinished && GUI.Button(new Rect(10, 172, 138, 48), "Show sharing modal"))
	//      {
	//          Everyplay.ShowSharingModal();
	//          #if UNITY_EDITOR
	//          Debug.Log("The sharing modal is not available in the Unity editor. Please compile and run on a device.");
	//          #endif
	//      }
	//  }
}