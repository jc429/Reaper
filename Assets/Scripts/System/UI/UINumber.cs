using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UINumber : MonoBehaviour
{
	public Sprite[] digitSprites;
	Image _image;
	
	public int currentDigit;

    // Start is called before the first frame update
    void Awake()
    {
        _image = GetComponent<Image>();
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
		_image.sprite = digitSprites[currentDigit];
		SetEnabled(true);
	}
}
