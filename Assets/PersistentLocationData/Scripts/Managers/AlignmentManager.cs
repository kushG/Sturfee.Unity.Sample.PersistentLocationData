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

	[Header("Multiframe UI")]
	public GameObject GazeTargetPrefab;  // TODO: Change name to 'Prefab'
	public GameObject Cursor;
	public GameObject Arrow;

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

//	[Header("UI")]
//	[HideInInspector]
	private bool _sessionReady = false;
	private bool _coveredArea = false;

	private int _angle = 50;
//	private int _targetCount = 3;

	private void Awake()
	{            
		Instance = this;

		SturfeeEventManager.Instance.OnSessionReady += OnSessionReady;
		SturfeeEventManager.Instance.OnSessionFailed += OnSessionFailed;
		SturfeeEventManager.Instance.OnCoverageCheckComplete += OnCoverageCheckComplete;
		SturfeeEventManager.Instance.OnLocalizationComplete += OnLocalizationComplete;
//		SturfeeEventManager.Instance.OnLocalizationLoading += OnLocalizationLoading;

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
//		SturfeeEventManager.Instance.OnLocalizationLoading -= OnLocalizationLoading;

	}

	public void SetScanButton(string scanButtonText)
	{
		ScanButton.GetComponentInChildren<Text> ().text = scanButtonText;

		if (_sessionReady && _coveredArea)
		{
			BackButton.SetActive (true);
			ScanButton.SetActive (true);
		}
		else
		{
			ScreenMessageController.Instance.SetText ("Initializing Session...");
		}
	}

	public void OnScanClick()
	{
		ScreenMessageController.Instance.SetText ("Aligning Camera...");
		BackButton.SetActive (false);

//		MultiframeManager.Instance.OnScanButtonClick ();

		_isScanning = true;

		print ("*** MULTIFRAME CALL START ***");

		StartCoroutine(MultiframeCallAsync());

		//		AlignmentManager.Instance.Capture ();

		//		AlignmentInstrText.SetActive (true);
		//		XRSessionManager.GetSession ().PerformLocalization ();
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
		while (_multiframeRequestId != _totalTargets)
		{
			yield return null;
		}

		print ("*** MULTIFRAME CALL COMPLETE ***");

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
//			Camera xrCamera = GameObject.FindGameObjectWithTag("XRCamera").GetComponent<Camera>();
			//Debug.DrawRay(xrCamera.transform.position, xrCamera.transform.forward * 1000, Color.green, 2000);


//			RaycastHit2D hit2D = Physics2D.Raycast(xrCamera.transform.position, xrCamera.transform.forward);
//			if (hit2D.transform.gameObject == target)
//			{                    
//				done = true;
//			}
//			else
//			{
//				yield return null;
//			}


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

		target.GetComponent<GazeTarget> ().TargetAlignedSuccess ();
//		Destroy(target);

		AddToMultiframeLocalizationCall();

	}

	private void AddToMultiframeLocalizationCall()
	{
		_multiframeRequestId++;

		//If reset is called, send -1 otherwise the count
		XRSessionManager.GetSession().PerformLocalization((_resetCalled) ? -1 : _totalTargets /*TargetCount*/);

		if (_multiframeRequestId > 1 && _multiframeRequestId < _totalTargets)
		{
			// Activate the next gaze target
			StartCoroutine( CheckGazeTargetHit(_gazeTargets[_multiframeRequestId - 1]) );
		}

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
			ScreenMessageController.Instance.ClearText ();

			_coveredArea = true;

			if (!GameManager.Instance.HasSaveDataPanel.activeSelf)
			{
				ScanButton.SetActive (true);

				if (GameManager.Instance.HasSaveData)
				{
					BackButton.SetActive (true);
				}
			}
		}
	}
	#endregion

	#region LOCALIZATION
	private void OnLocalizationComplete (Sturfee.Unity.XR.Core.Constants.Enums.AlignmentStatus status)
	{
		if (status == Sturfee.Unity.XR.Core.Constants.Enums.AlignmentStatus.Done)
		{
			ScreenMessageController.Instance.SetText ("Camera Alignment Complete", 3);
			GameManager.Instance.InitializeGame ();
		}
		else
		{
			// Error

			if (status == Sturfee.Unity.XR.Core.Constants.Enums.AlignmentStatus.OutOfCoverage)
			{
				ScreenMessageController.Instance.SetText("Localization not available at this location");
			}
			else if (status == Sturfee.Unity.XR.Core.Constants.Enums.AlignmentStatus.IndoorsError)
			{
				ScreenMessageController.Instance.SetText("Localization Failed: Indoors Error");
			}
			else if (status == Sturfee.Unity.XR.Core.Constants.Enums.AlignmentStatus.RequestError)
			{
				ScreenMessageController.Instance.SetText("Localization Failed: Request Error");
			}
			else if (status == Sturfee.Unity.XR.Core.Constants.Enums.AlignmentStatus.Error)
			{
				ScreenMessageController.Instance.SetText("Localization failed");
			}

			GameManager.Instance.ResetToStartScreen ();
		}

	}
	#endregion

}
