using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class UnlockTable {
	const bool DEBUG_ALL_UNLOCKS = false;

	static bool[] unlocks = new bool[13];
	static bool[] activeUnlocks = new bool[13];

	static UnlockID currentPaletteMode;
	static PurchaseItem palletCaller;

	public static void Clear(){
		for(int i = 0; i < 12; i++){
			unlocks[i] = false;
			activeUnlocks[i] = false;
		}
		unlocks[(int)UnlockID.BlackMode] = true;
		activeUnlocks[(int)UnlockID.BlackMode] = true;
		currentPaletteMode = UnlockID.BlackMode;
		palletCaller = null;
	}

	public static void UnlockPower(UnlockID unlockID){
		unlocks[(int)unlockID] = true;
		SetPowerActive(unlockID);
	}

	public static void SetPowerActive(UnlockID unlockID, bool active = true, PurchaseItem caller = null){
		activeUnlocks[(int)unlockID] = active;
		//Debug.Log("Power "+ unlockID + (active ? " " : " de") + "activated");
		if(unlockID == UnlockID.DownSlash){
			if(GameController.instance.player != null){
				GameController.instance.player.SetDownSlashUnlocked(active);
			}
		}
		if(active && caller != null){
			switch(unlockID){
			case UnlockID.RedMode:
				if(palletCaller != null){
					palletCaller.unlockID = currentPaletteMode;
				}
				palletCaller = caller;
				caller.unlockID = UnlockID.BlackMode;
				currentPaletteMode = UnlockID.RedMode;
				caller.unlockID = UnlockID.BlackMode;
				GameController.instance.player.Anim.SetPalette(PaletteIndex.Red);
				break;
			case UnlockID.BlueMode:
				if(palletCaller != null){
					palletCaller.unlockID = currentPaletteMode;
				}
				palletCaller = caller;
				currentPaletteMode = UnlockID.BlueMode;
				caller.unlockID = UnlockID.BlackMode;
				GameController.instance.player.Anim.SetPalette(PaletteIndex.Blue);
				break;
			case UnlockID.GreenMode:
				if(palletCaller != null){
					palletCaller.unlockID = currentPaletteMode;
				}
				palletCaller = caller;
				caller.unlockID = UnlockID.BlackMode;
				currentPaletteMode = UnlockID.GreenMode;
				caller.unlockID = UnlockID.BlackMode;
				GameController.instance.player.Anim.SetPalette(PaletteIndex.Green);
				break;
			case UnlockID.BlackMode:
				if(palletCaller != null){
					palletCaller.unlockID = currentPaletteMode;
				}
				palletCaller = null;
				currentPaletteMode = UnlockID.BlackMode;
				GameController.instance.player.Anim.SetPalette(PaletteIndex.Black);
				break;
			default:
				break;
			}
		}
	}

	public static bool PowerUnlocked(UnlockID unlockID){
		if(GameController.DEBUG_MODE && DEBUG_ALL_UNLOCKS){
			return true;
		}
		return unlocks[(int)unlockID];
	}

	public static bool PowerActive(UnlockID unlockID){
		return activeUnlocks[(int)unlockID];
	}
}

public enum UnlockID{
	Jump,
	AirJump,
	WallJump,
	Slash,
	DownSlash,
	Crouch,
	MoveSpeed1,
	MoveSpeed2,
	DashSlash,
	RedMode,
	BlueMode,
	GreenMode,
	BlackMode
}
