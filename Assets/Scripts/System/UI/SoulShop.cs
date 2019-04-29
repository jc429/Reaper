using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoulShop : MonoBehaviour
{
	public GameObject shopPanel;
	static bool isOpen;
	public static bool IsOpen{
		get { return isOpen; }
	}

    // Start is called before the first frame update
    void Start()
    {
        CloseShop();
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape)){
			if(isOpen){
				CloseShop();
			}
			else{
				OpenShop();
			}
		}
    }

	public void OpenShop(){
		GameController.FreezeGame(true);
		shopPanel.SetActive(true);
		isOpen = true;
	}

	public void CloseShop(){
		GameController.FreezeGame(false);
		shopPanel.SetActive(false);
		isOpen = false;
	}
}
