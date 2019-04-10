using UnityEngine;
using System.Collections;


public class LevelTwoBigSpiderController : MonoBehaviour 
{
	private enum m_spiderStates														//大蜘蛛状态
	{	
		idleBorn = 0,																//出生前空闲状态
		born, 																		//出生状态
		idle,																		//空闲状态
		run,																		//移动状态
		attack,																		//光波攻击状态
		injury,																		//受伤状态
		die,																		//死亡状态
	}
	
	private delegate void spiderStateHandler(m_spiderStates _newState);				//定义委托和事件
	private static event spiderStateHandler onStateChange; 
	private m_spiderStates m_spiderCurrState = m_spiderStates.idle;					//蜘蛛初始状态
	private Animator m_spiderAnimator = null;										//蜘蛛动画器

	public LevelTwoSpiderEggController smallSpiderCtr;
	public GameObject[] m_ironDoorBtn; 												//铁门按钮
	public GameObject m_ironDoor;													//铁门
	public GameObject m_spiderHero;													//主角
	public GameObject[] m_spiderWave;												//蜘蛛的光柱
	public Transform[] m_spiderMoveEdge;											//蜘蛛移动的范围
	public GameObject[] m_heroIronDoorEdge;											//铁门的主角边界			
	public UISprite m_spiderUIPanelEye;												//蜘蛛血条面板的眼睛		

	private int m_ironDoorState = 0;												//铁门状态（0关闭 1上升 2打开 3下落 4结束）
	private float m_ironDoorTimer = 5f;												//铁门开启时间
	private int m_spiderState = 0;													//蜘蛛状态
	private float m_spiderTimer = 3f;												//蜘蛛计时器
	private bool m_spiderInjuryAttack = false;										//蜘蛛是否处于受伤攻击阶段
	private float m_spiderSpeed = 0.03f;											//蜘蛛移动速度
	private int m_doorBtnIndex = 0;													//碰到的门开关索引
	private float m_spiderHeroDisX = 0;												//蜘蛛和主角之间的水平距离
	private int m_currIronDoorState = 0;											//铁门当前状态
	private float m_spiderRunLedge;													//蜘蛛运动的左右边界
	private float m_spiderRunRedge;		

	private bool isCheckPos = true;//是否需要检测spider的POs

	void OnEnable()																	//对象可用时 加入到订阅者列表中
	{
		onStateChange += OnStateChange;
	}
	void OnDisable()																//不可用时，从订阅者列表中退出
	{
		onStateChange -= OnStateChange;
	}
	
	void Start()
	{

        m_spiderAnimator = this.GetComponent<Animator> ();							//获取蜘蛛的动画组件
		m_currIronDoorState = 5;
	}
	
	void Update()
	{
        if (LevelTwoGameManager.Instance.GetBloodNum() <= 0) return;

		if (isCheckPos && transform.position.x <=  m_spiderMoveEdge[1].position.x) {//打破蜘蛛卵，将其引出牢笼。
			if (LevelTwoGameManager.Instance.GetAchieveGot(2) == 0)
			{
				AudioManager.Instance.TipsSoundPlay(Global.GetInstance().audioName_AchieveGet);
				LevelTwoGameManager.Instance.SetAchieveGot(2, 1);
				LevelTwoGameManager.Instance.SetMessageType(3, "您获得了成就【调虎离山】");
			}
			isCheckPos = false;
		}

        IronStateCkeck();															//铁门状态相关
		SpiderStateCheck ();														//蜘蛛状态相关

		if(m_spiderState>0&&m_spiderState<3)
		{
			m_spiderHeroDisX = m_spiderHero.transform.position.x - this.transform.position.x;
			if(Mathf.Abs (m_spiderHeroDisX)<=1.5f)
				LevelTwoGameManager.Instance.SetHeroBloodReduce(0.01f);				//主角血量减少
		}

		if(m_spiderCurrState==m_spiderStates.run)									//蜘蛛当前状态为跑
		{
			if(m_currIronDoorState!=m_ironDoorState)
			{
				if(m_ironDoorState==0)
				{
					if(this.transform.position.x<m_ironDoor.transform.position.x)
					{
						m_spiderRunLedge = m_spiderMoveEdge[0].position.x;
						m_spiderRunRedge = m_spiderMoveEdge[1].position.x;
					}
					else if(this.transform.position.x>m_ironDoor.transform.position.x)
					{
						m_spiderRunLedge = m_spiderMoveEdge[2].position.x;
						m_spiderRunRedge = m_spiderMoveEdge[3].position.x;
					}
				}
				else
				{
					m_spiderRunLedge = m_spiderMoveEdge[0].position.x;
					m_spiderRunRedge = m_spiderMoveEdge[3].position.x;
				}
				m_currIronDoorState = m_ironDoorState;
			}
			if(m_spiderHeroDisX>0.3f)												//主角在右边
			{
				if(this.transform.position.x<=m_spiderRunRedge)						//蜘蛛未到右边界
					m_spiderSpeed = 0.03f;											//蜘蛛有向右的速度
				else 																//蜘蛛到达右边界
					m_spiderSpeed = 0f;												//蜘蛛停止运动
				
			}
			else if(m_spiderHeroDisX<-0.3f)											//主角在左边
			{
				if(this.transform.position.x>=m_spiderRunLedge)						//蜘蛛未碰到左边界
					m_spiderSpeed = -0.03f;											//蜘蛛有向左的速度
				else 																//蜘蛛到达左边界
					m_spiderSpeed = 0f; 											//蜘蛛停止运动
			}
			else 																	//主角和蜘蛛基本重合
				m_spiderSpeed = 0f;													//蜘蛛停止移动
			this.transform.Translate (m_spiderSpeed, 0f, 0f);						//蜘蛛移动
		}
		else if(m_spiderCurrState==m_spiderStates.attack)							//如果当前处于攻击状态
		{
			if(this.GetComponent<SpriteRenderer>().sprite.name=="spider30")			//根据蜘蛛帧数显示光波
			{
				m_spiderWave[0].SetActive(false);
				m_spiderWave[1].SetActive(true);
			}
			else if(this.GetComponent<SpriteRenderer>().sprite.name=="spider31")
			{
				m_spiderWave[0].SetActive(true);
				m_spiderWave[1].SetActive(false);
			}
			else if(this.GetComponent<SpriteRenderer>().sprite.name=="spider32")
			{
				m_spiderWave[0].SetActive(false);
				m_spiderWave[1].SetActive(true);
			}
		}
	}
	
	void IronStateCkeck()																//铁门状态检测
	{
		switch(m_ironDoorState)														//判定铁门当前状态
		{
		case 0:																		//初始关闭状态
			Bounds rr1 = m_spiderHero.GetComponent<Collider2D>().bounds;							//主角的包围盒
			Rect r1 = new Rect(rr1.center.x - rr1.size.x / 2,
			                   rr1.center.y - rr1.size.y / 2,
			                   rr1.size.x, rr1.size.y);
			for(int i=0; i<m_ironDoorBtn.Length; i++)	
			{
				Bounds rr2 = m_ironDoorBtn[i].GetComponent<Collider>().bounds;								//获取铁门按钮碰撞盒
				Rect r2 = new Rect(rr2.center.x - rr2.size.x / 2, rr2.center.y - rr2.size.y / 2, rr2.size.x, rr2.size.y);
				if(r1.Overlaps(r2))														//主角碰到按钮
				{
                        AudioManager.Instance.ThingSoundPlay(Global.GetInstance().audioName_LevelTwo_LevelTwoIronDoor);
                        m_ironDoorBtn[i].SetActive(true);										//开启按钮
					m_ironDoorState = 1;												//进入下一阶段	
					m_ironDoor.GetComponent<Animator>().SetBool("IronDoorDownEnd", false);	//铁门下落结束
					m_ironDoor.GetComponent<Animator>().SetBool("IronDoorOpen", true);	//铁门上升
					m_ironDoorTimer = 5f;												//开启上升计时器
					m_doorBtnIndex = i;
					m_heroIronDoorEdge[0].SetActive(false);
					m_heroIronDoorEdge[1].SetActive(false);
					break;
				}
			}
			
			break;
		case 1:																			//开启等待下落
			m_ironDoorTimer -= Time.deltaTime;
			if(m_ironDoorTimer<1f&&m_ironDoorTimer>0.5f)							//还有一秒按钮提示
				m_ironDoorBtn[m_doorBtnIndex].GetComponent<Animator>().SetBool("IronDoorBtnBlink", true);
			else if(m_ironDoorTimer<0)												//需要下落
			{
				m_ironDoorState = 2;
				m_ironDoor.GetComponent<Animator>().SetBool("IronDoorOpen", false);	//关闭上升开关
				m_ironDoor.GetComponent<Animator>().SetBool("IronDoorClose", true);	//进入下落阶段
				m_ironDoorTimer = 0.62f;
                    AudioManager.Instance.ThingSoundPlay(Global.GetInstance().audioName_LevelTwo_IronDoorDown);
			}
			break;
		case 2:
			m_ironDoorTimer -= Time.deltaTime;										//下落阶段计时器
			if(m_ironDoorTimer<0.3f&&m_ironDoorTimer>=0)							//下落一定程度进行碰撞检测
			{					
				float _disSpiderX = m_ironDoor.transform.position.x - this.transform.position.x;	//判定此时蜘蛛和铁门的距离
				float _disHeroX = m_ironDoor.transform.position.x - m_spiderHero.transform.position.x;

				if (Mathf.Abs (_disHeroX) < 1.7f) {
					LevelTwoGameManager.Instance.SetHeroBloodReduce (0.6f);
				}	

			 	if(Mathf.Abs (_disSpiderX)<2.5f)								//如果足够近
				{
					if(m_spiderState==2)											//如果当前蜘蛛已经出生
					{
						LevelTwoGameManager.Instance.SetSpiderBloodReduce(1f);		//蜘蛛set 0

						if (LevelTwoGameManager.Instance.GetAchieveGot(7) == 0)//引诱蜘蛛，用铁栅栏将其扎死。
						{
							AudioManager.Instance.TipsSoundPlay(Global.GetInstance().audioName_AchieveGet);
							LevelTwoGameManager.Instance.SetAchieveGot(7, 1);
							LevelTwoGameManager.Instance.SetMessageType(3, "您获得了成就【关门捉贼】");
						}
					}
				}

			}
			else if(m_ironDoorTimer<0)
				m_ironDoorState = 3;
			break;
		case 3:
			m_ironDoorBtn[m_doorBtnIndex].SetActive(false);
			m_ironDoorBtn[m_doorBtnIndex].GetComponent<Animator>().SetBool("IronDoorBtnBlink", false);
			m_ironDoor.GetComponent<Animator>().SetBool("IronDoorClose", false);	//关闭下落动画
			m_ironDoor.GetComponent<Animator>().SetBool("IronDoorDownEnd", true);	//铁门下落结束
			m_ironDoorState = 0;													//铁门状态复位 依旧可以下落
			m_heroIronDoorEdge[0].SetActive(true);
			m_heroIronDoorEdge[1].SetActive(true);
			break;
		}
	}
	
	void SpiderStateCheck()
	{
		switch(m_spiderState)														//判定蜘蛛阶段
		{
		case 0:																		//未出生阶段
			if(LevelTwoGameManager.Instance.GetSpiderHit())							//如果蜘蛛挨打
			{
                    LevelTwoGameManager.Instance.ActiveSpiderLifePanel();				//开启蜘蛛生命值面板
				m_spiderState = 1;													//进入下一阶段
				m_spiderAnimator.SetBool("SpiderBorn", true);						//蜘蛛出生
				m_spiderTimer = 3f;													//蜘蛛计时器就位
			}
			break;
		case 1:
			m_spiderTimer -= Time.deltaTime;										//出生时长
			if(m_spiderTimer<0)														//出生完毕
			{
				m_spiderAnimator.SetBool("SpiderBorn", false);						//蜘蛛出生完毕
				m_spiderAnimator.SetBool("SpiderBornDone", true);					//转至空闲状态
				m_spiderState = 2;													//蜘蛛下一阶段
				LevelTwoGameManager.Instance.SetSpiderHit(false);					//关闭受伤
			}
			break;
		case 2:
			if (LevelTwoGameManager.Instance.GetSpiderDie ()) {							//蜘蛛血量为0
				if (onStateChange != null)
					onStateChange (m_spiderStates.die);								//蜘蛛进入死亡状态

				m_spiderState = 3;																				
				m_spiderTimer = 2.1f;												//蜘蛛死亡动画时长
				m_spiderUIPanelEye.spriteName = "lifeBar_spider_eye3";				//改变UI面板蜘蛛眼睛图
				LevelTwoGameManager.Instance.SetCurrAddMoney (35);
				LevelTwoGameManager.Instance.SetMessageType (2, "35金币");         //获取金币
				if (smallSpiderCtr.isLittleSpiderLive) {

					if (LevelTwoGameManager.Instance.GetAchieveGot (33) == 0) {
						AudioManager.Instance.TipsSoundPlay (Global.GetInstance ().audioName_AchieveGet);
						LevelTwoGameManager.Instance.SetAchieveGot (33, 1);
						LevelTwoGameManager.Instance.SetMessageType (3, "您获得了成就【擒贼擒王】");
						smallSpiderCtr.SetSmallSpiderDead ();//杀死小蜘蛛
					}
				}
			}
			if (!m_spiderInjuryAttack)												//如果蜘蛛不在受伤攻击阶段
			{
				if(LevelTwoGameManager.Instance.GetSpiderHit())						//如果蜘蛛被打
				{
					if(onStateChange!=null)
						onStateChange(m_spiderStates.injury);//转至受伤状态
					m_spiderInjuryAttack = true;									//进入受伤攻击状态
					m_spiderTimer = 0.6f;											//受伤阶段计时器
				}
				else 																//如果蜘蛛没被打
				{
					if(m_spiderHeroDisX>0)											//根据主角朝向确定蜘蛛朝向
						this.transform.localScale = new Vector3(-1f, 1f, 1f);		
					else
						this.transform.localScale = new Vector3(1f, 1f, 1f);		
					if(Mathf.Abs(m_spiderHeroDisX)<=15f)							//如果距离太近
					{
						if(onStateChange!=null)											
							onStateChange(m_spiderStates.run);						//转至移动状态
					}
					else 															//如果距离不够近
					{
						if(onStateChange!=null)
							onStateChange(m_spiderStates.idle);						//蜘蛛进入空闲状态
					}
				}
			}
			else 																	//如果当前处于受伤攻击状态
			{
				m_spiderTimer -= Time.deltaTime;									//开启计时器
				if(m_spiderTimer<0)													//计时器结束
				{
					if(m_spiderCurrState==m_spiderStates.injury)					//如果当前蜘蛛为受伤状态
					{
						if(onStateChange!=null)															
							onStateChange(m_spiderStates.attack);					//蜘蛛转至攻击状态
						m_spiderTimer = 2f;											//开启攻击计时器
					}
					else if(m_spiderCurrState==m_spiderStates.attack)				//如果当前蜘蛛为攻击状态
					{
						m_spiderInjuryAttack = false;								//结束受伤攻击阶段
						LevelTwoGameManager.Instance.SetSpiderHit(false);			//蜘蛛受伤关闭
						if(onStateChange!=null)													
							onStateChange(m_spiderStates.idle);						//蜘蛛转为空闲状态
						m_spiderWave[0].SetActive(false);
						m_spiderWave[1].SetActive(false);
					}
					
				}
			}
			break;
		case 3:																		//蜘蛛死亡
			m_spiderTimer -= Time.deltaTime;
			if(m_spiderTimer<0)
			{
				Destroy(this.GetComponent<Animator>());								//移除蜘蛛动画组件
				this.GetComponent<SpriteRenderer>().enabled = false;				//隐藏蜘蛛图片
				this.tag = "used";

                if (GameDataManager.Instance.gameData.GameCurrentData.seedState[1] == 0) //种子的状态（0未得到 1得到未用 2种下）
                    LevelTwoGameManager.Instance.SetEndItem(1);                         //可以显示通关物品，生成种子

                    m_spiderState = 4;													//进入蜘蛛死亡后阶段
			}
			break;
		}
	}
	
	void CloseValue()																//关闭所有动画控制变量
	{
		m_spiderAnimator.SetBool("SpiderBornDone", false);
		m_spiderAnimator.SetBool("SpiderIdleInjury", false);
		m_spiderAnimator.SetBool("SpiderIdleRun", false);
		m_spiderAnimator.SetBool("SpiderRunInjury", false);
		m_spiderAnimator.SetBool("SpiderAttack", false);
		m_spiderAnimator.SetBool("SpiderAttackIdle", false);
		m_spiderAnimator.SetBool("SpiderAttackRun", false);
		m_spiderAnimator.SetBool("SpiderAttackInjury", false);
		m_spiderAnimator.SetBool("SpiderRunIdle", false);
		
	}
	
	void OnStateChange(m_spiderStates _newState)									//蜘蛛状态改变时调用
	{
		if(_newState==m_spiderCurrState)											//如果状态未发生变化
			return;																	//返回
		switch(_newState)															//判定新状态
		{
		case m_spiderStates.run:													//新状态为跑
			CloseValue();															//关闭所有变量
			if(m_spiderCurrState==m_spiderStates.idle)								//蜘蛛当前为idle
				m_spiderAnimator.SetBool("SpiderIdleRun", true);
			else if(m_spiderCurrState==m_spiderStates.attack)						//蜘蛛当前为attack
				m_spiderAnimator.SetBool("SpiderAttackRun", true);
			break;

		case m_spiderStates.injury:                                                 //新状态为受伤
                AudioManager.Instance.ThingSoundPlay(Global.GetInstance().audioName_LevelTwo_SpiderInjury);
                m_spiderUIPanelEye.spriteName = "lifeBar_spider_eye2";					//改变UI面板蜘蛛眼睛图
			CloseValue();
			if(m_spiderCurrState==m_spiderStates.idle)								//蜘蛛当前为idle
				m_spiderAnimator.SetBool("SpiderIdleInjury", true);
			else if(m_spiderCurrState==m_spiderStates.attack)						//蜘蛛当前为attack
				m_spiderAnimator.SetBool("SpiderAttackInjury", true);
			else if(m_spiderCurrState==m_spiderStates.run)							//蜘蛛当前为run
				m_spiderAnimator.SetBool("SpiderRunInjury", true);
			break;
			
		case m_spiderStates.idle:													//新状态为空闲
			CloseValue();
			if(m_spiderCurrState==m_spiderStates.run)								//蜘蛛当前为idle
				m_spiderAnimator.SetBool("SpiderRunIdle", true);
			else if(m_spiderCurrState==m_spiderStates.attack)						//蜘蛛当前为攻击
				m_spiderAnimator.SetBool("SpiderAttackIdle", true);
			break;
		case m_spiderStates.attack:													//蜘蛛当前为攻击
			CloseValue();
			m_spiderAnimator.SetBool("SpiderAttack", true);
			break;
		case m_spiderStates.die:													//新状态为死
			CloseValue();															//关闭所有变量
			if(m_spiderCurrState==m_spiderStates.idle)								//蜘蛛当前为空闲状态
				m_spiderAnimator.SetBool("SpiderIdleDie", true);
			else if(m_spiderCurrState==m_spiderStates.run)							//蜘蛛当前为跑
				m_spiderAnimator.SetBool("SpiderRunDie", true);
			else if(m_spiderCurrState==m_spiderStates.injury)						//蜘蛛当前为受伤
				m_spiderAnimator.SetBool("SpiderInjuryDie", true);
			else if(m_spiderCurrState==m_spiderStates.attack)						//蜘蛛当前为攻击
			{
				m_spiderWave[0].SetActive(false);									//关闭光波
				m_spiderWave[1].SetActive(false);
				m_spiderAnimator.SetBool("SpiderAttackDie", true);					//开启蜘蛛死亡
			}
			break;
		}
		if(m_spiderCurrState==m_spiderStates.injury)
			m_spiderUIPanelEye.spriteName = "lifeBar_spider_eye1";					//改变UI面板蜘蛛眼睛图
		m_spiderCurrState = _newState;												//更换蜘蛛当前状态
	}
	
}
