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

	int facing;
	public int Facing{
		get{ return facing; }
	}

	public GameObject slashHitbox;
	float slashHitboxOffsetX = 0.8f;
	float slashHitboxOffsetY = 0.15f;

    // Start is called before the first frame update
    void Awake()
    {
        _animator = GetComponent<Animator>();
		_spriteRenderer = GetComponent<SpriteRenderer>();
		_palSprite = GetComponent<PaletteSprite>();
    }

    // Update is called once per frame
    void Update()
    {
		
    }

	public void SetFacing(float f){
		facing = PMath.GetSign(f);
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

	public void PlaySlashAnim(){
		_animator.SetBool("Slashing", true);
		//_palSprite.StartFlash();
	}

	public void SlashEnded(){
		_animator.SetBool("Slashing", false);
	}

	public void ActivateHitbox(){
		slashHitbox.transform.localPosition = new Vector3(Facing * slashHitboxOffsetX, slashHitboxOffsetY);
		slashHitbox.gameObject.SetActive(true);
	}

	public void DeactivateHitbox(){
		slashHitbox.transform.localPosition = Vector3.zero;
		slashHitbox.gameObject.SetActive(false);
	}
}
