﻿using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using Sturfee.Unity.XR.Core.Events;
using Sturfee.Unity.XR.Core.Session;
using Sturfee.Unity.XR.Package.Utilities;

// Checks for and loads save game data, and handles camera localization
public class GameManager : MonoBehaviour {

	public static GameManager Instance;

	[Header("UI")]
	public GameObject HasSaveDataPanel;		// Only appears if save data exists
	public GameObject ScanButton;
	public GameObject BackButton;
	public GameObject AlignmentInstrText;

	[Header("Player")]
	public PlayerUiController PlayerUi;

	[Header("AR Item Prefabs")]
	public GameObject Tier1ItemPrefab;
	public GameObject Tier2ItemPrefab;
	public GameObject Tier3ItemPrefab;

//	[Header("Other")]
	[HideInInspector]
	public bool HasSaveData;
	public static bool LoadGame;

	private bool _hasSaveData;
	private bool _loadGame = false;
	private bool _sessionReady = false;

	void Awake()
	{
		Instance = this;

//		SturfeeEventManager.Instance.OnSessionReady += OnSessionReady;
//		SturfeeEventManager.Instance.OnLocalizationComplete += OnLocalizationComplete;

		HasSaveDataPanel.SetActive (false);
//		ScanButton.SetActive (false);
		BackButton.SetActive (false);
//		AlignmentInstrText.SetActive (false);
	}

	void Start ()
	{
		_hasSaveData = SaveLoadManager.HasSaveData ();
		if (_hasSaveData)
		{
			HasSaveDataPanel.SetActive (true);
		}
		else
		{
			ScreenMessageController.Instance.SetText ("Initializing Session...");
		}
	}

	// TODO: Change this name
	public void OnStartScreenButtonClick(bool loadGame)
	{

		_loadGame = loadGame;
		LoadGame = loadGame;
		HasSaveDataPanel.SetActive (false);

		// TODO: Move this code below to AlignmentManager

		string scanButtonText;// = ScanButton.GetComponentInChildren<Text>().text;
		if (loadGame)
		{
			SaveLoadManager.Load ();
			scanButtonText = "Scan (Load Game)";
//			ScanButton.GetComponentInChildren<Text>().text = "Scan (Load Game)";
		}
		else
		{
			scanButtonText = "Scan (New Game)";
//			ScanButton.GetComponentInChildren<Text>().text = "Scan (New Game)";
		}

		AlignmentManager.Instance.SetScanButton (scanButtonText);

//		ScanButton.GetComponentInChildren<Text> ().text = scanButtonText;
//
//		if (_sessionReady)
//		{
//			BackButton.SetActive (true);
//			ScanButton.SetActive (true);
//		}
//		else
//		{
//			ScreenMessageController.Instance.SetText ("Initializing Session...");
//		}
	}

//	public void OnScanClick()
//	{
//		ScreenMessageController.Instance.SetText ("Aligning Camera...");
//		BackButton.SetActive (false);
//
//		MultiframeManager.Instance.OnScanButtonClick ();
////		AlignmentManager.Instance.Capture ();
//
////		AlignmentInstrText.SetActive (true);
////		XRSessionManager.GetSession ().PerformLocalization ();
//	}

	// Sturfee event called when the Sturfee XR Session is ready to be used
//	private void OnSessionReady()
//	{
//		ScreenMessageController.Instance.ClearText ();
//
//		_sessionReady = true;
//		if (!HasSaveDataPanel.activeSelf)
//		{
////			AlignmentManager.Instance.
//			ScanButton.SetActive (true);
//
//			if (_hasSaveData)
//			{
//				BackButton.SetActive (true);
//			}
//		}
//	}

//	// Sturfee event called when camera alignment completes
//	private void OnLocalizationComplete(Sturfee.Unity.XR.Core.Constants.Enums.AlignmentStatus status)
//	{
//		AlignmentInstrText.SetActive (false);
//		if (status == Sturfee.Unity.XR.Core.Constants.Enums.AlignmentStatus.Done)
//		{
////			SturfeeEventManager.Instance.OnSessionReady -= OnSessionReady;
//			SturfeeEventManager.Instance.OnLocalizationComplete -= OnLocalizationComplete;
//
//			ScreenMessageController.Instance.SetText ("Camera Alignment Complete", 3);
//		
//			// TODO: Temporary coroutine until next SDK fixes this issue (Current: v 0.9.1)
//			StartCoroutine(InitializeGame());
//
//			if (_loadGame)
//			{
//				SaveLoadManager.LoadGameData ();
//			}
//		}
//		else if (status == Sturfee.Unity.XR.Core.Constants.Enums.AlignmentStatus.IndoorsError)
//		{
//			ScreenMessageController.Instance.SetText ("Indoors Error. Please try again outside.", 3);
//			ResetToScanPanel ();
//		}
//		else if (status == Sturfee.Unity.XR.Core.Constants.Enums.AlignmentStatus.Error)
//		{
//			ScreenMessageController.Instance.SetText ("Alignment Failed", 3);
//			ResetToScanPanel ();
//		}
//	}

	private void ResetToScanPanel()
	{
		if (!_hasSaveData)
		{
			ScanButton.SetActive (true);
		}
		else
		{
			OnStartScreenButtonClick (_loadGame);
		}
	}

	private IEnumerator InitializeGame()
	{
		// Need to wait one frame for corrected GPS position to update for map calls
		yield return null;

		StreetMap.Instance.InitializeMap();
		PlayerUi.Initialize ();
	}

	// TEMP NAME, CHANGE LATER
	public void InitializeGame2()
	{
		if (LoadGame)
		{
			SaveLoadManager.LoadGameData ();
		}

		StreetMap.Instance.InitializeMap();
		PlayerUi.Initialize ();
	}

	public void ResetToStartScreen()
	{
		if (!_hasSaveData)
		{
			ScanButton.SetActive (true);
		}
		else
		{
			OnStartScreenButtonClick (LoadGame);
		}
	}
}