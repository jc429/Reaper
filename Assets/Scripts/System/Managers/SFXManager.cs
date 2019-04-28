using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SFXManager : MonoBehaviour
{
	[SerializeField]
	AudioSource _audioSource;
    // Start is called before the first frame update
    void Awake(){
    	GameController.sfxManager = this;
    }


	public static void PlayClip(AudioClip clip){
		GameController.sfxManager._audioSource.PlayOneShot(clip);
	}
}
