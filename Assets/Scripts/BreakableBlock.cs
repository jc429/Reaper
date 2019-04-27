using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BreakableBlock : MonoBehaviour
{
	SpriteRenderer _spriteRenderer;
	public Sprite[] spriteList;
	public int hp;
    // Start is called before the first frame update
    void Awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

	void TakeDamage(){
		hp--;
		hp = Mathf.Clamp(hp, 0, spriteList.Length - 1);
		_spriteRenderer.sprite = spriteList[(spriteList.Length - 1) - hp];
		if(hp == 0){
			Die();
		}
	}

	void Die(){
		
	}
}
