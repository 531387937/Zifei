using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;
using System.Xml;

public class GameManager : MonoBehaviour 
{
	public static GameManager Instance;
	public GameObject m_sceneUIPanel;											//场景中的UI面板
    public GameObject m_gameManagerHero;										//主角
	public GameObject[] m_usualBtn;												//常用按钮 摇杆+背包+存档+泡泡纸
	public GameObject[] m_countryScene;											//场景图（0村庄 1主角家 2村长家 3主角家）
	public GameObject m_sceneNameInfo;											//场景名称提示条
	public UILabel m_sceneNameInfoLabel;										//场景名称提示条上的文字
	public UISprite m_lifeObj;													//生命物体
	public UILabel m_heroMoneyNumLabel;											//主角当前钱数
	public GameObject[] m_heroMask;												//主角的四个面具


	public BackPackController backpackCtr;
	public SmithyController smithyCtr;
	private UISpriteAnimation m_sceneNameInfoAni;								//场景名称提示条的动画控制器
	private int m_sceneNameInfoState = 0;										//场景名称提示条状态
	private float m_sceneNameInfoTimer = 1f;									//场景名称提示条计时器
	private int m_gameStartState = 0;											//游戏开始状态变量
	private int m_currentMoneyNum = 0;										    //玩家当前拥有的钱数
	private string m_dialogText;												//对话内容
	private bool m_downCheckActive = false;										//当前是否能够向下走
	private int m_backpackIndex = 0;											//当前打开的背包页面
	private int m_stoveState = 0;												//锅炉状态（0初始坏 1提示扳手修 2修好）
	private bool m_smithyOpen = false;											//铁匠铺买卖界面是否打开
	private bool m_barOpen = false;												//酒馆买卖界面是否打开
	private int m_levelState = 0;												//是否需要场景切换按钮（0村庄）
	private int[] m_itemNum = new int[20];										//物品数量
	private int m_itemEquipIndex = 0;											//当前装备的道具
	private bool m_closeJoyStick = false;										//关闭摇杆
	private int[] m_taskIndexState = new int[7];								//每个任务状态
	private int m_currScene = 0;												//玩家当前所处的场景（0村庄 1主角家 2村长家 3酒馆）
	private int m_currSaveIndex = 0;											//当前存至的档位编号
	private bool[] m_countryPanel = new bool[5];                                //村庄中的展板是否打开		（0老虎机）

    private int m_letterReadState = 0;                                          //读信阶段
    private int m_waterBoxRepairState = 0;//水阀的状态
    private int m_barTaskState  = 0;//酒馆的状态
	private int m_jarState = 0;													//罐子的状态（0初始 1已调查 2已碎）
    private int m_HomeCoinState = 0;

    private int m_messageType;													//消息类型(1不用金币 2金币 3一句话)
	private string m_messageContent;											//消息文字
	private int m_chooseOpen = 0;												//是否需要弹出选项框(0需要 1弹出醉鬼)
	private bool m_meetJar = false;												//玩家是否碰到罐子
	private int m_saveNPCNum = 0;												//主角遇到的NPC编号		
    
	private int [] m_flowerState = new int[3];									//花的状态
	private int [] m_lightState = new int[3];                                   //灯的状态
    private int  m_lightHouseState = 0;                                             //灯塔的状态
	private int  m_lightHouseStepState = 0;                                             //灯塔的阶段状态
    
    private int[] m_keyState = new int[4];										//四个钥匙状态（0未得到 1可以获得 2获得）
	private int[] m_seedState = new int[3];                                     //三个种子的状态（0未得到 1得到了 2种下得到燃料）

    private int m_endItemState = 0;

    private int m_headHomeGameState = 0;										//村长家游戏状态(0按下喇叭 1开始游戏 2完成游戏 3箱子打开)
	private int [] m_achieveGot = new int[36];									//36计是否得到								
	private bool m_helpPanelOpen = false;										//帮助展板是否打开
	private float[] m_volume = new float[2];									//音量大小

    public GameObject QuitPanelObj; // 退出游戏的界面

    [HideInInspector]
    public StartSceneController startSceneController;

    private LightHouseController lightHouseController;
    public DrunkardStateController druckarStateController;

    public WaterBoxController waterBoxController;
    public JarController jarController;
    public CameraController cameraCtr;
    public HeadHomeGame headHomeGame;

    [HideInInspector]
    public bool isPlatformType_Mobild = false;//游戏的运行平台区分

    [HideInInspector]
    public bool isUsualBtnCanClick = false;




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




	//-------------------------------------

    void Awake()
	{

		Instance = this;                                                        //单例化该类
        startSceneController = this.GetComponent<StartSceneController>();
        lightHouseController = this.GetComponent<LightHouseController>();

        for (int i = 0; i < m_volume.Length; i++)
            m_volume[i] = 1f;

        for (int i = 0; i < m_usualBtn.Length; i++)
            m_usualBtn[i].SetActive(true);


#if UNITY_ANDROID || UNITY_IPHONE
        isPlatformType_Mobild = true;
#elif UNITY_STANDALONE_WIN
        isPlatformType_Mobild = false;
#endif

    }


    void Start()
	{

        GameDataManager.Instance.Load();
  //      if (GameDataManager.Instance.gameData.GameCurrentData.startState)						    //如果需要开启开始界面
		//{
            CloseUsualBtn();													                    //关闭常用按钮
                startSceneController.enabled = true;			                                        //开启开始界面函数

            SetVolume(0, GameDataManager.Instance.gameData.GameCurrentData.BGMVolume);          //设置当前音量情况
            SetVolume(1, GameDataManager.Instance.gameData.GameCurrentData.SoundVolume);
        //}
  //      else 																	//不需要开启开始界面
		//{
  //          ReadData();//读取存档中的数据

  //          m_sceneNameInfoState = 1;											//播放场景名称提示条
  //      }
	}

    public  void ReadData() {

        m_gameManagerHero.transform.position = GameDataManager.Instance.gameData.GameCurrentData.heroPosition;//指定主角位置
        m_currentMoneyNum = GameDataManager.Instance.gameData.GameCurrentData.heroMoney;    //指定当前钱数
        m_heroMoneyNumLabel.text = m_currentMoneyNum.ToString();            //指定当前右上主角状态栏钱数
        m_lifeObj.fillAmount = GameDataManager.Instance.gameData.GameCurrentData.heroLife;//更改当前主角生命情况

        SetItemEquipIndex(GameDataManager.Instance.gameData.GameCurrentData.equipedIndex);      //指定主角的装备类型
		m_currSaveIndex = GameDataManager.Instance.gameData.GameCurrentData.saveDataIndex;



        for (int i = 0; i < m_itemNum.Length; i++)                              //更改当前物品情况
            m_itemNum[i] = GameDataManager.Instance.gameData.GameCurrentData.items[i];

        for (int i = 0; i < m_achieveGot.Length; i++)                           //指定当前36计获得情况
            m_achieveGot[i] = GameDataManager.Instance.gameData.GameCurrentData.achieveState[i];

		for (int i = 0; i < m_taskIndexState.Length; i++) {                       //指定当前任务完成情况
			m_taskIndexState[i] = GameDataManager.Instance.gameData.GameCurrentData.countryTaskState[i];
		}
        for (int i = 0; i < m_seedState.Length; i++)                                //种子状态
            m_seedState[i] = GameDataManager.Instance.gameData.GameCurrentData.seedState[i];

        for (int i = 0; i < m_keyState.Length; i++)                             //钥匙状态
            m_keyState[i] = GameDataManager.Instance.gameData.GameCurrentData.keyState[i];


        for (int i = 0; i < m_lightState.Length; i++)
            m_lightState[i] = GameDataManager.Instance.gameData.GameCurrentData.lightState[i];

        for (int i = 0; i < m_flowerState.Length; i++)
            m_flowerState[i] = GameDataManager.Instance.gameData.GameCurrentData.flowerState[i];


        m_letterReadState = GameDataManager.Instance.gameData.GameCurrentData.letterReadState;
        m_waterBoxRepairState = GameDataManager.Instance.gameData.GameCurrentData.waterBoxRepairSatate;
        m_jarState = GameDataManager.Instance.gameData.GameCurrentData.jarSatate;
        m_barTaskState = GameDataManager.Instance.gameData.GameCurrentData.barTaskState;
        m_HomeCoinState = GameDataManager.Instance.gameData.GameCurrentData.homeCoinState;
        m_stoveState = GameDataManager.Instance.gameData.GameCurrentData.stoveState;

        m_lightHouseState = GameDataManager.Instance.gameData.GameCurrentData.lightHouseState;
		m_lightHouseStepState =GameDataManager.Instance.gameData.GameCurrentData.lightHouseStepState;   //灯塔的阶段状态
        
		m_headHomeGameState = GameDataManager.Instance.gameData.GameCurrentData.headHomeGameState;

        SetVolume(0, GameDataManager.Instance.gameData.GameCurrentData.BGMVolume);          //设置当前音量情况
        SetVolume(1, GameDataManager.Instance.gameData.GameCurrentData.SoundVolume);

        GameDataManager.Instance.gameData.GameCurrentData.startState = true;                //当前位于村庄

        GetComponent<SmithyController>().enabled = true;                //开启买卖界面函数
        GetComponent<TaskController>().enabled = true;                  //开启任务控制函数
        GetComponent<BackPackController>().enabled = true;             //开启背包函数

        if (GameDataManager.Instance.gameData.GameCurrentData.getEndItemState != 0)                  //如果当前获得通关物品
        {
            m_endItemState = GameDataManager.Instance.gameData.GameCurrentData.getEndItemState;//灯塔状态
            GameDataManager.Instance.gameData.GameCurrentData.getEndItemState = 0;               //得到的通关物品编号归0
            m_gameStartState = 5;                                           //指定摄像机位置 灯塔特写
			SetLightHouseStepState(0);
			CloseUsualBtn();                                                //关闭常用按钮
        }
        else
		{
			m_endItemState = GameDataManager.Instance.gameData.GameCurrentData.getEndItemState;//灯塔状态
            m_gameStartState = 3;                                           //开始界面播放结束
            OpenUsualBtn();
        }


			m_meetJar = false;												//玩家是否碰到罐子


        lightHouseController.InitLightData();//灯塔设置状态
        druckarStateController.InitDrunkardState();//酒鬼设置状态
        waterBoxController.ResetWaterSwitchState();//水阀设置状态
        jarController.ResetJarState();//罐子设置状态

        //GameDataManager.Instance.Save();                                    //存储数据
    }



    void Update()
	{
		if(m_sceneNameInfoState!=0)												//如果需要播放场景名称提示条
			SceneNameInfoAni();                                                 //场景名称提示条动画

        if (Input.GetKeyDown(KeyCode.Escape))//点击退出的按钮
        {
            ShowQuitPanel();//显示 退出面板
        }

		ShowMessage ();

    }
    

    public void ShowQuitPanel() {
		
		if (backpackCtr.m_backPackOpening)
			backpackCtr.CloseBackPack ();

		if (smithyCtr.m_smithyState != 0)
			smithyCtr.CloseSmithy ();

		if(smithyCtr.m_barState != 0)
			smithyCtr.CloseBar ();

		CloseUsualBtn();
        QuitPanelObj.SetActive(true);
    }

    public void OnClickCancelBtn()
    {
        QuitPanelObj.SetActive(false);
        OpenUsualBtn();

    }

    public void OnClickQuitBtn()
    {


        //Instance.SaveCurrData();
        GameDataManager.Instance.gameData.GameCurrentData = SaveData();
        GameDataManager.Instance.Save();															//存储文件

        OpenUsualBtn();
        GameQiut();
    }

    void GameQiut() {

    #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
    #else
                Application.Quit();
    #endif

    }


	void SceneNameInfoAni()														//场景名称提示条动画
	{
		switch(m_sceneNameInfoState)											//判断当前场景名称提示条状态
		{
		case 1:																	//第一阶段
			m_sceneNameInfo.SetActive(true);									//打开场景名称提示条
			m_sceneNameInfoLabel.text = "海滨小城";								//当前场景名称
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
		m_closeJoyStick = true;
        isUsualBtnCanClick = false;

    }

	public bool GetJoyStickState()												//摇杆是否可用变量
	{
		return m_closeJoyStick;
	}
	public void SetJoyStickState(bool _joyStickState)
	{
		m_closeJoyStick = _joyStickState;
	}

    public int GetReadLetterState()                                             //外部获取信件读取状态
    {
        return m_letterReadState;
    }
    public void SetReadLetterState(int _state)                                  //外部设置信件读取状态
    {
        m_letterReadState = _state;
    }


    public int GetWaterBoxRepairState()                                             //外部获取水阀读取状态
    {
        return m_waterBoxRepairState;
    }
    public void SetWaterBoxRepairState(int _state)                                  //外部设水阀读取状态
    {
        m_waterBoxRepairState = _state;
    }


    public int GetJarState()                                                    //外部获取罐子状态
    {
        return m_jarState;
    }
    public void SetJarState(int _state)                                         //外部设置罐子状态
    {
        m_jarState = _state;
    }


    public int GetBarTaskState()                                             //外部获取酒馆附近任务读取状态
    {
        return m_barTaskState;
    }
    public void SetBarTaskState(int _state)                                  //外部设酒馆附近任务读取状态
    {
        m_barTaskState = _state;
        //if (m_barTaskState == 2)
        //    m_gameManagerHero.transform.GetChild(0).gameObject.SetActive(true);//开启道具装备
        //else
        //    m_gameManagerHero.transform.GetChild(0).gameObject.SetActive(false);//隐藏道具装备
    }

    public int GetHomeCoinState()                                                    //外部获取 hero家金币状态
    {
        return m_HomeCoinState;
    }
    public void SetHomeCoinState(int _state)                                         //外部设置hero家金币状态
    {
        m_HomeCoinState = _state;
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


	public int GetTaskIndexState(int _taskIndex)								//获取指定编号任务的状态
	{
		return m_taskIndexState [_taskIndex];
	}
	public void SetTaskIndexState(int _taskIndex, int _taskState)				//设定指定编号任务的状态
	{
		m_taskIndexState[_taskIndex] = _taskState;
	}

	public int GetGameStartState()												//外部获取游戏是否开始状态变量
	{
		return m_gameStartState;
	}
	public void SetGameStartState(int _startState)								//外部设置游戏开始状态变量
	{
		m_gameStartState = _startState;
		//if(m_gameStartState==2)
		//	this.GetComponent<StartSceneController>().enabled = true;
	}

	public float GetBloodNum()													//外部获取主角血量值
	{
		return m_lifeObj.fillAmount;
	}

    public void SetHeroBloodReduce(float _reduceNum)							//主角生命值减少量
	{
		m_lifeObj.fillAmount -= _reduceNum;
	}

	public int GetCurrMoney()													//外部获取玩家当前拥有的钱数
	{
        return m_currentMoneyNum;
	}

	public void SetCurrMoney(int _currMoney)									//外部指定玩家当前拥有的钱数
	{
		m_currentMoneyNum = _currMoney;
		m_heroMoneyNumLabel.text = m_currentMoneyNum.ToString();
		//GameDataManager.Instance.gameData.GameCurrentData.heroMoney = m_currentMoneyNum;


    }

    public void SetCurrAddMoney(int _addNum)									//外部指定当前玩家增加的钱数
	{
		m_currentMoneyNum += _addNum;
		m_heroMoneyNumLabel.text = m_currentMoneyNum.ToString();
		//GameDataManager.Instance.gameData.GameCurrentData.heroMoney = m_currentMoneyNum;
	}

	public string GetDialogText()												//获取对话内容（为打字机考虑）
	{
		return m_dialogText;
	}

    public void SetDialogText(string _text)										//指定对话内容（为打字机考虑）
	{
		m_dialogText = _text;
	}

	public void SetDownActive(bool _downSet)									//设置是否可以下行（在碰撞脚本里）
	{
		m_downCheckActive = _downSet;
	}

    public bool GetDownActive()													//获取是否可以下行
	{
		return m_downCheckActive;
	}

	public int GetBpIndex()														//获取当前打开的背包页面
	{
		return m_backpackIndex;
	}

    public void SetBpIndex(int _index)											//指定当前打开的背包页面
	{
		m_backpackIndex = _index;
	}

	public int GetStoveState()													//外部获取锅炉状态
	{
		return m_stoveState;
	}

    public void SetStoveState(int _stoveState)									//外部更改锅炉状态
	{
		m_stoveState = _stoveState;
	}

	public bool GetSmithyState()												//获取铁匠铺是否打开状态
	{
		return m_smithyOpen;
	}
	public void SetSmithyState(bool _smithyState)								//外部更改铁匠铺状态
	{
		m_smithyOpen = _smithyState;
	}

	public int GetCurrentScene()													//外部读取当前所处的场景
	{
		return m_currScene;
	}

    public void SetCurrentScene(int _sceneIndex)
	{
		m_currScene = _sceneIndex;
	}

	public int GetLevelIndex()													//外部获取需要转至的关卡
	{
		return m_levelState;
	}

    public void SetLevelIndex(int _levelIndex)									//外部更改需要转至的关卡
	{
		CloseUsualBtn ();														//关闭常用按钮
        for (int i = 0; i < m_usualBtn.Length; i++)
            m_usualBtn[i].SetActive(false);

        GameDataManager.Instance.gameData.GameCurrentData.heroLife = m_lifeObj.fillAmount; ;
        GameDataManager.Instance.gameData.GameCurrentData.heroMoney = m_currentMoneyNum;		//记录主角当前钱数
        GameDataManager.Instance.gameData.GameCurrentData.heroPosition = m_gameManagerHero.transform.position;//记录主角当前在村庄中的位置
        GameDataManager.Instance.gameData.GameCurrentData.equipedIndex = m_itemEquipIndex;

        for (int i = 0; i < m_itemNum.Length; i++)                                  //记录当前物品情况
            GameDataManager.Instance.gameData.GameCurrentData.items[i] = m_itemNum[i];
        for (int i = 0; i < m_achieveGot.Length; i++)                               //记录当前36计获得情况
            GameDataManager.Instance.gameData.GameCurrentData.achieveState[i] = m_achieveGot[i];
        for (int i = 0; i < m_taskIndexState.Length; i++)                           //记录当前任务状态
            GameDataManager.Instance.gameData.GameCurrentData.countryTaskState[i] = m_taskIndexState[i];
        for (int i = 0; i < m_seedState.Length; i++)                                    //记录当前种子情况
            GameDataManager.Instance.gameData.GameCurrentData.seedState[i] = m_seedState[i];
        for (int i = 0; i < m_keyState.Length; i++)                                  //记录当前钥匙情况
            GameDataManager.Instance.gameData.GameCurrentData.keyState[i] = m_keyState[i];

        GameDataManager.Instance.gameData.GameCurrentData.BGMVolume = m_volume[0];				//记录当前音量情况
        GameDataManager.Instance.gameData.GameCurrentData.SoundVolume = m_volume[1];														//存储文件
        GameDataManager.Instance.gameData.GameCurrentData = SaveData();
        GameDataManager.Instance.Save();										//存下数据
		m_levelState = _levelIndex;
	}

	public int GetHeadHomeState()												//外部获取村长家状态
	{
		return m_headHomeGameState;
	}

    public void SetHeadHomeState(int _state)									//外部设置村长家状态
	{
		m_headHomeGameState = _state;
	}

	public bool GetBarState()													//获取酒馆是否打开状态
	{
		return m_barOpen;
	}
	public void SetBarState(bool _barState)										//外部更改酒馆状态
	{
		m_barOpen = _barState;
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
    public void SetItemEquipIndex(int _ID)                                      //外部设置当前装备的编号
    {
            m_itemEquipIndex = _ID;

        for (int i = 0; i < m_heroMask.Length; i++)                                 //关闭所有道具
            m_heroMask[i].SetActive(false);

        if (_ID != 0)                                                               //如果使用了道具
            m_heroMask[_ID - 1].SetActive(true);
    }

	public bool GetPanelState(int _panelIndex)									//外部设置指定展板的开启状态
	{
		return m_countryPanel[_panelIndex];
	}
	public void SetPanelState(int _panelIndex, bool _panelState)				//外部获取指定展板的开启状态
	{
		m_countryPanel[_panelIndex] = _panelState;
	}

	public int GetChooseState()													//外部获取是否需要弹出选项框
	{
		return m_chooseOpen;
	}
	public void SetChooseState(int _state)										//外部设置是否需要弹出选项框
	{
		m_chooseOpen = _state;
	}

	public int GetKeyState(int _index)											//外部获取三种钥匙的状态
	{
		return m_keyState [_index];
	}
	public void SetKeyState(int _index, int _state)								//外部设置三种钥匙的状态
	{
		m_keyState [_index] = _state;
	}

	public int GetSeedState(int _index)											//外部获取三种种子的状态
	{
		return m_seedState [_index];
	}
	public void SetSeedState(int _index, int _state)							//外部设置三个种子的状态
	{
		m_seedState [_index] = _state;
	}

	public bool GetMeetJar()													//外部获取主角是否碰到罐子
	{
		return m_meetJar;
	}
	public void SetMeetJar(bool _meetJar)										//外部设置主角是否碰到罐子
	{
		m_meetJar = _meetJar;
	}

	public int GetSaveNPCNum()													//外部获取当前最后对话的NPC编号
	{
		return m_saveNPCNum;
	}
	public void SetSaveNPCNum(int _num)											//外部设置最后对话的NPC编号
	{
		m_saveNPCNum = _num;
	}


    public int GetLightState(int index)                                             //外部获取指定编号灯的状态
    {
        return m_lightState[index];
    }
    public void SetLightState(int index, int _state)                                //外部设置指定编号灯的状态
    {
        m_lightState[index] = _state;
    }
    public int GetFlowerState(int index)                                             //外部获取指定编号花的状态
    {
        return m_flowerState[index];
    }
    public void SetFlowerState(int index, int _state)                                //外部设置指定编号花的状态
    {
        m_flowerState[index] = _state;
    }

    public int GetLightHouseStepState()
    {
        return m_lightHouseStepState;
    }
    public void SetLightHouseStepState(int _state)
    {
        m_lightHouseStepState = _state;
    }



    public int GetLightHouseState()                                                //外部获取指定编号灯塔的状态
    {
        return m_lightHouseState;
    }
    public void SetLightHouseState(int _state)                                   //外部设置指定编号灯塔的状态
    {
        m_lightHouseState = _state;
    }

    public int GetEndItemState()                                                //外部获取指定编号灯塔的状态
    {
        return m_endItemState;
    }
    public void SetEndItemState(int _state)                                   //外部设置指定编号灯塔的状态
    {
        m_endItemState = _state;
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

    public void SetVolume(int _index, float _volume)                            //外部设置音量情况
    {
        m_volume[_index] = _volume;

        if (_index == 0)                                                          //如果是BGM
        {
            GameDataManager.Instance.gameData.GameCurrentData.BGMVolume = m_volume[_index];
            AudioManager.Instance.SetBGMVolume(_volume);
        }else 																	//如果是音效
		{
            GameDataManager.Instance.gameData.GameCurrentData.SoundVolume = m_volume[_index];
            AudioManager.Instance.SetSoundVolume(_volume);

        }
    }

	public bool GetHelpPanelOpen()												//外部获取是否打开帮助面板
	{
		return m_helpPanelOpen;
	}
	public void SetHelpPanelOpen(bool _open)									//外部设置是否打开帮助面板
	{
		m_helpPanelOpen = _open;
	}

	public string GetSaveSceneName(int _index)									//外部获取存档点的场景名
	{
		string _sceneName = " ";			
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
			_sceneName = "海滨小城";
		else if(_levelIndex==4)
			_sceneName = "迷失森林";
		else if(_levelIndex==5)
			_sceneName = "蜘蛛洞穴";
		else if(_levelIndex==6)
			_sceneName = "龙吼火山";

		return _sceneName;
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

	public int GetSaveNPCNum(int _index)							//外部获取存档时最后遇到的NPC编号
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

	public bool GetSaveBoxUsed(int _index)
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

	public void InitData()														//数据初始化
	{
        GameDataManager.Instance.gameData.GameCurrentData = new GameSaveData();
	
        GameDataManager.Instance.gameData.GameCurrentData.BGMVolume = m_volume[0];
        GameDataManager.Instance.gameData.GameCurrentData.SoundVolume = m_volume[1];
        GameDataManager.Instance.gameData.GameCurrentData.newHeroState = false;
        GameDataManager.Instance.Save();                                        //存档情况归零
		ReadData();//读取存档中的数据


        this.GetComponent<SmithyController>().enabled = true;					//开启买卖控制函数
		this.GetComponent<TaskController>().enabled = true;						//开启任务控制函数
		this.GetComponent<BackPackController>().enabled = true;                 //开启背包函数

        m_sceneNameInfoState = 1;

	}

	public void ChangeCurrScene(int _newScene)									//村庄中更换当前所处场景
	{
		for(int i=0; i<m_countryScene.Length; i++)								//遍历所有村庄图
			m_countryScene[i].SetActive(false);                                 //全部隐藏

        m_countryScene [_newScene].SetActive (true);							//开启需要在的村庄图
		m_currScene = _newScene;												//完成切换
		if (m_currScene != 0)
			CheckBtnController.Instance.m_thingDialogState = false;
		
    }

	public void GetSaveData(int _saveIndex)															//当前需要读档
	{
		this.GetComponent<SmithyController>().enabled = false;										//关闭买卖控制函数
		this.GetComponent<BackPackController>().enabled = false;									//关闭背包控制函数
		GameSaveData _saveDataTemp = new GameSaveData ();
		switch(_saveIndex)
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
        case 3:
            _saveDataTemp = GameDataManager.Instance.gameData.GameCurrentData;//点击继续按钮
            break;

        }

        _saveDataTemp.newHeroState = false;
        m_gameManagerHero.transform.position = _saveDataTemp.heroPosition;                        //指定当前主角位置
        m_currentMoneyNum = _saveDataTemp.heroMoney;											//指定当前钱数
		m_heroMoneyNumLabel.text = m_currentMoneyNum.ToString();                                    //指定右上主角状态栏钱数
		m_lifeObj.fillAmount = _saveDataTemp.heroLife;											//更改当前生命情况
        SetItemEquipIndex(_saveDataTemp.equipedIndex);
		m_currSaveIndex = _saveDataTemp.saveDataIndex;

        for (int i=0; i<_saveDataTemp.items.Length; i++)											//更改当前物品情况
			m_itemNum[i] = _saveDataTemp.items[i];

        for (int i=0; i<_saveDataTemp.countryTaskState.Length; i++)								//指定当前任务完成情况
			m_taskIndexState[i] = _saveDataTemp.countryTaskState[i];

        for (int i = 0; i < _saveDataTemp.achieveState.Length; i++)                               // get 当前36计获得情况
            m_achieveGot[i] = _saveDataTemp.achieveState[i];

        for (int i = 0; i < _saveDataTemp.seedState.Length; i++)                                //get 种子状态
            m_seedState[i] = _saveDataTemp.seedState[i];

        for (int i = 0; i < _saveDataTemp.keyState.Length; i++)                             //get 钥匙状态
            m_keyState[i] = _saveDataTemp.keyState[i];

        for (int i = 0; i < _saveDataTemp.lightState.Length; i++)                             //
            m_lightState[i] = _saveDataTemp.lightState[i];

        for (int i = 0; i < _saveDataTemp.flowerState.Length; i++)                             //
            m_flowerState[i] = _saveDataTemp.flowerState[i];


        SetReadLetterState( _saveDataTemp.letterReadState);                                    //信件的状态
        SetWaterBoxRepairState(_saveDataTemp.waterBoxRepairSatate);
        SetJarState(_saveDataTemp.jarSatate);                                             //罐子的状态
        SetBarTaskState(_saveDataTemp.barTaskState);                                         //酒馆任务的状态
        SetHomeCoinState(_saveDataTemp.homeCoinState);
        SetStoveState(_saveDataTemp.stoveState);
        SetEndItemState(_saveDataTemp.getEndItemState);
        SetLightHouseState(_saveDataTemp.lightHouseState);
        SetLightHouseStepState(_saveDataTemp.lightHouseStepState);
        SetHeadHomeState(_saveDataTemp.headHomeGameState);

		m_meetJar = false;	//重置状态											//玩家是否碰到罐子



        if (m_currScene != _saveDataTemp.levelIndex && _saveDataTemp.levelIndex < 4)			//需要更换当前场景（且是村庄中的场景）
			ChangeCurrScene(_saveDataTemp.levelIndex);

		m_saveNPCNum = _saveDataTemp.NPCIndex;                                                      //更改当前遇到的最后NPC编号

        SetVolume(0, _saveDataTemp.BGMVolume);          //设置当前音量情况
        SetVolume(1, _saveDataTemp.SoundVolume);


        lightHouseController.InitLightData();//灯塔设置状态
        druckarStateController.InitDrunkardState();//酒鬼设置状态
        waterBoxController.ResetWaterSwitchState();//水阀设置状态
        jarController.ResetJarState();//罐子设置状态
        headHomeGame.ResetCabinetState();//重置密室门的状态

        //--------------------------------------------------------------
        GameDataManager.Instance.gameData.GameCurrentData = _saveDataTemp;


        this.GetComponent<SmithyController>().enabled = true;										//激活买卖控制函数
		this.GetComponent<BackPackController>().enabled = true;										//激活背包控制函数
		this.GetComponent<TaskController>().enabled = true;											//开启任务控制函数
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
		GameDataManager.Instance.Save ();
	}

	public int SaveCurrData(bool AutoSave=true)																		//当前需要存档
	{

        GameSaveData dataTemp = SaveData();
        GameDataManager.Instance.gameData.GameCurrentData = dataTemp;



        //-----------------------查找存档的档位-------------------------------------------------------
        if (!AutoSave)
        {
            if (!GameDataManager.Instance.gameData.GameSaveData1.used)                                  //如果存档点1未被使用
                m_currSaveIndex = 0;
            else if (!GameDataManager.Instance.gameData.GameSaveData2.used)                         //如果存档点2未被使用
                m_currSaveIndex = 1;
            //else if(!GameDataManager.Instance.gameData.GameSaveData3.used)							//如果存档点3未被使用
            //	m_currSaveIndex = 2;
            else                                                                                        //如果三个存档点均被使用
            {
                m_currSaveIndex = GameDataManager.Instance.gameData.GameCurrentData.saveDataIndex;
                if (m_currSaveIndex != 1)                                                                   //从最近一次存档点索引开始向后覆盖
                    m_currSaveIndex++;
                else
                    m_currSaveIndex = 0;
            }
            GameDataManager.Instance.gameData.GameCurrentData.saveDataIndex = m_currSaveIndex;
            switch (m_currSaveIndex)                                                                        //判定当前存档点编号
            {
                case 0:
                    GameDataManager.Instance.gameData.GameSaveData1 = dataTemp;                     //更改存档点1信息
                    break;
                case 1:
                    GameDataManager.Instance.gameData.GameSaveData2 = dataTemp;                     //更改存档点2信息
                    break;
                //case 2:
                //    GameDataManager.Instance.gameData.GameSaveData3 = dataTemp;                     //更改存档点3信息
                //    break;
            }
        }
        if(AutoSave)
        {
            GameDataManager.Instance.gameData.GameSaveData3 = dataTemp;
        }
		GameDataManager.Instance.Save ();															//存储文件

        return m_currSaveIndex;																		//返回当前存档点编号
	}
    //保存当前的数据
    public GameSaveData SaveData() {

        GameSaveData _saveDataTemp = new GameSaveData();                                           //生成临时变量

        _saveDataTemp.newHeroState = false;
        _saveDataTemp.startState = false;
        _saveDataTemp.used = true;                                                              //该存档点已使用
        _saveDataTemp.levelIndex = m_currScene;													//获取当前场景号(（0村庄 1主角家 2村长家 3主角家）)
        _saveDataTemp.gameSceneName = SceneManager.GetActiveScene().name;//保存当前的scene名称

        _saveDataTemp.heroPosition = m_gameManagerHero.transform.position;                      //获取当前主角位置
        _saveDataTemp.heroMoney = m_currentMoneyNum;                                            //获取当前钱数
        _saveDataTemp.heroLife = m_lifeObj.fillAmount;                                         //获取当前生命值
		_saveDataTemp.equipedIndex = m_itemEquipIndex;
		_saveDataTemp.saveDataIndex = m_currSaveIndex;


        for (int i = 0; i < _saveDataTemp.items.Length; i++)                                        //获取当前物品拥有情况
            _saveDataTemp.items[i] = m_itemNum[i];

        for (int i = 0; i < _saveDataTemp.countryTaskState.Length; i++)                               //获取当前任务完成情况
            _saveDataTemp.countryTaskState[i] = m_taskIndexState[i];

        for (int i = 0; i < _saveDataTemp.achieveState.Length; i++)                               //保存当前36计获得情况
            _saveDataTemp.achieveState[i] = m_achieveGot[i];


        for (int i = 0; i < _saveDataTemp.seedState.Length; i++)                                //save 种子状态
            _saveDataTemp.seedState[i] = m_seedState[i];

        for (int i = 0; i < _saveDataTemp.keyState.Length; i++)                             //save 钥匙状态
            _saveDataTemp.keyState[i] = m_keyState[i];

        for (int i = 0; i < _saveDataTemp.lightState.Length; i++)                             //
            _saveDataTemp.lightState[i] = m_lightState[i];

        for (int i = 0; i < _saveDataTemp.flowerState.Length; i++)                             //
            _saveDataTemp.flowerState[i] = m_flowerState[i];



        _saveDataTemp.letterReadState = GetReadLetterState();//信件的状态
        _saveDataTemp.waterBoxRepairSatate = GetWaterBoxRepairState();
        _saveDataTemp.jarSatate = GetJarState();//罐子的状态
        _saveDataTemp.barTaskState = GetBarTaskState();//酒馆任务的状态
        _saveDataTemp.homeCoinState = GetHomeCoinState();
        _saveDataTemp.stoveState = GetStoveState();
        _saveDataTemp.getEndItemState = GetEndItemState();
        _saveDataTemp.lightHouseState = GetLightHouseState();
        _saveDataTemp.lightHouseStepState = GetLightHouseStepState();
        _saveDataTemp.headHomeGameState = GetHeadHomeState();
        

        _saveDataTemp.savedTime = System.DateTime.Now;                                              //获取当前系统时间
        _saveDataTemp.NPCIndex = m_saveNPCNum;                                                      //获取当前遇到的最后NPC

        _saveDataTemp.BGMVolume = m_volume[0];              //记录当前音量情况
        _saveDataTemp.SoundVolume = m_volume[1];

        return _saveDataTemp;
    }
}
