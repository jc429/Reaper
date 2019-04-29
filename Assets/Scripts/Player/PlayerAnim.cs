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

	int facing;
	public int Facing{
		get{ return facing; }
	}

	public GameObject slashHitbox;
	float slashHitboxOffsetX = 0.8f;
	float slashHitboxOffsetY = 0.4f;

	public GameObject hurtbox;
	public GameObject interactbox;

    // Start is called before the first frame update
    void Awake()
    {
        _animator = GetComponent<Animator>();
		_spriteRenderer = GetComponent<SpriteRenderer>();
		_palSprite = GetComponent<PaletteSprite>();
		_audioSource = GetComponent<AudioSource>();
		SetFacing(1);
		DeactivateHitbox();
    }

    // Update is called once per frame
    void Update()
    {
		
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
	}

	public void SetMoving(bool moving){
		_animator.SetBool("Moving", moving);
	}

	public void PlayJumpSound(){
		_audioSource.PlayOneShot(jumpClip);
	}

	public void PlaySlashAnim(){
		//_animator.SetBool("Slashing", true);
		_animator.SetTrigger("Slashing");
		_audioSource.PlayOneShot(slashClip);
		//_palSprite.StartFlash();
	}

	public void PlayDashSlashAnim(){
		//_animator.SetBool("Slashing", true);
		_animator.SetTrigger("Dashing");
		_audioSource.PlayOneShot(slashClip);
		//_palSprite.StartFlash();
	}

	public void PlayHurtAnim(){
		DisableInteractions();
		_palSprite.StartFlash();
		_animator.SetTrigger("Hurt");
	}

	public void FinishHurtAnim(){
		_movement.hitstun = false;
		EnableInteractions();
	}

	public void PlayDieAnim(){
		DisableInteractions();
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

	public void UnlockScytheJump(){
		_animator.SetBool("ScytheJump",true);
	}

	public void ActivateHitbox(){
		slashHitbox.transform.localPosition = new Vector3(Facing * slashHitboxOffsetX, slashHitboxOffsetY);
		slashHitbox.SetActive(true);
		hurtbox.SetActive(false);
	}

	public void DeactivateHitbox(){
		slashHitbox.SetActive(false);
		hurtbox.SetActive(true);
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
