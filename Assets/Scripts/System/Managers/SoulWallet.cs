using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class SoulWallet
{
	static int soulCount;
	public static int SoulCount{
		get{
			return soulCount;
		}
	}

	public static void Reset(){
		soulCount = 0;
	}

	public static void CollectSoul(SoulCollectible sc){
		soulCount++;
	}

	public static int LoseSouls(){
		int loss = 0;
		if(soulCount == 0){
			GameController.instance.player.Die();
		}
		else if(soulCount == 1){
			soulCount = 0;
		}
		else if(soulCount < 25){
			loss = soulCount - 1;
		}
		else{
			loss = Mathf.Min(soulCount / 2, 50);
		}
		soulCount -= loss;
		return loss;
	}
}
