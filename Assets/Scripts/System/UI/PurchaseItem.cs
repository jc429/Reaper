using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PurchaseItem : MonoBehaviour
{
	public UnlockID unlockID;
	public int itemCost;
	bool purchased;
	public bool Purchased{
		get{ return purchased; }
	}
	public UINumber[] uiNumbers;
	int buttonWidth = 24;
	public Image buttonPanel;
	public Image iconPanel;
	public Sprite obtainedPanel; 
	//public PaletteSprite[] palSprites;

    // Start is called before the first frame update
    void Start()
    {
        UpdateCount(itemCost);
    }

    // Update is called once per frame
    void Update()
    {
		if(purchased && buttonWidth > 1){
			buttonWidth--;
			buttonPanel.rectTransform.sizeDelta = new Vector2(buttonWidth,13);
		}
    }

	public void UpdateCount(int count)
    {
    	count = Mathf.Clamp(count,0,999);
		int rDigit = 2;
		if(count < 10){
			rDigit = 0;
		}
		else if(count < 100){
			rDigit = 1;
		}

		for(int i = 0; i < 3; i++){
			if(i > rDigit){
				uiNumbers[i].SetEnabled(false);
			}
			else{
				int num = count / (int)Mathf.Pow(10,rDigit - i);
				uiNumbers[i].SetDigit(num);
			}
		}
	}

	public void AttemptPurchase(){
		
		if(itemCost <= SoulWallet.SoulCount){
			ConfirmPurchase();
		}
		if(GameController.DEBUG_MODE){
			ConfirmPurchase();
		}
	}

	public void ConfirmPurchase(){
		SoulWallet.SpendSouls(itemCost);
		UnlockTable.UnlockPower(unlockID);
		purchased = true;
		iconPanel.sprite = obtainedPanel;
		for(int i = 0; i < 3; i++){
			uiNumbers[i].SetEnabled(false);
		}
	}
}
