using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
	const float xBound = 10f;
	const float yBound = 7.5f;
	const int screenWidth = 20;
	const int screenHeight = 15;

	public Transform targetPlayer;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void LateUpdate()
    {
        if(targetPlayer != null){
			UpdatePosition();
		}
    }

	void UpdatePosition(){
		Vector3 v = targetPlayer.position;
		Vector3 pos = transform.position;
		while(pos.x + xBound < v.x){
			pos.x += screenWidth;
		}
		while(pos.x - xBound > v.x){
			pos.x -= screenWidth;
		}
		while(pos.y + yBound < v.y){
			pos.y += screenHeight;
		}
		while(pos.y - yBound > v.y){
			pos.y -= screenHeight;
		}
		transform.position = pos;
	}
}
