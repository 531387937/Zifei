using UnityEngine;
using System.Collections;

public class LevelThreeCheckBtnController : MonoBehaviour 
{
	public GameObject m_longPressPanel;											//长按出现的三个按钮
	public GameObject[] m_levelThreeCollider;									//物品碰撞盒（门 石碑）
	public GameObject[] m_levelThreeSelectBox;									//物品选中盒（门 石碑）

	public GameObject m_levelThreeHero;											//第二关主角
	public GameObject m_levelThreeCheckBtn;										//第二关查看按钮

	public Transform m_levelThreeHeroBulletPos;									//主角发射武器的位置
	public GameObject m_stone;													//普通石头
	public GameObject m_frogStone;												//狐狸石头

	public GameObject[] m_longPressPanelBtn;									//长按出现的面板的三个按钮

	public GameObject m_endGetItem;												//通关时获得的物品


	private int m_threeCheckIndex = 0;											//检测物序号
	private float m_btnDownTimer = 0f;											//按钮按下计时器
	private int m_btnDownState = 0;										

	private int m_itemState = 0;												//通关物品状态
	private float m_itemTimer = 3f;												//通关物品计时器
	private BoxCollider m_collider;								
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
		UIEventListener.Get (m_levelThreeCheckBtn).onPress = OnThreeCheckBtnPress;//一直按着查看按钮
		UIEventListener.Get (m_longPressPanelBtn[0]).onClick = OnFrogStoneBtnClick;	//单击狐狸面板按钮
		UIEventListener.Get (m_longPressPanelBtn[1]).onClick = OnGlassWaveBtnClick;	//单击眼镜按钮
		UIEventListener.Get (m_longPressPanelBtn[2]).onClick = OnBoomerangBtnClick;	//单击回旋镖按钮

        shotFreq = normalStoneShotFreq;

    }


    void OnFrogStoneBtnClick(GameObject _frogBtn)                               //按下狐狸石头按钮
    {
        AudioManager.Instance.SoundPlay(Global.GetInstance().audioName_BtnClick2);
        if (LevelThreeGameManager.Instance.GetItemNum(6) != 0)
        {
            if (LevelThreeGameManager.Instance.GetItemEquipIndex() == 3)
            {
                LevelThreeGameManager.Instance.SetItemEquipIndex(0);
                shotFreq = normalStoneShotFreq;
            }
            else
            {
                LevelThreeGameManager.Instance.SetItemEquipIndex(3);
                shotFreq = frogStoneShotFreq;

            }
        }
    }

    void OnGlassWaveBtnClick(GameObject _waveBtn)                               //按下眼镜光按钮
    {
        AudioManager.Instance.SoundPlay(Global.GetInstance().audioName_BtnClick2);
        if (LevelThreeGameManager.Instance.GetItemNum(7) != 0)
        {
            if (LevelThreeGameManager.Instance.GetItemEquipIndex() == 4)
            {
                LevelThreeGameManager.Instance.SetItemEquipIndex(0);
                shotFreq = normalStoneShotFreq;
            }
            else
            {
                LevelThreeGameManager.Instance.SetItemEquipIndex(4);
                shotFreq = glassWaveShotFreq;
            }

        }
    }

    void OnBoomerangBtnClick(GameObject _boomerangBtn)                          //按下回旋镖按钮
    {
        AudioManager.Instance.SoundPlay(Global.GetInstance().audioName_BtnClick2);

        if (LevelThreeGameManager.Instance.GetItemNum(5) != 0)
        {
            if (LevelThreeGameManager.Instance.GetItemEquipIndex() == 2)
            {
                LevelThreeGameManager.Instance.SetItemEquipIndex(0);
                shotFreq = normalStoneShotFreq;
            }
            else
            {
                LevelThreeGameManager.Instance.SetItemEquipIndex(2);
                shotFreq = boomerShotFreq;
            }
        }
    }





    void OnThreeCheckBtnPress(GameObject _btn, bool _down)
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
				OnThreeCheckBtnClick();											//调用点击右下按钮函数
			}
		}
	}

	
	void OnThreeCheckBtnClick()
	{
		if(m_threeCheckIndex==1)
			LevelThreeGameManager.Instance.SetTurnToCountry(2);
		else if(m_threeCheckIndex==2)
			LevelThreeGameManager.Instance.SetMessageType(3, "向着光指引的方向");
		else if(m_endItemCheck&&m_itemState==2)									//如果当前物品处于第三阶段
			m_itemState = 3;                                                    //物品进入销毁阶段
        else if (isCanShot)
        {
			int _bulletType = LevelThreeGameManager.Instance.GetItemEquipIndex();
			switch(_bulletType)												//判定当前发射的子弹类型
			{
			//case 0:																//发射的子弹为普通石头
			//	m_heroSoundObj.GetComponent<AudioSource> ().clip = m_heroClip[0];
			//	m_heroSoundObj.GetComponent<AudioSource>().Play ();
			//	GameObject cloneBullet = (GameObject)Instantiate(
			//		m_stone, m_levelThreeHeroBulletPos.position, transform.rotation);	//初始化子弹
			//	break;

			case 1:																//发射的子弹为普通石头
                    AudioManager.Instance.HeroAttackSoundPlay(Global.GetInstance().audioName_Stone);
                    GameObject cloneBullet1 = Instantiate(
					m_stone, m_levelThreeHeroBulletPos.position, transform.rotation);	//初始化子弹
                        isCanShot = false;
                    break;
			case 2:                                                             //发射的子弹为回旋镖
                    if (!LevelThreeGameManager.Instance.GetBoomerangOut())
                    {
                        AudioManager.Instance.HeroAttackSoundPlay(Global.GetInstance().audioName_Boomerang);
                        LevelThreeGameManager.Instance.SetBoomerangOut(true);		//发射回旋镖
                    }    isCanShot = false;
                    break;
			case 3:                                                             //发射的子弹为狐狸子弹
                    AudioManager.Instance.HeroAttackSoundPlay(Global.GetInstance().audioName_Stone);
				GameObject cloneFrogBullet = Instantiate(
					m_frogStone, m_levelThreeHeroBulletPos.position, transform.rotation);	//初始化子弹
                        isCanShot = false;
                    break;
			case 4:                                                             //发射的子弹为眼镜光
                    if (!LevelThreeGameManager.Instance.GetGlassWaveEmit())
                    {
                        AudioManager.Instance.HeroAttackSoundPlay(Global.GetInstance().audioName_GlassWave);
                        LevelThreeGameManager.Instance.SetGlassWaveEmit(true);      //发射回旋镖
                    }   isCanShot = false;
                    break;
			}
		}
	}

	void EndItemCheck()															//过关物品状态检测
	{
		switch(m_itemState)
		{
		case 0:																	//第一阶段
			if(LevelThreeGameManager.Instance.GetEndItem()==1)					//如果通关 需生成通关物品
			{
				m_endGetItem.SetActive(true);									//开启过关物品
				m_itemState = 1;												//下一阶段
				iTween.MoveTo(m_endGetItem, iTween.Hash("position", new Vector3(18.1f,1.83f,-0.5f)			
				                                           ,"time", 2f
				                                           ,"easeType", iTween.EaseType.easeOutBounce));
				m_itemTimer = 2f;
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
			Bounds rr1 = m_levelThreeHero.GetComponent<Collider2D>().bounds;					//主角的包围盒
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
			LevelThreeGameManager.Instance.SetMessageType(1, "红色种子");		//提示消息
			LevelThreeGameManager.Instance.SetItemNum(18, 1);					//背包中获得
			LevelThreeGameManager.Instance.SetEndItem(2);						//通关物品已获得
			m_itemState = 5;
			break;
		}
	}
	
	void Update()
	{
        if (LevelThreeGameManager.Instance.GetBloodNum() <= 0) return;

        if (!LevelThreeGameManager.Instance.isPlatformType_Mobild && LevelThreeGameManager.Instance.isUsualBtnCanClick)
            if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.KeypadEnter))
            {
                OnThreeCheckBtnPress(gameObject, false);
            }
            else if (Input.GetKey(KeyCode.Space) || Input.GetKeyDown(KeyCode.KeypadEnter))
            {
                // OnThreeCheckBtnPress(gameObject, true);

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

        if (LevelThreeGameManager.Instance.GetEndItem()!=2)
			EndItemCheck ();

		if(m_btnDownState==1)
		{
			m_btnDownTimer += Time.deltaTime;
			if(m_btnDownTimer>=0.5f)
			{
				m_longPressPanel.SetActive(true);
				for(int i=0; i<m_longPressPanelBtn.Length; i++)
					m_longPressPanelBtn[i].transform.GetChild(0).gameObject.SetActive(false);
				if(LevelThreeGameManager.Instance.GetItemNum(5)!=0)
					m_longPressPanelBtn[2].transform.GetChild(0).gameObject.SetActive(true);
				if(LevelThreeGameManager.Instance.GetItemNum(6)!=0)
					m_longPressPanelBtn[0].transform.GetChild(0).gameObject.SetActive(true);
				if(LevelThreeGameManager.Instance.GetItemNum(7)!=0)
					m_longPressPanelBtn[1].transform.GetChild(0).gameObject.SetActive(true);
			}
		}

		Bounds rr1 = m_levelThreeHero.GetComponent<Collider2D>().bounds;						//主角的包围盒
		Rect r1 = new Rect(rr1.center.x - rr1.size.x / 2,
		                   rr1.center.y - rr1.size.y / 2,
		                   rr1.size.x, rr1.size.y);

		for(int i=0; i<m_levelThreeCollider.Length; i++)
		{
			Bounds _collider = m_levelThreeCollider[i].GetComponent<Collider>().bounds;
			Rect _colliderRect = new Rect(_collider.center.x - _collider.size.x / 2, _collider.center.y - _collider.size.y / 2, _collider.size.x, _collider.size.y);
			if(r1.Overlaps(_colliderRect))	
			{
				m_levelThreeSelectBox[i].SetActive(true);
				m_threeCheckIndex = i+1;
				break;
			}

			if(i==(m_levelThreeCollider.Length-1)&&m_threeCheckIndex!=0)
			{
				m_threeCheckIndex = 0;
				m_levelThreeSelectBox[0].SetActive(false);
				m_levelThreeSelectBox[1].SetActive(false);
			}
		}
	}
}
