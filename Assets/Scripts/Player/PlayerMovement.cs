using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour {
	Rigidbody _rigidbody;
	[SerializeField]
	PlayerAnim _anim;

	float groundMoveSpeed = 4f;
	float airMoveSpeed = 3.2f;
	float jumpSpeed = 9f;

	const int maxAirJumps = 1;
	int numAirJumps;
	bool isGrounded;

	public Vector2Int coords = new Vector2Int();

	public bool controlsLocked;
	public bool isDead;

	public bool IsMoving{
		get{
			return _rigidbody.velocity.x != 0;
		}
	}
	// Use this for initialization
	void Start () {
		_rigidbody = GetComponent<Rigidbody>();
		GameController.instance.player = this;
		Reset();
	}

	void Reset(){
		controlsLocked = false;
		isDead = false;
	}
	
	// Update is called once per frame
	void Update () {
		isGrounded = Grounded();
		_anim.SetGrounded(isGrounded);
		if(controlsLocked){
			return;
		}
		BasicMovement();
		if (Input.GetKeyDown(KeyCode.Space)) {
			Jump();
		}
		else if (Input.GetKeyDown(KeyCode.LeftShift)) {
			Slash();
		}
		if (isGrounded) {
			numAirJumps = maxAirJumps;
		}
		UpdateCoords();
	}

	void FixedUpdate () {
		if (!isGrounded && _rigidbody.velocity.y < 5f) {
			_rigidbody.AddForce(Physics.gravity * 2);
		}
	}

	void BasicMovement() {
		Vector3 moveInputs = Vector3.zero;
		moveInputs.x = Input.GetAxis("Horizontal");
		moveInputs.z = Input.GetAxis("Vertical");
		_anim.SetMoving(moveInputs.x != 0);
		_anim.SetFacing(moveInputs.x);

		Vector3 movement = CheckMovement(moveInputs);
		
		Vector3 movepos = transform.position;
		movepos.x += movement.x; 
		//movepos.z += movement.z;

		transform.position = movepos;

	}

	void Jump() {
		if (!isGrounded) {
			AirJump();
			return;
		}
		//	float jumpForce = 400f;
		//	_rigidbody.AddForce(Vector3.up * jumpForce);
		_anim.PlayJumpSound();
		Vector3 vel = _rigidbody.velocity;
		vel.y = jumpSpeed;
		_rigidbody.velocity = vel;
		
	}

	void AirJump() {
		if (numAirJumps <= 0) {
			return;
		}
		_anim.PlayJumpSound();
		numAirJumps--;
		Vector3 vel = _rigidbody.velocity;
		vel.y = jumpSpeed;
		_rigidbody.velocity = vel;
	}

	void Slash() {
		_anim.PlaySlashAnim();
	}

	
	public void TakeDamage(){
		Debug.Log("ow!");
		int soulsLost = SoulWallet.LoseSouls();
		
	}

	public void Die(){
		controlsLocked = true;
		_anim.PlayDieAnim();
	}


	public void UpdateCoords(){
		int intX = Mathf.RoundToInt(transform.position.x - 0.5f + GameController.xBound);
		int intY = Mathf.RoundToInt(transform.position.y + GameController.yBound);

		coords.x = intX / GameController.screenWidth;
		coords.y = intY / GameController.screenHeight;

		//hacky fix to prevent 4 rooms giving (0,0 as a result)
		if(intX < 0){
			coords.x -= 1;
		}
		if(intY < 0){
			coords.y -= 1;
		}
	}

	public bool Grounded() {
		bool groundboys;

		//	Debug.Log(groundmask.value);	
		//groundmask = ~groundmask;	
		//who the hell knows if you're supposed to invert the mask? not the documentation! gotta try both!
		bool groundboysL = Physics.Raycast(transform.position + new Vector3(-0.425f, 0, 0), Vector3.down, 0.55f, Layers.GetGroundMask(false));
		bool groundboysM = Physics.Raycast(transform.position, Vector3.down, 0.6f, Layers.GetGroundMask(false));
		bool groundboysR = Physics.Raycast(transform.position + new Vector3(0.425f, 0, 0), Vector3.down, 0.55f, Layers.GetGroundMask(false));
		groundboys = (groundboysL || groundboysM || groundboysR);
		//can't use transform.up either for god knows what reason, you gotta use Vector3's versions 
		Debug.DrawRay(transform.position, 0.6f * Vector3.down, (groundboysM ? Color.green : Color.white));
		Debug.DrawRay(transform.position + new Vector3(-0.425f, 0, 0), Vector3.down * 0.55f, (groundboysL ? Color.green : Color.white));
		Debug.DrawRay(transform.position + new Vector3(0.425f, 0, 0), Vector3.down * 0.55f, (groundboysR ? Color.green : Color.white));
		return groundboys;
	}

	public Vector3 CheckMovement(Vector3 movedir) {
		if (movedir.Equals(Vector3.zero)) return Vector3.zero;
		RaycastHit r;
		Vector3 origin = transform.position;
		float[] distlist = new float[9];
		float halfX = 0.5f;
		float halfY = 0.45f;
		float halfZ = 0.05f;

		//Vector3 movedist = new Vector3(moveSpeed * Time.deltaTime, 0, movedir.z * moveSpeed * Time.deltaTime);
		float moveSpeed = isGrounded ? groundMoveSpeed : airMoveSpeed;

		Physics.Raycast(origin + new Vector3(0, 0.8f, 0), new Vector3(movedir.x, 0, 0), out r, 0.5f + (moveSpeed * Time.deltaTime),Layers.GetGroundMask(false));
		distlist[0] = r.distance;
		Physics.Raycast(origin + new Vector3(0, 0.7f, 0), new Vector3(movedir.x, 0, 0), out r, 0.5f + (moveSpeed * Time.deltaTime),Layers.GetGroundMask(false));
		distlist[1] = r.distance;
		Physics.Raycast(origin + new Vector3(0, 0.55f, 0), new Vector3(movedir.x, 0, 0), out r, 0.5f + (moveSpeed * Time.deltaTime),Layers.GetGroundMask(false));
		distlist[2] = r.distance;
		Physics.Raycast(origin + new Vector3(0, 0.4f, 0), new Vector3(movedir.x, 0, 0), out r, 0.5f + (moveSpeed * Time.deltaTime),Layers.GetGroundMask(false));
		distlist[3] = r.distance;
		Physics.Raycast(origin + new Vector3(0, 0.2f, 0), new Vector3(movedir.x, 0, 0), out r, 0.5f + (moveSpeed * Time.deltaTime),Layers.GetGroundMask(false));
		distlist[4] = r.distance;
		Physics.Raycast(origin + new Vector3(0, 0f, 0), new Vector3(movedir.x, 0, 0), out r, 0.5f + (moveSpeed * Time.deltaTime),Layers.GetGroundMask(false));
		distlist[5] = r.distance;
		Physics.Raycast(origin + new Vector3(0, -0.2f, 0), new Vector3(movedir.x, 0, 0), out r, 0.5f + (moveSpeed * Time.deltaTime),Layers.GetGroundMask(false));
		distlist[6] = r.distance;
		Physics.Raycast(origin + new Vector3(0, -0.35f, 0), new Vector3(movedir.x, 0, 0), out r, 0.5f + (moveSpeed * Time.deltaTime),Layers.GetGroundMask(false));
		distlist[7] = r.distance;
		Physics.Raycast(origin + new Vector3(0, -0.45f, 0), new Vector3(movedir.x, 0, 0), out r, 0.5f + (moveSpeed * Time.deltaTime),Layers.GetGroundMask(false));
		distlist[8] = r.distance;

		float shortest = 0;
		foreach (float f in distlist) {
			if (shortest == 0) {
				shortest = f;
				continue;
			}
			if (f == 0) {
				continue;
			}
			if (f < shortest) {
				shortest = f;
			}
		}

		//Debug.Log(0.5f + (moveSpeed * Time.deltaTime));
		if (shortest > 0) {
			movedir.x *= (shortest - 0.5f);
		}
		else {
			movedir.x *= moveSpeed * Time.deltaTime;
		}
		/*
		r = new RaycastHit();
		origin = transform.position;
		distlist = new float[9];

		Physics.Raycast(origin, new Vector3(0, 0, movedir.z), out r, 0.5f + (moveSpeed * Time.deltaTime));
		distlist[0] = r.distance;
		Physics.Raycast(origin + new Vector3(0, 0.5f, 0), new Vector3(0, 0, movedir.z), out r, 0.5f + (moveSpeed * Time.deltaTime));
		distlist[1] = r.distance;
		Physics.Raycast(origin + new Vector3(halfX, 0.5f, 0), new Vector3(0, 0, movedir.z), out r, 0.5f + (moveSpeed * Time.deltaTime)); 
		distlist[2] = r.distance;
		Physics.Raycast(origin + new Vector3(halfX, 0, 0), new Vector3(0, 0, movedir.z), out r, 0.5f + (moveSpeed * Time.deltaTime));
		distlist[3] = r.distance;
		Physics.Raycast(origin + new Vector3(halfX, -0.5f, 0), new Vector3(0, 0, movedir.z), out r, 0.5f + (moveSpeed * Time.deltaTime));
		distlist[4] = r.distance;
		Physics.Raycast(origin + new Vector3(0, -0.5f, 0), new Vector3(0, 0, movedir.z), out r, 0.5f + (moveSpeed * Time.deltaTime));
		distlist[5] = r.distance;
		Physics.Raycast(origin + new Vector3(-halfX, -0.5f, 0), new Vector3(0, 0, movedir.z), out r, 0.5f + (moveSpeed * Time.deltaTime));
		distlist[6] = r.distance;
		Physics.Raycast(origin + new Vector3(-halfX, 0f, 0), new Vector3(0, 0, movedir.z), out r, 0.5f + (moveSpeed * Time.deltaTime));
		distlist[7] = r.distance;
		Physics.Raycast(origin + new Vector3(-halfX, 0.5f, 0), new Vector3(0, 0, movedir.z), out r, 0.5f + (moveSpeed * Time.deltaTime));
		distlist[8] = r.distance;
		//Debug.Log(0.5f + (moveSpeed * Time.deltaTime));
		shortest = 0;
		foreach (float f in distlist) {
			if (shortest == 0) {
				shortest = f;
				continue;
			}
			if (f == 0) {
				continue;
			}
			if (f < shortest) {
				shortest = f;
			}
		}

		if (shortest > 0) {
			movedir.z *= (shortest - 0.5f);
		}
		else {
			movedir.z *= moveSpeed * Time.deltaTime;
		}*/

		return movedir;
	}

}
