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

	public void PlayDieAnim(){
		_animator.SetTrigger("Dead");
	}

	public void SetPlayerDead(){
		_movement.isDead = true;
	}

	public void SlashEnded(){
		//_animator.SetBool("Slashing", false);
	}

	public void ActivateHitbox(){
		slashHitbox.transform.localPosition = new Vector3(Facing * slashHitboxOffsetX, slashHitboxOffsetY);
		slashHitbox.gameObject.SetActive(true);
	}

	public void DeactivateHitbox(){
		slashHitbox.gameObject.SetActive(false);
		slashHitbox.transform.localPosition = Vector3.zero;
	}
}
