using UnityEngine;
using System.Collections;

public class HeadBattleHatWave : MonoBehaviour
{
	void OnTriggerEnter2D(Collider2D colliderObj)					//进入碰撞检测区域
	{
		if(colliderObj.tag=="Hero")									//打中主角
			HeadBattleGameManager.Instance.SetHeroBloodReduce(0.2f);	//主角受伤
	}
}
