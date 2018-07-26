using System.Collections;
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
//	public GameObject ScanButton;
//	public GameObject BackButton;

	[Header("Player")]
	public PlayerUiController PlayerUi;

	[Header("AR Item Prefabs")]
	public GameObject Tier1ItemPrefab;
	public GameObject Tier2ItemPrefab;
	public GameObject Tier3ItemPrefab;

//	[Header("Other")]
	[HideInInspector]
	public bool HasSaveData;		// TODO: maybe remove this too, have AlignmentManager directly reference this
	public bool LoadGame;  	// TODO: Remove this

	private bool _hasSaveData;
	private bool _loadGame = false;
	private bool _sessionReady = false;

	void Awake()
	{
		Instance = this;

		HasSaveDataPanel.SetActive (false);
//		ScanButton.SetActive (false);
//		BackButton.SetActive (false);
	}

	void Start ()
	{
		_hasSaveData = SaveLoadManager.HasSaveData ();
		HasSaveData = _hasSaveData;
		if (_hasSaveData)
		{
			HasSaveDataPanel.SetActive (true);
		}
		else
		{
			ScreenMessageController.Instance.SetText ("Initializing Session...");
		}
	}
		
	public void OnSaveDataStartScreenClick(bool loadGame)
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
		}
		else
		{
			scanButtonText = "Scan (New Game)";
		}

		AlignmentManager.Instance.SetScanButton (scanButtonText);
	}
		
	public void InitializeGame()
	{
		if (LoadGame)
		{
			SaveLoadManager.LoadGameData ();
		}

		StreetMap.Instance.InitializeMap();
		PlayerUi.Initialize ();
	}

//	public void ResetToStartScreen()
//	{
//		if (!_hasSaveData)
//		{
//			ScanButton.SetActive (true);
//		}
//		else
//		{
//			OnSaveDataStartScreenClick (LoadGame);
//		}
//	}
}
