using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class UnlockTable {
	public static bool rightUnlocked = true;
	public static bool leftUnlocked = true;

	public static bool canJump = false;
	public static int numAirJumps = 0;
	public static int airDriftLevel = 0;

	

	public static void UnlockPower(PowerUnlock unlock){
		switch(unlock){
			case PowerUnlock.Jump:
				canJump = true;
				break;
			case PowerUnlock.AirJump:
				numAirJumps++;
				break;




			default:
				break;
		}
	}
}

public enum PowerUnlock{
	Jump,
	AirJump,
	Slash
}
