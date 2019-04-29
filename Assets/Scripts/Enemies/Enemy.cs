using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : Destructible
{
	Animator _anim;
	SpriteRenderer _spriteRenderer;
	PaletteSprite _palSprite;

	public int maxHP = 1;
	int hp;
	
	public SoulCollectible soulPrefab;

	int facing;
	public int startFacing = 1;
	
	float dieDuration = 0.2f;
	float deathTimer = -1;

	public float moveSpeed = 4f;
	public bool move = true;

    // Start is called before the first frame update
    protected virtual void Awake()
    {
		_anim = GetComponentInChildren<Animator>();
        _spriteRenderer = GetComponentInChildren<SpriteRenderer>();
		_palSprite = GetComponentInChildren<PaletteSprite>();
    }

	protected virtual void Start(){
		hp = maxHP;
		facing = startFacing;
	}

    // Update is called once per frame
    protected virtual void Update()
    {
        if(deathTimer > 0){
			deathTimer -= Time.deltaTime;
			if(deathTimer <= 0){
				Die();
			}
		}
		else if(move){
			Vector3 v = Vector3.zero;
			Vector3 moveDir = new Vector3(facing,0);
			v.x = moveSpeed * Time.deltaTime;
			
			Vector3 origin = transform.position;
			RaycastHit r;
			Physics.Raycast(origin, moveDir, out r, 0.5f + v.x, Layers.GetGroundMask(false));
			if(r.distance > 0){
				moveDir.x *= (r.distance - 0.5f);
				SetFacing(-1 * facing); 
			}
			else{
				moveDir.x *= v.x;
			}
			transform.position += moveDir;

			bool gf = Physics.Raycast(origin + new Vector3(facing * 0.4f, -0.5f), Vector3.down, out r, 0.65f, Layers.GetGroundMask(false));
			bool gb = Physics.Raycast(origin + new Vector3(facing * -0.4f, -0.5f), Vector3.down, out r, 0.65f, Layers.GetGroundMask(false));
			if(gb && !gf){
				SetFacing(-1 * facing); 
			}
			Debug.DrawRay(origin + new Vector3(facing * 0.4f, -0.5f), 0.65f * Vector3.down, (gf ? Color.green : Color.white));
			Debug.DrawRay(origin + new Vector3(facing * -0.4f, -0.5f), 0.65f * Vector3.down, (gb ? Color.green : Color.white));
		}
    }

	void OnTriggerEnter(Collider other){
		if(other.tag.Equals("Player")){
			GameController.instance.player.TakeDamage();
		}
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

	public override void TakeDamage(int dmg = 1){
		if(_palSprite != null){
			_palSprite.StartFlash();
		}
		hp -= dmg;
		hp = Mathf.Clamp(hp, 0, maxHP);

		if(hp <= 0){
			deathTimer = dieDuration;
		}
	}

	void Die(){
		SoulCollectible soul = Instantiate(soulPrefab);
		soul.transform.position = this.transform.position;
		soul.SetLaunchVelocity();
		GameObject poof = Instantiate(GameController.instance.poofPrefab);
		poof.transform.position = this.transform.position;
		Destroy(gameObject);
	}
}
