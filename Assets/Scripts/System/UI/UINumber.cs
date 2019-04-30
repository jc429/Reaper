using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UINumber : MonoBehaviour
{
	public Sprite[] digitSprites;
	public Sprite[] dullSprites;
	Image _image{
		get{ return GetComponent<Image>(); }
	}
	
	public int currentDigit;

	public bool useDullSprites;

    // Start is called before the first frame update
    void Awake()
    {
    }

    // Update is called once per frame
    void Update()
    {
        
    }

	public void SetEnabled(bool b){
		_image.enabled = b;
	}

	public void SetDigit(int digit){
		digit = digit % 10;
		currentDigit = digit;
		_image.sprite = (useDullSprites) ? dullSprites[currentDigit] : digitSprites[currentDigit];
		SetEnabled(true);
	}

	public void SetDull(bool dull){
		useDullSprites = dull;
		_image.sprite = (useDullSprites) ? dullSprites[currentDigit] : digitSprites[currentDigit];
	}
}
