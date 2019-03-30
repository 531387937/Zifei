using UnityEngine;
using System.Collections;

public class LevelThreeLightController : MonoBehaviour 
{
	public GameObject m_lightHero;										//主角
	public GameObject m_lightRingObj;									//闪烁的光圈
	public Transform[] m_lightRingPos;									//闪烁的光圈的位置
	public GameObject[] m_endLightObj;									//闪烁的光图
	public GameObject m_newEdgeL;										//新的左边界

	private float m_lightTimer = 5f;									//光束计时器

	private int m_endLightState = 0;									//最后的光图状态		
	private int m_endLIghtBlinkState = 0;								//最后光图闪状态
	private int m_pillarIndex = 0;										//需要亮的柱子编号
	private int m_blinkCount= 0;										//游戏次数

	void Update()
	{
        if (LevelThreeGameManager.Instance.GetBloodNum() <= 0) return;
        
		switch(m_endLightState)
		{
		case 0:
			if((m_lightRingPos[1].transform.position.x - m_lightHero.transform.position.x)<=0f)	//主角到达第二根柱子
			{
				m_endLightState = 1;
				m_newEdgeL.GetComponent<BoxCollider2D>().enabled = true;						//开启新的左边界
				LevelThreeGameManager.Instance.SetCamTrackingValue(false);						//关闭相机跟随
				m_lightTimer = 1f;
			}
			break;
		case 1:																					//进入光束游戏阶段
			m_lightTimer -= Time.deltaTime;
			if(m_lightTimer<0)
			{
				m_endLightState = 2;
				m_endLIghtBlinkState = 1;
			}
			break;
		case 2:
			EndLightGame();
			break;
		case 3:
			m_endLightState = 4;
			LevelThreeGameManager.Instance.SetCamTrackingValue(true);
			m_newEdgeL.GetComponent<BoxCollider2D>().enabled = false;
			break;
		}
	}

	void EndLightGame()
	{
		switch(m_endLIghtBlinkState)
		{
		case 1:
			m_pillarIndex = Random.Range(0, 3);													//需要亮的柱子编号
			m_lightRingObj.transform.position = m_lightRingPos[m_pillarIndex].position;			//赋予光圈位置
			m_lightRingObj.SetActive(true);														//光圈开始闪
			m_lightTimer = 1f;																	//光圈闪烁计时器
			m_blinkCount++;
			m_endLIghtBlinkState = 2;
			break;
		case 2:
			m_lightTimer -= Time.deltaTime;
			if(m_lightTimer<0)
			{
				m_endLightObj[m_pillarIndex].SetActive(true);									//开启光波
				m_lightTimer = 3f;
				m_endLIghtBlinkState = 3;
                    AudioManager.Instance.ThingSoundPlay(Global.GetInstance().audioName_LevelThree_LevelThreeLightWave);
			}
			break;
		case 3:
			m_lightTimer -= Time.deltaTime;
			float _disX = m_lightHero.transform.position.x - m_lightRingPos[m_pillarIndex].position.x;
			if(Mathf.Abs(_disX)>=0.5f)															//如果主角没有到达指定柱子
			   LevelThreeGameManager.Instance.SetHeroBloodReduce(0.005f);					
			if(m_lightTimer<0)
			{
				m_endLIghtBlinkState = 4;
				m_endLightObj[m_pillarIndex].SetActive(false);									//关闭光波
				m_lightRingObj.SetActive(false);												//关闭光圈
				m_lightTimer = 1f;																//暂停一
			}
			break;
		case 4:
			m_lightTimer -= Time.deltaTime;
			if(m_lightTimer<0)
			{
				if(m_blinkCount==5)
				{
					m_endLIghtBlinkState = 0;													//游戏结束不玩了
					m_endLightState = 3;

					LevelThreeGameManager.Instance.isAccessCurrentScene = true;

                    if (GameDataManager.Instance.gameData.GameCurrentData.seedState[2] == 0) //种子的状态（0未得到 1得到未用 2种下）
                        LevelThreeGameManager.Instance.SetEndItem(1);                               //可以掉落通关物品,生成种子

					//在戴着 狐狸帽子的时候，通过了
					if (LevelThreeGameManager.Instance.GetItemEquipIndex() == 3 && LevelThreeGameManager.Instance.GetAchieveGot(8) == 0)//
					{
						AudioManager.Instance.TipsSoundPlay(Global.GetInstance().audioName_AchieveGet);
						LevelThreeGameManager.Instance.SetAchieveGot(8, 1);
						LevelThreeGameManager.Instance.SetMessageType(3, "您获得了成就【浑水摸鱼】");
					}
               	}
				else
					m_endLIghtBlinkState = 1;													//开始新一轮闪烁
			}
			break;
		}
	}
}
