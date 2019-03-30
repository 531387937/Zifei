using UnityEngine;
using System.Collections;

public class LevelOneStoneController : MonoBehaviour 
{
	private float m_stoneSpeed = 0.002f;							//石头下落速度
	private int m_stoneDir = 0;

	void Start()
	{
		m_stoneDir = LevelOneGameManager.Instance.GetMonkeyStoneDir ();
	}

	void OnTriggerEnter2D(Collider2D colliderObj)					//进入碰撞检测区域
	{
		if(colliderObj.tag=="Hero")
		{
			Destroy(this.gameObject);								//销毁石头
			LevelOneGameManager.Instance.SetHeroBloodReduce(0.05f);	//主角损失血量
		}
	}

	void Update()
	{
		switch(m_stoneDir)
		{
		case 0:
			m_stoneSpeed += 0.005f;										//保证石头加速下落
			if(this.transform.position.y>0)								//如果石头还没落出底边界
				this.transform.Translate(0f, -m_stoneSpeed, 0f);		//石头下落
			else 														//石头落出底边界
				Destroy(this.gameObject);								//销毁石头
			break;
		case 1:
			if(this.transform.position.x>-4.9f)							//如果石头还没落出左边界
				this.transform.Translate(-0.1f, 0f, 0f);				//石头向左飞
			else 														//石头落出左边界
				Destroy(this.gameObject);								//销毁石头
			break;
		case 2:
			if(this.transform.position.x<4.9f)							//如果石头还没落出右边界
				this.transform.Translate(0.1f, 0f, 0f);					//石头向右飞
			else 														//石头落出右边界
				Destroy(this.gameObject);								//销毁石头
			break;
		}

	}
}
