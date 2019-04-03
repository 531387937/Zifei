using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;

public class HeadBattleGameManager : MonoBehaviour 
{

    public GameObject deadPanelMainmenuBtnObj;
    public GameObject deadPanelObj;//死亡界面

    public GameObject winPanelMainmenuBtnObj;
    public GameObject winPanelObj;//win界面

    public static HeadBattleGameManager Instance;								//实例化
	public GameObject m_headBattleHero;											//主角

	public UISprite m_heroLifeSprite;											//主角生命值
    public UILabel heroMoneyLabel;                                              //hero money
	private string m_messageContent;											//消息文字
	private int m_messageType;													//消息类型(1不用金币 2金币 3一句话)


    public GameObject[] m_countryHeadPicSelect;									
	public UIProgressBar m_countryHeadLifeBar;									//村长血量条
	public GameObject[] m_usualBtn;												//常用按钮        摇杆+背包+存档+泡泡纸
	public GameObject m_btnArrow;												//按钮箭头

	private bool m_boomerangOut = false;										//回旋镖是否发射出去
	private bool m_glassWaveEmit = false;                                       //眼镜光发出

    public GameObject[] m_heroMask;												//主角面具(0狐狸帽子 1眼镜 2德鲁伊 3胡子)

    private int m_itemEquipIndex = 0;
    private int[] m_achieveGot = new int[36];                                   //36计是否得到								

    //private int m_letterReadState = 0;                                          //读信阶段
    //private int m_waterBoxRepairState = 0;//水阀的状态
    //private int m_barTaskState = 0;//酒馆的状态
    //private int m_jarState = 0;													//罐子的状态（0初始 1已调查 2已碎）
    //private int m_HomeCoinState = 0;

    private float[] m_volume = new float[2];                                    //音量大小

    private bool m_headInjury = false;											//村长是否受伤
	private bool m_heroInjury = false;											//主角是否受伤
	private int m_attackType = 0;												//攻击方式
	private float m_attackTypeTimer = 3f;										//操作方式更换计时器
	private int m_battleState = 0;												//战斗状态
	private string m_diaText;													//对话内容
    private GameObject countryHeadLifeBarObj;

    [HideInInspector]
    public bool isUsualBtnCanClick = false;

    public bool isPlatformType_Mobild = false;//游戏的运行平台区分








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

        UIEventListener.Get(deadPanelMainmenuBtnObj).onClick = OnDeadMainMenuBtnClick;                  //点击按钮
        UIEventListener.Get(winPanelMainmenuBtnObj).onClick = OnWinMainMenuBtnClick;                  //点击按钮
        m_headBattleHero.SetActive(true);
        m_usualBtn[3].SetActive(true);

#if UNITY_ANDROID || UNITY_IPHONE
        isPlatformType_Mobild = true;
#elif UNITY_STANDALONE_WIN
        isPlatformType_Mobild = false;
#endif
    }


    private void Start()
    {
        CloseUsualBtn();
        GameDataManager.Instance.Load();

        m_heroLifeSprite.fillAmount = GameDataManager.Instance.gameData.GameCurrentData.heroLife;//获取生命值

        var m_currMoneyNum = GameDataManager.Instance.gameData.GameCurrentData.heroMoney;       //获取主角金币数
        heroMoneyLabel.text = m_currMoneyNum.ToString();


        for (int i = 0; i < m_achieveGot.Length; i++)                               //get当前36计获得情况
            m_achieveGot[i] = GameDataManager.Instance.gameData.GameCurrentData.achieveState[i];

		SetItemEquipIndex(GameDataManager.Instance.gameData.GameCurrentData.equipedIndex);

		countryHeadLifeBarObj = m_countryHeadLifeBar.transform.parent.gameObject;

        SetVolume(0, GameDataManager.Instance.gameData.GameCurrentData.BGMVolume);              //指定当前音量情况
        SetVolume(1, GameDataManager.Instance.gameData.GameCurrentData.SoundVolume);
    }

    void Update()
	{
        if (m_heroLifeSprite.fillAmount <= 0) return;
        if (m_countryHeadLifeBar.value <= 0) return;
     
		if(m_battleState>=3)
		{
			m_attackTypeTimer -= Time.deltaTime;
			if(m_attackTypeTimer<=0)												//每三秒更换一种操作方式
			{
				for(int i=0; i<m_countryHeadPicSelect.Length; i++)
					m_countryHeadPicSelect[i].SetActive(false);
				m_attackType = Random.Range(0,4);
				m_attackTypeTimer = 3f;			
				if(m_attackType!=3)
					m_countryHeadPicSelect[m_attackType].SetActive(true);
			}
		}

		ShowMessage ();

	}

	public void OpenUsualBtn()													//开启常用按钮
	{
        m_usualBtn[4].SetActive(true);//joydtick UI
		m_usualBtn[0].SetActive(true);//joystick
        //m_usualBtn[1].GetComponent<BoxCollider>().enabled = true;
        //m_usualBtn[2].GetComponent<BoxCollider>().enabled = true;
        //m_usualBtn[3].GetComponent<BoxCollider>().enabled = true;
        isUsualBtnCanClick = true;

    }
    public void CloseUsualBtn()													//关闭常用按钮
	{
		m_usualBtn[0].SetActive(false);
        m_usualBtn[4].SetActive(false);//joydtick UI

        //m_usualBtn[1].GetComponent<BoxCollider>().enabled = false;
        //m_usualBtn[2].GetComponent<BoxCollider>().enabled = false;

        //m_usualBtn[3].GetComponent<BoxCollider>().enabled = false;
        isUsualBtnCanClick = false;

    }
    public float GetVolume(int _index)                                          //外部获取音量情况
    {
        return m_volume[_index];
    }

    public void SetVolume(int _index, float _volume)                            //外部设置音量情况
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

    public int GetItemEquipIndex()                                              //外部获取当前装备的道具编号
    {
        return m_itemEquipIndex;
    }
    public void SetItemEquipIndex(int _ID)                                      //外部设置当前装备的编号
    {
        m_itemEquipIndex = _ID;
        for (int i = 0; i < m_heroMask.Length; i++)                                 //关闭所有道具
            m_heroMask[i].SetActive(false);
        if (_ID != 0)                                                               //如果使用了道具
        {

            if (_ID == 1)
                m_heroMask[3].SetActive(true);                                    //开启使用的道具
            else if (_ID == 2)
                m_heroMask[2].SetActive(true);                                    //开启使用的道具
            else if (_ID == 3)
                m_heroMask[0].SetActive(true);                                    //开启使用的道具
            else if (_ID == 4)
                m_heroMask[1].SetActive(true);                                    //开启使用的道具

        }

    }


    public int GetAchieveGot(int _index)                                        //外部获取36计获取情况
    {
        return m_achieveGot[_index];
    }

    public void SetAchieveGot(int _index, int _got)                         //外部设置36计获取情况
    {
        m_achieveGot[_index] = _got;
        //m_heroSoundObj.GetComponent<AudioSource>().clip = m_heroSoundClip[1];
        //m_heroSoundObj.GetComponent<AudioSource>().Play();
    }

    public float GetHeroScaleX()												//外部获取主角当前朝向
	{
		return m_headBattleHero.transform.localScale.x;
	}


	public bool GetHeroInjury()													//外部获取主角是否受伤
	{
		return m_heroInjury;
	}

    public void SetHeroInjury(bool _injury)										//外部设置主角是否受伤变量
	{
		m_heroInjury = _injury;
	}


    public float GetHeroBlood() {

        return m_heroLifeSprite.fillAmount;
    }

    private void OnDeadMainMenuBtnClick(GameObject go)
    {
        //m_audioObj[1].GetComponent<AudioSource>().Play();

        //Global.GetInstance().loadName = "SceneOne";
        //SceneManager.LoadScene("SceneOne");
        deadPanelObj.SetActive(false);
        m_battleState = 5;

    }


    public void SetHeroBloodReduce(float _num)
	{

		m_heroInjury = true;


        m_heroLifeSprite.fillAmount -= _num;
        if (m_heroLifeSprite.fillAmount <= 0)
		{
			GameDataManager.Instance.gameData.GameCurrentData.startState = true;
            GameDataManager.Instance.gameData.GameCurrentData.newHeroState = true;               //当前的数据改为 新数据
            //GameDataManager.Instance.Save();

            m_usualBtn[3].SetActive(false);
            m_usualBtn[4].SetActive(false);
			if (GetAchieveGot(30) == 0)
			{
				AudioManager.Instance.TipsSoundPlay(Global.GetInstance().audioName_AchieveGet);
				SetAchieveGot(30, 1);
				SetMessageType(3, "您获得了成就【借尸还魂】");
			}

            AudioManager.Instance.ThingSoundPlay(Global.GetInstance().audioName_DeadSound);
            m_headBattleHero.SetActive(false);
            deadPanelObj.SetActive(true);//显示死亡面板

           
		}
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
    //        m_headBattleHero.transform.GetChild(0).gameObject.SetActive(true);//开启道具装备
    //    else
    //        m_headBattleHero.transform.GetChild(0).gameObject.SetActive(false);//隐藏道具装备
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

    public int GetAttackType()													//外部获取攻击类型
	{
		return m_attackType;
	}

    public void SetAttackType(int _type)										//外部设置攻击类型
	{
		m_attackType = _type;
	}

	public bool GetHeadInjury()
	{
		return m_headInjury;
	}

    public void SetHeadInjury(bool _injury)
	{
		m_headInjury = _injury;
	}


    private void OnWinMainMenuBtnClick(GameObject go)
    {
        SaveCurrentData();
        //m_audioObj[1].GetComponent<AudioSource>().Play();
		winPanelMainmenuBtnObj.SetActive(false);
        Global.GetInstance().loadName = "SceneOne";
        SceneManager.LoadScene("SceneOne");

    }


    public void SetHeadBloodReduce(float _num)
	{

        if (!countryHeadLifeBarObj.activeSelf)
            countryHeadLifeBarObj.SetActive(true);

        m_headInjury = true;
		m_countryHeadLifeBar.value -= _num;

		if(m_countryHeadLifeBar.value<=0)
		{
			m_battleState = 4;
			GameDataManager.Instance.gameData.GameCurrentData.startState = true;
			//GameDataManager.Instance.Save();
            AudioManager.Instance.ThingSoundPlay(Global.GetInstance().audioName_SuccessSound);
			if (GetAchieveGot(4) == 0)
			{
				AudioManager.Instance.TipsSoundPlay(Global.GetInstance().audioName_AchieveGet);
				SetAchieveGot(4, 1);
				SetMessageType(3, "您获得了成就 【反客为主】");
			}    
            m_usualBtn[3].SetActive(false);
            m_usualBtn[4].SetActive(false);
            winPanelObj.SetActive(true);//显示胜利面板

        }
    }

	public int GetGameState()													//外部获取当前游戏状态
	{
		return m_battleState;
	}

	public void SetGameState(int _gameState)									//外部设置当前游戏状态
	{
		m_battleState = _gameState;

		if(_gameState==3)
			m_btnArrow.SetActive(false);
	}


	public void ClearMessage(){

		m_messageType = 0;
		m_messageContent = "";
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

	public int GetMessageType()													//外部获取需提示的消息类型
	{
		return m_messageType;
	}

	public string GetDialogText()
	{
		return m_diaText;
	}

	public void SetDialogText(string _text)
	{
		m_diaText = _text;
	}


    void SaveCurrentData() {
        GameDataManager.Instance.gameData.GameCurrentData.gameSceneName = SceneManager.GetActiveScene().name;//保存当前的scene名称
        GameDataManager.Instance.gameData.GameCurrentData.heroLife = m_heroLifeSprite.fillAmount;                                         //获取当前生命值
        GameDataManager.Instance.gameData.GameCurrentData.equipedIndex = m_itemEquipIndex;

        for (int i = 0; i < 36; i++)                               //保存当前36计获得情况
            GameDataManager.Instance.gameData.GameCurrentData.achieveState[i] = m_achieveGot[i];
    }

}
