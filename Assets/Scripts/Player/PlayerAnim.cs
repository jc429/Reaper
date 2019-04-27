using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnim : MonoBehaviour
{
	Animator _animator;
	SpriteRenderer _spriteRenderer;
	[SerializeField]
	PlayerMovement _movement;

	int facing;
    // Start is called before the first frame update
    void Awake()
    {
        _animator = GetComponent<Animator>();
		_spriteRenderer = GetComponent<SpriteRenderer>();
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
	}

	public void SlashEnded(){
		_animator.SetBool("Slashing", false);
	}
}
