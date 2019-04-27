using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoulCollectible : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
	
	void OnTriggerEnter(Collider other){
		if(other.tag.Equals("Player")){
			Collect();
		}
	}

	void Collect(){
		SoulWallet.CollectSoul(this);
		Destroy(this.gameObject);
	}
}