using UnityEngine;

public class Timer {
	public float time;
	public float duration; 
	public bool isFinished;
	public bool isActive;
	
	public void SetActive(bool active){
		isActive = active;
	}

	public void AdvanceTimer(float dTime){
		time += dTime;
		if(time >= duration){
			time = duration;
			Finish();
		}
	}

	public void Finish(){
		isFinished = true;
	}

	public void ResetTimer(){
		isFinished = false;
		time = 0;
		isActive = false;
	}

	public float GetCompletionPercentage(){
		return time/duration;
	}

}

public class TimerV3 : Timer{
	public Vector3 start;
	public Vector3 end;

	public void SetAttributes(Vector3 startPos, Vector3 endPos, float length = 1){
		start = startPos;
		end = endPos;
		duration = length;
		ResetTimer();
	}
}

public class TimerF : Timer {
	public float start;
	public float end;

	
	public void SetAttributes(float startPos, float endPos, float length = 1){
		start = startPos;
		end = endPos;
		duration = length;
		ResetTimer();
	}
}

