using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SFXManager : MonoBehaviour
{
	[SerializeField]
	AudioSource _audioSource;

	public AudioClip coinClip;
	public AudioClip clickClip;

    // Start is called before the first frame update
    void Awake(){
    	GameController.sfxManager = this;
    }


	public static void PlayClip(AudioClip clip){
		if(GameController.sfxManager._audioSource.isPlaying){
			GameController.sfxManager._audioSource.Stop();
		}
		GameController.sfxManager._audioSource.PlayOneShot(clip);
	}
	public static void PlayClickClip(){
		PlayClip(GameController.sfxManager.clickClip);
	}
	public static void PlayCoinClip(){
		PlayClip(GameController.sfxManager.coinClip);
		
	}
}
