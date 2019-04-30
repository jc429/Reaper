using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHitbox : MonoBehaviour
{
	public int damage = 1; 
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

	void OnTriggerEnter(Collider other){
	//	Debug.Log("SMASH");
		Destructible destructible = other.GetComponent<Destructible>();
		if(destructible != null){
			destructible.TakeDamage(damage);
		}
	}
}
