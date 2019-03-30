using UnityEngine;
using System.Collections;

public class LevelThreeSwordController : MonoBehaviour 
{
	private float m_swordSpeedX = 0.2f;								//剑的水平速度

	void OnTriggerEnter2D(Collider2D colliderObj)					//进入碰撞检测区域
	{
		if(colliderObj.tag=="Hero")									//打中主角
		{
			Destroy(this.gameObject);								//销毁石头
			LevelThreeGameManager.Instance.SetHeroBloodReduce(0.005f);//主角血量减少
		}
	}

	void Update()
	{
		m_swordSpeedX += 0.05f;
		this.transform.Translate (-m_swordSpeedX, 0f, 0f);			//剑水平向右飞
		if(this.transform.position.x<=-25f)							//剑飞出右边界 消失
			Destroy(this.gameObject);	
	}

}
