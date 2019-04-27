#pragma warning disable 0162 //only meant to surpress the warning about unreachable code for erasing data -- TODO DELETE LATER
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;


//TODO: Clean this bad boy UP 
public class GameController : MonoBehaviour {
	public const int _levelCeiling = 10;				//max height allowed 
	public const bool _restartOnLevelCompletion = true;
	const bool ERASE_ALL_DATA_ON_START = false;
	public static bool DEBUG_MODE = true;
	
	public static GameController instance;			//the active instance of the game manager
	AudioManager audioManager;
	public static RoomManager roomManager;


	public PauseMenu pauseMenu;
	public OptionsMenu optionsMenu;
	public bool paused;

	public static readonly float xBound = 10f;
	public static readonly float yBound = 7.5f;
	public static readonly int screenWidth = 20;
	public static readonly int screenHeight = 15;

	

	// Use this for initialization
	void Awake() {
		if (instance == null) {
			instance = this;
			DontDestroyOnLoad(this.gameObject);
		}
		else if(instance != this) {
			Destroy(this.gameObject);
		}
		if (DEBUG_MODE && ERASE_ALL_DATA_ON_START) {
			LevelManager.EraseAllLevelData();
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
		if (paused) {
			Time.timeScale = 0;
		}
		else {
			Time.timeScale = 1;
		}
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
				ResetLevel();
			}

			if(DEBUG_MODE){
				if(Input.GetKeyDown(KeyCode.RightAlt)){
				//	ScreenCapture.CaptureScreenshot("C:/Users/edibl_000/Pictures/games/frog stack/GameCap/test.png");
					
					Debug.Log("Screen capture saved!");
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



	public void LockPlayerMovement(){
		
	}
	

	/********************* Level Management *********************/

	public void StartLevel(Scene scene, LoadSceneMode mode) {
		if(TransitionManager.GetScreenTransition() != null)
			TransitionManager.GetScreenTransition().StartTransitionIn();
		
	}

	public void ResetGame() {

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



