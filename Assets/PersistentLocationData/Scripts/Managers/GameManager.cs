using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
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
	public GameObject LookTrigger;

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
//		LookTrigger.SetActive (true);

		_hasSaveData = SaveLoadManager.HasSaveData ();
		HasSaveData = _hasSaveData;
		if (_hasSaveData)
		{
			HasSaveDataPanel.SetActive (true);
		}
		else
		{
			LookTrigger.GetComponent<LookUpTrigger> ().IsEnabled = true;
//			LookTrigger.SetActive(true);
			ScreenMessageController.Instance.SetText ("Initializing Session...");
		}
	}
		
	// If save data exists, then pressing 'New Game' or 'Load Game' button will lead to this call
	public void OnSaveDataStartScreenClick(bool loadGame)
	{

		_loadGame = loadGame;
		LoadGame = loadGame;
		HasSaveDataPanel.SetActive (false);

		// TODO: Consider giving Alignment manager its own private vairable for if loading game or not, then move this code over there

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

		LookTrigger.GetComponent<LookUpTrigger> ().IsEnabled = true;
//		LookTrigger.SetActive(true);
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

	public void ResetGame(float timer = 0)
	{
		Invoke ("TimedGameReset", timer);
	}

	private void TimedGameReset(/*float time*/)
	{
//		yield return new WaitForSeconds (time);
		SaveLoadManager.Unload ();
		SceneManager.LoadScene ("Game");
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
