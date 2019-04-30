using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

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

	public int RightEdge{
		get{ return position + length; }
	}
}


public class Room : MonoBehaviour
{
	public GameObject collisionArea;
	public GameObject entityContainer;

	public bool fixedLayout; 

	Vector2Int roomCoords;
	public Vector2Int RoomCords{
		get{ return roomCoords; }
	}

	public bool isRegistered = false;

	[NamedArrayAttribute (new string[] {"North","East","South","West"})]
	public Room[] neighbors = new Room[4];

	public RoomBorderOpening[] borderOpenings = new RoomBorderOpening[4];


	public Tilemap tileMap;
	public Tile tile;

	List<Vector2Int> spawns;
	int[,] solidTiles;
	List<Vector2Int> floorTiles; //solid tiles with gaps above them

	public Enemy enemyPrefab;
	List<Enemy> enemyList;

	public BreakableBlock blockPrefab;
	List<Vector2Int> blockLocations;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }

	public void Initialize(){
		solidTiles = new int[21,15];
		spawns = new List<Vector2Int>();
		floorTiles = new List<Vector2Int>();
		enemyList = new List<Enemy>();
		blockLocations = new List<Vector2Int>();
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

	public void Generate(){
		GenerateEdges();
		GenerateInternals();
		SetEnemySpawnLocations();
	}

	void GenerateEdges(){
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
					Vector2Int blockPos = Vector2Int.zero;
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

					SetSolidTile(blockPos);
					//GameObject block = Instantiate(blockPrefab);
					//block.transform.parent = blockContainer.transform;
					//block.transform.localPosition = blockPos;
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
					AddCollider(colStart,colEnd);
				}
				else{
					colEnd.x = borderOpenings[i].position;
					if(colEnd.x >= colStart.x){
						AddCollider(colStart,colEnd);
					}
					colStart.x = borderOpenings[i].position + borderOpenings[i].length + 1;
					colEnd.x = 19;
					AddCollider(colStart,colEnd);
				}
				break;
			case 1:
				colStart.x = colEnd.x = 20;
				colStart.y = 1;
				if(borderOpenings[i].length == 0){
					colEnd.y = 13;
					AddCollider(colStart,colEnd);
				}
				else{
					colEnd.y = borderOpenings[i].position;
					if(colEnd.y >= colStart.y){
						AddCollider(colStart,colEnd);
					}
					colStart.y = borderOpenings[i].position + borderOpenings[i].length + 1;
					colEnd.y = 13;
					AddCollider(colStart,colEnd);
				}
				break;
			case 2:
				colStart.y = colEnd.y = 0;
				colStart.x = 1;
				if(borderOpenings[i].length == 0){
					colEnd.x = 19;
					AddCollider(colStart,colEnd);
				}
				else{
					colEnd.x = borderOpenings[i].position;
					if(colEnd.x >= colStart.x){
						AddCollider(colStart,colEnd);
					}
					colStart.x = borderOpenings[i].position + borderOpenings[i].length + 1;
					colEnd.x = 19;
					AddCollider(colStart,colEnd);
				}
				break;
			case 3:
				colStart.x = colEnd.x = 0;
				colStart.y = 1;
				if(borderOpenings[i].length == 0){
					colEnd.y = 13;
					AddCollider(colStart,colEnd);
				}
				else{
					colEnd.y = borderOpenings[i].position;
					if(colEnd.y >= colStart.y){
						AddCollider(colStart,colEnd);
					}
					colStart.y = borderOpenings[i].position + borderOpenings[i].length + 1;
					colEnd.y = 13;
					AddCollider(colStart,colEnd);
				}
				break;
			}
			
		}
	}

	void GenerateInternals(){
		int r1 = Random.Range(0,6);
		int r2 = Random.Range(0,6);
		int numPlatforms = (r1 + r2) / 2;
		for(int i = 0; i < numPlatforms; i++){
			Vector2Int startPos = new Vector2Int();
			Vector2Int size = new Vector2Int();
			startPos.x = Random.Range(2,18);
			startPos.y = Random.Range(2,12);
			size.x = Random.Range(1,18-startPos.x);
			size.y = Random.Range(1,12-startPos.y);

			for(int j = 0; j <= size.x; j++){
				for(int k = 0; k <= size.y; k++){
					Vector2Int blockPos = new Vector2Int(startPos.x + j, startPos.y + k);
					SetSolidTile(blockPos);
				}
			}

			AddCollider(startPos,startPos+size);
		}

		SetBlockLocations();

		for(int x = 1; x < 20; x++){
			for(int y = 0; y < 14; y++){
				if(CheckTile(x,y) == 1 && CheckTile(x,y+1) == 0){
					floorTiles.Add(new Vector2Int(x,y+1));
				}
			}
		}
	}

	void SetBlockLocations(){
		int numBlocks = Random.Range(0,7);

		for(int i = 0; i < numBlocks; i++){
			int sizeX = Random.Range(1,4);
			int sizeY = Random.Range(1,4);
			int posX = Random.Range(1,20 - sizeX);
			int posY = Random.Range(1,13 - sizeY);

			Vector2Int blockPos = new Vector2Int(posX,posY);
			for(int x = 0; x < sizeX; x++){
				for(int y = 0; y < sizeY; y++){
					Vector2Int pos = blockPos + new Vector2Int(x,y);
					if(CheckTile(pos) == 0){
						AddBlock(pos);
						solidTiles[pos.x,pos.y] = 1;
					}
				}
			}
		}
		
	}

	void AddBlock(Vector2Int pos){
		blockLocations.Add(pos);
		BreakableBlock block = Instantiate(blockPrefab);
		block.transform.parent = entityContainer.transform;
		block.transform.localPosition = new Vector3(pos.x, pos.y);
	}

	void SetEnemySpawnLocations(){
		spawns.Clear();
		int r1 = Random.Range(0,8);
		int r2 = Random.Range(0,8);
		int numSpawns = (r1 + r2) / 2;
		int numValidFloorTiles = 17 - borderOpenings[2].length;
		if(numSpawns > numValidFloorTiles){
			numSpawns = numValidFloorTiles;
		}

		for(int i = 0; i < numSpawns; i++){
			int r = Random.Range(0,floorTiles.Count);
			//dont allow spawns right near an upper border opening
			if(floorTiles[r].y > 10){
				int xPos = floorTiles[r].x;
				if(xPos >= borderOpenings[0].position && xPos <= borderOpenings[0].RightEdge){
					continue;
				}
			}
			if(!spawns.Contains(floorTiles[r])){
				spawns.Add(floorTiles[r]);
			}
			/*Vector2Int spawnPos = new Vector2Int();
			spawnPos.x = Random.Range(2,18);
			spawnPos.y = Random.Range(1,10);
			
			if(CheckTile(spawnPos) == 0){
				spawns.Add(spawnPos);
			}
			else{
				//if we're within a solid block try to keep climbing to spawn on top of it
				for(int y2 = spawnPos.y + 1; y2 <= 10; y2++){
					spawnPos.y = y2;
					if(CheckTile(spawnPos) == 0){
						spawns.Add(spawnPos);
						break;
					}
				}
			}*/



		}
	}

	public void SetEntitiesActive(bool active){
		entityContainer.SetActive(active);
	}

	public void SpawnEnemies(){
		for(int i = 0; i < spawns.Count; i++){
			Enemy e = Instantiate(enemyPrefab);
			int r = Random.Range(0,2);
			e.startFacing = (r < 1) ? -1 : 1;
			e.transform.parent = entityContainer.transform;
			e.transform.localPosition = new Vector3(spawns[i].x, spawns[i].y);
			enemyList.Add(e);
		}
	}

	public void DespawnEnemies(){
		foreach(Enemy e in enemyList){
			Destroy(e);
		}
	}

	public void SetSolidTile(Vector2Int pos){
		if(pos.x < 0 || pos.x > 20 || pos.y < 0 || pos.y > 14){
			return;
		}
		solidTiles[pos.x,pos.y] = 1;
		tileMap.SetTile(new Vector3Int(pos.x, pos.y,0),tile);
	}

	public int CheckTile(Vector2Int pos){
		if(pos.x < 0 || pos.x > 20 || pos.y < 0 || pos.y > 14){
			return -1;
		}
		return solidTiles[pos.x,pos.y];
	}
	
	public int CheckTile(int x, int y){
		if(x < 0 || x > 20 || y < 0 || y > 14){
			return -1;
		}
		return solidTiles[x,y];
	}

	public void AddCollider(Vector2 startPos, Vector2 endPos){
		BoxCollider bc;
		bc = collisionArea.AddComponent(typeof(BoxCollider)) as BoxCollider;
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
