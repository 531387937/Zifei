using UnityEngine;
using System.Collections;

public class HeadBattleCheckBtnController : MonoBehaviour 
{
	public GameObject m_longPressPanel;											//长按出现的三个按钮
	public GameObject m_headBattleHero;											//第二关主角
	public GameObject m_headBattleCheckBtn;										//第二关查看按钮
	
	public Transform m_headBattleHeroBulletPos;									//主角发射武器的位置
	public GameObject m_stone;													//普通石头
	public GameObject m_frogStone;												//狐狸石头
	
	public GameObject[] m_longPressPanelBtn;									//长按出现的面板的三个按钮

	public GameObject[] m_heroMask;												//主角面具(0狐狸帽子 1眼镜 2德鲁伊 3胡子)
	public UILabel m_dialogLabel;												//对话框
	public Transform m_dialogPos;												//对话框的位置


	private int m_bulletType = 0;												//当时可发射的子弹类型
	private bool m_headBattleDoorCheck = false;									//第二关是否碰到门检测
	private float m_btnDownTimer = 0f;											//按钮按下计时器
	private int m_btnDownState = 0;										
	private int m_checkBtnState = 0;
	private string[] m_dialogText = new string[3];								//对话内容

    private HeadBattleTypeWriter typeWriter;
    private BoxCollider checkBtnCollider;


    public float normalStoneShotFreq = 0.8f;
    public float AdultMaskStoneShotFreq = 0.5f;
    public float frogStoneShotFreq = 0.5f;
    public float boomerShotFreq = 1f;
    public float glassWaveShotFreq = 1f;

    private bool isCanShot = false;
    private float shotFreq = 0f;
    private float shotTimer = 0f;
    private int[] m_itemNum = new int[20];										//物品数量


    void Awake()
	{

        checkBtnCollider = m_headBattleCheckBtn.GetComponent<BoxCollider>();
		UIEventListener.Get (m_headBattleCheckBtn).onPress = OnThreeCheckBtnPress;//一直按着查看按钮

        UIEventListener.Get (m_longPressPanelBtn[0]).onClick = OnFrogStoneBtnClick;	//单击狐狸面板按钮
        UIEventListener.Get (m_longPressPanelBtn[1]).onClick = OnGlassWaveBtnClick;	//单击眼镜按钮
		UIEventListener.Get (m_longPressPanelBtn[2]).onClick = OnBoomerangBtnClick; //单击回旋镖按钮
        UIEventListener.Get (m_longPressPanelBtn[3]).onClick = OnAdultMaskBtnClick; //单击面具面板按钮


        shotFreq = normalStoneShotFreq;

      
    }

    void Start()
	{
		GameDataManager.Instance.Load();
		for (int i = 0; i < m_itemNum.Length; i++)                                        //获取当前物品拥有情况
			m_itemNum[i] = GameDataManager.Instance.gameData.GameCurrentData.items[i];

		int equipedIndex = GameDataManager.Instance.gameData.GameCurrentData.equipedIndex;

		m_longPressPanelBtn[0].GetComponent<BoxCollider>().enabled = m_itemNum[6] != 0;
		m_longPressPanelBtn[1].GetComponent<BoxCollider>().enabled = m_itemNum[7] != 0;
		m_longPressPanelBtn[2].GetComponent<BoxCollider>().enabled = m_itemNum[5] != 0;
		m_longPressPanelBtn[3].GetComponent<BoxCollider>().enabled = m_itemNum[4] != 0;

		m_heroMask[0].SetActive(m_itemNum[6] != 0);
		m_heroMask[1].SetActive(m_itemNum[7] != 0);
		m_heroMask[2].SetActive(m_itemNum[5] != 0);
		m_heroMask[3].SetActive(m_itemNum[4] != 0);

		m_bulletType = 0;
		if (equipedIndex == 1)
			m_bulletType = 4;
		else if (equipedIndex == 2)
			m_bulletType = 3;
		else if (equipedIndex == 3)
			m_bulletType = 1;
		else if (equipedIndex == 4)
			m_bulletType = 2;


		m_dialogText [0] = "想不到你竟然还活着!";
		m_dialogText [1] = "我的孙女？哈哈哈…只有傻瓜才会相信!";
		m_dialogText [2] = "比我优秀的勇者都不应该出现的,所以你可以永远的消失了！";
	}

	void CloseDialogBox()
	{
		m_dialogLabel.gameObject.SetActive (false);
		if(m_dialogLabel.gameObject.GetComponent("HeadBattleTypeWriter"))					//如果存在打印机脚本
			Destroy(m_dialogLabel.gameObject.GetComponent("HeadBattleTypeWriter"));			//移除打印机脚本
		m_dialogLabel.text = " ";
        HeadBattleGameManager.Instance.OpenUsualBtn();

    }

	void OpenDialogBox(int _index)
	{
		if(_index==0)
		{
			HeadBattleGameManager.Instance.SetGameState(1);								//村长转为说话阶段
		}
		m_dialogLabel.gameObject.SetActive (true);
		Vector3 _screenPos = Camera.main.WorldToScreenPoint (m_dialogPos.position);		//世界坐标转为屏幕坐标	
		Vector3 _uiScreenPos = UICamera.mainCamera.ScreenToWorldPoint(_screenPos);		//转为NGUI世界坐标
		_uiScreenPos.z = 0f;															//NGUI坐标中去掉Z轴
		m_dialogLabel.gameObject.transform.position = _uiScreenPos;						//指定对话框位置
		m_dialogLabel.text = " ";										//置空对话内容
		HeadBattleGameManager.Instance.SetDialogText (m_dialogText [_index]);
        typeWriter = m_dialogLabel.gameObject.GetComponent<HeadBattleTypeWriter>();
        if (typeWriter != null)
        {
            Destroy(typeWriter);
        }
        typeWriter = m_dialogLabel.gameObject.AddComponent<HeadBattleTypeWriter>();                     //添加打印机效果

    }


    void OnAdultMaskBtnClick(GameObject _frogBtn)                               //按下胡子按钮
    {
        AudioManager.Instance.SoundPlay(Global.GetInstance().audioName_BtnClick2);
        if (m_bulletType == 4)
        {
            m_bulletType = 0;
            shotFreq = normalStoneShotFreq;
            HeadBattleGameManager.Instance.SetItemEquipIndex(0);
        }
        else
        {

            m_bulletType = 4;
            shotFreq = AdultMaskStoneShotFreq;
            HeadBattleGameManager.Instance.SetItemEquipIndex(1);

        }
    }

    void OnFrogStoneBtnClick(GameObject _frogBtn)								//按下狐狸石头按钮
	{
        AudioManager.Instance.SoundPlay(Global.GetInstance().audioName_BtnClick2);


		if(m_bulletType==1)
		{
            m_bulletType = 0;
            shotFreq = normalStoneShotFreq;
            HeadBattleGameManager.Instance.SetItemEquipIndex(0);
        }
		else
		{

			if (HeadBattleGameManager.Instance.GetAchieveGot(3) == 0)
			{
				AudioManager.Instance.TipsSoundPlay(Global.GetInstance().audioName_AchieveGet);
				HeadBattleGameManager.Instance.SetAchieveGot(3, 1);
				HeadBattleGameManager.Instance.SetMessageType(3, "您获得了成就 【反间计】");
			}  
            m_bulletType = 1;
            shotFreq = frogStoneShotFreq;
            HeadBattleGameManager.Instance.SetItemEquipIndex(3);

        }
    }
	
	void OnGlassWaveBtnClick(GameObject _waveBtn)								//按下眼镜光按钮
	{
        AudioManager.Instance.SoundPlay(Global.GetInstance().audioName_BtnClick2);


		if(m_bulletType==2)
		{
			m_bulletType = 0;
            shotFreq = normalStoneShotFreq;
            HeadBattleGameManager.Instance.SetItemEquipIndex(0);

        }
        else
		{

			if (HeadBattleGameManager.Instance.GetAchieveGot(5) == 0)
			{
				AudioManager.Instance.TipsSoundPlay(Global.GetInstance().audioName_AchieveGet);
				HeadBattleGameManager.Instance.SetAchieveGot(5, 1);
				HeadBattleGameManager.Instance.SetMessageType(3, "您获得了成就 【釜底抽薪】");
			}
			m_bulletType = 2;
            shotFreq = glassWaveShotFreq;
            HeadBattleGameManager.Instance.SetItemEquipIndex(4);

        }
    }
	
	void OnBoomerangBtnClick(GameObject _boomerangBtn)							//按下回旋镖按钮
	{
        AudioManager.Instance.SoundPlay(Global.GetInstance().audioName_BtnClick2);


		if(m_bulletType==3)
		{
			m_bulletType = 0;
            shotFreq = normalStoneShotFreq;
            HeadBattleGameManager.Instance.SetItemEquipIndex(0);

        }
        else
		{

			if (HeadBattleGameManager.Instance.GetAchieveGot(18) == 0)
			{
				AudioManager.Instance.TipsSoundPlay(Global.GetInstance().audioName_AchieveGet);
				HeadBattleGameManager.Instance.SetAchieveGot(18, 1);
				HeadBattleGameManager.Instance.SetMessageType(3, "您获得了成就 【树上开花】");
			}
            HeadBattleGameManager.Instance.SetItemEquipIndex(2);
            m_bulletType = 3;
            shotFreq = boomerShotFreq;
        }
	}
	
	void OnThreeCheckBtnPress(GameObject _btn, bool _down)
	{
		switch(m_checkBtnState)
		{
		case 0:
		case 1:
		case 2:
			if (m_checkBtnState == 2) {
				if (HeadBattleGameManager.Instance.GetAchieveGot(22) == 0)
				{
					AudioManager.Instance.TipsSoundPlay(Global.GetInstance().audioName_AchieveGet);
					HeadBattleGameManager.Instance.SetAchieveGot(22, 1);
					HeadBattleGameManager.Instance.SetMessageType(3, "您获得了成就【笑里藏刀】");
				}
			}

			if(!_down)
			{
				OpenDialogBox(m_checkBtnState);
				m_checkBtnState++;
			}

			break;
		case 3:
			CloseDialogBox();
			m_checkBtnState = 4;
			HeadBattleGameManager.Instance.SetGameState(2);
			break;
		case 4:
			if(_down)
			{
				m_btnDownState = 1;
				m_btnDownTimer = 0f;
			}
			else
			{
				m_btnDownState = 2;
				if(m_btnDownTimer<0.5f)												//短按		
				{
					m_longPressPanel.SetActive(false);								//关闭长按出现的三个按钮面板
					OnCheckBtnClick();											//调用点击右下按钮函数
				}
			}
			break;
		}

	}
	
	
	void OnCheckBtnClick()
	{
        if (isCanShot)
        {
            switch (m_bulletType)                                               //判定当前发射的子弹类型
            {
                case 0:                                                             //发射的子弹为普通石头
                    AudioManager.Instance.HeroAttackSoundPlay(Global.GetInstance().audioName_Stone);
                    GameObject cloneBullet = (GameObject)Instantiate(
                        m_stone, m_headBattleHeroBulletPos.position, transform.rotation);   //初始化子弹
                    isCanShot = false;
                    break;
                case 1:                                                             //发射的子弹为狐狸子弹
                    AudioManager.Instance.HeroAttackSoundPlay(Global.GetInstance().audioName_Stone);

                    GameObject cloneFrogBullet = (GameObject)Instantiate(
                        m_frogStone, m_headBattleHeroBulletPos.position, transform.rotation);   //初始化子弹
                    isCanShot = false;
                    break;
                case 2:                                                             //发射的子弹为眼镜光
                    if (!HeadBattleGameManager.Instance.GetGlassWaveEmit())
                    {
                        AudioManager.Instance.HeroAttackSoundPlay(Global.GetInstance().audioName_GlassWave);
                        HeadBattleGameManager.Instance.SetGlassWaveEmit(true);      //发射眼镜光
                    }
                    isCanShot = false;
                    break;
                case 3:                                                             //发射的子弹为回旋镖
                    if (!HeadBattleGameManager.Instance.GetBoomerangOut())
                    {
                        AudioManager.Instance.HeroAttackSoundPlay(Global.GetInstance().audioName_Boomerang);
                        HeadBattleGameManager.Instance.SetBoomerangOut(true);       //发射回旋镖
                    }
                    isCanShot = false;
                    break;
                case 4:   //发射的子弹
                        AudioManager.Instance.HeroAttackSoundPlay(Global.GetInstance().audioName_Stone);
                    GameObject cloneBullet2 =Instantiate(
                        m_stone, m_headBattleHeroBulletPos.position, transform.rotation);   //初始化子弹
                    isCanShot = false;
                    break;
            }
        }
	}

	void Update()
	{
        if (HeadBattleGameManager.Instance.GetHeroBlood() <= 0) return;

        if (!HeadBattleGameManager.Instance.isPlatformType_Mobild && HeadBattleGameManager.Instance.isUsualBtnCanClick)
        {
                if (HeadBattleGameManager.Instance.isUsualBtnCanClick && (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.KeypadEnter)))
            {
                OnCheckBtnClick();
            }
        }

        if (!isCanShot)//计算发射子弹的频率时间
        {
            shotTimer -= Time.deltaTime;
            if (shotTimer <= 0)
            {
                shotTimer = shotFreq;
                isCanShot = true;
            }
        }

        if (typeWriter != null && m_checkBtnState <= 3 )
        {
            checkBtnCollider.enabled  = ! typeWriter.isActive;//在村长说话期间是不能点击按钮的
            
        }

        if (m_btnDownState==1)
		{
			m_btnDownTimer += Time.deltaTime;
			if(m_btnDownTimer>=0.5f)
				m_longPressPanel.SetActive(true);
		}
	}
}
