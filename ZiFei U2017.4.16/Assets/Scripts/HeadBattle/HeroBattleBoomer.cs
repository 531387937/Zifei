using UnityEngine;
using System.Collections;

public class HeroBattleBoomer : MonoBehaviour 
{
	public Transform m_boomerangBirthPos;												//回旋镖的起始位置
	private float m_boomerangSpeed = 0.5f;												//回旋镖移动速度
	private float m_heroScaleX = 1;														//主角朝向
	private int m_bomerangState = 0;													//回旋镖的状态

	void OnTriggerEnter2D(Collider2D colliderObj)										//进入碰撞检测区域
	{
		if(m_bomerangState==1)
		{
			if(colliderObj.tag=="CountryHead")											//打中村长
			{
				m_bomerangState = 2;													//回旋镖回来
				if(HeadBattleGameManager.Instance.GetAttackType()==2||HeadBattleGameManager.Instance.GetAttackType()==3)
					HeadBattleGameManager.Instance.SetHeadBloodReduce(0.04f);
				else
					HeadBattleGameManager.Instance.SetHeadBloodReduce(0.02f);
			}
			else if(colliderObj.tag=="EdgeLeft"||colliderObj.tag=="EdgeRight")			//打中边界
				m_bomerangState = 2;													//回旋镖回来
		}
	}


	void Update()
	{
		switch(m_bomerangState)															//判定回旋镖状态
		{
		case 0:
			if(HeadBattleGameManager.Instance.GetBoomerangOut())						//如果发射了回旋镖
			{
				this.gameObject.transform.position = m_boomerangBirthPos.position;
				this.gameObject.GetComponent<SpriteRenderer>().enabled = true;
				m_heroScaleX = HeadBattleGameManager.Instance.GetHeroScaleX ();			//获取此时主角的朝向
				m_boomerangSpeed = 0.3f*m_heroScaleX;									//回旋镖移动速度
				m_bomerangState = 1;													//回旋镖下一状态
			}
			break;
		case 1:																			//回旋镖正在飞出去
			if(m_boomerangSpeed>0)														//如果正向右飞
			{
				if(this.transform.position.x<30f)										//如果回旋镖未超出边界
					this.transform.Translate(m_boomerangSpeed, 0f, 0f);					//回旋镖飞出去
				else 																	//回旋镖飞出右边界
					m_bomerangState = 2;												//回旋镖下一状态
			}
			else 																		//如果正向左飞
			{
				if(this.transform.position.x>-30f)										//如果回旋镖未超出边界
					this.transform.Translate(m_boomerangSpeed, 0f, 0f);					//回旋镖飞出去
				else 																	//回旋镖飞出右边界
					m_bomerangState = 2;												//回旋镖下一状态
			}
			break;
		case 2:																			//回旋镖要回来
			iTween.MoveTo(this.gameObject, iTween.Hash(
				"position",m_boomerangBirthPos.position,"time",0.2f,"onComplete","OnBoomerangMoveEnd"));
			m_bomerangState = 3;														//下一状态
			break;
		case 3:
			break;
		}
	}
	
	void OnBoomerangMoveEnd()															//回旋镖已回归
	{
		m_bomerangState = 0;															//回旋镖可继续监测发射
		HeadBattleGameManager.Instance.SetBoomerangOut(false);							//回旋镖射出状态归零
		this.gameObject.GetComponent<SpriteRenderer>().enabled = false;
	}
}
