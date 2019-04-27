using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Room : MonoBehaviour
{
	
	Vector2Int roomCoords;
	public Vector2Int RoomCords{
		get{ return roomCoords; }
	}

	public bool isRegistered;


    // Start is called before the first frame update
    void Start()
    {
		SetCoords(0,0);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

	public void SetCoords(int x, int y){
		Vector2Int coords = new Vector2Int(x,y);
		SetCoords(coords);
	}

	public void SetCoords(Vector2Int coords){
		if(isRegistered){
			GameController.roomManager.RemoveRoom(this, roomCoords);
		}
		GameController.roomManager.RegisterRoom(this, coords);
		roomCoords = coords;
		Vector3 v = new Vector3(coords.x * GameController.screenWidth, coords.y * GameController.screenHeight);
		transform.localPosition = v;
	}
}
