#pragma warning disable 0162 //only meant to surpress the warning about unreachable code for erasing data -- TODO DELETE LATER
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


//TODO: Clean this bad boy UP 
public class GameController : MonoBehaviour {
	public const int _levelCeiling = 10;				//max height allowed 
	public const bool _restartOnLevelCompletion = true;
	const bool ERASE_ALL_DATA_ON_START = false;
	public static bool DEBUG_MODE = false;
	
	public static GameController instance;			//the active instance of the game manager
	AudioManager audioManager;
	public static RoomManager roomManager;
	public static SFXManager sfxManager;


	public PauseMenu pauseMenu;
	public OptionsMenu optionsMenu;

	public static readonly float xBound = 10f;
	public static readonly float yBound = 7.5f;
	public static readonly int screenWidth = 20;
	public static readonly int screenHeight = 15;

	public PlayerMovement player;

	public GameObject poofPrefab;
	public GameObject hitSparkPrefab;
	public SoulCollectible soulPrefab;

	// Use this for initialization
	void Awake() {
		
		if (instance == null) {
			instance = this;
			DontDestroyOnLoad(this.gameObject);
		}
		else if(instance != this) {
			Destroy(this.gameObject);
		}
		audioManager = GetComponentInChildren<AudioManager>();

		LevelManager.LoadLevelData();
		SceneManager.sceneLoaded += StartLevel;

		CleanLevel();


 
	}

	void Start() {
	//	screenTransition = GameObject.FindGameObjectWithTag("EffectsCamera").GetComponent<ScreenTransition>();
	//	screenTransition.StartTransitionIn();
	//	Screen.SetResolution(288, 160, false);
	//	SpawnFrog();
	
	}
	
	// Update is called once per frame
	void Update () {
		if (TransitionManager.IsTransitioning()) {
			if (TransitionManager.GetScreenTransition().FadedOut()) {
				TransitionManager.GoToDestination();	
				
			}
		}
		else{	//if not transitioning 
			
			if (VirtualController.PauseButtonPressed()) {
				
				if (pauseMenu != null) {
					if (optionsMenu != null && optionsMenu.open) {
						optionsMenu.CloseOptionsMenu();
					}
					else {
						pauseMenu.Toggle();
					}
				}
			}
			if (VirtualController.ResetButtonPressed()) {
				ResetGame();
			}

			if(DEBUG_MODE){
				if(Input.GetKeyDown(KeyCode.RightAlt)){
					string time = System.DateTime.Now.ToString("yyyy'-'MM'-'dd'--'HH'-'mm'-'ss");
					string path = System.IO.Path.Combine(Application.persistentDataPath, "Pictures/screenshot " + time + ".png");
					ScreenCapture.CaptureScreenshot(path);
					Debug.Log("Screen capture saved! " + path);
				}

				if(Input.GetKeyDown(KeyCode.Keypad0)){
					RenderSettings.ToggleFullscreen();
				}
				if(Input.GetKeyDown(KeyCode.Keypad1)){
					RenderSettings.SetRenderScale(1);
				}
				if(Input.GetKeyDown(KeyCode.Keypad2)){
					RenderSettings.SetRenderScale(2);
				}
				if(Input.GetKeyDown(KeyCode.Keypad3)){
					RenderSettings.SetRenderScale(3);
				}
				if(Input.GetKeyDown(KeyCode.Keypad4)){
					RenderSettings.SetRenderScale(4);
				}
			}
		}
	}

	void LateUpdate(){
	
	}

	public static void FreezeGame(bool b = true){
		Time.timeScale = b ? 0 : 1;
	}

	public void LockPlayerMovement(bool b = true){
		player.controlsLocked = b;
	}
	

	/********************* Level Management *********************/

	public void StartLevel(Scene scene, LoadSceneMode mode) {
		if(TransitionManager.GetScreenTransition() != null)
			TransitionManager.GetScreenTransition().StartTransitionIn();
		
	}

	public void ResetGame() {
		player.Anim.SetPalette(0);
		SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
		SoulWallet.Reset();
	}

	public void CleanLevel(){
		
	}

	public void ResetLevel() {
		Debug.Log("Resetting");
		

		CleanLevel();
		TransitionManager.TransitionToLevel(SceneDest.SD_CurrentScene);
	}

	public void AdvanceLevel() {
		CompleteLevel();
		
		TransitionManager.TransitionToLevel(SceneDest.SD_NextScene);
		
	}

	void CompleteLevel() {
		Debug.Log("Level complete!");
		LevelManager.SetRecord(LevelManager.curWorld, LevelManager.curLevel, 0,0,false);
		LevelManager.CompleteCurrentLevel();
		LevelManager.UnlockNextLevel();
	}

	public void GoToMainMenu() {
		

		TransitionManager.TransitionToLevel(SceneDest.SD_MainMenu);

	}

	public void ClearAllData() {
		Debug.Log("clearing all data");
		LevelManager.EraseAllLevelData();
		LevelManager.LoadLevelData();
		ResetLevel();
	}

	public static AudioManager GetAudioManager(){
		return instance.audioManager;
	}

	
	public void HaltBGM() {
		audioManager.Stop();
	}

}



