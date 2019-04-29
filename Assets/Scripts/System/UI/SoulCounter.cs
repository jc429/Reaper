using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SoulCounter : MonoBehaviour
{
	public TextMeshProUGUI textbox;

	public UINumber[] uiNumbers;

	int targetCount;
	int currentCount;
    // Start is called before the first frame update
    void Start()
    {
		currentCount = 0;
		UpdateCount(SoulWallet.SoulCount);
    }

	void Update(){
		if(currentCount > SoulWallet.SoulCount){
			UpdateCount(--currentCount);
		}
		if(currentCount < SoulWallet.SoulCount){
			UpdateCount(++currentCount);
		}
	}

    public void UpdateCount(int count)
    {
    	count = Mathf.Clamp(count,0,9999);
		int rDigit = 3;
		if(count < 10){
			rDigit = 0;
		}
		else if(count < 100){
			rDigit = 1;
		}
		else if(count < 1000){
			rDigit = 2;
		}

		for(int i = 0; i < 4; i++){
			if(i > rDigit){
				uiNumbers[i].SetEnabled(false);
			}
			else{
				int num = count / (int)Mathf.Pow(10,rDigit - i);
				uiNumbers[i].SetDigit(num);
			}
		}

    }
	



}
