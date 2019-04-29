using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour {
	Rigidbody _rigidbody{
		get{ return GetComponent<Rigidbody>(); }
	}
	[SerializeField]
	PlayerAnim _anim;

	const float baseMoveSpeedG = 4f;
	const float baseMoveSpeedA = 3.2f;
	const float spdMulti = 1.3f;
	float GroundMoveSpeed{
		get{
			float f = baseMoveSpeedG;
			if(UnlockTable.PowerUnlocked(UnlockID.MoveSpeed1)){
				f *= spdMulti;
			}
			if(UnlockTable.PowerUnlocked(UnlockID.MoveSpeed2)){
				f *= spdMulti;
			}
			return f;
		}
	}
	float AirMoveSpeed{
		get{
			float f = baseMoveSpeedA;
			if(UnlockTable.PowerUnlocked(UnlockID.MoveSpeed1)){
				f *= spdMulti;
			}
			if(UnlockTable.PowerUnlocked(UnlockID.MoveSpeed2)){
				f *= spdMulti;
			}
			return f;
		}
	}
	float jumpSpeed = 9f;

	const int maxAirJumps = 1;
	int numAirJumps;
	bool isGrounded;

	public Vector2Int coords = new Vector2Int();

	public bool controlsLocked;
	public bool isDead;
	public bool hitstun;

	public bool IsMoving{
		get{
			return _rigidbody.velocity.x != 0;
		}
	}
	// Use this for initialization
	void Start () {
		GameController.instance.player = this;
		Reset();
	}

	void Reset(){
		controlsLocked = false;
		isDead = false;
		hitstun = false;
	}
	
	// Update is called once per frame
	void Update () {
		isGrounded = Grounded();
		_anim.SetGrounded(isGrounded);
		if(controlsLocked || hitstun){
			return;
		}
		BasicMovement();
		if (Input.GetKeyDown(KeyCode.Space)) {
			if(UnlockTable.PowerUnlocked(UnlockID.Jump)){
				Jump();
			}
		}
		else if (Input.GetKeyDown(KeyCode.LeftShift)) {
			if(Input.GetAxis("Horizontal") != 0 && UnlockTable.PowerUnlocked(UnlockID.DashSlash)){
				DashSlash(PMath.GetSign(Input.GetAxis("Horizontal")));
			}
			else if(UnlockTable.PowerUnlocked(UnlockID.Slash)){
				Slash();
			}
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
		Vector3 v = _rigidbody.velocity;
		v.y = Mathf.Max(v.y, -20);
		_rigidbody.velocity = v;
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
			if(FacingAndTouchingWall() && UnlockTable.PowerUnlocked(UnlockID.WallJump)){

			}
			else if(UnlockTable.PowerUnlocked(UnlockID.AirJump)){
				AirJump();
			}
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

	void DashSlash(int dir){
		if(dir == 0){
			dir = _anim.Facing;
		}
		_anim.PlayDashSlashAnim();
	}

	public void UnlockDownSlash(){
		_anim.UnlockScytheJump();
	}
	
	public void TakeDamage(){
		//Debug.Log("ow!");
		int soulsLost = SoulWallet.LoseSouls();
		if(soulsLost < 0){
			Die();
		}
		hitstun = true;
		_rigidbody.velocity = new Vector3(-2f * _anim.Facing, 1,0);
		_anim.PlayHurtAnim();
	
	}

	public void Die(){
		controlsLocked = true;
		_anim.PlayDieAnim();
	}


	public void UpdateCoords(){
		float offX = (transform.position.x > -15) ? -0.5f : 0.5f;
		int intX = Mathf.RoundToInt(transform.position.x + offX + GameController.xBound);
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

	bool FacingAndTouchingWall(){
		return false;
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
		float[] distlist = new float[5];

		//Vector3 movedist = new Vector3(moveSpeed * Time.deltaTime, 0, movedir.z * moveSpeed * Time.deltaTime);
		float moveSpeed = isGrounded ? GroundMoveSpeed : AirMoveSpeed;
		float spd =  0.5f + (moveSpeed * Time.deltaTime);
		Vector3 dir = new Vector3(movedir.x, 0);
		Physics.Raycast(origin + new Vector3(0, 0.8f, 0), dir, out r, spd, Layers.GetGroundMask(false));
		Debug.DrawRay(origin + new Vector3(0, 0.8f, 0), spd * dir, (Color.white));
		distlist[0] = r.distance;
		Physics.Raycast(origin + new Vector3(0, 0.5f, 0), dir, out r, spd, Layers.GetGroundMask(false));
		Debug.DrawRay(origin + new Vector3(0, 0.5f, 0), spd * dir, (Color.white));
		distlist[1] = r.distance;
		Physics.Raycast(origin + new Vector3(0, 0.2f, 0), dir, out r, spd, Layers.GetGroundMask(false));
		Debug.DrawRay(origin + new Vector3(0, 0.2f, 0), spd * dir, (Color.white));
		distlist[2] = r.distance;
		Physics.Raycast(origin + new Vector3(0, -0.15f, 0), dir, out r, spd, Layers.GetGroundMask(false));
		Debug.DrawRay(origin + new Vector3(0, -0.15f, 0), spd * dir, (Color.white));
		distlist[3] = r.distance;
		Physics.Raycast(origin + new Vector3(0, -0.45f, 0), dir, out r, spd, Layers.GetGroundMask(false));
		Debug.DrawRay(origin + new Vector3(0, -0.45f, 0), spd * dir, (Color.white));
		distlist[4] = r.distance;

		float shortest = float.MaxValue;
		foreach (float f in distlist) {
			if (f == 0) {
				continue;
			}
			if (f < shortest) {
				shortest = f;
			}
		}

		//Debug.Log(0.5f + (moveSpeed * Time.deltaTime));
		if (shortest > 0 && shortest < 100f) {
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
