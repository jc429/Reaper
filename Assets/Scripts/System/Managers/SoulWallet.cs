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
}
