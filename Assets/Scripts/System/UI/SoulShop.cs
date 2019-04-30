using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SoulShop : MonoBehaviour
{
	public GameObject shopPanel;
	static bool isOpen;
	public static bool IsOpen{
		get { return isOpen; }
	}

	public Image shopButton;
	public Sprite shoppingSprite;
	public Sprite backSprite;

	public PurchaseItem[] items;

    // Start is called before the first frame update
    void Start()
    {
        CloseShop();
    }

    // Update is called once per frame
    void Update()
    {
        if(VirtualController.PauseButtonPressed()){
			ToggleShop();
		}
    }

	public void OpenShop(){
		SFXManager.PlayClickClip();
		GameController.FreezeGame(true);
		UpdateItems();
		shopPanel.SetActive(true);
		isOpen = true;
		shopButton.sprite = backSprite;
	}

	public void CloseShop(){
		SFXManager.PlayClickClip();
		GameController.FreezeGame(false);
		shopPanel.SetActive(false);
		isOpen = false;
		shopButton.sprite = shoppingSprite;
	}

	public void ToggleShop(){
		if(isOpen){
			CloseShop();
		}
		else{
			OpenShop();
		}
	}

	public void UpdateItems(){
		foreach(PurchaseItem item in items){
			item.CheckCost();
		}
	}
}
