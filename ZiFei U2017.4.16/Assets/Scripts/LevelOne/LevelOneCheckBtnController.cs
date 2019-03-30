using UnityEngine;
using System.Collections;

public class LevelOneCheckBtnController : MonoBehaviour 
{
	public GameObject m_checkBtn;												//查看按钮
	public GameObject m_levelOneHero;											//主角
	public GameObject m_doorIcon;												//门图标
	public Transform[] m_levelHeight;											//每一层级的高度
	public GameObject[] m_canCheckThingCollider;									//可得到的物品包围盒
	public GameObject m_levelOneHeroStone;										//主角发出的子弹
	public Transform m_levelOneHeroBulletPos;									//主角发射子弹的位置

	public GameObject m_frogStone;												//主角发出的狐狸石头
	public GameObject m_longPressPanel;											//长按出现的三个按钮
	public GameObject[] m_longPressPanelBtn;									//长按出现的面板的三个按钮

	public GameObject m_endGetItem;												//通关时获得的物品

	private bool m_checkState;													//玩家碰到物体决定当前状态（0无 1碰到可转金币物体）
	private int m_levelDisState = 0;											//下落层级检查阶段
	private float m_heroCheckStartPosY;											//检测初始主角的竖直位置
	private float m_oneLevelHeight;												//一层地图的高度
	private int m_canCheckThingIndex = 0;										//可获得物品的编号
	private bool m_boxOpenCheck = false;										//箱子打开阶段检测

	private float m_btnDownTimer = 0f;											//按钮按下计时器
	private int m_btnDownState = 0;			

	private int m_itemState = 0;												//通关物品状态
	private float m_itemTimer = 3f;												//通关物品计时器
	private BoxCollider m_collider;								
	private bool m_endItemCheck = false;										//是否检测到通关物品

    public float normalStoneShotFreq = 1f;
    public float frogStoneShotFreq = 1f;
    public float boomerShotFreq = 2f;
    public float glassWaveShotFreq = 2f;

    private bool isCanShot = false;
    private float shotFreq = 0f;
    private float shotTimer = 0f;

    void Awake()
	{
		UIEventListener.Get (m_checkBtn).onPress = OnOneCheckBtnPress;			//查看按钮侦听

		UIEventListener.Get (m_longPressPanelBtn[0]).onClick = OnFrogStoneBtnClick;	//单击狐狸面板按钮
		UIEventListener.Get (m_longPressPanelBtn[1]).onClick = OnGlassWaveBtnClick;	//单击眼镜按钮
		UIEventListener.Get (m_longPressPanelBtn[2]).onClick = OnBoomerangBtnClick;	//单击回旋镖按钮
	}

	void Start()
	{
		m_oneLevelHeight = m_levelHeight [1].position.y - m_levelHeight [0].position.y;
        shotFreq = normalStoneShotFreq;

    }

	void OnFrogStoneBtnClick(GameObject _frogBtn)								//按下狐狸石头按钮
	{
        AudioManager.Instance.SoundPlay(Global.GetInstance().audioName_BtnClick2);
		if(LevelOneGameManager.Instance.GetItemNum(6)!=0)
		{
            if (LevelOneGameManager.Instance.GetItemEquipIndex() == 3)
            {
                LevelOneGameManager.Instance.SetItemEquipIndex(0);
                shotFreq = normalStoneShotFreq;
            }
            else
            {
                LevelOneGameManager.Instance.SetItemEquipIndex(3);
                shotFreq = frogStoneShotFreq;

            }
        }
    }
	
	void OnGlassWaveBtnClick(GameObject _waveBtn)								//按下眼镜光按钮
	{
        AudioManager.Instance.SoundPlay(Global.GetInstance().audioName_BtnClick2);
		if(LevelOneGameManager.Instance.GetItemNum(7)!=0)
		{
            if (LevelOneGameManager.Instance.GetItemEquipIndex() == 4)
            {
                LevelOneGameManager.Instance.SetItemEquipIndex(0);
                shotFreq = normalStoneShotFreq;
            }
            else
            {
                LevelOneGameManager.Instance.SetItemEquipIndex(4);
                shotFreq = glassWaveShotFreq;
            }

        }
    }
	
	void OnBoomerangBtnClick(GameObject _boomerangBtn)							//按下回旋镖按钮
	{
        AudioManager.Instance.SoundPlay(Global.GetInstance().audioName_BtnClick2);
		if(LevelOneGameManager.Instance.GetItemNum(5)!=0)
		{
            if (LevelOneGameManager.Instance.GetItemEquipIndex() == 2)
            {
                LevelOneGameManager.Instance.SetItemEquipIndex(0);
                shotFreq = normalStoneShotFreq;
            }
            else
            {
                LevelOneGameManager.Instance.SetItemEquipIndex(2);
                shotFreq = boomerShotFreq;
            }
        }
	}

	
	void OnOneCheckBtnPress(GameObject _btn, bool _down)
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
				OnOneCheckBtnClick();											//调用点击右下按钮函数
			}
		}
	}

	void OnOneCheckBtnClick()
	{
		if(m_checkState)															//当前碰到可查看物品
		{
			switch (m_canCheckThingIndex)
			{
			case 1:																	//碰到门
				LevelOneGameManager.Instance.SetTurnToCountry(2);//通关
				break;
			case 2:																	//碰到箱子
				m_canCheckThingCollider[m_canCheckThingIndex-1].GetComponent<Animator>().SetBool("BoxOpen", true);
				m_boxOpenCheck = true;												//进入箱子打开检测阶段
				m_canCheckThingCollider[m_canCheckThingIndex-1].tag = "used";		//箱子被标注为已使用
				break;
			case 3:																	//碰到肉
			case 4:
				if (LevelOneGameManager.Instance.GetBloodNum() >= 1f) {//hero满血，将鸡腿放到backpack中
					LevelOneGameManager.Instance.SetMessageType(1, "  一个鸡腿");			//提示获得鸡腿
					LevelOneGameManager.Instance.SetItemNum(2,LevelOneGameManager.Instance.GetItemNum(2)+1);
				} else {
				
					LevelOneGameManager.Instance.SetMessageType(1, "  20点体力");			//提示获得2滴血
					LevelOneGameManager.Instance.SetHeroBloodReduce(-0.2f);				//主角血量增加
				}
				m_canCheckThingCollider[m_canCheckThingIndex-1].tag = "used";		//取消碰撞检测
				m_canCheckThingCollider[m_canCheckThingIndex-1].SetActive(false);	//隐藏
				break;
			}
			m_checkState = false;
		}
		else if(m_endItemCheck&&m_itemState==2)									//如果当前物品处于第三阶段
			m_itemState = 3;													//物品进入销毁阶段
		else  if (isCanShot)
            {
			    int _bulletType = LevelOneGameManager.Instance.GetItemEquipIndex();
            switch (_bulletType)                                                //判定当前发射的子弹类型
            {
                //case 0:																//发射的子弹为普通石头//不带装备
                //	GameObject cloneBullet =Instantiate(
                //		m_levelOneHeroStone, m_levelOneHeroBulletPos.position, transform.rotation);	//初始化子弹
                //	break;
                case 1:                                                             //发射的子弹为普通石头//戴胡子
                    AudioManager.Instance.HeroAttackSoundPlay(Global.GetInstance().audioName_Stone);
                    GameObject cloneBullet1 = Instantiate(
                        m_levelOneHeroStone, m_levelOneHeroBulletPos.position, transform.rotation); //初始化子弹
                    isCanShot = false;
                    break;


                case 2:																//发射的子弹为回旋镖
                    if (!LevelOneGameManager.Instance.GetBoomerangOut())
                    {
                        LevelOneGameManager.Instance.SetBoomerangOut(true);     //发射回旋镖
                        AudioManager.Instance.HeroAttackSoundPlay(Global.GetInstance().audioName_Boomerang);
                    }
                    isCanShot = false;
                    break;

                case 3:																//发射的子弹为狐狸子弹
                    AudioManager.Instance.HeroAttackSoundPlay(Global.GetInstance().audioName_Stone);
                    GameObject cloneFrogBullet = Instantiate(
                        m_frogStone, m_levelOneHeroBulletPos.position, transform.rotation); //初始化子弹
                    isCanShot = false;
                    break;

                case 4:                                                             //发射的子弹为眼镜光
                    if (!LevelOneGameManager.Instance.GetGlassWaveEmit())
                    {
                        AudioManager.Instance.HeroAttackSoundPlay(Global.GetInstance().audioName_GlassWave);
                        LevelOneGameManager.Instance.SetGlassWaveEmit(true);        //发激光
                    }
                        isCanShot = false;
                        break;
            }
		}
	}

	void EndItemCheck()															//过关物品状态检测
	{
		switch(m_itemState)
		{
		case 0:																	//第一阶段
			if(LevelOneGameManager.Instance.GetEndItem()==1)					//如果通关 需生成通关物品
			{
				Vector3 _itemPos = LevelOneGameManager.Instance.GetMonkeyPos();
				m_endGetItem.transform.position = _itemPos;
				Vector3 _targetPos = Vector3.zero;
				if(_itemPos.y>=42f)
					_itemPos.y=41.97f;
				else if(_itemPos.y<=9.7f)
					_itemPos.y=-0.3f;
				else
				{
					if(_itemPos.x>=1.67f)
					{
						if(_itemPos.y<=19.5f)
							_itemPos.y = 9.45f;
						else
							_itemPos.y = 19.21f;
					}
					else if(_itemPos.x<=-1.67f)
					{
						if(_itemPos.y<=19.5f)
							_itemPos.y = -0.3f;
						else if(_itemPos.y>=22.75f)
							_itemPos.y = 22.45f;
						else
							_itemPos.y = 19.21f;
					}
					else
					{
						if(_itemPos.y<=22.75f)
							_itemPos.y = 9.45f;
						else
							_itemPos.y = 22.45f;
					}
				}
				m_endGetItem.SetActive(true);									//开启过关物品
				m_itemState = 1;												//下一阶段
				iTween.MoveTo(m_endGetItem, iTween.Hash("position", _itemPos			
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
			Bounds rr1 = m_levelOneHero.GetComponent<Collider2D>().bounds;						//主角的包围盒
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
			LevelOneGameManager.Instance.SetMessageType(1, "绿色种子");			//提示消息
			LevelOneGameManager.Instance.SetItemNum(16, 1);						//背包中获得
			LevelOneGameManager.Instance.SetEndItem(2, Vector3.zero);			//通关物品已获得
			m_itemState = 5;
			break;
		}
	}

	void Update()
	{
        if (LevelOneGameManager.Instance.GetBloodNum() <= 0) return;

        if (!LevelOneGameManager.Instance.isPlatformType_Mobild &&  LevelOneGameManager.Instance.isUsualBtnCanClick)

            if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.KeypadEnter))
            {
                OnOneCheckBtnPress(gameObject, false);
            }
            else if (Input.GetKey(KeyCode.Space) || Input.GetKey(KeyCode.KeypadEnter))
            {
               // OnOneCheckBtnPress(gameObject, true);

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

        if (LevelOneGameManager.Instance.GetEndItem()!=2)
			EndItemCheck ();

		if(m_btnDownState==1)													//如果按下右下按钮
		{
			m_btnDownTimer += Time.deltaTime;									//开始计时
			if(m_btnDownTimer>=0.5f)											//如果按下超过0.5s
			{
				m_longPressPanelBtn[2].transform.GetChild(0).gameObject.SetActive(LevelOneGameManager.Instance.GetItemNum(5) != 0);
				m_longPressPanelBtn[0].transform.GetChild(0).gameObject.SetActive(LevelOneGameManager.Instance.GetItemNum(6) != 0);
				m_longPressPanelBtn[1].transform.GetChild(0).gameObject.SetActive(LevelOneGameManager.Instance.GetItemNum(7) != 0);
				m_longPressPanel.SetActive(true);
			}
		}

		if(m_boxOpenCheck)
		{
			if(m_canCheckThingCollider[1].GetComponent<SpriteRenderer>().sprite.name=="ani_chest4")
			{
				m_boxOpenCheck = false;												//关闭箱子检测
                int addMoneyCount = Random.Range(10,26); 
				LevelOneGameManager.Instance.SetCurrAddMoney(addMoneyCount);					//增加金币数量
				LevelOneGameManager.Instance.SetMessageType(2, addMoneyCount.ToString()+"金币");			//提示获得15金币

				if (LevelOneGameManager.Instance.GetAchieveGot(17) == 0)
				{
					AudioManager.Instance.TipsSoundPlay(Global.GetInstance().audioName_AchieveGet);
					LevelOneGameManager.Instance.SetAchieveGot(17, 1);
					LevelOneGameManager.Instance.SetMessageType(3, "您获得了成就【顺手牵羊】");
				}

			}
		}
		ItemColliderCheck ();
		HeroDownDisCheck();
	}





	void ItemColliderCheck()														//是否碰到物品包围盒检测
	{
		Bounds rr1 = m_levelOneHero.GetComponent<Collider2D>().bounds;								//主角的包围盒
		Rect r1 = new Rect(rr1.center.x - rr1.size.x / 2,
		                   rr1.center.y - rr1.size.y / 2,
		                   rr1.size.x, rr1.size.y);
		
		for(int i=0; i<m_canCheckThingCollider.Length; i++)							//遍历所有可调查物品的包围盒
		{
			if(m_canCheckThingCollider[i].tag=="LevelOneCheckThing")				//如果该物品当前还处于激活状态			
			{
				Bounds rr2 = m_canCheckThingCollider[i].GetComponent<Collider>().bounds;			//获取该物品的包围盒
				Rect r2 = new Rect(rr2.center.x - rr2.size.x / 2, rr2.center.y - rr2.size.y / 2, rr2.size.x, rr2.size.y);
				if(r1.Overlaps(r2))													//碰到物体	
				{
					if(i==0)														//碰到的是门
						m_doorIcon.SetActive(true);									//开启门的图标
					m_checkState = true; 												//进入检测阶段
					m_canCheckThingIndex = i+1;
					break;
				}
				else if((i==m_canCheckThingCollider.Length-1)&&m_checkState)
				{
					m_checkState = false;
					m_doorIcon.SetActive(false);		
				}
			}
			else if((i==m_canCheckThingCollider.Length-1)&&m_checkState)
			{
				m_checkState = false;
				m_doorIcon.SetActive(false);		
			}
		}
	}

	void HeroDownDisCheck()														//主角下落的层级检测
	{
		switch(m_levelDisState)
		{
		case 0:
			if(m_levelOneHero.GetComponent<Rigidbody2D>().velocity.y<-2f)				//如果正在下落
			{
				m_levelDisState = 1;													//进入检测阶段
				m_heroCheckStartPosY = m_levelOneHero.transform.position.y;				//获取此时主角的竖直位置
			}
			break;
		case 1:
			if(m_levelOneHero.GetComponent<Rigidbody2D>().velocity.y>-2f)				//如果当前下落停止
			{
				float _heroCheckDis = m_heroCheckStartPosY - m_levelOneHero.transform.position.y;	//获取停止检测时主角的竖直位移差
				if(_heroCheckDis > m_oneLevelHeight*2f)
					LevelOneGameManager.Instance.SetHeroBloodReduce(0.1f);				//生命减少
				m_levelDisState = 0;													//竖直唯一状态检测归零
			}
			break;
		}
	}

}
