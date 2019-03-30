using UnityEngine;
using System.Collections;

public class LevelTwoCheckBtnController : MonoBehaviour 
{
	public static LevelTwoCheckBtnController Instance;							//实例化
	public GameObject m_levelTwoDoorIcon;										//门的图标
	public GameObject m_levelTwoHero;											//第二关主角
	public GameObject[] m_levelTwoCheckObjCollider;								//第二关可查看物品包围盒
	public GameObject m_levelTwoCheckBtn;										//第二关查看按钮
	public GameObject m_levelTwoHeroStone;										//主角攻击用的石头
	public GameObject m_levelTwoFrogStone;										//主角攻击用的狐狸石头
	public Transform m_levelTwoHeroBulletPos;									//主角发射武器的位置
	public Sprite[] m_bagSprite;												//书包图
	public GameObject[] m_diaryPanel;											//日记面板（0页面 1第二页文字 2遮罩）

	public GameObject m_longPressPanel;											//长按出现的三个按钮
	public GameObject[] m_longPressPanelBtn;									//长按出现的面板的三个按钮


	private int m_diaryOpenState = 0;											//日记打开阶段
	private int m_diaryCurrPage = 1;											//日记当前打开的页数
	private bool m_levelTwoCheck = false;										//第二关是否碰到物品检测
	private int m_checkObjIndex = 0;											//检测物编号
	private int m_openDiary = 0;												//是否查看日记本(0未查看 1正在查看 2已经查看)
	
	private float m_btnDownTimer = 0f;											//按钮按下计时器
	private int m_btnDownState = 0;			

	public GameObject m_endGetItem;												//通关时获得的物品
	private int m_itemState = 0;												//通关物品状态
	private float m_itemTimer = 3f;												//通关物品计时器
	private BoxCollider m_collider;												//通关物品包围盒
	private bool m_endItemCheck = false;                                        //是否检测到通关物品

    public float normalStoneShotFreq = 0.5f;
    public float frogStoneShotFreq = 0.5f;
    public float boomerShotFreq = 1f;
    public float glassWaveShotFreq = 1f;

    private bool isCanShot = false;
    private float shotFreq = 0f;
    private float shotTimer = 0f;

    void Awake()
	{
		Instance = this;
		UIEventListener.Get (m_levelTwoCheckBtn).onPress = OnTwoCheckBtnPress;	//按下查看按钮
		UIEventListener.Get (m_longPressPanelBtn[0]).onClick = OnFrogStoneBtnClick;	//单击狐狸面板按钮
		UIEventListener.Get (m_longPressPanelBtn[1]).onClick = OnGlassWaveBtnClick;	//单击眼镜按钮
		UIEventListener.Get (m_longPressPanelBtn[2]).onClick = OnBoomerangBtnClick;	//单击回旋镖按钮

        shotFreq = normalStoneShotFreq;
    }


    void OnFrogStoneBtnClick(GameObject _frogBtn)                               //按下狐狸石头按钮
    {
        AudioManager.Instance.SoundPlay(Global.GetInstance().audioName_BtnClick2);
        if (LevelTwoGameManager.Instance.GetItemNum(6) != 0)
        {
            if (LevelTwoGameManager.Instance.GetItemEquipIndex() == 3)
            {
                LevelTwoGameManager.Instance.SetItemEquipIndex(0);
                shotFreq = normalStoneShotFreq;
            }
            else
            {
                LevelTwoGameManager.Instance.SetItemEquipIndex(3);
                shotFreq = frogStoneShotFreq;

            }
        }
    }

    void OnGlassWaveBtnClick(GameObject _waveBtn)                               //按下眼镜光按钮
    {
        AudioManager.Instance.SoundPlay(Global.GetInstance().audioName_BtnClick2);
        if (LevelTwoGameManager.Instance.GetItemNum(7) != 0)
        {
            if (LevelTwoGameManager.Instance.GetItemEquipIndex() == 4)
            {
                LevelTwoGameManager.Instance.SetItemEquipIndex(0);
                shotFreq = normalStoneShotFreq;
            }
            else
            {
                LevelTwoGameManager.Instance.SetItemEquipIndex(4);
                shotFreq = glassWaveShotFreq;
            }

        }
    }

    void OnBoomerangBtnClick(GameObject _boomerangBtn)                          //按下回旋镖按钮
    {
        AudioManager.Instance.SoundPlay(Global.GetInstance().audioName_BtnClick2);
        if (LevelTwoGameManager.Instance.GetItemNum(5) != 0)
        {
            if (LevelTwoGameManager.Instance.GetItemEquipIndex() == 2)
            {
                LevelTwoGameManager.Instance.SetItemEquipIndex(0);
                shotFreq = normalStoneShotFreq;
            }
            else
            {
                LevelTwoGameManager.Instance.SetItemEquipIndex(2);
                shotFreq = boomerShotFreq;
            }
        }
    }


    void OnTwoCheckBtnPress(GameObject _btn, bool _down)
	{
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
				OnTwoCheckBtnClick();											//调用点击右下按钮函数
			}
		}
	}

	void OnTwoCheckBtnClick()													//按下第二关的查看按钮
	{
		if(m_levelTwoCheck)														//如果按下查看按钮
		{
			if(m_checkObjIndex==1)												//按下门按钮
				LevelTwoGameManager.Instance.SetTurnToCountry(2);				//回村
			else if(m_checkObjIndex==2 )     //按下书包按钮
			{
				m_openDiary = 1;												//开启日记本
				LevelTwoGameManager.Instance.CloseUsualBtn();					//关闭常用按钮

			}
		}
		else if(m_endItemCheck&&m_itemState==2)									//如果当前物品处于第三阶段
			m_itemState = 3;                                                    //物品进入销毁阶段
        else if (isCanShot)
        {
			int _bulletType = LevelTwoGameManager.Instance.GetItemEquipIndex();
			switch(_bulletType)												//判定当前发射的子弹类型
			{
			//case 0:																//发射的子弹为普通石头
			//	m_heroSoundObj.GetComponent<AudioSource> ().clip = m_heroClip[0];
			//	m_heroSoundObj.GetComponent<AudioSource>().Play ();
			//	GameObject cloneBullet = (GameObject)Instantiate(
			//		m_levelTwoHeroStone, m_levelTwoHeroBulletPos.position, transform.rotation);	//初始化子弹
			//	break;
			case 1:																//发射的子弹为普通石头
                    AudioManager.Instance.HeroAttackSoundPlay(Global.GetInstance().audioName_Stone);
                    GameObject cloneBullet1 = (GameObject)Instantiate(
					    m_levelTwoHeroStone, m_levelTwoHeroBulletPos.position, transform.rotation);	//初始化子弹
                        isCanShot = false;
                    break;
			case 2:                                                             //发射的子弹为回旋镖
                    if (!LevelTwoGameManager.Instance.GetBoomerangOut())
                    {
                        AudioManager.Instance.HeroAttackSoundPlay(Global.GetInstance().audioName_Boomerang);
                        LevelTwoGameManager.Instance.SetBoomerangOut(true);     //发射回旋镖
                    }   isCanShot = false;
                    break;
			case 3:                                                             //发射的子弹为狐狸子弹
                    AudioManager.Instance.HeroAttackSoundPlay(Global.GetInstance().audioName_Stone);
				GameObject cloneFrogBullet = (GameObject)Instantiate(
					m_levelTwoFrogStone, m_levelTwoHeroBulletPos.position, transform.rotation);	//初始化子弹
                        isCanShot = false;
                    break;
			case 4:                                                             //发射的子弹为眼镜光
                    if (!LevelTwoGameManager.Instance.GetGlassWaveEmit())
                    {
                        AudioManager.Instance.HeroAttackSoundPlay (Global.GetInstance().audioName_GlassWave);
                        LevelTwoGameManager.Instance.SetGlassWaveEmit(true);        //发射回旋镖
                    }   isCanShot = false;
                    break;
			}
		}
	}



	private bool isWatchedDiary = false;//是否查看了笔记

	void CheckDiary()
	{
		switch(m_diaryOpenState)
		{
		case 0:
			m_diaryPanel[0].SetActive(true);									//打开日记面板及第一页
			m_diaryPanel[2].SetActive(true);									//打开遮罩
			m_diaryPanel[0].GetComponent<UISprite>().spriteName = "diary_bkg1";	//打开第一页
			m_diaryCurrPage = 1;
			m_diaryOpenState = 1;
			break;
		case 1:
			if(Input.GetMouseButtonDown(0))		
			{
				Ray ray = UICamera.mainCamera.ScreenPointToRay (Input.mousePosition);//获取点击位置
				RaycastHit hit;
				if (Physics.Raycast(ray, out hit)) 									//检测鼠标点击区域
				{
					if(hit.collider.gameObject.tag == "MaskPic")						//按下后方遮罩
					{
						for(int i=0; i<m_diaryPanel.Length; i++)					//关闭所有面板
							m_diaryPanel[i].SetActive(false);			

						m_diaryOpenState = 0;
						LevelTwoGameManager.Instance.OpenUsualBtn();                //开启常用按钮
						if (isWatchedDiary && LevelTwoGameManager.Instance.GetItemNum(19) == 0)
                        {
                            LevelTwoGameManager.Instance.SetMessageType(1, "一本日记");	//获取一本日记消息
					        LevelTwoGameManager.Instance.SetItemNum(19, 1);					//背包中获得
							m_levelTwoCheckObjCollider[1].tag = "used";	
							m_openDiary = 2;											//日记可以关闭了
							m_levelTwoCheckObjCollider[1].GetComponent<SpriteRenderer>().sprite = m_bagSprite[1];	//书包变为打开状态
						}else{
							m_openDiary = 0;
							m_levelTwoCheckObjCollider[1].GetComponent<SpriteRenderer>().sprite = m_bagSprite[0];	//书包变为未打开状态

						}

					}
					else if(hit.collider.gameObject.tag == "DiaryPage")				//如果点击了日记页
					{
						if(m_diaryCurrPage==1)										//当前为第一页
						{
							m_diaryPanel[0].GetComponent<UISprite>().spriteName = "diary_bkg2";	//打开第二页
							m_diaryPanel[1].SetActive(true);						//打开第二页文字
							m_diaryCurrPage = 2;									//更改当前页码索引
							isWatchedDiary = true;
						}
						else if(m_diaryCurrPage==2)									//当前为第二页
						{
							m_diaryPanel[0].GetComponent<UISprite>().spriteName = "diary_bkg1";	//打开第一页
							m_diaryPanel[1].SetActive(false);						//隐藏第二页文字
							m_diaryCurrPage = 1;
						}
					}
				}
			}
			
			break;
		}
	}

	void EndItemCheck()															//过关物品状态检测
	{
		switch(m_itemState)
		{
		case 0:																	//第一阶段
			if(LevelTwoGameManager.Instance.GetEndItem()==1)					//如果通关 需生成通关物品
			{
				m_endGetItem.SetActive(true);									//开启过关物品
				m_itemState = 1;												//下一阶段
				iTween.MoveTo(m_endGetItem, iTween.Hash("position", new Vector3(21f,1.84f,-0.5f)			
				                                        ,"time", 2f
				                                        ,"easeType", iTween.EaseType.easeOutBounce));
				m_itemTimer = 2f;												//开启下落计时器
			}
			break;
		case 1:																	//第二阶段
			m_itemTimer -= Time.deltaTime; 
			if(m_itemTimer<0)
			{
				m_itemState = 2;												//下一阶段
				m_collider = m_endGetItem.gameObject.AddComponent<BoxCollider>();//为通关物品添加包围盒
			}
			break;
		case 2:																	//第四阶段
			Bounds rr1 = m_levelTwoHero.GetComponent<Collider2D>().bounds;					//主角的包围盒
			Rect r1 = new Rect(rr1.center.x - rr1.size.x / 2,
			                   rr1.center.y - rr1.size.y / 2,
			                   rr1.size.x, rr1.size.y);
			
			Bounds _endItem = m_collider.GetComponent<Collider>().bounds;						//通关物品的包围盒
			Rect __endItemRect = new Rect(_endItem.center.x - _endItem.size.x / 2, _endItem.center.y - _endItem.size.y / 2, _endItem.size.x, _endItem.size.y);
			if(r1.Overlaps(__endItemRect))										//碰到通关物品
				m_endItemCheck = true;
			else 																//未碰到
				m_endItemCheck = false;
			break;
		case 3:																	//第五阶段
			Destroy(m_endGetItem.gameObject);									//销毁通关物品
			LevelTwoGameManager.Instance.SetMessageType(1, "蓝色种子");		//提示消息
			LevelTwoGameManager.Instance.SetItemNum(17, 1);					//背包中获得
			LevelTwoGameManager.Instance.SetEndItem(2);						//通关物品已获得
			m_itemState = 5;
			break;
		}
	}

	void Update()
	{
        if (LevelTwoGameManager.Instance.GetBloodNum() <= 0) return;

        if (!LevelTwoGameManager.Instance.isPlatformType_Mobild && LevelTwoGameManager.Instance.isUsualBtnCanClick)
            if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.KeypadEnter))
            {
                OnTwoCheckBtnPress(gameObject, false);
            }
            else if (Input.GetKey(KeyCode.Space) || Input.GetKeyDown(KeyCode.KeypadEnter))
            {
                // OnTwoCheckBtnPress(gameObject, true);

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

        if (LevelTwoGameManager.Instance.GetEndItem()!=2)
			EndItemCheck();

		if(m_btnDownState==1)													//如果按下右下按钮
		{
			m_btnDownTimer += Time.deltaTime;									//开始计时
			if(m_btnDownTimer>=0.5f)											//如果按下超过0.5s
			{
				m_longPressPanel.SetActive(true);
				for(int i=0; i<m_longPressPanelBtn.Length; i++)
					m_longPressPanelBtn[i].transform.GetChild(0).gameObject.SetActive(false);
				if(LevelTwoGameManager.Instance.GetItemNum(5)!=0)
					m_longPressPanelBtn[2].transform.GetChild(0).gameObject.SetActive(true);
				if(LevelTwoGameManager.Instance.GetItemNum(6)!=0)
					m_longPressPanelBtn[0].transform.GetChild(0).gameObject.SetActive(true);
				if(LevelTwoGameManager.Instance.GetItemNum(7)!=0)
					m_longPressPanelBtn[1].transform.GetChild(0).gameObject.SetActive(true);
			}
		}

		if(m_openDiary==1)														//如果正在查看日记本
			CheckDiary();
		
		Bounds rr1 = m_levelTwoHero.GetComponent<Collider2D>().bounds;							//主角的包围盒
		Rect r1 = new Rect(rr1.center.x - rr1.size.x / 2,
		                   rr1.center.y - rr1.size.y / 2,
		                   rr1.size.x, rr1.size.y);

		for(int i=0; i<m_levelTwoCheckObjCollider.Length; i++)					//遍历所有可选物品包围盒
		{
			if(m_levelTwoCheckObjCollider[i].tag=="LevelTwoCheckObj")
			{
				Bounds rr2 = m_levelTwoCheckObjCollider[i].GetComponent<Collider>().bounds;
				Rect r2 = new Rect(rr2.center.x - rr2.size.x / 2, rr2.center.y - rr2.size.y / 2, rr2.size.x, rr2.size.y);
				if(r1.Overlaps(r2))													//主角碰到可选物品
				{
                    if (i == 0)                                                        //主角碰到门
                    {
                        m_levelTwoDoorIcon.SetActive(true);                         //显示门图标
                        m_levelTwoCheck = true;                                         //当前碰到可选物品
                        m_checkObjIndex = i + 1;
                    }
                    else
                    {
                        if (i == 1 && (LevelTwoGameManager.Instance.GetItemNum(19) == 0))                        //主角碰到书包
                        {
                            m_levelTwoCheckObjCollider[i].GetComponent<SpriteRenderer>().sprite = m_bagSprite[2];//书包变为选中图
                            m_levelTwoCheck = true;                                         //当前碰到可选物品
                            m_checkObjIndex = i + 1;
                        }
                        else
                        {
                            m_levelTwoCheck = false;                                         //当前未碰到可选物品
                            m_checkObjIndex = 0;
                        }
                    }
                    break;
				}
				else if(m_levelTwoCheck&&i==(m_levelTwoCheckObjCollider.Length-1))	//当前未碰到检测物品
				{
					m_checkObjIndex = 0;
					m_levelTwoDoorIcon.SetActive(false);							//隐藏门图标
					if(m_openDiary!=2)												//如果日记本未查看
						m_levelTwoCheckObjCollider[1].GetComponent<SpriteRenderer>().sprite = m_bagSprite[0];
					else 															//如果日记本已得到
						m_levelTwoCheckObjCollider[1].GetComponent<SpriteRenderer>().sprite = m_bagSprite[1];
					m_levelTwoCheck = false;										//检测变量归0
				}
			}
			else if(m_levelTwoCheck&&i==(m_levelTwoCheckObjCollider.Length-1))	//当前未碰到检测物品
			{
				m_checkObjIndex = 0;
				m_levelTwoDoorIcon.SetActive(false);							//隐藏门图标
				if(m_openDiary!=2)
					m_levelTwoCheckObjCollider[1].GetComponent<SpriteRenderer>().sprite = m_bagSprite[0];
				else
					m_levelTwoCheckObjCollider[1].GetComponent<SpriteRenderer>().sprite = m_bagSprite[1];
				m_levelTwoCheck = false;										//检测变量归0
			}
		}

	}


}
