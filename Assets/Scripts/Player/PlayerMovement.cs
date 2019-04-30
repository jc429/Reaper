using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour {
	Rigidbody _rigidbody{
		get{ return GetComponent<Rigidbody>(); }
	}
	[SerializeField]
	PlayerAnim _anim;
	public PlayerAnim Anim{
		get { return _anim; }
	}

	const float baseMoveSpeedG = 4f;
	const float baseMoveSpeedA = 3.2f;
	const float spdMulti = 1.3f;
	float GroundMoveSpeed{
		get{
			float f = baseMoveSpeedG;
			if(UnlockTable.PowerActive(UnlockID.MoveSpeed1)){
				f *= spdMulti;
			}
			if(UnlockTable.PowerActive(UnlockID.MoveSpeed2)){
				f *= spdMulti;
			}
			return f;
		}
	}
	float AirMoveSpeed{
		get{
			float f = baseMoveSpeedA;
			if(UnlockTable.PowerActive(UnlockID.MoveSpeed1)){
				f *= spdMulti;
			}
			if(UnlockTable.PowerActive(UnlockID.MoveSpeed2)){
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

	float dashPauseTimer;
	float dashPauseDuration = 0.15f;

	float wallJumpLockTimer;
	float wallJumpLockDuration = 0.2f;


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
		if(dashPauseTimer > 0){
			dashPauseTimer -= Time.deltaTime;
			_rigidbody.velocity = Vector3.zero;
			if(dashPauseTimer <= 0){
				dashPauseTimer = 0;
				_anim.StopDashSlashAnim();
			}
			return;
		}
		if(wallJumpLockTimer > 0){
			wallJumpLockTimer -= Time.deltaTime;
			if(wallJumpLockTimer <= 0){
				wallJumpLockTimer = 0;
				Vector3 v = _rigidbody.velocity;
				v.x *= 0.65f;
				v.y *= 0.75f;
				_rigidbody.velocity = v;
			}
			return;
		}
		isGrounded = Grounded();
		_anim.SetGrounded(isGrounded);
		if(controlsLocked || hitstun){
			return;
		}
		BasicMovement();
		if (VirtualController.JumpButtonPressed()) {
			if(UnlockTable.PowerActive(UnlockID.Jump)){
				Jump();
			}
		}
		else if (!_anim.IsCrouching && VirtualController.ActionButtonPressed()) {
			if(VirtualController.GetAxisHorizontal() != 0 && UnlockTable.PowerActive(UnlockID.DashSlash)){
				DashSlash(PMath.GetSign(VirtualController.GetAxisHorizontal()));
			}
			else if(UnlockTable.PowerActive(UnlockID.Slash)){
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
		moveInputs.x = VirtualController.GetAxisHorizontal();
		moveInputs.z = VirtualController.GetAxisVertical();
		_anim.SetMoving(moveInputs.x != 0);
		_anim.SetFacing(moveInputs.x);
		if(UnlockTable.PowerActive(UnlockID.Crouch)){
			_anim.SetCrouchInput(moveInputs.z < 0);
		}

		if(moveInputs.x != 0 && _rigidbody.velocity.x != 0){
			_rigidbody.velocity = new Vector3(0, _rigidbody.velocity.y);
		}

		Vector3 movement = CheckMovement(moveInputs);
		
		Vector3 movepos = transform.position;
		movepos.x += movement.x; 
		//movepos.z += movement.z;

		transform.position = movepos;

	}

	void Jump() {
		if (!isGrounded) {
			if(FacingAndTouchingWall() && UnlockTable.PowerActive(UnlockID.WallJump)){
				WallJump();
			}
			else if(UnlockTable.PowerActive(UnlockID.AirJump)){
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
		GameObject spark = Instantiate(GameController.instance.hitSparkPrefab);
		spark.transform.position = transform.position;
	}

	void WallJump(){
		int wallDir = _anim.Facing;
		_anim.SetFacing(-1 * wallDir);
		float wallKickSpeed = 6f;
		Vector3 vel = new Vector3(wallKickSpeed * -1 * wallDir, jumpSpeed);
		_rigidbody.velocity = vel;
		wallJumpLockTimer = wallJumpLockDuration;
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
		if(dir == 0){
			return;
		}

		RaycastHit r;
		Vector3 origin = transform.position;
		float[] distlist = new float[5];
		float dashDist = 4f;
		Vector3 dirVec = new Vector3(dir, 0);
		int gMask = Layers.GetGroundMask(false);
		
		Physics.Raycast(origin + new Vector3(0, 0.8f, 0), dirVec, out r, dashDist, gMask);
		Debug.DrawRay(origin + new Vector3(0, 0.8f, 0), dashDist * dirVec, (Color.white));
		distlist[0] = r.distance;
		Physics.Raycast(origin + new Vector3(0, 0.5f, 0), dirVec, out r, dashDist, gMask);
		Debug.DrawRay(origin + new Vector3(0, 0.5f, 0), dashDist * dirVec, (Color.white));
		distlist[1] = r.distance;
		Physics.Raycast(origin + new Vector3(0, 0.2f, 0), dirVec, out r, dashDist, gMask);
		Debug.DrawRay(origin + new Vector3(0, 0.2f, 0), dashDist * dirVec, (Color.white));
		distlist[2] = r.distance;
		Physics.Raycast(origin + new Vector3(0, -0.15f, 0), dirVec, out r, dashDist, gMask);
		Debug.DrawRay(origin + new Vector3(0, -0.15f, 0), dashDist * dirVec, (Color.white));
		distlist[3] = r.distance;
		Physics.Raycast(origin + new Vector3(0, -0.45f, 0), dirVec, out r, dashDist, gMask);
		Debug.DrawRay(origin + new Vector3(0, -0.45f, 0), dashDist * dirVec, (Color.white));
		distlist[4] = r.distance;

		float shortest = dashDist;
		foreach (float f in distlist) {
			if (f == 0) {
				continue;
			}
			if (f < shortest) {
				shortest = f;
			}
		}
		shortest -= 0.5f;
		if(shortest < 0){
			shortest = 0;
		}
		
		_anim.PlayDashSlashAnim(shortest * dir);
		Vector3 endpos = origin + (shortest * dirVec);
		transform.position = endpos;
		_rigidbody.velocity = Vector3.zero;
		dashPauseTimer = dashPauseDuration;
	}

	public void SetDownSlashUnlocked(bool unlocked){
		_anim.SetDownSlashActive(unlocked);
	}

	
	public bool CanUncrouch(){
		Vector3 origin = transform.position;
		Vector3 dir = Vector3.up;
		float length = 0.9f;
		int gMask = Layers.GetGroundMask(true);
		bool hit = false;
		hit |= Physics.Raycast(origin, dir,length, gMask);
		hit |= Physics.Raycast(origin + new Vector3(-0.45f, 0), dir,length, gMask);
		hit |= Physics.Raycast(origin + new Vector3(0.45f, 0), dir,length, gMask);
		return !hit;
	}

	
	public void TakeDamage(){
		if(hitstun || dashPauseTimer > 0){
			return;
		}
		//Debug.Log("ow!");
		int soulsLost = SoulWallet.LoseSouls();
		if(soulsLost < 0){
			Die();
		}
		else{
			int soulSpawns = 1;
			if(soulsLost < 5){
				soulSpawns = soulsLost;
			}
			else if(soulsLost < 30){
				soulSpawns = (soulsLost / 2) + 3;
			}
			else{
				soulSpawns = 15;
			}
			for(int i = 0; i < soulSpawns; i++){
				SoulCollectible soul = Instantiate(GameController.instance.soulPrefab);
				soul.transform.position = transform.position;
				Vector3 launchVel = new Vector3(0,4);
				launchVel.x = Random.Range(-6,6);
				launchVel.y += Random.Range(0,8);
				soul.SetLaunchVelocity(launchVel);
			}
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
		if(_anim.Facing == 0){
			return false;
		}
		//if not pushing into wall via input or velocity, dont walljump
		if(!(PMath.GetSign(VirtualController.GetAxisHorizontal()) == _anim.Facing)
		&& !(PMath.GetSign(_rigidbody.velocity.x) == _anim.Facing)){
			return false;
		}
		
		Vector3 dir = new Vector3(_anim.Facing,0);
		RaycastHit r;
		Vector3 origin = transform.position;
		float spd = 0.55f;
		int gMask = Layers.GetGroundMask(true);
		if(Physics.Raycast(origin + new Vector3(0, 0.8f, 0), dir, out r, spd, gMask)
		|| Physics.Raycast(origin + new Vector3(0, 0.5f, 0), dir, out r, spd, gMask)
		|| Physics.Raycast(origin + new Vector3(0, 0.2f, 0), dir, out r, spd, gMask)
		|| Physics.Raycast(origin + new Vector3(0, -0.15f, 0), dir, out r, spd, gMask)
		|| Physics.Raycast(origin + new Vector3(0, -0.45f, 0), dir, out r, spd, gMask)){

			return true;
		}
		else{
			return false;
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
		float[] distlist = new float[5];
		float moveSpeed = isGrounded ? GroundMoveSpeed : AirMoveSpeed;
		float spd =  0.5f + (moveSpeed * Time.deltaTime);
		Vector3 dir = new Vector3(movedir.x, 0);
		int gMask = Layers.GetGroundMask(true);
		if(!_anim.IsCrouching){
			Physics.Raycast(origin + new Vector3(0, 0.8f, 0), dir, out r, spd, gMask);
			Debug.DrawRay(origin + new Vector3(0, 0.8f, 0), spd * dir, (Color.white));
			distlist[0] = r.distance;
			Physics.Raycast(origin + new Vector3(0, 0.5f, 0), dir, out r, spd, gMask);
			Debug.DrawRay(origin + new Vector3(0, 0.5f, 0), spd * dir, (Color.white));
			distlist[1] = r.distance;
		}
		Physics.Raycast(origin + new Vector3(0, 0.2f, 0), dir, out r, spd, gMask);
		Debug.DrawRay(origin + new Vector3(0, 0.2f, 0), spd * dir, (Color.white));
		distlist[2] = r.distance;
		Physics.Raycast(origin + new Vector3(0, -0.15f, 0), dir, out r, spd, gMask);
		Debug.DrawRay(origin + new Vector3(0, -0.15f, 0), spd * dir, (Color.white));
		distlist[3] = r.distance;
		Physics.Raycast(origin + new Vector3(0, -0.45f, 0), dir, out r, spd, gMask);
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
