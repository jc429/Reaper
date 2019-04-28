using System.Collections;
using System.Collections.Generic;
using UnityEngine;

enum Directions{
	North = 0,
	East = 1,
	South = 2,
	West = 3
}

[System.Serializable]
public struct RoomBorderOpening{
	public int position;
	public int length;
}


public class Room : MonoBehaviour
{
	public GameObject blockPrefab;
	public GameObject blockContainer;

	public bool fixedLayout; 

	Vector2Int roomCoords;
	public Vector2Int RoomCords{
		get{ return roomCoords; }
	}

	public bool isRegistered = false;

	[NamedArrayAttribute (new string[] {"North","East","South","West"})]
	public Room[] neighbors = new Room[4];

	public RoomBorderOpening[] borderOpenings = new RoomBorderOpening[4];

    // Start is called before the first frame update
    void Start()
    {

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

	public void GenerateEdges(){
		if(fixedLayout) return;
		for(int i = 0; i < 4; i++){
			bool vertical = (i % 2 != 0);
			int realEstate = vertical ? 13 : 19;

			/* calculate openings  */
			//if neighbor exists, match their border
			if(neighbors[i] != null){
				int oppositeDir = (i + 2) % 4;
				borderOpenings[i] = neighbors[i].borderOpenings[oppositeDir];

				//vertical borders are shared between rooms, no need to construct a second copy
				
			}
			//otherwise create one randomly
			else{
				int minGap = vertical ? 3 : 2;
				int maxGap = vertical ? 12 : 7;
				const int minHeightVerticalEdge = 0;
				const int maxHeightVerticalEdge = 12;

				//generate gap length
				int gapLength = Random.Range(0, maxGap);
				if(gapLength < minGap){
					gapLength = 0;
					borderOpenings[i].length = 0;
					borderOpenings[i].position = 0;
					
				}
				else{
					if(vertical){
						int pos = Random.Range(minHeightVerticalEdge,maxHeightVerticalEdge - gapLength);
						borderOpenings[i].length = gapLength;
						borderOpenings[i].position = pos;
					}
					else{
						int pos = Random.Range(0,realEstate - gapLength);
						borderOpenings[i].length = gapLength;
						borderOpenings[i].position = pos;
					}
				}
			}

			/* construct the tile sprites */
			for(int k = 0; k < realEstate; k++){
				if(k < borderOpenings[i].position || k >= (borderOpenings[i].position + borderOpenings[i].length)){
					Vector3 blockPos = Vector3.zero;
					switch(i){
					case 0:
						blockPos.x = k+1;
						blockPos.y = 14;
						break;
					case 1:
						blockPos.x = 20;
						blockPos.y = k+1;
						break;
					case 2:
						blockPos.x = k+1;
						blockPos.y = 0;
						break;
					case 3:
						blockPos.x = 0;
						blockPos.y = k+1;
						break;
					}

					GameObject block = Instantiate(blockPrefab);
					block.transform.parent = blockContainer.transform;
					block.transform.localPosition = blockPos;
				}
			}

			/* add colliders */
			Vector2Int colStart = Vector2Int.zero, colEnd = Vector2Int.zero;
			switch(i){
			case 0:
				colStart.y = colEnd.y = 14;
				colStart.x = 1;
				if(borderOpenings[i].length == 0){
					colEnd.x = 19;
					AddBorderCollider(colStart,colEnd);
				}
				else{
					colEnd.x = borderOpenings[i].position;
					if(colEnd.x >= colStart.x){
						AddBorderCollider(colStart,colEnd);
					}
					colStart.x = borderOpenings[i].position + borderOpenings[i].length + 1;
					colEnd.x = 19;
					AddBorderCollider(colStart,colEnd);
				}
				break;
			case 1:
				colStart.x = colEnd.x = 20;
				colStart.y = 1;
				if(borderOpenings[i].length == 0){
					colEnd.y = 13;
					AddBorderCollider(colStart,colEnd);
				}
				else{
					colEnd.y = borderOpenings[i].position;
					if(colEnd.y >= colStart.y){
						AddBorderCollider(colStart,colEnd);
					}
					colStart.y = borderOpenings[i].position + borderOpenings[i].length + 1;
					colEnd.y = 13;
					AddBorderCollider(colStart,colEnd);
				}
				break;
			case 2:
				colStart.y = colEnd.y = 0;
				colStart.x = 1;
				if(borderOpenings[i].length == 0){
					colEnd.x = 19;
					AddBorderCollider(colStart,colEnd);
				}
				else{
					colEnd.x = borderOpenings[i].position;
					if(colEnd.x >= colStart.x){
						AddBorderCollider(colStart,colEnd);
					}
					colStart.x = borderOpenings[i].position + borderOpenings[i].length + 1;
					colEnd.x = 19;
					AddBorderCollider(colStart,colEnd);
				}
				break;
			case 3:
				colStart.x = colEnd.x = 0;
				colStart.y = 1;
				if(borderOpenings[i].length == 0){
					colEnd.y = 13;
					AddBorderCollider(colStart,colEnd);
				}
				else{
					colEnd.y = borderOpenings[i].position;
					if(colEnd.y >= colStart.y){
						AddBorderCollider(colStart,colEnd);
					}
					colStart.y = borderOpenings[i].position + borderOpenings[i].length + 1;
					colEnd.y = 13;
					AddBorderCollider(colStart,colEnd);
				}
				break;
			}
			
		}
	}

	public void AddBorderCollider(Vector2 startPos, Vector2 endPos){
		BoxCollider bc;
		bc = blockContainer.AddComponent(typeof(BoxCollider)) as BoxCollider;
		bc.center = Vector2.Lerp(startPos,endPos,0.5f);
		bc.size = new Vector3(endPos.x - startPos.x + 1, endPos.y - startPos.y + 1, 0.2f);
	}

	public void Activate(){
		gameObject.SetActive(true);
	}

	public void Deactivate(){
		gameObject.SetActive(false);
	}
}
