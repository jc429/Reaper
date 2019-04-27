using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PaletteSprite : MonoBehaviour
{
	SpriteRenderer _spriteRenderer;
	Texture2D paletteTex;
	Color[] mSpriteColors;

	float flashTimer = 0.0f;
	const float flashDuration = 0.1f;

    // Start is called before the first frame update
    void Awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
		InitPaletteTex();
		
    }

    // Update is called once per frame
    void Update()
    {
        if (flashTimer > 0.0f) {
			flashTimer -= Time.deltaTime;
			if (flashTimer <= 0.0f){
				ResetColors();
			}
		}
    }

	public void InitPaletteTex(){
		Texture2D colorSwapTex;
		colorSwapTex = (Texture2D)_spriteRenderer.material.GetTexture("_SwapTex");
		if(colorSwapTex == null){
			colorSwapTex = new Texture2D(256, 1, TextureFormat.RGBA32, false, false);
			colorSwapTex.filterMode = FilterMode.Point;
			/*
			for (int i = 0; i < colorSwapTex.width; ++i){
				colorSwapTex.SetPixel(i, 0, new Color(0.0f, 0.0f, 0.0f, 0.0f));
			}
			colorSwapTex.Apply();*/	
			_spriteRenderer.material.SetTexture("_SwapTex", colorSwapTex);
		}
	
	
		mSpriteColors = new Color[colorSwapTex.width];
		paletteTex = colorSwapTex;
	}

	public void SwapColor(int index, Color color){
		mSpriteColors[index] = color;
		paletteTex.SetPixel(index, 0, color);
	}

	void FlashColor(Color color){
		for (int i = 0; i < paletteTex.width; ++i){
			mSpriteColors[i] = paletteTex.GetPixel(i,0);
			paletteTex.SetPixel(i, 0, color);
		}
		paletteTex.Apply();
	}

	public void ResetColors(){
		for (int i = 0; i < paletteTex.width; ++i){
			paletteTex.SetPixel(i, 0, mSpriteColors[i]);
		}
		paletteTex.Apply();
	}

	public void StartFlash(Color color){
		flashTimer = flashDuration;
		FlashColor(color);
	}
}
