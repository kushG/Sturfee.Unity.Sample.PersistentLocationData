using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Sturfee.Unity.XR.Core.Events;
using Sturfee.Unity.XR.Core.Session;
//using Sturfee.Unity.XR.Core.Constants;
//using Sturfee.Unity.XR.Package.Utilities;


// Customized Alignment script - adjusts the alignments circles or 'Gaze Targets' to be in one direction from the starting point, instead of to the left and right
// 	- Also adds an arrow that points directly towards the circle regardless of camera position
public class AlignmentManager : MonoBehaviour {

	public static AlignmentManager Instance;

	// TODO: finish incorporating 'CheckCoverageOnStart'
	// TODO: Possibly change these public variables to private

	[Header("Buttons")]
	public GameObject ScanButton;
	public GameObject BackButton;

	// TODO: Change these to private variables
	[Header("Multiframe UI")]
	public GameObject GazeTargetPrefab;  // TODO: Change name to 'Prefab'
	public GameObject Cursor;
	public GameObject Arrow;
	public GameObject ScanAnimation;
	public GameObject LookTrigger;

	[Header("Other")]
	public Camera XrCamera;

//	[Header("Coverage")]
//	public bool CheckCoverageOnStart; // TODO: probably remove this bool and just check

	private List<GameObject> _gazeTargets;
	private int _totalTargets = 3;  // TODO: Change name - user only sees 2 targets....
	private int _targetCount;
	private bool _isScanning; 
	private int _multiframeRequestId;
	private bool _resetCalled;
	private bool _midAlignmentError = false;

//	[Header("UI")]
//	[HideInInspector]
	private bool _sessionReady = false;
	private bool _coveredArea = false;
	private bool _lookedUp = false;

	private int _angle = 50;
//	private int _targetCount = 3;

	private void Awake()
	{            
		Instance = this;

		SturfeeEventManager.Instance.OnSessionReady += OnSessionReady;
		SturfeeEventManager.Instance.OnSessionFailed += OnSessionFailed;
		SturfeeEventManager.Instance.OnCoverageCheckComplete += OnCoverageCheckComplete;
		SturfeeEventManager.Instance.OnLocalizationComplete += OnLocalizationComplete;
		SturfeeEventManager.Instance.OnLocalizationLoading += OnLocalizationLoading;

		LookUpTrigger.OnUserLookedUp += HandleOnLookUpComplete;

		ScanButton.SetActive (false);
		BackButton.SetActive(false);
		Cursor.SetActive (false);
		Arrow.SetActive (false);
	}

	private void Start () {
		_targetCount = _totalTargets - 1;

	}

	private void OnDestroy()
	{
		// *IMPORTANT* Unregister event handlers for Sturfee events

		// XR SESSION
		SturfeeEventManager.Instance.OnSessionReady -= OnSessionReady;
		SturfeeEventManager.Instance.OnSessionFailed -= OnSessionFailed;

		// LOCALIZATION
		SturfeeEventManager.Instance.OnCoverageCheckComplete -= OnCoverageCheckComplete;
		SturfeeEventManager.Instance.OnLocalizationComplete -= OnLocalizationComplete;
		SturfeeEventManager.Instance.OnLocalizationLoading -= OnLocalizationLoading;

		LookUpTrigger.OnUserLookedUp -= HandleOnLookUpComplete;

	}

	public void SetScanButton(string scanButtonText)
	{

		ScanButton.GetComponentInChildren<Text> ().text = scanButtonText;

//		LookUpTrigger.OnUserLookedUp += HandleOnLookUpComplete;

		EnableScanButton ();
//		bool scanEnabled = EnableScanButton ();
//		if (!EnableScanButton () /*scanEnabled*/)
//		{
//			ScreenMessageController.Instance.SetText ("Initializing Session...");
//		}

//		if (_sessionReady && _coveredArea)
//		{
//			BackButton.SetActive (true);
//			ScanButton.SetActive (true);
//		}
//		else
//		{
//			ScreenMessageController.Instance.SetText ("Initializing Session...");
//		}
	}

	public void OnScanClick()
	{
		ScanButton.SetActive (false);
//		_midAlignmentError = false;

		ScreenMessageController.Instance.SetText ("Align center of screen with circles");
		BackButton.SetActive (true);

//		MultiframeManager.Instance.OnScanButtonClick ();

		_isScanning = true;

		print ("*** MULTIFRAME CALL START ***");

		StartCoroutine(MultiframeCallAsync());

		//		AlignmentManager.Instance.Capture ();

		//		AlignmentInstrText.SetActive (true);
		//		XRSessionManager.GetSession ().PerformLocalization ();
	}

	public void OnBackClick()
	{
		if (_isScanning)
		{
			_resetCalled = true;
			ScanButton.SetActive (true);
			if (!GameManager.Instance.HasSaveData)
			{
				BackButton.SetActive (false);
			}
			ScreenMessageController.Instance.ClearText ();
		}
		else
		{
			ScanButton.SetActive (false);
			BackButton.SetActive (false);
			GameManager.Instance.HasSaveDataPanel.SetActive (true);
		}
	}

	public List<GameObject> GetGazeTargets()
	{
		return _gazeTargets;
	}

	private IEnumerator MultiframeCallAsync()
	{
		print ("*** MULTIFRAME CALL ACTIVE ***");

		//Setup variables for starting multiframe requests
		SetupMultiframe();

		//Create gaze targets
		CreateGazeTargets();

		//wait till all the requests are complete
		while (_multiframeRequestId != _totalTargets /*&& !_error*/)
		{
			print ("*** MULTIFRAME CALL ASYNC WHILE LOOP ***");
			yield return null;
		}

		print ("*** MULTIFRAME CALL COMPLETE ***");

//		if (_error)
//		{
//			while(_gazeTargets.Count > 0)
//			{
//				_gazeTargets.RemoveAt (0);
//			}
//		}

		Cursor.SetActive(false);


		_isScanning = false;

		Arrow.SetActive (false);

	}

	private void SetupMultiframe()
	{
		_resetCalled = false;

		_gazeTargets = new List<GameObject>();

		Cursor.SetActive(true);

		_multiframeRequestId = 0;

		AddToMultiframeLocalizationCall();
	}

	private void CreateGazeTargets()
	{
		for (int i = 1; i <= _targetCount; i++)
		{
			int j;
			Vector3 dir;


			var gaze = Instantiate (GazeTargetPrefab);
			dir = Quaternion.AngleAxis (_angle * i, Vector3.up) * (XRSessionManager.GetSession ().GetXRCameraOrientation () * Vector3.forward);
			gaze.transform.position = XRSessionManager.GetSession ().GetXRCameraPosition () + (dir * 10.0f);
			gaze.transform.LookAt (XrCamera.transform /*GameObject.FindGameObjectWithTag ("XRCamera").transform*/);

			gaze.name = "Gaze target (" + (_angle * i) + ")";

			_gazeTargets.Add (gaze);

			gaze.SetActive (false);
		}

		Arrow.SetActive (true);
		StartCoroutine(CheckGazeTargetHit(_gazeTargets[0]));
	}



	private IEnumerator CheckGazeTargetHit(GameObject target)
	{
		_gazeTargets [_multiframeRequestId - 1].SetActive (true);
		Arrow.GetComponent<MultiframeArrow>().SetTarget (target.transform);

		RaycastHit hit;
		bool done = false;

		while (!done && !_resetCalled)
		{


			//TODO: Exclude building/terrain layers
			if (Physics.Raycast(XrCamera.transform.position, XrCamera.transform.forward, out hit, Mathf.Infinity))//, ~LayerMask.NameToLayer("Multiframe UI")))
			{
				if (hit.transform.gameObject == target)
				{                    
					done = true;
				}
				else
				{
					yield return null;
				}
			}
			else
			{
				yield return null;
			}
		}

		if (!_resetCalled)
		{
			target.GetComponent<GazeTarget> ().TargetAlignedSuccess ();
		}
		else
		{
			Destroy(target);
		}

		AddToMultiframeLocalizationCall();

	}

	private void AddToMultiframeLocalizationCall()
	{
//		if (!_error)
//		{
//		print ("ERROR: " + _error);

		_multiframeRequestId++;

		try
		{

			//If reset is called, send -1 otherwise the count
			XRSessionManager.GetSession ().PerformLocalization ((_resetCalled) ? -1 : _totalTargets /*TargetCount*/);
		}
		catch(Exception e)
		{
			Debug.Log ("In Exception");
//				ScanButton.SetActive (true);
		}

		print ("AFTER PERFORM LOCALIZATION CALL");

		if (/*!_error && !_resetCalled &&*/ _multiframeRequestId > 1 && _multiframeRequestId < _totalTargets)
		{
			print ("BEFORE CHECK GAZE TARGET HIT");

			// Activate the next gaze target
			StartCoroutine (CheckGazeTargetHit (_gazeTargets [_multiframeRequestId - 1]));

			print ("AFTER CHECK GAZE TARGET HIT");
		}
//		}

	}

	// Event called form LookUpTrigger
	private void HandleOnLookUpComplete()
	{
		print ("Look up Complete");
		_lookedUp = true;
		EnableScanButton ();
		GameManager.Instance.LookTrigger.SetActive (false);
	}

	#region XR SESSION
	private void OnSessionReady()
	{
		_sessionReady = true;
		XRSessionManager.GetSession().CheckCoverage();
	}

	private void OnSessionFailed (string error)
	{
		ScreenMessageController.Instance.SetText("Session Intialization failed : " + error);
	}

	private void OnCoverageCheckComplete (bool result)
	{
		if (result == false)
		{
			ScreenMessageController.Instance.SetText ("Localization not available at this location\nLocalization calls won't work", 5);
		}
		else
		{
			print ("Coverage Check Complete");

			ScreenMessageController.Instance.ClearText ();

			_coveredArea = true;


			if (!GameManager.Instance.HasSaveDataPanel.activeSelf)
			{
				EnableScanButton ();

//				ScanButton.SetActive (true);
//
//				if (GameManager.Instance.HasSaveData)
//				{
//					BackButton.SetActive (true);
//				}
			}
		}
	}
	#endregion

	#region LOCALIZATION
	private void OnLocalizationLoading()
	{
		ScreenMessageController.Instance.ClearText ();
		ScanAnimation.SetActive (true);
		BackButton.SetActive (false);
	}

	private void OnLocalizationComplete (Sturfee.Unity.XR.Core.Constants.Enums.AlignmentStatus status)
	{
		ScanAnimation.SetActive (false);

		if (status == Sturfee.Unity.XR.Core.Constants.Enums.AlignmentStatus.Done)
		{
			ScreenMessageController.Instance.SetText ("Camera Alignment Complete", 3);
			GameManager.Instance.InitializeGame ();
		}
		else
		{
			// Error
//			bool midAlignmentError = false;
//			_error = true;

			if (status == Sturfee.Unity.XR.Core.Constants.Enums.AlignmentStatus.OutOfCoverage)
			{
				ScreenMessageController.Instance.SetText("Localization not available at this location");
			}
			else if (status == Sturfee.Unity.XR.Core.Constants.Enums.AlignmentStatus.IndoorsError)
			{
				ScreenMessageController.Instance.SetText("Localization Failed: Indoors Error\nResetting Game");
				print ("INDOORS ERROR");
				_midAlignmentError = true;
//				ScanButton.SetActive (true);
			}
			else if (status == Sturfee.Unity.XR.Core.Constants.Enums.AlignmentStatus.RequestError)
			{
				ScreenMessageController.Instance.SetText("Localization Failed: Request Error\nResetting Game");
				print ("REQUEST ERROR");
				_midAlignmentError = true;
			}
			else if (status == Sturfee.Unity.XR.Core.Constants.Enums.AlignmentStatus.Error)
			{
				ScreenMessageController.Instance.SetText("Localization failed");
			}

//			if (midAlignmentError)
//			{
//
//			}
//			ScanButton.SetActive (true);

			// Required reset for current Sturfee SDK version. Plan to fix this problem in next release.
			if (_midAlignmentError)
			{
				GameManager.Instance.ResetGame (3);
			}
			else
			{
				print ("RESETTING TO START SCREEN");
				ResetToStartScreen ();
			}
		}

	}
	#endregion

	private void ResetToStartScreen()
	{
		if (GameManager.Instance.HasSaveData)
		{
			print ("HAS SAVE DATA");
			GameManager.Instance.OnSaveDataStartScreenClick (GameManager.Instance.LoadGame); // TODO: CHANGE THIS!!!
		}
		else
		{
			print ("TURN ON SCAN BUTTON");
			ScanButton.SetActive (true);  // TODO: What about back button???? -> EnableScanButton???
			print ("SCAN BUTTON ACTIVE");
		}
	}

	// Enables Scan button if all prerequisites are met
	private bool EnableScanButton()
	{
		bool enableScan = !ScanButton.activeSelf && _sessionReady && _coveredArea && _lookedUp;

		if (enableScan /*_sessionReady && _coveredArea && _lookedUp*/)
		{
			ScanButton.SetActive (true);

			// TODO: decide to turn on back button
			if (GameManager.Instance.HasSaveData)
			{
				BackButton.SetActive (true);
			}

//			return true;
		}
//		else
//		{
//			return false;
//		}
		return enableScan;
	}
}
