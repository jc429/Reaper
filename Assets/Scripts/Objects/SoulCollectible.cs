using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoulCollectible : MonoBehaviour
{
	Rigidbody _rigidbody{
		get{ return GetComponent<Rigidbody>(); }
	}
	Vector3 launchVelocity = new Vector3(0,10f,0);
	public AudioClip collectSFX;

    // Start is called before the first frame update
    void Start()
    {
	
    }

    // Update is called once per frame
    void Update()
    {
		
    }

	void FixedUpdate () {
		//if (_rigidbody.velocity.y < 0f) {
			_rigidbody.AddForce(Physics.gravity * 2);
		//}
	}

	void OnTriggerEnter(Collider other){
		if(other.tag.Equals("Player")){
			Collect();
		}
	}

	void Collect(){
		SFXManager.PlayClip(collectSFX);
		SoulWallet.CollectSoul(this);
		gameObject.SetActive(false);
		//Destroy(this.gameObject);
	}

	public void SetLaunchVelocity(Vector3 vel){
		_rigidbody.velocity = vel;
	}

	public void SetLaunchVelocity(){
		_rigidbody.velocity = launchVelocity;
	}
}