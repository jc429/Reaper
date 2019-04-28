using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BreakableBlock : Destructible
{
	SpriteRenderer _spriteRenderer;
	PaletteSprite _palSprite;

	public Sprite[] spriteList;
	public int hp;

	float dieDuration = 0.2f;
	float deathTimer = -1;

    // Start is called before the first frame update
    void Awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
		_palSprite = GetComponent<PaletteSprite>();
    }

    // Update is called once per frame
    void Update()
    {
        if(deathTimer > 0){
			deathTimer -= Time.deltaTime;
			if(deathTimer <= 0){
				Die();
			}
		}
    }

	public override void TakeDamage(int dmg = 1){
		if(_palSprite != null){
			_palSprite.StartFlash();
		}
		hp -= dmg;
		hp = Mathf.Clamp(hp, 0, spriteList.Length - 1);
		_spriteRenderer.sprite = spriteList[(spriteList.Length - 1) - hp];
		if(hp <= 0){
			deathTimer = dieDuration;
		}
	}

	void Die(){
		gameObject.SetActive(false);
	}
}
