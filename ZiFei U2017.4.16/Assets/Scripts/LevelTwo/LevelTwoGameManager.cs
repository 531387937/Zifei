using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine.SceneManagement;

public class LevelTwoGameManager : MonoBehaviour
{
	public static LevelTwoGameManager Instance;

    public GameObject deadPanelMainmenuBtnObj;
    public GameObject deadPanelObj;//死亡界面

    public GameObject[] m_usualBtn;												//常用按钮 摇杆+背包+存档+泡泡纸
    public GameObject uiBackPageObj;

    public UISprite m_heroLifeObj;												//主角血量物品
	public UILabel m_heroMoneyLabel;											//主角金钱标签
	public GameObject m_levelTwoHero;											//主角
	public GameObject m_spiderLifePanel;										//蜘蛛生命面板
	public UISprite m_spiderLifeObj;											//大蜘蛛血量物品
	public GameObject m_sceneNameInfo;											//场景名称提示条
	public UILabel m_sceneNameInfoLabel;										//场景名称提示条上的文字
	public GameObject[] m_heroMask;												//主角的道具


	private UISpriteAnimation m_sceneNameInfoAni;								//场景名称提示条的动画控制器
	private int m_sceneNameInfoState = 0;										//场景名称提示条状态
	private float m_sceneNameInfoTimer = 1f;									//场景名称提示条计时器

	private int[] m_itemNum = new int[20];										//物品数量
	private int m_itemEquipIndex = 0;											//当前装备的道具
	private int[] m_taskIndexState = new int[7];								//每个任务状态
	private int m_currSaveIndex = 0;											//当前存至的档位编号
	private int m_saveNPCNum = 7;												//主角遇到的NPC编号	


    //private int m_letterReadState = 0;                                          //读信阶段
    //private int m_waterBoxRepairState = 0;//水阀的状态
    //private int m_barTaskState = 0;//酒馆的状态
    //private int m_jarState = 0;                                                 //罐子的状态（0初始 1已调查 2已碎）
    //private int m_HomeCoinState = 0;

    private bool m_diaryOpen = false;											//是否打开日记
	private bool m_spiderHit = false;											//蜘蛛是否被打中
	//private bool m_closeJoyStick = false;										//关闭摇杆
	private int m_currMoneyNum;													//当前主角的钱数
	private bool m_heroInjury = false;											//主角是否受伤
	private int m_messageType;                                                  //消息类型(1不用金币 2金币 3一句话)
    private string m_messageContent;											//消息文字
	private int m_turnToCountry = 0;											//是否回到村庄
	private bool m_spiderDie = false;											//大蜘蛛是否死亡 血量小于0
	private bool m_boomerangOut = false;										//回旋镖是否发射出去
	private bool m_glassWaveEmit = false;										//眼镜光发出
	private int m_getEndItem = 0;
	private int[] m_achieveGot = new int[36];									//36计是否得到
	private float[] m_volume = new float[2];                                    //音量大小

    private int turnCountryValue = 0;

	[HideInInspector]
    public bool isPlatformType_Mobild = false;//游戏的运行平台区分


	[HideInInspector]
    public bool isUsualBtnCanClick = false;
    private static int m_backpackIndex = 0;

	[HideInInspector]
	public bool isAccessCurrentScene = false;//成功闯过当前的关卡
	[HideInInspector]
	public bool isHaveDrumstick = false;//是否有鸡腿


	private bool isNeedTurnScene = false;
	public float turnSceneDelayTime = 1f;
	private float turnSceneTimer = 0f;


	//message的结构
	private struct MyMessage
	{
		public int messageType;
		public string messageContent;

		public void Init(){
			messageType = 0;
			messageContent = "";
		}

	}

	private readonly static  Queue<MyMessage> messageQueue = new Queue<MyMessage> ();//消息的队列
	[HideInInspector]
	public bool isCanShowMsg = true;//是否可以显示消息




	//-----------------------------------------------------------------------------------------------------------------------------



    void Awake()
	{
		Instance = this;
        UIEventListener.Get(deadPanelMainmenuBtnObj).onClick = OnDeadMainMenuBtnClick;                  //点击"main Menu"按钮
        m_levelTwoHero.SetActive(true);
        m_usualBtn[3].SetActive(true);
        m_usualBtn[0].SetActive(true);
        m_usualBtn[4].SetActive(true);

#if UNITY_ANDROID || UNITY_IPHONE
        isPlatformType_Mobild = true;
#elif UNITY_STANDALONE_WIN
        isPlatformType_Mobild = false;
#endif
    }

    void Start()
	{
		GameDataManager.Instance.Load ();										//加载数据
		m_heroLifeObj.fillAmount = GameDataManager.Instance.gameData.GameCurrentData.heroLife;//获取生命值
		m_currMoneyNum = GameDataManager.Instance.gameData.GameCurrentData.heroMoney;		//获取主角金币数
		m_heroMoneyLabel.text = m_currMoneyNum.ToString ();


        for (int i=0; i<m_itemNum.Length; i++)									//获取物品数量情况
			m_itemNum[i] =  GameDataManager.Instance.gameData.GameCurrentData.items[i];		
		for(int i=0; i<m_taskIndexState.Length; i++)							//指定当前任务完成情况
			m_taskIndexState[i] = GameDataManager.Instance.gameData.GameCurrentData.countryTaskState[i];
		for(int i=0; i<m_achieveGot.Length; i++)								//获取当前成就情况
			m_achieveGot[i] = GameDataManager.Instance.gameData.GameCurrentData.achieveState[i];

		if (m_itemNum [2] != 0)
			isHaveDrumstick = true;

		m_itemEquipIndex = GameDataManager.Instance.gameData.GameCurrentData.equipedIndex;	//获取当前装备的道具
		SetItemEquipIndex (m_itemEquipIndex);
        SetVolume(0, GameDataManager.Instance.gameData.GameCurrentData.BGMVolume);				//指定当前音量情况
		SetVolume (1, GameDataManager.Instance.gameData.GameCurrentData.SoundVolume);

		m_sceneNameInfoState = 1;                                               //播放场景名称提示条


        OpenUsualBtn();
        GameObject.Find("UI Root").SetActive(true);
    }


    void Update()
	{

		if(m_sceneNameInfoState!=0)												//如果需要播放场景名称提示条
			SceneNameInfoAni();													//场景名称提示条动画
	

		//转换scene的倒计时
		if (isNeedTurnScene) {
			
			turnSceneTimer += Time.deltaTime;
			if (turnSceneTimer >= turnSceneDelayTime ) {
				m_turnToCountry = turnCountryValue;
				isNeedTurnScene = false;
				turnSceneTimer = 0f;
			}
		}


		ShowMessage ();

	}
	
	void SceneNameInfoAni()														//场景名称提示条动画
	{
		switch(m_sceneNameInfoState)											//判断当前场景名称提示条状态
		{
		case 1:																	//第一阶段
			m_sceneNameInfo.SetActive(true);									//打开场景名称提示条
			m_sceneNameInfoLabel.text = "蜘蛛洞穴";								//当前场景名称
			m_sceneNameInfoAni = m_sceneNameInfo.gameObject.AddComponent<UISpriteAnimation>();//添加场景名称提示条动画
			m_sceneNameInfoAni.loop = false;									//动画不循环
			m_sceneNameInfoAni.namePrefix = "ani_info_sceneNameStart_";			//指定动画图
			m_sceneNameInfoAni.framesPerSecond = 10;							//帧率 
			m_sceneNameInfoLabel.alpha = 0f;									//alpha为0
			m_sceneNameInfoState = 2;											//下一阶段
			break;
		case 2:																	//第二阶段
			m_sceneNameInfoLabel.alpha += Time.deltaTime*2;						//场景名称提示文字淡入
			if(!m_sceneNameInfoAni.isPlaying)									//场景名称提示条播放完成
			{
				m_sceneNameInfoLabel.alpha = 1f;								//场景名称提示条文字完全显示
				m_sceneNameInfoState = 3;										//场景名称提示条进入下一阶段
				Destroy(m_sceneNameInfo.GetComponent<UISpriteAnimation>());		//销毁场景名称提示条动画组件
			}
			break;
		case 3:																	//第三阶段
			m_sceneNameInfoTimer -= Time.deltaTime;								//计时器 场景名称提示条显示时长
			if(m_sceneNameInfoTimer<0)											//场景名称提示条显示完毕
			{
				m_sceneNameInfoLabel.alpha = 0f;								//文字alpha变为0
				m_sceneNameInfoState = 4;										//下一阶段
			}
			break;
		case 4:																	//第四阶段
			m_sceneNameInfoAni = m_sceneNameInfo.gameObject.AddComponent<UISpriteAnimation>();	//添加场景名称提示条退出动画
			m_sceneNameInfoAni.loop = false;									//动画不循环
			m_sceneNameInfoAni.namePrefix = "ani_info_sceneNameEnd_";			//指定动画图
			m_sceneNameInfoAni.framesPerSecond = 10;							//帧率 
			m_sceneNameInfoState = 5;											//下一阶段
			break;
		case 5:																	//第五阶段											
			if(!m_sceneNameInfoAni.isPlaying)									//场景名称提示条退出动画播完
			{
				Destroy(m_sceneNameInfo.GetComponent<UISpriteAnimation>());		//移除场景名称提示条退出动画组件
				m_sceneNameInfo.SetActive(false);								//隐藏场景名称提示条
				m_sceneNameInfoState = 0;										//场景名称提示条归零
			}
			break;
		}
	}

	public bool GetHeroInjury()													//外部获取主角是否受伤
	{
		return m_heroInjury;
	}
	public void SetHeroInjury(bool _injury)										//外部设置主角是否受伤变量
	{
		m_heroInjury = _injury;
	}

    public void SetHeroBloodReduce(float _reduceNum)							//主角生命值减少量
	{
        if (m_heroLifeObj.fillAmount <= 0) return;
        if (_reduceNum > 0)
        {
        AudioManager.Instance.HeroSoundPlay(Global.GetInstance().audioName_HeroBloodReduce);
		m_heroInjury = true;													//主角受伤

        }
        m_heroLifeObj.fillAmount -= _reduceNum;
        if (m_heroLifeObj.fillAmount <= 0)
            SetTurnToCountry(1);
    
        else if (m_heroLifeObj.fillAmount >= 1)
            m_heroLifeObj.fillAmount = 1;
	}

	public bool GetSpiderDie()
	{
		return m_spiderDie;														//外部获取大蜘蛛是否死亡
	}
	public void SetSpiderBloodReduce(float _reduceNum)
	{

        m_spiderLifeObj.GetComponent<UIProgressBar> ().value -= _reduceNum;

        if (m_spiderLifeObj.GetComponent<UIProgressBar>().value <= 0)               //蜘蛛死亡
        {
            AudioManager.Instance.ThingSoundPlay(Global.GetInstance().audioName_LevelTwo_SpiderDown);
            m_spiderLifeObj.GetComponent<UIProgressBar>().value = 0f;
            m_spiderDie = true;

			isAccessCurrentScene = true;



        }
        else {
            AudioManager.Instance.ThingSoundPlay(Global.GetInstance().audioName_LevelTwo_SpiderInjury);

        }
	}
	public void ActiveSpiderLifePanel()
	{
        if (!m_spiderLifePanel.activeSelf)
        {
            AudioManager.Instance.ThingSoundPlay(Global.GetInstance().audioName_LevelTwo_SpiderBirth);
            m_spiderLifePanel.SetActive (true);
        }
	}


	public void ClearMessage(){

		m_messageType = 0;
		m_messageContent = "";
	}

	public int GetMessageType()													//外部获取需提示的消息类型
	{
		return m_messageType;
	}
	public string GetMessageContent()											//外部获取需提示的消息内容
	{
		return m_messageContent;
	}
	public void SetMessageType(int _type, string _content)						//外部指定需提示的消息类型及内容
	{
		//检测是否有message需要显示，如果有加入到消息队列中
		lock (messageQueue) {

			MyMessage msg = new MyMessage ();
			msg.messageType = _type;
			msg.messageContent = _content;
			messageQueue.Enqueue (msg);
		}

	}


	void ShowMessage(){
		if (messageQueue.Count > 0) {
			if (!isCanShowMsg)
				return;

			isCanShowMsg = false;//及时关闭消息显示的开关，防止接下来的消息将正在显示的消息覆盖掉

			//获取消息队列中的消息
			MyMessage msg = messageQueue.Dequeue ();
			m_messageType = msg.messageType;
			m_messageContent = msg.messageContent;
			if (m_messageType==2)
				AudioManager.Instance.ThingSoundPlay(Global.GetInstance().audioName_GoldCoin);
			else if(m_messageType == 1 || m_messageType == 3)
				AudioManager.Instance.TipsSoundPlay(Global.GetInstance().audioName_MessageTips);
			msg.Init ();
		}

	}

    public void OpenUsualBtn()													//开启常用按钮
	{
		m_usualBtn[0].SetActive(true);
		m_usualBtn[1].GetComponent<BoxCollider>().enabled = true;
		m_usualBtn[2].GetComponent<BoxCollider>().enabled = true;
		m_usualBtn[3].GetComponent<BoxCollider>().enabled = true;

        isUsualBtnCanClick = true;

    }
    public void CloseUsualBtn()													//关闭常用按钮
	{
		m_usualBtn[0].SetActive(false);
		m_usualBtn[1].GetComponent<BoxCollider>().enabled = false;
		m_usualBtn[2].GetComponent<BoxCollider>().enabled = false;
		m_usualBtn[3].GetComponent<BoxCollider>().enabled = false;
		//m_closeJoyStick = true;

        isUsualBtnCanClick = false;

    }

 //   public bool GetJoyStickState()												//摇杆是否可用
	//{
	//	return m_closeJoyStick;
	//}
	//public void SetJoyStickState(bool _joyStickState)
	//{
	//	m_closeJoyStick = _joyStickState;
	//}

    #region
    //public int GetReadLetterState()                                             //外部获取信件读取状态
    //{
    //    return m_letterReadState;
    //}
    //public void SetReadLetterState(int _state)                                  //外部设置信件读取状态
    //{
    //    m_letterReadState = _state;
    //}


    //public int GetWaterBoxRepairState()                                             //外部获取水阀读取状态
    //{
    //    return m_waterBoxRepairState;
    //}
    //public void SetWaterBoxRepairState(int _state)                                  //外部设水阀读取状态
    //{
    //    m_waterBoxRepairState = _state;
    //}


    //public int GetJarState()                                                    //外部获取罐子状态
    //{
    //    return m_jarState;
    //}
    //public void SetJarState(int _state)                                         //外部设置罐子状态
    //{
    //    m_jarState = _state;
    //}


    //public int GetBarTaskState()                                             //外部获取酒馆附近任务读取状态
    //{
    //    return m_barTaskState;
    //}
    //public void SetBarTaskState(int _state)                                  //外部设酒馆附近任务读取状态
    //{
    //    m_barTaskState = _state;
    //    if (m_barTaskState == 2)
    //        m_levelTwoHero.transform.GetChild(0).gameObject.SetActive(true);//开启道具装备
    //    else
    //        m_levelTwoHero.transform.GetChild(0).gameObject.SetActive(false);//隐藏道具装备
    //}

    //public int GetHomeCoinState()                                                    //外部获取罐子状态
    //{
    //    return m_HomeCoinState;
    //}
    //public void SetHomeCoinState(int _state)                                         //外部设置罐子状态
    //{
    //    m_HomeCoinState = _state;
    //}

    #endregion

    public int GetTurnToCountry()												//外部获取是否回村变量
	{
		return m_turnToCountry;
	}


    private void OnDeadMainMenuBtnClick(GameObject go) {
        AudioManager.Instance.SoundPlay(Global.GetInstance().audioName_BtnClick2);
        m_turnToCountry = turnCountryValue;

    }

    public void SetTurnToCountry(int _turnCountry)                              //外部设定是否回村变量
    {

		turnCountryValue = _turnCountry;

        CloseUsualBtn();

		for (int i = 0; i < m_usualBtn.Length; i++)
			m_usualBtn [i].SetActive (false);
		
        if (_turnCountry == 1)                                                      //需要回村
        {
            AudioManager.Instance.SoundPlay(Global.GetInstance().audioName_DeadSound);
            m_levelTwoHero.SetActive(false);
            m_usualBtn[3].SetActive(false);
            m_usualBtn[4].SetActive(false);
            uiBackPageObj.SetActive(false);
            
			if (GetAchieveGot(30) == 0)
			{
				AudioManager.Instance.TipsSoundPlay(Global.GetInstance().audioName_AchieveGet);
				SetAchieveGot(30, 1);
				SetMessageType(3, "您获得了成就【借尸还魂】");
			}

			deadPanelObj.SetActive(true);//显示死亡面板

            GameDataManager.Instance.gameData.GameCurrentData.startState = true;               //需要开启开始界面
            GameDataManager.Instance.gameData.GameCurrentData.newHeroState = true;               //当前的数据改为 新数据
            GameDataManager.Instance.gameData.GameCurrentData.getEndItemState = 0;


        }
        else if (_turnCountry == 2)//通关是的存档保存数据，并且切换场景
        {

            GameDataManager.Instance.gameData.GameCurrentData.heroLife = m_heroLifeObj.fillAmount;
            GameDataManager.Instance.gameData.GameCurrentData.heroMoney = m_currMoneyNum;
            GameDataManager.Instance.gameData.GameCurrentData.startState = false;           //不需要开启开始界面

			if (isAccessCurrentScene && !isHaveDrumstick) {
				if (GetAchieveGot(11) == 0)
				{
					AudioManager.Instance.TipsSoundPlay(Global.GetInstance().audioName_AchieveGet);
					SetAchieveGot(11, 1);
					SetMessageType(3, "您获得了成就【空城计】");
				}
			}

            if (m_itemNum[17] ==1  && m_itemNum[13] == 0) //lan色燃料的状态（0未得到 1得到）
                GameDataManager.Instance.gameData.GameCurrentData.getEndItemState = 2;                  //第2关成功标识
            else
                GameDataManager.Instance.gameData.GameCurrentData.getEndItemState = 0;					//

			isNeedTurnScene= true;

        }
        else if (_turnCountry == 3)//取消通关，存档数据，并且切换场景
        {
            GameDataManager.Instance.gameData.GameCurrentData.heroLife = m_heroLifeObj.fillAmount;
            //GameDataManager.Instance.gameData.GameCurrentData.heroMoney = m_currMoneyNum;
            GameDataManager.Instance.gameData.GameCurrentData.getEndItemState = 0;
            GameDataManager.Instance.gameData.GameCurrentData.startState = true;			//不需要开启开始界面

			if (GetAchieveGot(35) == 0)
			{
				AudioManager.Instance.TipsSoundPlay(Global.GetInstance().audioName_AchieveGet);
				SetAchieveGot(35, 1);
				SetMessageType(3, "您获得了成就【走为上】");
			}

			isNeedTurnScene= true;

		}
        for (int i = 0; i < m_itemNum.Length; i++)                          //获取当前物品拥有情况
            GameDataManager.Instance.gameData.GameCurrentData.items[i] = m_itemNum[i];
        for (int i = 0; i < m_achieveGot.Length; i++)                          //获取当前achieve拥有情况
            GameDataManager.Instance.gameData.GameCurrentData.achieveState[i] = m_achieveGot[i];

        GameDataManager.Instance.gameData.GameCurrentData.equipedIndex = m_itemEquipIndex;
        GameDataManager.Instance.gameData.GameCurrentData.BGMVolume = m_volume[0];      //记录当前音量情况
        GameDataManager.Instance.gameData.GameCurrentData.SoundVolume = m_volume[1];
        GameDataManager.Instance.Save();                                    //存档



    }


    public int GetAchieveGot(int _index)										//外部获取36计获取情况
	{
		return m_achieveGot[_index];
	}
	
	public void SetAchieveGot(int _index, int _got)							//外部设置36计获取情况
	{
		m_achieveGot[_index] = _got;

    }



    public float GetVolume(int _index)											//外部获取音量情况
	{
		return m_volume [_index];
	}
	
	public void SetVolume(int _index, float _volume)							//外部设置音量情况
	{
        m_volume[_index] = _volume;
        if (_index == 0)                                                           //如果是BGM
        {
            GameDataManager.Instance.gameData.GameCurrentData.BGMVolume = m_volume[_index];
            AudioManager.Instance.SetBGMVolume(_volume);                                                                            //如果是音效
        }
        else
        {
            GameDataManager.Instance.gameData.GameCurrentData.SoundVolume = m_volume[_index];
            AudioManager.Instance.SetSoundVolume(_volume);
        }
    }

	public float GetHeroScaleX()												//外部获取主角当前朝向
	{
		return m_levelTwoHero.transform.localScale.x;
	}
	public bool GetDiaryOpen()													//外部获取日记是否打开变量
	{
		return m_diaryOpen;
	}
	public void SetDiaryOpen(bool _open)										//外部设置日记是否打开变量
	{
		m_diaryOpen = _open;
	}

	public bool GetSpiderHit()													//外部获取蜘蛛是否被打中
	{
		return m_spiderHit;
	}
	public void SetSpiderHit(bool _hit)											//外部设置蜘蛛是否被打中变量
	{
		m_spiderHit = _hit;
	}

    public int GetBpIndex()                                                     //获取当前打开的背包页面
    {
        return NewMethod();
    }

    private static int NewMethod()
    {
        return m_backpackIndex;
    }

    public void SetBpIndex(int _index)                                          //指定当前打开的背包页面
    {
        m_backpackIndex = _index;
    }

    public bool GetBoomerangOut()												//外部获取回旋镖是否发射出去
	{
		return m_boomerangOut;
	}
	public void SetBoomerangOut(bool _out)										//外部设置回旋镖是否发射出去
	{
		m_boomerangOut = _out;
	}

	public bool  GetGlassWaveEmit()												//外部获取是否发出眼镜光
	{
		return m_glassWaveEmit;
	}
	public void SetGlassWaveEmit(bool _emit)									//外部设置是否发出眼镜光
	{
		m_glassWaveEmit = _emit;
	}

	public int GetEndItem()														//外部获取是否通关
	{
		return m_getEndItem;
	}
	public void SetEndItem(int _item)											//外部设置是否通关
	{
		m_getEndItem = _item;
		if(_item==2)
		{
			GameDataManager.Instance.gameData.GameCurrentData.seedState[1] = 1;					//获得第二关的通关物品
			GameDataManager.Instance.gameData.GameCurrentData.keyState[2] = 1;					//获得第二关的钥匙
		}
	}

	public float GetBloodNum()													//外部获取主角血量值
	{
		return m_heroLifeObj.fillAmount;
	}
	
	public int GetCurrMoney()													//外部获取玩家当前拥有的钱数
	{	
		return m_currMoneyNum;
	}
	public void SetCurrMoney(int _currMoney)									//外部指定玩家当前拥有的钱数
	{
		m_currMoneyNum = _currMoney;
		m_heroMoneyLabel.text = m_currMoneyNum.ToString();
	}
	public void SetCurrAddMoney(int _addNum)									//外部指定当前玩家增加的钱数
	{
		m_currMoneyNum += _addNum;
		m_heroMoneyLabel.text = m_currMoneyNum.ToString();
	}
	
	public int GetItemNum(int _ID)												//外部获取指定物品数量
	{
		return m_itemNum [_ID];
	}
	public void SetItemNum(int _ID, int _num)									//外部设置指定物品数量
	{
		m_itemNum [_ID] = _num;
	}
	
	public int GetItemEquipIndex()												//外部获取当前装备的道具编号
	{
		return m_itemEquipIndex;
	}
	public void SetItemEquipIndex(int _ID)										//外部设置当前装备的编号
	{
		m_itemEquipIndex = _ID;
		for(int i=0; i<m_heroMask.Length; i++)									//关闭所有道具
			m_heroMask[i].SetActive(false);
		if(_ID!=0)																//如果使用了道具
		{
			m_heroMask[_ID-1].SetActive(true);									//开启使用的道具

			if (_ID == 2 && GetAchieveGot(18) == 0)
			{
				AudioManager.Instance.TipsSoundPlay(Global.GetInstance().audioName_AchieveGet);
				SetAchieveGot(18, 1);
				SetMessageType(3, "您获得了成就 【树上开花】");
			}

			if (_ID == 3 && GetAchieveGot(3) == 0)
			{
				AudioManager.Instance.TipsSoundPlay(Global.GetInstance().audioName_AchieveGet);
				SetAchieveGot(3, 1);
				SetMessageType(3, "您获得了成就 【反间计】");
			}  

			if (_ID == 4 && GetAchieveGot(5) == 0)
			{
				AudioManager.Instance.TipsSoundPlay(Global.GetInstance().audioName_AchieveGet);
				SetAchieveGot(5, 1);
				SetMessageType(3, "您获得了成就 【釜底抽薪】");
			}    
		}
	}
	
	public int GetTaskIndexState(int _taskIndex)								//获取指定编号任务的状态
	{
		return m_taskIndexState [_taskIndex];
	}
	public void SetTaskIndexState(int _taskIndex, int _taskState)				//设定指定编号任务的状态
	{
		m_taskIndexState[_taskIndex] = _taskState;
	}
	
	public int SaveCurrData()																		//当前需要存档
	{
		GameSaveData _saveDataTemp = new GameSaveData ();                                           //生成临时变量

        _saveDataTemp.startState = false;
        _saveDataTemp.used = true;																//该存档点已使用
		_saveDataTemp.levelIndex = 5;                                                               //获取当前场景号(第六关
        _saveDataTemp.gameSceneName = SceneManager.GetActiveScene().name;//保存当前的scene名称

        _saveDataTemp.heroPosition = m_levelTwoHero.transform.position;							//获取当前主角位置
		_saveDataTemp.heroMoney = m_currMoneyNum;												//获取当前钱数
		_saveDataTemp.heroLife = m_heroLifeObj.fillAmount;										//获取当前生命值
		_saveDataTemp.equipedIndex = m_itemEquipIndex;

        for (int i=0; i<_saveDataTemp.items.Length; i++)											//获取当前物品拥有情况
			_saveDataTemp.items[i] = m_itemNum[i];

        for (int i=0; i<_saveDataTemp.countryTaskState.Length; i++)								//获取当前任务完成情况
			_saveDataTemp.countryTaskState[i] = m_taskIndexState[i];

        for (int i = 0; i < _saveDataTemp.achieveState.Length; i++)                               //保存当前36计获得情况
            _saveDataTemp.achieveState[i] = m_achieveGot[i];


        _saveDataTemp.savedTime = System.DateTime.Now;												//获取当前系统时间
		_saveDataTemp.NPCIndex = 9;                                                                 //获取当前遇到的最后NPC

        GameDataManager.Instance.gameData.GameCurrentData = _saveDataTemp;



        //-----------------------查找存档的档位-------------------------------------------------------
        if (!GameDataManager.Instance.gameData.GameSaveData1.used)									//如果存档点1未被使用
			m_currSaveIndex = 0;
		else if(!GameDataManager.Instance.gameData.GameSaveData2.used)							//如果存档点2未被使用
			m_currSaveIndex = 1;
		else if(!GameDataManager.Instance.gameData.GameSaveData3.used)							//如果存档点3未被使用
			m_currSaveIndex = 2;
		else 																						//如果三个存档点均被使用
		{


            if (m_currSaveIndex != 2)                                                                   //从最近一次存档点索引开始向后覆盖
                m_currSaveIndex++;
            else
                m_currSaveIndex = 0;

        }
		switch(m_currSaveIndex)																		//判定当前存档点编号
		{
		case 0:																						
			GameDataManager.Instance.gameData.GameSaveData1 = _saveDataTemp;						//更改存档点1信息
			break;
		case 1:
			GameDataManager.Instance.gameData.GameSaveData2 = _saveDataTemp;						//更改存档点2信息
			break;
		case 2:
			GameDataManager.Instance.gameData.GameSaveData3 = _saveDataTemp;						//更改存档点3信息
			break;
		}
		GameDataManager.Instance.Save ();															//存储文件
		_saveDataTemp = null;																		//临时变量置空

		return m_currSaveIndex;																		//返回当前存档点编号
	}
	
	public void GetSaveData(int _saveIndex)															//当前需要读档
	{
		GetComponent<LevelTwoBackPackController>().enabled = false;							//关闭背包控制函数
		GameSaveData _saveDataTemp = new GameSaveData ();											//新建一个存档信息
		switch(_saveIndex)																			//判定当前点击的存档盒
		{
		case 0:																						//点击了存档点1
			_saveDataTemp = GameDataManager.Instance.gameData.GameSaveData1;
			break;
		case 1:																						//点击了存档点2
			_saveDataTemp = GameDataManager.Instance.gameData.GameSaveData2;
			break;
		case 2:																						//点击了存档点3
			_saveDataTemp = GameDataManager.Instance.gameData.GameSaveData3;
			break;
		}


		m_levelTwoHero.transform.position = _saveDataTemp.heroPosition;							//指定当前主角位置
		m_currMoneyNum = _saveDataTemp.heroMoney;											//指定当前钱数
		m_heroMoneyLabel.text = m_currMoneyNum.ToString();										//指定右上主角状态栏钱数
        SetItemEquipIndex(_saveDataTemp.equipedIndex);
        m_heroLifeObj.fillAmount = _saveDataTemp.heroLife;										//更改当前生命情况

		for(int i=0; i<_saveDataTemp.items.Length; i++)											//更改当前物品情况
			m_itemNum[i] = _saveDataTemp.items[i];	
		for(int i=0; i<_saveDataTemp.countryTaskState.Length; i++)								//指定当前任务完成情况
			m_taskIndexState[i] = _saveDataTemp.countryTaskState[i];


        for (int i = 0; i < _saveDataTemp.achieveState.Length; i++)                               // get 当前36计获得情况
            m_achieveGot[i] = _saveDataTemp.achieveState[i];

        m_saveNPCNum = _saveDataTemp.NPCIndex;                                                      //更改当前遇到的最后NPC编号


        SetVolume(0, _saveDataTemp.BGMVolume);          //设置当前音量情况
        SetVolume(1, _saveDataTemp.SoundVolume);


        GameDataManager.Instance.gameData.GameCurrentData = _saveDataTemp;

        _saveDataTemp = null;																		//临时变量置空
		this.GetComponent<LevelTwoBackPackController>().enabled = true;							//激活背包控制函数
	}
	
	public void DeleteSaveData(int _index)															//需要删除存档点
	{
		switch(_index)																				//判定删除的存档点编号
		{
		case 0:																															
			GameDataManager.Instance.gameData.GameSaveData1 = new GameSaveData();						//存档点1置空
			break;
		case 1:
			GameDataManager.Instance.gameData.GameSaveData2 = new GameSaveData();						//存档点2置空
			break;
		case 2:
			GameDataManager.Instance.gameData.GameSaveData3 = new GameSaveData();						//存档点3置空
			break;
		}
		GameDataManager.Instance.Save ();															//清空相应存档信息
	}

	public string GetSaveSceneName(int _index)									//外部获取存档点的场景名
	{
		string _name = " ";			
		int _levelIndex = 0;
		switch(_index)															//判定存档点								
		{
		case 0:
			_levelIndex = GameDataManager.Instance.gameData.GameSaveData1.levelIndex;
			break;
		case 1:
			_levelIndex = GameDataManager.Instance.gameData.GameSaveData2.levelIndex;
			break;
		case 2:
			_levelIndex = GameDataManager.Instance.gameData.GameSaveData3.levelIndex;
			break;
		}
		if(_levelIndex<=3)														//根据场景编号确定名称
			_name = "海滨小城";
		else if(_levelIndex==4)
			_name = "迷失森林";
		else if(_levelIndex==5)
			_name = "蜘蛛洞穴";
		else if(_levelIndex==6)
			_name = "龙吼火山";
		return _name;
	}
	
	public System.DateTime GetSaveDataTime(int _index)							//外部获取存档时的时间
	{
		System.DateTime _time = System.DateTime.Now;
		switch(_index)
		{
		case 0:
			_time = GameDataManager.Instance.gameData.GameSaveData1.savedTime;
			break;
		case 1:
			_time = GameDataManager.Instance.gameData.GameSaveData2.savedTime;
			break;
		case 2:
			_time = GameDataManager.Instance.gameData.GameSaveData3.savedTime;
			break;
		}
		return _time;
	}
	
	public int GetSaveNPCNum(int _index)										//外部获取存档时最后遇到的NPC编号
	{
		int _num = 0;
		switch(_index)
		{
		case 0:
			_num = GameDataManager.Instance.gameData.GameSaveData1.NPCIndex;
			break;
		case 1:
			_num = GameDataManager.Instance.gameData.GameSaveData2.NPCIndex;
			break;
		case 2:
			_num = GameDataManager.Instance.gameData.GameSaveData3.NPCIndex;
			break;
		}
		return _num;
	}
	
	public bool GetSaveBoxUsed(int _index)										//外部获取存档盒是否已经使用
	{
		bool _used = false;
		switch(_index)
		{
		case 0:
			_used = GameDataManager.Instance.gameData.GameSaveData1.used;
			break;
		case 1:
			_used = GameDataManager.Instance.gameData.GameSaveData2.used;
			break;
		case 2:
			_used = GameDataManager.Instance.gameData.GameSaveData3.used;
			break;
		}
		return _used;
	}

  
}
