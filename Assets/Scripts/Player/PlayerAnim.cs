using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnim : MonoBehaviour
{
	Animator _animator;
	SpriteRenderer _spriteRenderer;
	[SerializeField]
	PlayerMovement _movement;
	PaletteSprite _palSprite;

	AudioSource _audioSource;
	public AudioClip jumpClip;
	public AudioClip slashClip;
	public AudioClip hurtClip;
	public AudioClip dieClip;

	int facing;
	public int Facing{
		get{ return facing; }
	}

	public PlayerHitbox slashHitbox;
	public bool downSlashEnabled;
	public PlayerHitbox jumpHitbox;
	float slashHitboxOffsetX = 0.8f;
	float slashHitboxOffsetY = 0.4f;
	Vector3 slashHitboxSize = new Vector3(1,0.8f,0.2f);
	Vector3 dashHitboxSize = new Vector3(1,1.2f,0.2f);

	public GameObject hurtbox;
	public GameObject interactbox;
	public SpriteRenderer dashTrail;
	float trailDuration = 0.05f;
	float trailTimer = 0;
	float currentDashLength;

	float invulnTimer;
	float invulnDuration;
	const float defaultInvulnDuration = 1f;

	bool crouchInput;
	bool isCrouching;
	public bool IsCrouching{
		get{ return isCrouching; }
	}
	public BoxCollider levelCollider;
	Vector3 colliderCenterStanding = new Vector3(0,0.2f,0);
	Vector3 colliderSizeStanding = new Vector3(0.9f,1.4f,1);
	Vector3 colliderCenterCrouching = new Vector3(0,-0.1f,0);
	Vector3 colliderSizeCrouching = new Vector3(0.9f,0.8f,1);

    // Start is called before the first frame update
    void Awake()
    {
        _animator = GetComponent<Animator>();
		_spriteRenderer = GetComponent<SpriteRenderer>();
		_palSprite = GetComponent<PaletteSprite>();
		_audioSource = GetComponent<AudioSource>();
		SetFacing(1);
		DeactivateHitbox();
		DisableTrail();
		jumpHitbox.gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
		if(invulnTimer > 0){
			invulnTimer -= Time.deltaTime;
			_spriteRenderer.enabled = !_spriteRenderer.enabled;
			if(invulnTimer <= 0){
				EndInvuln();
			}
		}
		if(trailTimer > 0){
			trailTimer -= Time.deltaTime;
			float len = Mathf.Lerp(0, currentDashLength, trailTimer / trailDuration);
			dashTrail.transform.localScale = new Vector3(len,1,1);
			if(trailTimer <= 0){
				trailTimer = 0;
				DisableTrail();
			}
		}
		if(!UnlockTable.PowerActive(UnlockID.Crouch)){
			crouchInput = false;
		}
		if(isCrouching){
			if(!crouchInput && _movement.CanUncrouch()){
				Uncrouch();
			}
		}
		else{
			if(crouchInput){
				Crouch();
			}
		}
    }

	public void SetPalette(PaletteIndex index){
		_palSprite.SetPalette(index);
	}

	public void SetFacing(float f){
		int fSign = PMath.GetSign(f);
		if(fSign != 0){
			facing = fSign;
		}
		if(facing < 0){
			_spriteRenderer.flipX = true;
		}
		if(facing > 0){
			_spriteRenderer.flipX = false;
		}
	}

	public void SetGrounded(bool grounded){
		_animator.SetBool("Grounded", grounded);
		if(downSlashEnabled){
			jumpHitbox.gameObject.SetActive(!grounded);
		}
	}

	public void SetCrouchInput(bool inputApplied){
		crouchInput = inputApplied;
	}

	void SetCrouching(bool crouch){
		isCrouching = crouch;
		_animator.SetBool("Crouching", isCrouching);
	}

	public void Crouch(){
		SetCrouching(true);
		levelCollider.center = colliderCenterCrouching;
		levelCollider.size = colliderSizeCrouching;
	}

	public void Uncrouch(){
		SetCrouching(false);
		levelCollider.center = colliderCenterStanding;
		levelCollider.size = colliderSizeStanding;
	}

	public void SetMoving(bool moving){
		_animator.SetBool("Moving", moving);
	}

	public void PlayHurtSound(){
		_audioSource.PlayOneShot(hurtClip);
	}

	public void PlayJumpSound(){
		_audioSource.PlayOneShot(jumpClip);
	}

	public void SetInvulnTimer(float duration = defaultInvulnDuration){
		duration = Mathf.Max(duration,0);
		invulnDuration = duration;
		invulnTimer = invulnDuration;
	}

	public void EndInvuln(){
		_spriteRenderer.enabled = true;
		invulnTimer = 0;
		hurtbox.SetActive(true);
	}

	public void PlaySlashAnim(){
		//_animator.SetBool("Slashing", true);
		slashHitbox.GetComponent<BoxCollider>().size = slashHitboxSize;
		slashHitbox.damage = 1;
		_animator.SetTrigger("Slashing");
		//_palSprite.StartFlash();
	}

	public void PlaySlashSound(){
		_audioSource.PlayOneShot(slashClip);
	}

	public void PlayDashSlashAnim(float dashLength){
		//_animator.SetBool("Slashing", true);
		_animator.SetBool("Dashing", true);
		_audioSource.PlayOneShot(slashClip);
		//_palSprite.StartFlash();
		Vector3 hitboxSize = dashHitboxSize;
		hitboxSize.x = Mathf.Abs(dashLength) + 2;
		slashHitbox.transform.localPosition = new Vector3((dashLength * 0.5f), slashHitboxOffsetY);
		slashHitbox.GetComponent<BoxCollider>().size = hitboxSize;
		slashHitbox.damage = 2;
		slashHitbox.gameObject.SetActive(true);
		slashHitbox.transform.parent = null;
		hurtbox.SetActive(false);
		dashTrail.gameObject.SetActive(true);
		currentDashLength = Mathf.Abs(dashLength);
		dashTrail.transform.localScale = new Vector3(currentDashLength,1,1);
		dashTrail.flipX = (dashLength < 0);
		trailTimer = trailDuration;
	}

	public void StopDashSlashAnim(){
		//_animator.SetBool("Slashing", true);
		_animator.SetBool("Dashing",false);
		_audioSource.PlayOneShot(slashClip);
		//_palSprite.StartFlash();
		hurtbox.SetActive(true);
		DeactivateHitbox();
		DisableTrail();
	}

	void DisableTrail(){
		dashTrail.gameObject.SetActive(false);
		dashTrail.transform.localScale = new Vector3(0,1,1);
	}


	public void PlayHurtAnim(){
		DisableInteractions();
		PlayHurtSound();
		_palSprite.StartFlash();
		_animator.SetTrigger("Hurt");
	}

	public void FinishHurtAnim(){
		_movement.hitstun = false;
		interactbox.SetActive(true);
		SetInvulnTimer();
	}

	public void PlayDieAnim(){
		DisableInteractions();
		_audioSource.PlayOneShot(dieClip);
		_palSprite.StartFlash();
		_animator.SetBool("Dead",true);
	}

	public void SetPlayerDead(){
		_movement.isDead = true;
		_animator.SetBool("Dead",true);
	}

	public void SlashEnded(){
		//_animator.SetBool("Slashing", false);
	}

	public void SetDownSlashActive(bool active){
		_animator.SetBool("ScytheJump",active);
		downSlashEnabled = active;
	}

	public void ActivateHitbox(){
		slashHitbox.transform.localPosition = new Vector3(Facing * slashHitboxOffsetX, slashHitboxOffsetY);
		slashHitbox.gameObject.SetActive(true);
		hurtbox.SetActive(false);
	}

	public void DeactivateHitbox(){
		slashHitbox.gameObject.SetActive(false);
		hurtbox.SetActive(true);
		slashHitbox.transform.parent = this.transform.parent;
		slashHitbox.transform.localPosition = Vector3.zero;
	}

	public void EnableInteractions(){
		hurtbox.SetActive(true);
		interactbox.SetActive(true);
	}

	public void DisableInteractions(){
		hurtbox.SetActive(false);
		interactbox.SetActive(false);
	}
}
