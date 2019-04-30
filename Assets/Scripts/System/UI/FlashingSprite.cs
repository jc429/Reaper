using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FlashingSprite : MonoBehaviour
{
	public float speed;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        float val = Mathf.Sin(Time.time * speed);
		GetComponent<Image>().enabled = (val < 0);


    }
}
