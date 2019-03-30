using UnityEngine;
using System.Collections;

public class LevelTwoSpiderWaveController : MonoBehaviour 
{
	void OnTriggerEnter2D(Collider2D colliderObj)					//进入碰撞检测区域
	{
		if(colliderObj.tag=="Hero")									//打中主角
			LevelTwoGameManager.Instance.SetHeroBloodReduce(0.1f);	//主角受伤
	}
}
