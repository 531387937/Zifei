using UnityEngine;
using System.Collections;

public class LevelOneMonkeyController : MonoBehaviour 
{
	public enum m_monkeyStates													//猴子的状态
	{	
		idle = 0, 																//空闲状态	
		attack,																	//攻击状态
		die,																	//受伤状态
	}


    public enum m_monkeyChairStates 											//猴子底座状态
	{
		idle,																	//空闲状态
		smoke,																	//冒烟状态
		fire,																	//有火苗状态
	}


	public delegate void monkeyStateHandler(LevelOneMonkeyController.m_monkeyStates _newState);
	public static event monkeyStateHandler onMonkeyStateChange; 				//定义猴子的委托和事件
	public delegate void monkeyChairStateHandle(LevelOneMonkeyController.m_monkeyChairStates _newState); 
	public static event monkeyChairStateHandle onMonkeyChairStateChange;		//定义猴子底座的委托和事件


	public Transform m_heroTrans;												//获取主角位置
	public UISprite m_monkeyUIEye;											//UI面板猴子眼睛
    public AudioSource monkeyInjuryAudio;
    private float m_monkeyTimer = 2f;											//猴子攻击计时器	
	private int m_monkeyStateIndex = 1;											//猴子状态索引 （1idle 2attack）
	private float m_monkeyHorizontalMoveTimer = 1f;								//猴子水平运动随机数计时器
	private int m_monkeyHorizontalMoveState = 0;								//猴子水平运动状态值
	private float m_monkeyHorizontalSpeed = 0.05f;								//猴子水平运动速度
	private float m_monkeyVerticalMoveTimer = 1f;								//猴子水平运动随机数计时器
	private int m_monkeyVerticalMoveState = 0;									//猴子水平运动状态值
	private float m_monkeyVerticalSpeed = 0.1f;								//猴子水平运动速度
	private float m_monkeyInjuryTimer = 0f;										//猴子受伤计时器
	private int m_monkeyInjuryBlinkCount = 0;									//猴子受伤闪烁次数
	private float m_monkeyDieTimer = 1f;										//猴子死亡计时器




    void HorizontalMove()														//猴子水平运动控制
	{
		m_monkeyHorizontalMoveTimer -= Time.deltaTime;
		if(m_monkeyHorizontalMoveTimer<0)										//每一秒产生一个随机数
		{
			m_monkeyHorizontalMoveState = Random.Range (0, 5);
			m_monkeyHorizontalMoveTimer = 1f;
		}
		switch(m_monkeyHorizontalMoveState)										//根据随机数判定猴子作何运动
		{
		case 0:																	//猴子静止
			break;
		case 1:																	//猴子向右运动
		case 2:
			if(this.transform.position.x<3.72f)
				this.transform.Translate(m_monkeyHorizontalSpeed, 0f, 0f);
			break;
		case 3:																	//猴子向左运动
		case 4:
			if(this.transform.position.x>-3.88f)
				this.transform.Translate(-m_monkeyHorizontalSpeed, 0f, 0f);
			break;
		}
	}

	void VerticalMove()															//猴子竖直运动控制
	{
		bool _isUp = false;														//猴子是否上升
		float _verticalDis = this.transform.position.y - m_heroTrans.position.y;//猴子与主角的竖直距离
		if(_verticalDis>3.25&&_verticalDis<6.5)
		{
			m_monkeyVerticalMoveTimer -= Time.deltaTime;
			if(m_monkeyVerticalMoveTimer<0)
			{
				m_monkeyVerticalMoveState = Random.Range(0,3);
				m_monkeyVerticalMoveTimer = 1f;
			}
			if(m_monkeyVerticalMoveState!=0)
			{
				this.transform.Translate(0f, m_monkeyVerticalSpeed/2f, 0f);
				_isUp = true;
			}
		}
		else if(_verticalDis<3.25)
		{
			this.transform.Translate(0f, m_monkeyVerticalSpeed, 0f);
			_isUp = true;
		}
		if(_isUp)																//猴子正在上升
		{
			if(onMonkeyChairStateChange!=null)									//底座状态为喷火
				onMonkeyChairStateChange(LevelOneMonkeyController.m_monkeyChairStates.fire);
		}
		else 	
		{
			if(onMonkeyChairStateChange!=null)									//底座状态为冒烟
				onMonkeyChairStateChange(LevelOneMonkeyController.m_monkeyChairStates.fire);
		}
	}

	void MonkeyInjuryBlinkCheck()												//猴子受伤闪烁
	{
		if(m_monkeyInjuryBlinkCount==0)											//当前未闪烁
		{
			if(LevelOneGameManager.Instance.GetMonkeyInjury())					//如果猴子受伤
			{
                monkeyInjuryAudio.Play();
                m_monkeyUIEye.spriteName = "monkeyLifeBar_eye2";				//猴子UI面板眼睛改变
				m_monkeyInjuryBlinkCount = 1;									//开始第一次闪
				this.GetComponent<SpriteRenderer>().color = new Color(1,1,1,0.5f);//图片变淡
				m_monkeyInjuryTimer = 0;										//计时器就位
			}
		}
		else 																	//当前猴子在闪烁
		{
			m_monkeyInjuryTimer += Time.deltaTime;								//开启计时器
			if(m_monkeyInjuryTimer>=0.2f)										//需要闪
			{						
				m_monkeyInjuryBlinkCount ++;									//换图次数增加
				m_monkeyInjuryTimer = 0f;										//计时器归位
				if(m_monkeyInjuryBlinkCount==3)									//根据次数判定要显示的主角图shader
					this.GetComponent<SpriteRenderer>().color =new Color(1,1,1,0.5f);
				else if(m_monkeyInjuryBlinkCount==2||m_monkeyInjuryBlinkCount==4)
					this.GetComponent<SpriteRenderer>().color = new Color(1,1,1,1f);
				else if(m_monkeyInjuryBlinkCount==5)							//闪烁三次后
				{
					m_monkeyInjuryBlinkCount = 0;								//受伤模式结束
					LevelOneGameManager.Instance.SetMonkeyInjury(false);		//受伤闪烁结束
					m_monkeyUIEye.spriteName = "monkeyLifeBar_eye1";			//猴子UI面板眼睛改变
				}
			}
		}
	}

	void MonkeyStateChange()
	{
		m_monkeyTimer -= Time.deltaTime;										//猴子攻击时间间隔倒计时
		if(m_monkeyTimer<0)														//猴子需要攻击
		{
			if(m_monkeyStateIndex==1)											//猴子攻击状态1
			{
				if(onMonkeyStateChange!=null)
					onMonkeyStateChange(m_monkeyStates.attack);	//转至攻击状态
				m_monkeyStateIndex = 2;											//进入下一阶段
				m_monkeyTimer = 0.82f;											//攻击时间倒计时
			}
			else if(m_monkeyStateIndex==2)										//猴子攻击状态2
			{
				if(onMonkeyStateChange!=null)
					onMonkeyStateChange(m_monkeyStates.idle);		//转至空闲状态
				m_monkeyStateIndex = 1;											//攻击状态索引归零
				if(this.transform.position.y>22f)								//根据猴子当前高度判定猴子攻击时间间隔
					m_monkeyTimer = 2f;
				else if(this.transform.position.y>30f)
					m_monkeyTimer = 1f;
				else
					m_monkeyTimer = 1.5f;										
			}
		}
	}

	void MonkeyMove()															//猴子运动
	{
		HorizontalMove ();														//猴子水平运动
		if(this.transform.position.y<=43.6f)									//猴子没达到最上端
		{
			LevelOneGameManager.Instance.SetMonkeyStoneDir(0);					//石头向下
			VerticalMove ();													//猴子竖直运动
		}
		else 																	//猴子到达最上方的台子
		{
			if(this.transform.position.x<m_heroTrans.position.x)
				LevelOneGameManager.Instance.SetMonkeyStoneDir(2);				//石头向右
			else
				LevelOneGameManager.Instance.SetMonkeyStoneDir(1);				//石头向左
		}
	}

	void LateUpdate()	
	{
		if(LevelOneGameManager.Instance.GetMonkeyBlood()>0)
		{
			MonkeyInjuryBlinkCheck ();												//猴子受伤闪烁
			MonkeyMove ();														//猴子运动控制
			MonkeyStateChange ();												//猴子状态切换
		}
		else 																	//猴子死亡
		{
			m_monkeyUIEye.spriteName = "monkeyLifeBar_eye3";			        //猴子UI面板眼睛改变 
			if(onMonkeyStateChange!=null)
				onMonkeyStateChange(m_monkeyStates.die);	//转至死亡状态
			m_monkeyDieTimer -= Time.deltaTime;
			if(m_monkeyDieTimer>0)
				this.GetComponent<SpriteRenderer>().color = new Color(1,1,1,m_monkeyDieTimer);
			else
			{
                //判断是否已经获得过种子
                if (GameDataManager.Instance.gameData.GameCurrentData.seedState[0] == 0) //种子的状态（0未得到 1得到未用 2种下）
				{ 
					LevelOneGameManager.Instance.SetEndItem(1,transform.position);      //显示通关物品，生成种子
					if (LevelOneGameManager.Instance.GetAchieveGot(27) == 0)
					{
						AudioManager.Instance.TipsSoundPlay(Global.GetInstance().audioName_AchieveGet);
						LevelOneGameManager.Instance.SetAchieveGot(27, 1);
						LevelOneGameManager.Instance.SetMessageType(3, "您获得了成就【趁火打劫】");
					}
				}

				LevelOneGameManager.Instance.isAccessCurrentScene = true;//闯关成功

				int getCoinCount = Random.Range(15,26);
				LevelOneGameManager.Instance.SetMessageType(2,  getCoinCount.ToString()+"金币");      ///获得金币
				LevelOneGameManager.Instance.SetCurrAddMoney(getCoinCount);			//增加金币数量
                Destroy(this.gameObject);										//销毁猴子
			}
			
		}
	}

}
