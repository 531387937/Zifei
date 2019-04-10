using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class CheckBtnController : MonoBehaviour
{
    public GameObject ScneUIPanel;

    public static CheckBtnController Instance;
    public GameObject m_generalBtn;                                             //查看按钮
    public Transform[] m_diaPos;                                                //对话框位置
    public GameObject m_checkBtnPlayerObject;                                   //主角
    public GameObject m_dialogTipArrow;                                         //提示箭头
    public Transform[] m_NPCarrowPos;                                           //NPC的提示箭头的位置

    public GameObject[] m_homeSelectCollider;                                   //主角家的白色选中框事件包围盒
    public GameObject[] m_homeSelectBox;                                        //主角家中的物体选中框
    public GameObject[] m_headHomeSelectCollider;                               //村长家的白色选中框事件包围盒
    public GameObject[] m_headHomeSelectBox;                                    //村长家中的物体选中框
    public GameObject[] m_barSelectCollider;                                    //酒馆的白色选中框事件包围盒
    public GameObject[] m_barSelectBox;                                         //酒馆中的物体选中框
    public GameObject[] m_countryDiaSelectCollider;                             //村庄中需要对话的物体选中框
    public GameObject[] m_countryDiaSelectBox;                                  //村庄中需要对话的物体包围盒
    public Transform[] m_thingDiaPos;                                           //村庄中需要对话的物体对话框位置
    public GameObject[] m_countryDoorSelectCollider;                            //村庄中的室内门选中框事件包围盒
    public GameObject[] m_countryDoorSelectBox;                                 //村庄中的室内门选中框
    public GameObject[] m_countryNPCDiaCollider;                                //村庄中需要对话的NPC的包围盒

    public GameObject[] m_countryInDoor;                                        //村庄内部的门（0主角家门 1村长家门 2酒馆家门）
    public GameObject m_heroDoorIcon;                                           //主角头顶的门图标
    public GameObject m_slotPanel;                                              //开启老虎机


    public HeadHomeGame headHomeGame;

    public UIWidget m_doorMaskObj;                                              //开关门黑色遮罩


    private int m_dialogBtnCheck = 0;                                           //对话按钮是否按下变量(0未按下 1按下弹出对话 2按下弹出任务框)
    private int m_dialogIndex = -1;                                             //对话事件索引
    private bool m_dialogState = false;                                         //对话按钮状态
    private bool m_selectBoxState = false;                                      //白色选中框状态
	[HideInInspector]
	public bool m_thingDialogState = false;                                    //村庄中物品是否碰到状态
    private int m_thingDiaBtnCheck = 0;                                         //村庄中物品对话按钮是否按下变量
	[HideInInspector]
	public int m_thingDialogIndex = -1;                                        //村庄中物品对话事件索引
    private bool m_barThingState = false;                                       //酒馆中物品是否碰到状态
    private int m_homeThingIndex = 0;                                           //在村庄中碰到的物品索引
    private int m_headHomeThingIndex = 0;                                       //村长家事件编号
    private int m_selectDoor = 0;                                               //玩家处于的门的位置（0无 1主角家 2村长家 3酒馆）

    private int m_doorMaskState = 0;                                            //开关门遮罩状态（0未开始 1需要开关门 2场景切换 3完成）

    private bool m_npcDirL = false;                                             //NPC要面向左
    private Animator m_inDoorAni;                                               //村庄内部门动画
    private int m_waterBoxRepairState = 0;										//水阀修理阶段

    private float hideUITimer = -1f;
    private bool isHideUI = false;

    void Awake()
    {
        Instance = this;                                                        //单例查看事件
        UIEventListener.Get(m_generalBtn).onClick = OnCheckBtnClick;           //单击查看按钮

        //ScneUIPanel.SetActive(true);

    }


    void OnCheckBtnClick(GameObject _checkBtn)                                  //查询按钮侦听事件
    {
        if (m_dialogState)                                                      //与NPC对话状态按下
        {
            if (m_dialogBtnCheck == 0)                                              //当前非对话状态
                m_dialogBtnCheck = -1;                                          //要弹对话框 先去NPC里指定朝向
            else if (m_dialogBtnCheck == 2)                                     //如果当前需要弹出任务框
                m_dialogBtnCheck = 3;                                           //弹出任务框
            else if (m_dialogBtnCheck == 4)
            {
                if (m_dialogIndex == 2)                                         //如果遇到的是铁匠
                    m_dialogBtnCheck = 5;                                       //需要弹出铁匠铺
                else if (m_dialogIndex == 4)                                        //如果遇到醉鬼
                {
                    if (GameManager.Instance.GetItemNum(1) != 0 && GameManager.Instance.GetWaterBoxRepairState() == 0)
                    {
                        GameManager.Instance.SetChooseState(1);                 //需弹出选项框
                        m_dialogBtnCheck = 0;
                    }
                    else
                        m_dialogBtnCheck = 0;                                   //关闭对话
                }
                else if (m_dialogIndex == 6)                                        //如果遇到酒吧内部老板
                    m_dialogBtnCheck = 6;                                       //需要弹出酒吧买卖界面
                else                                                            //遇到的非铁匠
                    m_dialogBtnCheck = 0;                                       //关闭对话框
            }
        }

        if (m_thingDialogState)                                                 //村庄中物品对话状态按下
        {
            if (m_thingDialogIndex == 11)                                           //按下水阀开关
            {
                AudioManager.Instance.SoundPlay(Global.GetInstance().audioName_Country_Valve);

                m_countryDiaSelectBox[11].SetActive(false);                     //隐藏开关选框
                GameManager.Instance.SetWaterBoxRepairState(2);                 //水阀修理阶段更改
                GameManager.Instance.SetKeyState(0, 1);                         //当前可以获得绿钥匙
            }

            else if (m_thingDialogIndex == 4)                                       //按下灯塔按钮
            {
                if (GameManager.Instance.GetLightHouseState() == 5)             //可以开按钮
                {
                    AudioManager.Instance.ThingSoundPlay(Global.GetInstance().audioName_HouseLightLaser);
                    GameManager.Instance.SetLightHouseState(6);                 //灯塔按钮按下
                    m_thingDiaBtnCheck = 0;
                }
                else                                                            //当前不能按 出对话
                {
                    if (m_thingDiaBtnCheck == 0)
                    {
                        m_thingDiaBtnCheck = 1;
                        AudioManager.Instance.SoundPlay(Global.GetInstance().audioName_Country_Sad);
                    }
                    else
                    {
                        m_thingDiaBtnCheck = 0;
                    }
                }
            }

            else if (m_thingDialogIndex == 2)                                       //按下土丘1
            {
                if (GameManager.Instance.GetSeedState(0) == 1)
                {
                    GameManager.Instance.SetChooseState(2);                     //需弹出选项框
                    m_thingDiaBtnCheck = 0;
                }
                else
                {

                    if (m_thingDiaBtnCheck == 0)
                    {
                        m_thingDiaBtnCheck = 1;
                        AudioManager.Instance.SoundPlay(Global.GetInstance().audioName_Country_Sad);
                    }
                    else
                    {
                        m_thingDiaBtnCheck = 0;
                    }
                }
            }
            
            else if (m_thingDialogIndex == 17)                                      //按下土丘2
            {
                if (GameManager.Instance.GetSeedState(1) == 1)
                {
                    GameManager.Instance.SetChooseState(3);                     //需弹出选项框
                    m_thingDiaBtnCheck = 0;
                }
                else
                {
                    if (m_thingDiaBtnCheck == 0)
                    {
                        m_thingDiaBtnCheck = 1;
                        AudioManager.Instance.SoundPlay(Global.GetInstance().audioName_Country_Sad);
                    }
                    else
                    {
                        m_thingDiaBtnCheck = 0;
                    }
                }
            }
            else if (m_thingDialogIndex == 18)                                      //按下土丘3
            {
                if (GameManager.Instance.GetSeedState(2) == 1)
                {
                    GameManager.Instance.SetChooseState(4);                     //需弹出选项框
                    m_thingDiaBtnCheck = 0;
                }
                else
                {
                    if (m_thingDiaBtnCheck == 0)
                    {
                        m_thingDiaBtnCheck = 1;
                        AudioManager.Instance.SoundPlay(Global.GetInstance().audioName_Country_Sad);
                    }
                    else
                    {
                        m_thingDiaBtnCheck = 0;
                    }
                }
            }
            else if (m_thingDialogIndex == 8)                                       //按下第一个门绿门
            {

                if (GameManager.Instance.GetItemNum(8) != 0)                        //如果主角当前拥有绿钥匙							
                {
                    AudioManager.Instance.SoundPlay(Global.GetInstance().audioName_Country_LevelDoorOpen);

                    GameManager.Instance.SetLevelIndex(1);                      //切换至第一关
                    HideUIRoot();
                }
                else                                                            //如果主角当前未拥有绿钥匙

                {
                    if (m_thingDiaBtnCheck == 0)
                    {
                        m_thingDiaBtnCheck = 1;
                        AudioManager.Instance.SoundPlay(Global.GetInstance().audioName_Country_Sad);
                    }
                    else
                    {
                        m_thingDiaBtnCheck = 0;
                    }
                }

            }
            else if (m_thingDialogIndex == 6)                                       //按下第二个门
            {
                if ((GameManager.Instance.GetItemNum(8) != 0)
                     && (GameManager.Instance.GetItemNum(9) != 0))                        //如果主角当前拥有蓝钥匙							
                {
                    AudioManager.Instance.SoundPlay(Global.GetInstance().audioName_Country_LevelDoorOpen);

                    GameManager.Instance.SetLevelIndex(2);                      //切换至第二关
                    HideUIRoot();

                }
                else                                                            //如果主角当前未拥有蓝钥匙
                {
                    if (m_thingDiaBtnCheck == 0)
                    {
                        m_thingDiaBtnCheck = 1;
                        AudioManager.Instance.SoundPlay(Global.GetInstance().audioName_Country_Sad);
                    }
                    else
                    {
                        m_thingDiaBtnCheck = 0;
                    }
                }

            }
            else if (m_thingDialogIndex == 7)                                       //按下第三个门红门
            {
                if ((GameManager.Instance.GetItemNum(8) != 0)
                     && (GameManager.Instance.GetItemNum(9) != 0)
                    && (GameManager.Instance.GetItemNum(10) != 0))                      //如果主角当前拥有红钥匙							
                {
                    AudioManager.Instance.SoundPlay(Global.GetInstance().audioName_Country_LevelDoorOpen);
                    GameManager.Instance.SetLevelIndex(3);                      //切换至第三关
                    HideUIRoot();

                }
                else                                                            //如果主角当前未拥有红钥匙
                {
                    if (m_thingDiaBtnCheck == 0)
                    {
                        m_thingDiaBtnCheck = 1;
                        AudioManager.Instance.SoundPlay(Global.GetInstance().audioName_Country_Sad);
                    }
                    else
                    {
                        m_thingDiaBtnCheck = 0;
                    }
                }

            }

			else if (m_thingDialogIndex == 19)                                       //按下country head的门
			{
				if (GameManager.Instance.GetItemNum (11) != 0) {                       //如果主角当前拥有金钥匙		
					m_thingDiaBtnCheck = 0;
				
				} else {
					if (m_thingDiaBtnCheck == 0) {
						m_thingDiaBtnCheck = 1;
						AudioManager.Instance.SoundPlay (Global.GetInstance ().audioName_Country_Sad);
					} else {
						m_thingDiaBtnCheck = 0;
					}

				}
			}
			else if (m_thingDialogIndex >= 12 && m_thingDialogIndex < 15)               //碰到展板
                GameManager.Instance.SetPanelState(m_thingDialogIndex - 11, true);      //开启相应展板
            
			else if (m_thingDialogIndex == 15)                                          //碰到信箱
                GameManager.Instance.SetReadLetterState(1);                             //当前可以读信
            
			else if (m_thingDialogIndex == 16)                                          //如果碰到罐子
            {
                if (GameManager.Instance.GetJarState() == 0)                            //如果当前罐子我未被调查
                {

                    AudioManager.Instance.SoundPlay(Global.GetInstance().audioName_Country_Sad);
                    GameManager.Instance.SetJarState(1);                                //当前罐子已调查
                    m_thingDiaBtnCheck = 1;                                             //开启对话框
                }
                else                                                                    //如果罐子已调查
                {
                    m_thingDiaBtnCheck = 0;                                             //按下按钮没有反应
                    GameManager.Instance.SetJarState(0);
                }                                           
            }
            else
            {
                if (m_thingDiaBtnCheck == 0)
                {
                    m_thingDiaBtnCheck = 1;
                    AudioManager.Instance.SoundPlay(Global.GetInstance().audioName_Country_Sad);
                }
                else if (m_thingDiaBtnCheck == 1)
                {
                    //if (GameManager.Instance.GetTaskIndexState(7) == 1)
                    //{
                        
                    //    m_thingDiaBtnCheck = 1;
                    //}
                    if (m_thingDialogIndex == 0 && GameManager.Instance.GetStoveState() != 2 && GameManager.Instance.GetItemNum(3) != 0)
                    {
                        m_thingDiaBtnCheck = 2;
                    }
                    else
                        m_thingDiaBtnCheck = 0;
                }
            }
        }

        if (m_selectBoxState)                                                       //查看状态按下
            {
                switch (GameManager.Instance.GetCurrentScene())
                {
                    case 0:                                                         //当前处于村庄
                        if (m_selectDoor != 0)                                          //需要切换到室内
                        {
                            if (m_selectDoor == 3)
                            {
                                AudioManager.Instance.SoundPlay(Global.GetInstance().audioName_Country_BarDoor);
                               
								if (GameManager.Instance.GetAchieveGot(13) == 0)
                                {
                                AudioManager.Instance.TipsSoundPlay(Global.GetInstance().audioName_AchieveGet);
                                    GameManager.Instance.SetAchieveGot(13, 1);
                                    GameManager.Instance.SetMessageType(3, "您获得了成就【瞒天过海】");
                                }
                                
                            }

                            if (m_selectDoor == 2) //打开的是村长家的门
                            {
                                AudioManager.Instance.SoundPlay(Global.GetInstance().audioName_Country_CountryHeadDoor);
                                headHomeGame.ResetCabinetState();//重置密室 门的状态
                            }

                            if (m_selectDoor == 1) //打开的是hero家的门
                            {
                                AudioManager.Instance.SoundPlay(Global.GetInstance().audioName_Country_HeroHomeDoorr);
                            }

                            m_doorMaskState = 1;                                    //开始开关门遮罩
                            GameManager.Instance.CloseUsualBtn();                   //关闭常用按钮
                            m_doorMaskObj.gameObject.SetActive(true);               //打开开关门遮罩
                            m_inDoorAni = m_countryInDoor[m_selectDoor - 1].GetComponent<Animator>();
                            m_inDoorAni.SetBool("OpenDoor", true);

                        }
                        break;

                        case 1:                                                         //当前处于主角家
                            if (m_selectDoor == 0)                                          //村庄
                            {
                            AudioManager.Instance.SoundPlay(Global.GetInstance().audioName_Country_HeroHomeDoorr);

                            m_doorMaskState = 1;                                    //开始开关门遮罩
                                GameManager.Instance.CloseUsualBtn();                   //关闭常用按钮
                                m_doorMaskObj.gameObject.SetActive(true);               //打开开关门遮罩
                                //GameManager.Instance.SetCurrentScene(0);

                        }
                        else
                            {

                            switch (m_homeThingIndex)                               //判定当前在房间中选中的物品索引
                                {
                                    case 1:
                                        if (GameManager.Instance.GetKeyState(0) == 1 
                                        && GameManager.Instance.GetTaskIndexState(4) >= 3)
                                        {
                                                GameManager.Instance.SetMessageType(1, "绿色钥匙"); //获取绿色钥匙
                                                GameManager.Instance.SetItemNum(8, 1);
                                                GameManager.Instance.SetKeyState(0, 2);             //绿色钥匙已获取
                                        }
                                        else if (GameManager.Instance.GetKeyState(0) == 2 
                                        && GameManager.Instance.GetItemNum(16) == 1//已经获得绿色的种子
                                        && GameManager.Instance.GetKeyState(1) == 1)
                                        {
                                                GameManager.Instance.SetMessageType(1, "蓝色钥匙"); //获取蓝色钥匙
                                                GameManager.Instance.SetItemNum(9, 1);
                                                GameManager.Instance.SetKeyState(1, 2);             //蓝色钥匙已获取
                                        }
                                        else if (GameManager.Instance.GetKeyState(1) == 2
										&& GameManager.Instance.GetItemNum(17) == 1 //已经获得蓝色的种子
                                        && GameManager.Instance.GetKeyState(2) == 1)
                                        {
                                                GameManager.Instance.SetMessageType(1, "红色钥匙"); //获取红色钥匙
                                                GameManager.Instance.SetItemNum(10, 1);
                                                GameManager.Instance.SetKeyState(2, 2);             //红色钥匙已获取
                                        }
                                        else if (GameManager.Instance.GetKeyState(2) == 2
                                        && GameManager.Instance.GetItemNum(17) == 1 //已经获得红色的种子
                                        && GameManager.Instance.GetKeyState(3) == 1
                                        && GameManager.Instance.GetLightHouseState() == 6)//已经按下灯塔的按钮
                                        {
                                                GameManager.Instance.SetMessageType(1, "金色钥匙"); //获取黄色钥匙
                                                GameManager.Instance.SetItemNum(11, 1);
                                                GameManager.Instance.SetKeyState(3, 2);             //黄色钥匙已获取
                                        }
                                        break;
                                    case 2:                                                     //碰到展板
                                        GameManager.Instance.SetPanelState(4, true);            //开启主角家展板
                                        break;
                                    case 3:                                                     //碰到书柜	
                                        GameManager.Instance.SetMessageType(2, "20金币");     //获取金币消息
                                        GameManager.Instance.SetCurrAddMoney(20);               //外部指定玩家当前增加的钱数 

                                        GameManager.Instance.SetHomeCoinState(1);
                                        m_homeSelectCollider[m_homeThingIndex - 1].tag = "used";    //标注该书柜已翻过
                                        break;
                                }
                            }
                            break;

                        case 2:                                                         //当前处于村长家
                                if (m_selectDoor == 0)                                          //需返回村庄
                                {
                                AudioManager.Instance.SoundPlay(Global.GetInstance().audioName_Country_CountryHeadDoor);

                                m_doorMaskState = 1;                                    //开始开关门遮罩
                                    GameManager.Instance.CloseUsualBtn();                   //关闭常用按钮
                                    m_doorMaskObj.gameObject.SetActive(true);               //打开开关门遮罩
                                //GameManager.Instance.SetCurrentScene(0);
                                }
                                else if (m_headHomeThingIndex == 1)                         //按下喇叭
                                {
                                    if (GameManager.Instance.GetHeadHomeState() < 3)
                                        GameManager.Instance.SetHeadHomeState(1);
                                }
                                else if (m_headHomeThingIndex == 2)                         //按下boss战的门
                                {
                                GameDataManager.Instance.gameData.GameCurrentData = GameManager.Instance.SaveData();
                        GameDataManager.Instance.Save();                                                            //存储文件

                        Global.GetInstance().loadName = "HeadBattleScene";
                                    SceneManager.LoadScene("LoadingScene");
                                }
                                break;

                        case 3:                                                         //当前处于酒馆
							if (m_selectDoor == 0)                                          //需返回村庄
                            {
	                            AudioManager.Instance.SoundPlay(Global.GetInstance().audioName_Country_BarDoor);
	                            m_doorMaskState = 1;                                    //开始开关门遮罩
                                GameManager.Instance.CloseUsualBtn();                   //关闭常用按钮
                                m_doorMaskObj.gameObject.SetActive(true);               //打开开关门遮罩
                            //GameManager.Instance.SetCurrentScene(0);
                            }
                            if (m_barThingState)                                            //如果按下酒馆中的物品
                            {
                                m_barThingState = false;
                                GameManager.Instance.SetPanelState(0, true);
                            }
                            break;
                    }

            }

    }

    void Update()
    {
        
		if (!GameManager.Instance.isPlatformType_Mobild
            && GameManager.Instance.isUsualBtnCanClick 
            && (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.KeypadEnter)))
        {
            OnCheckBtnClick(m_generalBtn);
        }

        if (isHideUI)
        {
            hideUITimer -= Time.deltaTime;
            if (hideUITimer <= 0)
            {
                ScneUIPanel.SetActive(false);
                isHideUI = false;
            }
        }

        if (m_doorMaskState != 0)                                                   //如果开启开关门遮罩
        {
            if (m_doorMaskState == 1)                                               //开关门遮罩第一阶段
            {
                m_doorMaskObj.alpha += Time.deltaTime;                          //从白到黑
                if (m_doorMaskObj.alpha >= 1)                                       //全黑
                    m_doorMaskState = 2;                                        //进入第二阶段
            }
            if (m_doorMaskState == 2)                                               //开关门遮罩第二阶段
            {
                if (m_selectDoor != 0)
                {
                    m_inDoorAni.SetBool("OpenDoor", false);
                    m_inDoorAni.SetBool("CloseDoor", true);
                }

				GameManager.Instance.ChangeCurrScene(m_selectDoor);
                m_doorMaskState = 3;                                            //进入下一阶段
            }
            if (m_doorMaskState == 3)                                               //开关门遮罩第三阶段
            {
                m_doorMaskObj.alpha -= Time.deltaTime * 2f;                     //遮罩淡出
                if (m_doorMaskObj.alpha <= 0)                                       //如果遮罩全部淡出
                {
                    m_doorMaskObj.gameObject.SetActive(false);                  //关闭遮罩
                    m_doorMaskState = 0;                                        //关闭开关门遮罩

                    GameManager.Instance.OpenUsualBtn();
                }

            }
        }

		Bounds rr1 = m_checkBtnPlayerObject.GetComponent<Collider2D>().bounds;                  //主角的包围盒
        //Rect r1 = new Rect(rr1.center.x - rr1.size.x / 2,
        //                   rr1.center.y - rr1.size.y /2,
        //                   rr1.size.x, rr1.size.y);

        Rect r1 = new Rect(rr1.center.x - rr1.size.x / 2, rr1.center.y - rr1.size.y / 2, rr1.size.x, rr1.size.y);

        if (GameManager.Instance.GetCurrentScene() == 0)                            //当前处于村庄
        {
            for (int i = 0; i < m_countryNPCDiaCollider.Length; i++)                    //遍历NPC包围盒
            {
                Bounds rr2 = m_countryNPCDiaCollider[i].GetComponent<Collider>().bounds;        //获取NPC包围盒
                Rect r2 = new Rect(rr2.center.x - rr2.size.x / 2, rr2.center.y - rr2.size.y / 2, rr2.size.x, rr2.size.y);

				if (r1.Overlaps (r2)) {                                                //主角碰到对话事件

					//判断NPC的朝向
					var isDirL = false;
					if (m_checkBtnPlayerObject.transform.localScale.x < 0
					    && (m_checkBtnPlayerObject.transform.position.x > m_countryNPCDiaCollider [i].transform.position.x)) //主角向左走且在NPC左边
						isDirL = false; //右
					else if (m_checkBtnPlayerObject.transform.localScale.x > 0
					         && (m_checkBtnPlayerObject.transform.position.x < m_countryNPCDiaCollider [i].transform.position.x))
						isDirL = true;	//左


					if (i == 5) {                                             //如果碰到酒保
						if (GameManager.Instance.GetBarTaskState () != 2
						    && GameManager.Instance.GetItemEquipIndex () != 1) {                                //未装备成人面具
							m_npcDirL = isDirL;                              //NPC应面向方向
							m_dialogState = true;                           //碰到对话人物状态开启				
							m_dialogIndex = i;                              //获取对话索引（6个人）
							m_dialogTipArrow.SetActive (true);               //显示人物箭头（箭头运动）
							m_dialogTipArrow.transform.position = m_NPCarrowPos [i].position + new Vector3 (0f, Mathf.PingPong (Time.time / 1f, 0.2f));
							break;
						}
					} else if (i == 4) {                                            //如果主角碰到醉鬼
						if (GameManager.Instance.GetWaterBoxRepairState () == 0) {                     //如果当前醉鬼存在
							m_npcDirL = isDirL;                              
							m_dialogState = true;                           			
							m_dialogIndex = i;                             
							m_dialogTipArrow.SetActive (true);              
							m_dialogTipArrow.transform.position = m_NPCarrowPos [i].position + new Vector3 (0f, Mathf.PingPong (Time.time / 1f, 0.2f));
							break;
						}
					} else {                                                        //主角碰到的不是酒鬼和酒保
						m_npcDirL = isDirL;                                    
						m_dialogState = true;                                 
						m_dialogIndex = i;                                   
						m_dialogTipArrow.SetActive (true);                     
						m_dialogTipArrow.transform.position = m_NPCarrowPos [i].position + new Vector3 (0f, Mathf.PingPong (Time.time / 1f, 0.2f));
						break;
					}
				}

                if (i == (m_countryNPCDiaCollider.Length - 1) && m_dialogState)         //当前未碰到事件
                {
                    m_dialogState = false;                                          //碰到对话人物状态关闭	
                    m_dialogTipArrow.SetActive(false);                              //隐藏箭头
                    m_dialogBtnCheck = 0;                                           //恢复对话状态
                }
            }

            for (int i = 0; i < m_countryDiaSelectCollider.Length; i++)                 //遍历村庄中需要对话的物品
            {
                Bounds rr2 = m_countryDiaSelectCollider[i].GetComponent<Collider>().bounds;
                Rect r2 = new Rect(rr2.center.x - rr2.size.x / 2, rr2.center.y - rr2.size.y / 2, rr2.size.x, rr2.size.y);
                if (r1.Overlaps(r2))                                                    //主角碰到可对话可选物体
                {
                    if (i == 11)                                                      //主角碰到水阀开关 
                    {
                        if (GameManager.Instance.GetWaterBoxRepairState() == 1)       //如果当前可以修理
                        {
                            m_thingDialogState = true;                              //开启物品对话状态
                            m_thingDialogIndex = i;                                 //指定物品对话编号
                            m_countryDiaSelectBox[i].SetActive(true);               //显示水阀开关选中框
                        }
                    }                   
                    else if (i == 15)                                                   //主角碰到信
                    {
                        if(!CameraController.secondHelp)
                        {
                            CameraController.secondHelp = true;
                        }
                        if (GameManager.Instance.GetItemNum(0) == 0)              //当前信未获得
                        {
                            m_thingDialogState = true;                              //开启物品可对话状态
                            m_thingDialogIndex = i;                                 //获取碰到的物品编号
                            m_countryDiaSelectBox[i].SetActive(true);               //开启选中框
                        }
                        else
                        {
                            m_countryDiaSelectBox[i].SetActive(false);              //关闭选中框
                            m_thingDialogState = false;
                        }
                    }
                    else if (i == 16)                                                   //主角碰到罐子
                    {
                        if (GameManager.Instance.GetJarState() != 2)                    //当前罐子未碎
                        {
                            GameManager.Instance.SetMeetJar(true);                  //主角遇到罐子
                            m_thingDialogState = true;                              //开启物品可对话状态
                            m_thingDialogIndex = i;                                 //获取碰到的物品编号
                            m_countryDiaSelectBox[i].SetActive(true);               //开启选中框
                        }
                        else
                        {
                            m_thingDiaBtnCheck = 0;
                            m_countryDiaSelectBox[i].SetActive(false);              //关闭选中框
                            m_thingDialogState = false;
                        }
                    }
					else if (i == 5)//灯塔
					{
						if (GameManager.Instance.GetItemNum(12) == 0 
							|| GameManager.Instance.GetItemNum(13) == 0 
							|| GameManager.Instance.GetItemNum(14) == 0)                    //
						{
							m_thingDialogIndex = i;                                 //获取碰到的物品编号
							m_countryDiaSelectBox[i].SetActive(true);               //开启选中框
							m_thingDialogState = true;                              //开启物品可对话状态
						}
						else
						{
							m_thingDiaBtnCheck = 0;
							m_countryDiaSelectBox[i].SetActive(false);              //关闭选中框
							m_thingDialogState = false;
						}
					}
					else if (i == 2)//土丘1
					{
						if (GameManager.Instance.GetItemNum(12) == 0)                    //没有种下绿色种子并且获得绿色燃料
						{
							m_thingDialogIndex = i;                                 //获取碰到的物品编号
							m_countryDiaSelectBox[i].SetActive(true);               //开启选中框
							m_thingDialogState = true;                              //开启物品可对话状态
						}
						else
						{
							m_thingDiaBtnCheck = 0;
							m_countryDiaSelectBox[i].SetActive(false);              //关闭选中框
							m_thingDialogState = false;
						}
					}
                    else if (i == 17)//土丘2
                    {
                        if (GameManager.Instance.GetItemNum(13) == 0)                    //没有种下蓝色种子并且获得蓝色燃料
                        {
                            m_thingDialogIndex = i;                                 //获取碰到的物品编号
                            m_countryDiaSelectBox[i].SetActive(true);               //开启选中框
                            m_thingDialogState = true;                              //开启物品可对话状态
                        }
                        else
                        {
                            m_thingDiaBtnCheck = 0;
                            m_countryDiaSelectBox[i].SetActive(false);              //关闭选中框
                            m_thingDialogState = false;
                        }
                    }
                    else if (i == 18)//土丘3
                    {
                        if (GameManager.Instance.GetItemNum(14) == 0)                    //没有种下红色种子并且获得红色燃料
                        {
                            m_thingDialogIndex = i;                                 //获取碰到的物品编号
                            m_countryDiaSelectBox[i].SetActive(true);               //开启选中框
                            m_thingDialogState = true;                              //开启物品可对话状态
                        }
                        else
                        {
                            m_thingDiaBtnCheck = 0;
                            m_countryDiaSelectBox[i].SetActive(false);              //关闭选中框
                            m_thingDialogState = false;
                        }
                    }
                    
                    else if (i == 4)//灯塔的激活按钮
                    {
                        if (GameManager.Instance.GetLightHouseState() != 6)                    //按钮 没按过
                        {
                            m_thingDialogIndex = i;                                 //获取碰到的物品编号
                            m_countryDiaSelectBox[i].SetActive(true);               //开启选中框
                            m_thingDialogState = true;                              //开启物品可对话状态
                        }
                        else
                        {
                            m_thingDiaBtnCheck = 0;
                            m_countryDiaSelectBox[i].SetActive(false);              //关闭选中框
                            m_thingDialogState = false;
                        }
                    }
                    else                                                            //主角碰到的不是水阀开关
                    {
                        m_thingDialogState = true;                                  //开启物品可对话状态
                        m_thingDialogIndex = i;                                     //获取碰到的物品编号
                        m_countryDiaSelectBox[i].SetActive(true);                   //开启选中框
                    }
                    break;
                }

                if (i == (m_countryDiaSelectCollider.Length - 1) && m_thingDialogState) //当前未碰到可对话的物品事件
                {
                    m_thingDialogState = false;                                          //平时状态
                    Instance.SetThingDialogState(0);                                    //关闭物品可对话状态
                    for (int j = 0; j < m_countryDiaSelectBox.Length; j++)              //关闭所有的物品选中框
                        m_countryDiaSelectBox[j].SetActive(false);

                    GameManager.Instance.SetMeetJar(false);                         //主角未遇到罐子
                }
            }

            for (int i = 0; i < m_countryDoorSelectCollider.Length; i++)                    //遍历村庄中的门
            {
                Bounds rr2 = m_countryDoorSelectCollider[i].GetComponent<Collider>().bounds;
                Rect r2 = new Rect(rr2.center.x - rr2.size.x / 2, rr2.center.y - rr2.size.y / 2, rr2.size.x, rr2.size.y);
                if (r1.Overlaps(r2))                                                    //主角碰到可选物体
                {
                    m_selectBoxState = true;                                        //转至可选事件
                    if (i == 2)                                                     //如果碰到酒馆门
                    {
                        if (GameManager.Instance.GetItemEquipIndex() == 1 && GameManager.Instance.GetBarTaskState() == 2 )            //如果当前酒馆可进 装备成人面具
                        {
                            if (m_doorMaskState >= 1)
                                m_countryDoorSelectBox[i].SetActive(false);         
                            else
                                m_countryDoorSelectBox[i].SetActive(true);          //显示门的选中框

                            m_selectDoor = i + 1;                                       //指定当前碰到的门的编号
                            m_heroDoorIcon.SetActive(true);
                            break;
                        }
                    }
                    else if (i == 1)                        //如果碰到村长家门
                    {
						if (GameManager.Instance.GetItemNum (11) != 0) {                       //如果主角当前拥有金钥匙							
							if (m_doorMaskState >= 1)
								m_countryDoorSelectBox [i].SetActive (false);         //隐藏门的选中框
                            else
								m_countryDoorSelectBox [i].SetActive (true);          //显示门的选中框
							m_selectDoor = i + 1;                                       //指定当前碰到的门编号
							m_heroDoorIcon.SetActive (true);
						} 
                    }
                    else                                                        //如果碰到的hero home门
                    {
                        if (m_doorMaskState >= 1)
                            m_countryDoorSelectBox[i].SetActive(false);         //显示门的选中框
                        else
                            m_countryDoorSelectBox[i].SetActive(true);          //显示门的选中框
                        m_selectDoor = i + 1;                                       //指定当前碰到的门编号
                        m_heroDoorIcon.SetActive(true);
                        break;
                    }
                    break;
                }


                if (i == (m_countryDoorSelectCollider.Length - 1) && m_selectBoxState)      //当前未碰到门和信事件
                {
                    for (int j = 0; j < m_countryDoorSelectBox.Length; j++)             //关闭所有门的选中框
                        m_countryDoorSelectBox[j].SetActive(false);
                    m_selectBoxState = false;                                       //关闭可选状态
                    m_selectDoor = 0;                                               //门编号归零
                    m_heroDoorIcon.SetActive(false);
                }
            }
        }

        else if (GameManager.Instance.GetCurrentScene() == 1)                           //当前处于主角家
        {
            for (int i = 0; i < m_homeSelectCollider.Length; i++)                       //遍历选中物体事件
            {
                Bounds rr2 = m_homeSelectCollider[i].GetComponent<Collider>().bounds;
                Rect r2 = new Rect(rr2.center.x - rr2.size.x / 2, rr2.center.y - rr2.size.y / 2, rr2.size.x, rr2.size.y);
                if (r1.Overlaps(r2))                                                    //主角碰到可选物体
                {
                    if (i ==2 && GameManager.Instance.GetHomeCoinState() != 0)//对金币的处理
                        m_homeSelectCollider[i].tag = "used";
                    else
                        m_homeSelectCollider[i].tag = "Untagged";

                    if (m_homeSelectCollider[i].tag != "used")
                    {
                        m_selectBoxState = true;                                        //碰到事件变量
                        if (i == 3)                                                     //如果碰到的是门的包围盒
                        {
                            m_heroDoorIcon.SetActive(true);                             //开启门图标
                        /*if(m_checkBtnPlayerObject.transform.localScale.x<0)			//根据主角当前朝向决定出门图标的朝向
                            m_heroDoorIcon.transform.localScale = new Vector3(-1,1,1);
                        else
                            m_heroDoorIcon.transform.localScale = new Vector3(1,1,1);*/
                            m_selectDoor = 0;                                           //需返回村庄
                        }
                        else                                                            //主角碰到的不是门
                        {
                            m_selectDoor = 1;                                           //主角仍处在村庄
                            m_homeSelectBox[i].SetActive(true);                         //开启碰到物品选中框
                            m_heroDoorIcon.SetActive(false);                            //关闭门图标
                            m_homeThingIndex = i + 1;                                       //获取碰到物品索引
                        }
                    }
                    else
                    {
                        for (int j = 0; j < m_homeSelectBox.Length; j++)                        //遍历主角家的白色选中框
                            m_homeSelectBox[j].SetActive(false);                        //关闭

                        m_selectBoxState = false;                                       //碰到可选物品变量关闭
                        m_selectDoor = 1;                                               //主角仍处在主角家
                        m_heroDoorIcon.SetActive(false);                                //隐藏门图标
                        m_homeThingIndex = 0;                                           //碰到物品的编号归0
                    }
                    break;
                }
                if (i == (m_homeSelectCollider.Length - 1) && m_selectBoxState)         //当前未碰到事件
                {
                    for (int j = 0; j < m_homeSelectBox.Length; j++)                        //遍历主角家的白色选中框
                        m_homeSelectBox[j].SetActive(false);                        //关闭
                    m_selectBoxState = false;                                       //碰到可选物品变量关闭
                    m_selectDoor = 1;                                               //主角仍处在主角家
                    m_heroDoorIcon.SetActive(false);                                //隐藏门图标
                    m_homeThingIndex = 0;                                           //碰到物品的编号归0
                }
            }
        }

        else if (GameManager.Instance.GetCurrentScene() == 2)                           //当前处于村长家
        {
            for (int i = 0; i < m_headHomeSelectCollider.Length; i++)                   //遍历选中物体事件
            {
                Bounds rr2 = m_headHomeSelectCollider[i].GetComponent<Collider>().bounds;
                Rect r2 = new Rect(rr2.center.x - rr2.size.x / 2, rr2.center.y - rr2.size.y / 2, rr2.size.x, rr2.size.y);
                if (r1.Overlaps(r2))                                                    //主角碰到可选物体
                {
                    m_selectBoxState = true;                                        //转至可选事件
                    if (i == 0)//门
                    {
                        m_heroDoorIcon.SetActive(true);
                        /*if(m_checkBtnPlayerObject.transform.localScale.x<0)
                            m_heroDoorIcon.transform.localScale = new Vector3(-1,1,1);
                        else
                            m_heroDoorIcon.transform.localScale = new Vector3(1,1,1);*/
                        m_selectDoor = 0;
                    }
                    else
                    {
                        if (i == 2)// boss 的门
                        {
                            if (GameManager.Instance.GetHeadHomeState() == 4)               //柜子已移开
                            {
                                m_headHomeSelectBox[i].SetActive(true);
                                m_selectDoor = 2;
                                m_heroDoorIcon.SetActive(false);
                                m_headHomeThingIndex = i;
                            }
                            else
                            {
                                for (int j = 0; j < m_headHomeSelectBox.Length; j++)
                                    m_headHomeSelectBox[j].SetActive(false);
                                m_selectBoxState = false;
                                m_selectDoor = 2;
                                m_headHomeThingIndex = 0;
                                m_heroDoorIcon.SetActive(false);
                            }
                        }
                        else if (i == 1)//喇叭
                        {
                            if (GameManager.Instance.GetHeadHomeState() < 3)                //柜子
                            {
                                m_headHomeSelectBox[i].SetActive(true);
                                m_selectDoor = 2;
                                m_heroDoorIcon.SetActive(false);
                                m_headHomeThingIndex = i;
                            }
                            else
                            {
                                for (int j = 0; j < m_headHomeSelectBox.Length; j++)
                                    m_headHomeSelectBox[j].SetActive(false);
                                m_selectBoxState = false;
                                m_selectDoor = 2;
                                m_headHomeThingIndex = 0;
                                m_heroDoorIcon.SetActive(false);
                            }
                        }
                    }
                    break;
                }

                if (i == (m_headHomeSelectCollider.Length - 1) && m_selectBoxState)     //当前未碰到事件
                {
                    for (int j = 0; j < m_headHomeSelectBox.Length; j++)
                        m_headHomeSelectBox[j].SetActive(false);
                    m_selectBoxState = false;
                    m_selectDoor = 2;
                    m_headHomeThingIndex = 0;
                    m_heroDoorIcon.SetActive(false);
                }
            }
        }

        else if (GameManager.Instance.GetCurrentScene() == 3)                           //当前处于酒馆
        {
            for (int i = 0; i < m_barSelectCollider.Length; i++)                            //遍历选中物体事件
            {
                Bounds rr2 = m_barSelectCollider[i].GetComponent<Collider>().bounds;
                Rect r2 = new Rect(rr2.center.x - rr2.size.x / 2, rr2.center.y - rr2.size.y / 2, rr2.size.x, rr2.size.y);
                if (r1.Overlaps(r2))                                                    //主角碰到可选物体
                {
                    m_selectBoxState = true;                                        //转至可选事件
                    if (i == 0)                                                     //碰到出门包围盒
                    {
						m_selectDoor = 0;                                           //指定门编号为村庄
                        m_heroDoorIcon.SetActive(true);
                        /*if(m_checkBtnPlayerObject.transform.localScale.x<0)
                            m_heroDoorIcon.transform.localScale = new Vector3(-1,1,1);
                        else
                            m_heroDoorIcon.transform.localScale = new Vector3(1,1,1);*/
                    }
                    else if (i == 2)                                                    //碰到酒吧老板包围盒
                    {
						m_selectDoor = 3;                                           //门编号依旧为酒馆

                        if (m_checkBtnPlayerObject.transform.localScale.x < 0 
							&& (m_checkBtnPlayerObject.transform.position.x > m_barSelectCollider[i].transform.position.x))           //主角向左走且在NPC左边
                            m_npcDirL = false;                                      //NPC应面向右
                        else if (m_checkBtnPlayerObject.transform.localScale.x > 0 
							&& (m_checkBtnPlayerObject.transform.position.x < m_barSelectCollider[i].transform.position.x))
                            m_npcDirL = true;                                       //NPC应面向左 
                        
						m_dialogState = true;                                       //开启碰到NPC开关		
                        m_dialogIndex = 6;                                          //获取对话索引（6个人）
                        m_dialogTipArrow.SetActive(true);                           //显示人物箭头（箭头运动）
                        m_dialogTipArrow.transform.position = m_NPCarrowPos[6].position 
																+ new Vector3(0f, Mathf.PingPong(Time.time / 1f, 0.2f));
                    }
                    else                                                            //碰到的不是门
                    {
                        m_barSelectBox[i].SetActive(true);                          //开启白色选中框
                        m_selectDoor = 3;                                           //门编号依旧为酒馆
                        m_barThingState = true;                                     //开启碰到酒馆事件开关
                        m_heroDoorIcon.SetActive(false);                            //关闭门的标志
                        m_dialogTipArrow.SetActive(false);                          //关闭人物箭头
                        m_dialogBtnCheck = 0;                                       //恢复对话状态
                        m_dialogState = false;                                      //关闭碰到NPC开关	
                    }
                    break;
                }


                if (i == (m_barSelectCollider.Length - 1) && m_selectBoxState)              //当前未碰到事件
                {
                    for (int j = 0; j < m_barSelectBox.Length; j++)                     //关闭酒馆里所有选中框
                        m_barSelectBox[j].SetActive(false);
                    m_selectBoxState = false;                                       //关闭酒馆可选状态
                    m_barThingState = false;                                        //关闭酒馆中事件状态
                    m_selectDoor = 3;                                               //依旧在酒馆
                    m_dialogBtnCheck = 0;                                           //恢复对话状态
                    m_heroDoorIcon.SetActive(false);                                //关闭门的标志
                    m_dialogTipArrow.SetActive(false);                              //关闭人物箭头
                    m_dialogState = false;                                      //关闭碰到NPC开关
                }
            }
        }
    }

    public int GetDialogState()                                                     //获取是否处于对话状态
    {
        return m_dialogBtnCheck;
    }

    public void SetDialogState(int _dialog)                                         //设定是否处于对话状态
    {
        m_dialogBtnCheck = _dialog;
    }

    public int GetThingDialogState()                                                //获取是否处于物品对话状态
    {
        return m_thingDiaBtnCheck;
    }

    public void SetThingDialogState(int _thingDialog)                               //设定是否处于对话状态
    {
        m_thingDiaBtnCheck = _thingDialog;
    }

    public int GetDialogIndex()                                                     //获取对话索引
    {
        return m_dialogIndex;
    }

    public Vector3 GetDialogPos()                                                   //获取对话位置
    {
        return m_diaPos[m_dialogIndex].position;
    }

    public int GetThingDialogIndex()                                                //获取对话索引
    {
        return m_thingDialogIndex;
    }

    public Vector3 GetThingDialogPos()                                              //获取对话位置
    {
        if (m_checkBtnPlayerObject.transform.localScale.x < 0)                          //根据主角方向确定对话框位置
        {
            m_npcDirL = true;
            return m_thingDiaPos[0].position;
        }
        else
        {
            if (m_thingDialogIndex == 0 || m_thingDialogIndex == 1)
            {
                m_npcDirL = true;
                return m_thingDiaPos[1].position;
            }
            else
            {
                m_npcDirL = false;
                return m_thingDiaPos[0].position;
            }
        }
    }

    public bool GetNPCDirection()                                                   //外部获取NPC应该的朝向
    {
        return m_npcDirL;
    }




    void HideUIRoot() {
        hideUITimer = 1.5f;
        isHideUI = true;
    }


}