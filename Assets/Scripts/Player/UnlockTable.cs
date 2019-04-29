using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class UnlockTable {
	static bool[] unlocks = new bool[12];

	public static void Clear(){
		for(int i = 0; 0 < 12; i++){
			unlocks[i] = false;
		}
	}

	public static void UnlockPower(UnlockID unlockID){
		unlocks[(int)unlockID] = true;
		if(unlockID == UnlockID.DownSlash){
			if(GameController.instance.player != null){
				GameController.instance.player.UnlockDownSlash();
			}
		}
	}

	public static bool PowerUnlocked(UnlockID unlockID){
		return unlocks[(int)unlockID];
	}
}

public enum UnlockID{
	Jump,
	AirJump,
	WallJump,
	Slash,
	DownSlash,
	UpSlash,
	MoveSpeed1,
	MoveSpeed2,
	DashSlash,
	RedMode,
	BlueMode,
	GreenMode,
}
