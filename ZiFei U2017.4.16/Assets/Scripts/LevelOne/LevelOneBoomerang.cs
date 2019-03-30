using UnityEngine;
using System.Collections;

public class LevelOneBoomerang : MonoBehaviour 
{
	public Transform m_boomerangBirthPos;												//回旋镖初始位置
	private int m_dir;																	//发射方向
	private int m_bomerangState = 0;													//回旋镖状态
	private float m_boomerangSpeed = 0.4f;												//指定回旋镖速度

	void Start()
	{
		m_dir = LevelOneGameManager.Instance.GetHeroStoneDir ();						//获取主角当前方向
	}

	void OnTriggerEnter2D(Collider2D colliderObj)					//进入碰撞检测区域
	{
		if(colliderObj.tag=="Monkey")								//回旋镖回来
		{
			m_bomerangState = 2;
			LevelOneGameManager.Instance.SetMonkeyBloodReduce(0.05f);//造成伤害
		}
		else if(colliderObj.tag=="Slime1")
		{
			m_bomerangState = 2;
			LevelOneGameManager.Instance.SetSlimeInjury(0, true);
		}
		else if(colliderObj.tag=="Slime2")
		{
			m_bomerangState = 2;
			LevelOneGameManager.Instance.SetSlimeInjury(1, true);
		}
	}

	void Update()
	{
		switch(m_bomerangState)															//判定回旋镖状态
		{
		case 0:
			if(LevelOneGameManager.Instance.GetBoomerangOut())							//如果发射了回旋镖
			{
				this.gameObject.transform.position = m_boomerangBirthPos.position;		//指定初始位置
				this.gameObject.GetComponent<SpriteRenderer>().enabled = true;			//显示图片
				m_dir = LevelOneGameManager.Instance.GetHeroStoneDir ();				//获取主角当前方向
				m_bomerangState = 1;													//回旋镖下一状态
			}
			break;
		case 1:																			//回旋镖正在飞出去
			if(m_dir==0)												//回旋镖向上飞
			{
				if(this.transform.position.y<49f)						//如果石头还没落出上边界
					this.transform.Translate(0f, m_boomerangSpeed, 0f);	//石头上升
				else 													//石头落出上边界
					m_bomerangState = 2;
			}
			else if(m_dir==1)											//回旋镖朝左飞
			{
				if(this.transform.position.x>-4.9f)						//如果石头还没落出上边界
					this.transform.Translate(-m_boomerangSpeed, 0f, 0f);//石头下落
				else 													//石头落出底边界
					m_bomerangState = 2;
			}

			else if(m_dir==2)											//回旋镖朝右飞
			{
				if(this.transform.position.x<4.9f)						//如果石头还没落出右边界
					this.transform.Translate(m_boomerangSpeed, 0f, 0f);	//石头下落
				else 													//石头落出底边界
					m_bomerangState = 2;
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
		LevelOneGameManager.Instance.SetBoomerangOut(false);							//回旋镖射出状态归零
		this.gameObject.GetComponent<SpriteRenderer>().enabled = false;
	}
}
