using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LevelTwoTentacleController : MonoBehaviour 
{
	public GameObject m_tentacleHero;									//触角中的主角
	public GameObject[] m_tentacleColliderObj;							//触角的碰撞检测体
	public GameObject[] m_tentacle;										//触角

	private int[] m_tentacleState = new int[3];							//碰到触角的状态
	private float[] m_tentacleTimer = new float[3];						//触角1运动计时器
	private Rect m_heroColliderTect;									//主角碰撞包围盒

	
	//private bool isHurtByTentacle = false;
	private int touchTentacleCount = 0;

	List <int > tentacleIndexList = new List<int> ();



	void Update()
	{
        if (LevelTwoGameManager.Instance.GetBloodNum() <=0) return;

		Bounds rr1 = m_tentacleHero.GetComponent<Collider2D>().bounds;					//主角的包围盒
		m_heroColliderTect = new Rect(rr1.center.x - rr1.size.x / 2,
			rr1.center.y - rr1.size.y / 2,
			rr1.size.x, rr1.size.y);	
		for(int i=0; i<m_tentacle.Length; i++)							//遍历三个触角
			TentacleState(i);											//判定当前触角状态

		if (touchTentacleCount == 3) {
			if (LevelTwoGameManager.Instance.GetAchieveGot(12) == 0)
			{
				AudioManager.Instance.TipsSoundPlay(Global.GetInstance().audioName_AchieveGet);
				LevelTwoGameManager.Instance.SetAchieveGot(12, 1);
				LevelTwoGameManager.Instance.SetMessageType(3, "您获得了成就【连环计】");

				touchTentacleCount = -1;
			}
		}
	}

	void TentacleState(int _tentacleIndex)
	{
		switch(m_tentacleState[_tentacleIndex])											//判定触角的状态
		{
		case 0:	
			//该状态下可检测主角
			for(int i=_tentacleIndex*3; i<(_tentacleIndex*3+3); i++)					//遍历触角碰撞检测盒
			{
				Bounds rr2 = m_tentacleColliderObj[i].GetComponent<Collider>().bounds;					//获取触角的碰撞检测盒
				Rect r2 = new Rect(rr2.center.x - rr2.size.x / 2, rr2.center.y - rr2.size.y / 2, rr2.size.x, rr2.size.y);
				if(m_heroColliderTect.Overlaps(r2))										//如果主角碰到触角的碰撞检测盒
				{
                        AudioManager.Instance.ThingSoundPlay(Global.GetInstance().audioName_LevelTwo_LevelTwoTentacle);

                        m_tentacleState[_tentacleIndex] = 1;								//进入触角激活状态
					m_tentacle[_tentacleIndex].GetComponent<Animator>().SetBool("tentacleMove", true);	//开启触角下落动画
					m_tentacleTimer[_tentacleIndex] = 1.64f;							//触角下落计时器
					CalculateTouchCount(_tentacleIndex);
				}
			}
			break;
		case 1:																			//触角激活状态
			m_tentacleTimer[_tentacleIndex] -= Time.deltaTime;							//触角下落动画计时器
			if(m_tentacleTimer[_tentacleIndex]>0.2f&&m_tentacleTimer[_tentacleIndex]<1.5f)
			{
				Bounds rr2 = m_tentacleColliderObj[_tentacleIndex*3+2].GetComponent<Collider>().bounds;	//获取触角的碰撞检测盒
				Rect r2 = new Rect(rr2.center.x - rr2.size.x / 2, rr2.center.y - rr2.size.y / 2, rr2.size.x, rr2.size.y);
				if (m_heroColliderTect.Overlaps (r2)) {										//如果主角碰到触角的碰撞检测盒
					LevelTwoGameManager.Instance.SetHeroBloodReduce (0.005f);
					//isHurtByTentacle = true;//hero 受到 触手触手的攻击
				}
			}
			if(m_tentacleTimer[_tentacleIndex]<0)										//触角下落结束
			{
				m_tentacle[_tentacleIndex].GetComponent<Animator>().SetBool("tentacleMove", false);	//恢复静止动画
				m_tentacleState[_tentacleIndex] = 2;									//进入潜伏状态
				m_tentacleTimer[_tentacleIndex] = 5f;									//潜伏状态时长
			}
			break;
		case 2:																			//潜伏状态
			m_tentacleTimer[_tentacleIndex] -= Time.deltaTime;							//潜伏状态计时器
			if(m_tentacleTimer[_tentacleIndex]<0)
				m_tentacleState[_tentacleIndex] = 0;									//触角可被激活状态
			break;
		}
	}




	void CalculateTouchCount(int tentacleIndex){

		if (tentacleIndexList.Contains (tentacleIndex))
			return;
		tentacleIndexList.Add (tentacleIndex);
		touchTentacleCount++;
	}
}
