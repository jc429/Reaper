using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomManager : MonoBehaviour
{
	Dictionary<Vector2Int, Room> rooms;
	public Transform roomsParent;

	[SerializeField]
	Room roomPrefab;
	Vector2Int currentRoomCoords;

	public Room startingRoom;

    // Start is called before the first frame update
    void Awake()
    {
        GameController.roomManager = this;
		rooms = new Dictionary<Vector2Int, Room>();
		currentRoomCoords = Vector2Int.zero;
    }

	void Start(){
		startingRoom.SetCoords(0,0);
		GenerateRoom(new Vector2Int(1,0));
		GenerateRoom(new Vector2Int(-1,0));
		GenerateRoom(new Vector2Int(0,1));
		GenerateRoom(new Vector2Int(0,-1));
	}

	public void Reset(){
		rooms.Clear();
		startingRoom.SetCoords(0,0);
		GenerateRoom(new Vector2Int(1,0));
		GenerateRoom(new Vector2Int(-1,0));
		GenerateRoom(new Vector2Int(0,1));
		GenerateRoom(new Vector2Int(0,-1));
	}

    // Update is called once per frame
    void Update()
    {
		if(GameController.instance.player.coords != currentRoomCoords){
			UpdateCoordinates(GameController.instance.player.coords);
		}
    }

	void UpdateCoordinates(Vector2Int playerCoords){
		
		//if player moves left, activate rooms to their left
		if(playerCoords.x < currentRoomCoords.x){
			if(!rooms.ContainsKey(playerCoords + new Vector2Int(-1,0))){
				GenerateRoom(playerCoords + new Vector2Int(-1,0));
				GenerateRoom(playerCoords + new Vector2Int(0,1));
				GenerateRoom(playerCoords + new Vector2Int(0,-1));
			}
			ActivateRoom(playerCoords + new Vector2Int(-1,-1));
			ActivateRoom(playerCoords + new Vector2Int(-1,0));
			ActivateRoom(playerCoords + new Vector2Int(-1,1));
			DeactivateRoom(currentRoomCoords + new Vector2Int(1,-1));
			DeactivateRoom(currentRoomCoords + new Vector2Int(1,0));
			DeactivateRoom(currentRoomCoords + new Vector2Int(1,1));
		}
		//if player moves right, activate rooms to their right
		else if(playerCoords.x > currentRoomCoords.x){
			if(!rooms.ContainsKey(playerCoords + new Vector2Int(1,0))){
				GenerateRoom(playerCoords + new Vector2Int(1,0));
				GenerateRoom(playerCoords + new Vector2Int(0,1));
				GenerateRoom(playerCoords + new Vector2Int(0,-1));
			}
			ActivateRoom(playerCoords + new Vector2Int(1,-1));
			ActivateRoom(playerCoords + new Vector2Int(1,0));
			ActivateRoom(playerCoords + new Vector2Int(1,1));
			DeactivateRoom(currentRoomCoords + new Vector2Int(-1,-1));
			DeactivateRoom(currentRoomCoords + new Vector2Int(-1,0));
			DeactivateRoom(currentRoomCoords + new Vector2Int(-1,1));
		}

		//if player moves north, activate rooms to their north
		if(playerCoords.y > currentRoomCoords.y){
			if(!rooms.ContainsKey(playerCoords + new Vector2Int(0,1))){
				GenerateRoom(playerCoords + new Vector2Int(0,1));
				GenerateRoom(playerCoords + new Vector2Int(1,0));
				GenerateRoom(playerCoords + new Vector2Int(-1,0));
			}
			ActivateRoom(playerCoords + new Vector2Int(-1,1));
			ActivateRoom(playerCoords + new Vector2Int(0,1));
			ActivateRoom(playerCoords + new Vector2Int(1,1));
			DeactivateRoom(currentRoomCoords + new Vector2Int(-1,-1));
			DeactivateRoom(currentRoomCoords + new Vector2Int(0,-1));
			DeactivateRoom(currentRoomCoords + new Vector2Int(1,-1));
		}
		//if player moves south, activate rooms to their south
		else if(playerCoords.y < currentRoomCoords.y){
			if(!rooms.ContainsKey(playerCoords + new Vector2Int(0,-1))){
				GenerateRoom(playerCoords + new Vector2Int(0,-1));
				GenerateRoom(playerCoords + new Vector2Int(1,0));
				GenerateRoom(playerCoords + new Vector2Int(-1,0));
			}
			ActivateRoom(playerCoords + new Vector2Int(-1,-1));
			ActivateRoom(playerCoords + new Vector2Int(0,-1));
			ActivateRoom(playerCoords + new Vector2Int(1,-1));
			DeactivateRoom(currentRoomCoords + new Vector2Int(-1,1));
			DeactivateRoom(currentRoomCoords + new Vector2Int(0,1));
			DeactivateRoom(currentRoomCoords + new Vector2Int(1,1));
		}


		currentRoomCoords = GameController.instance.player.coords;
	}

	public Room GenerateRoom(Vector2Int coords){
		if(rooms.ContainsKey(coords)){
		//	Debug.Log("Room already exists!");
			return rooms[coords];
		}
		Room room = Instantiate(roomPrefab);

		room.SetCoords(coords);
		room.GenerateEdges();

		return room;
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
			r.neighbors[(int)Directions.North] = GetRoomByCoords(coords + new Vector2Int(0,1));
			r.neighbors[(int)Directions.East] = GetRoomByCoords(coords + new Vector2Int(1,0));
			r.neighbors[(int)Directions.West] = GetRoomByCoords(coords + new Vector2Int(-1,0));
			r.neighbors[(int)Directions.South] = GetRoomByCoords(coords + new Vector2Int(0,-1));

			for(int i = 0; i < 4; i++){
				if(r.neighbors[i] != null){
					r.neighbors[i].neighbors[(i+2)%4] = r;
				}
			}

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

	public void ActivateRoom(Vector2Int coords){
		Room r = GetRoomByCoords(coords);
		if(r != null){
			r.Activate();
		}
	}

	public void DeactivateRoom(Vector2Int coords){
		Room r = GetRoomByCoords(coords);
		if(r != null){
			r.Deactivate();
		}
	}
}
