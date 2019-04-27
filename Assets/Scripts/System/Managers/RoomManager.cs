using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomManager : MonoBehaviour
{
	Dictionary<Vector2Int, Room> rooms;
	public Transform roomsParent;

    // Start is called before the first frame update
    void Awake()
    {
        GameController.roomManager = this;
		rooms = new Dictionary<Vector2Int, Room>();
    }

	void Start(){

	}

    // Update is called once per frame
    void Update()
    {
        
    }

	public Room GetRoomByCoords(Vector2Int coords){
		if(rooms.ContainsKey(coords)){
			return rooms[coords];
		}
		else{
			return null;
		}
	}

	public void RegisterRoom(Room r, Vector2Int coords){
		if(!rooms.ContainsKey(coords)){
			rooms.Add(coords, r);
			r.transform.parent = roomsParent;
			r.isRegistered = true;
		}
	}

	public void RemoveRoom(Room r, Vector2Int coords){
		if(rooms.ContainsKey(coords)){
			if(rooms[coords] == r){
				rooms.Remove(coords);
				r.isRegistered = false;
			}
		}
	}
}
