using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
	

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
		while(pos.x + GameController.xBound < v.x){
			pos.x += GameController.screenWidth;
		}
		while(pos.x - GameController.xBound > v.x){
			pos.x -= GameController.screenWidth;
		}
		while(pos.y + GameController.yBound < v.y){
			pos.y += GameController.screenHeight;
		}
		while(pos.y - GameController.yBound > v.y){
			pos.y -= GameController.screenHeight;
		}
		transform.position = pos;
	}
}
