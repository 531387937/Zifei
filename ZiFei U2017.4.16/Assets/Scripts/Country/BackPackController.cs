using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Xml;

public class BackPackController : MonoBehaviour 
{
	public GameObject m_joyStick;                                                                       //摇杆按钮
    public GameObject[] m_usualBtn;																		//常用按钮
	public GameObject[] m_backPackPage;																	//背包系统书页 7
	public GameObject[] m_pageVirtualBtn;																//背包页虚拟按钮 0-3左 4-8右
	public Camera m_nguiCam;																			//NGUI摄像机

	public UISlider[] m_bpMusicSlider;																	//背包系统首页音乐滑条
	public GameObject[] m_bpMusicBtn;																	//背包系统首页音乐按钮
	public UISprite m_lifeObj;																			//背包系统生命物体
	public GameObject m_mainMenuBtn;																	//点击了主界面按钮
	public GameObject m_helpBtn;																		//点击了帮助按钮

	public GameObject[] m_backpackItemSprite;															//背包系统物品页物品框
	public UILabel[] m_bpItemLabel;																		//背包系统物品栏涉及文字（0数量 1名字 2介绍内容 3数量）
	private GameObject m_bpItemSelectBox;																//背包系统物品选中框
	public GameObject[] m_bpItemBtn;																	//背包系统物品界面按钮
	public GameObject[] m_bpHeroMask;																	//背包系统中主角的面具
    public UIScrollView itemInfoScrollView;

	public GameObject m_taskBox;																		//背包中任务标题栏预制体 
	public GameObject m_taskBoxPanel;																	//背包中任务标题栏平板 
	public UILabel m_taskContentBuble;																	//背包中任务内容
	public GameObject m_taskBoxSelect;																	//背包中任务栏选中框

    public GameObject achieveTitleObj;//“三十六计”titele
    public UILabel jiHaveCountUILabel; //36计拥有的数量
	private GameObject m_achieveSelectBox; 															//36计按钮选中框
	public UILabel[] m_achieveText;																		//36计文字介绍 （0题目 1内容）
	public GameObject[] m_backpackAchieveSprite;														//背包系统成就页物品框

	public GameObject[] m_bpSaveBox;																	//背包中存档页面存档条
	public GameObject[] m_bpSaveDeleteBtn;																//背包中存档页面存档栏的删除按钮
	public UILabel[] m_saveTimeLabel;																	//存档时间点 
	public UILabel[] m_saveSceneNameLabel;																//存档场景名
	public UISprite[] m_saveNPCPic;																		//存档最后对话NPC图
	public GameObject[] m_saveHeroPic;																	//存档界面的主角图
    public GameObject[] m_saveDesLabelObj;                                                              // 存档界面 描述信息

	public GameObject m_saveInfoPanel;																	//存档信息提示面板
	public UILabel m_saveInfoLabel;																		//存档信息提示文字
	public GameObject m_saveInfoSureBtn;																	//存档信息提示面板确定按钮
	public GameObject m_saveInfoCancelBtn;																//存档信息提示面板取消按钮
	public GameObject m_saveInfoOKBtn;                                                                  //存档信息提示面板OK按钮
    public GameObject m_bpMakerName;																	//制作人员名单
	public Transform[] m_bpMakerPos;																	//制作人员名单位置

	private XmlDocument m_itemXML;																		//物品xml文件
	private XmlNodeList m_itemXnl ;																		//读取物品文件的数组
	private XmlDocument m_taskXML;																		//任务xml文件
	private XmlNodeList m_taskXnl ;																		//读取任务文件的数组
	private int m_bpSelectItemIndex = 0;																//背包中当前选中的物品
	private int m_selectMyItemNumMax = 0;																//当前选中的物品所拥有的数量
	private int m_openPageCurrIndex = 0;																	//当前打开的书页编号
	[HideInInspector]
	public bool m_backPackOpening = false;																//背包是否打开
	public UISprite m_bookMark;																			//书签
	public UILabel m_bookMarkMoney;																		//书签上的钱数

	public GameObject m_bpMaskTip;

	private GameObject m_bpSelectBox;																	//背包中选中的物品图框
	private ArrayList m_taskBoxList = new ArrayList();													//用来存放背包中的任务栏
	private XmlDocument m_achieveXML;																	//成就xml文件
	private XmlNodeList m_achieveXnl ;																	//读取文件的数组

	private int m_bpTipState = 0;																		//背包提示状态
	private float m_bpTipTimer = 3f;                                                                    //背包提示计时器
    private GameObject _targetPackageItemObj;

    private int currentNeedDeletedataIndex;// 保存当前需要删除档位的index

    private SmithyController smithyAndbarController;

    void Awake()
	{

        smithyAndbarController = GetComponent <SmithyController> ();

        UIEventListener.Get (m_usualBtn[1].gameObject).onClick = OnBackPackBtnClick;					//点击背包按钮
		UIEventListener.Get (m_mainMenuBtn.gameObject).onClick = OnMainMenuBtnClick;					//点击"main Menu"按钮
		UIEventListener.Get (m_helpBtn.gameObject).onClick = OnHelpBtnClick;							//点击了帮助按钮

		UIEventListener.Get (m_pageVirtualBtn[0].gameObject).onClick = OnBackPackMainBtnClick;              //点击背包页面按钮
        UIEventListener.Get (m_pageVirtualBtn[1].gameObject).onClick = OnBackPackItemBtnClick;              //点击背包物品页面按钮
        UIEventListener.Get (m_pageVirtualBtn[2].gameObject).onClick = OnBackPackTaskBtnClick;				//点击背包任务页面按钮
		UIEventListener.Get (m_pageVirtualBtn[3].gameObject).onClick = OnBackPackAchieveBtnClick;	        //点击背包成就页面按钮
		UIEventListener.Get (m_pageVirtualBtn[4].gameObject).onClick = OnBackPackMainBtnClick;				//点击背包页面按钮
		UIEventListener.Get (m_pageVirtualBtn[5].gameObject).onClick = OnBackPackItemBtnClick;              //点击背包队伍页面按钮
        UIEventListener.Get (m_pageVirtualBtn[6].gameObject).onClick = OnBackPackTaskBtnClick;              //点击背包任务页面按钮
        UIEventListener.Get (m_pageVirtualBtn[7].gameObject).onClick = OnBackPackAchieveBtnClick;           //点击背包成就页面按钮
        UIEventListener.Get (m_pageVirtualBtn[8].gameObject).onClick = OnBackPackSaveDataBtnClick;			//点击背包存档页面按钮
		UIEventListener.Get (m_pageVirtualBtn[9].gameObject).onClick = OnBackPackMainBtnClick;				//点击背包页面按钮
	
		UIEventListener.Get (m_bpMusicBtn[0]).onClick = OnBpMusicClick;									//单击设置界面音乐按钮
		UIEventListener.Get (m_bpMusicBtn[1]).onClick = OnBpSoundClick;									//单击设置界面音效按钮

		UIEventListener.Get (m_bpItemBtn[0].gameObject).onClick = OnBpAddBtnClick;						//点击背包系统物品栏加号按钮
		UIEventListener.Get (m_bpItemBtn[1].gameObject).onClick = OnBpMinusBtnClick;					//点击背包系统物品栏减号按钮
		UIEventListener.Get (m_bpItemBtn[2].gameObject).onClick = OnBpUseBtnClick;						//点击背包系统物品栏使用按钮
		UIEventListener.Get (m_bpItemBtn[3].gameObject).onClick = OnBpUnEquipBtnClick;					//点击背包系统物品栏解除按钮
		UIEventListener.Get (m_bpItemBtn[4].gameObject).onClick = OnBpDeleteBtnClick;                   //点击背包系统物品栏删除按钮 --lisong

        UIEventListener.Get (m_usualBtn[2].gameObject).onClick = OnSaveBtnClick;						//点击存档按钮
		UIEventListener.Get (m_bpSaveBox[0].gameObject).onClick = OnBpSaveOneClick;						//点击背包系统存档点1
		UIEventListener.Get (m_bpSaveBox[1].gameObject).onClick = OnBpSaveTwoClick;						//点击背包系统存档点2
		UIEventListener.Get (m_bpSaveBox[2].gameObject).onClick = OnBpSaveThreeClick;					//点击背包系统存档点3
		UIEventListener.Get (m_bpSaveDeleteBtn[0].gameObject).onClick = OnBpSaveOneDeleteClick;			//点击背包系统存档点1删除按钮
		UIEventListener.Get (m_bpSaveDeleteBtn[1].gameObject).onClick = OnBpSaveTwoDeleteClick;			//点击背包系统存档点2删除按钮
		UIEventListener.Get (m_bpSaveDeleteBtn[2].gameObject).onClick = OnBpSaveThreeDeleteClick;		//点击背包系统存档点3删除按钮

        



    }

	void InitTaskBox()																					//根据当前任务完成情况初始化任务框
	{
		foreach(GameObject _taskBox in m_taskBoxList)													//遍历已有任务框
			Destroy(_taskBox);																			//销毁

		m_taskBoxList.Clear ();																			//清空数组

		for(int i=0; i<7; i++)																			//遍历所有任务
		{
			if(GameManager.Instance.GetTaskIndexState(i)!=0)											//如果当前任务已接受
			{
				GameObject _taskBox = NGUITools.AddChild (m_taskBoxPanel, m_taskBox);					//背包中新增任务栏
				_taskBox.GetComponent<UISprite>().name = (i+1).ToString();								//指定新增的任务栏名称为任务编号
				Vector3 _taskBoxPos = new Vector3 (0f, 299f-115f*i, 0f);								//计算新增的任务栏位置
				_taskBox.transform.localPosition = _taskBoxPos;											//指定新增任务栏位置
				GameObject _taskTitle = _taskBox.transform.GetChild (0).gameObject;						//新增的任务栏标题
				_taskTitle.GetComponent<UILabel> ().text = m_taskXnl [i].ChildNodes [0].InnerText;		//指定标题
				m_taskBoxList.Add (_taskBox);															//添加到背包任务栏列表
				UIEventListener.Get (_taskBox.gameObject).onClick = OnTaskBoxClick;						//注册背包中任务栏点击事件
			}
		}
	}

	void OnEnable()
	{

		string dataTask = Resources.Load("Task").ToString(); 						//读取任务相关数据
		m_taskXML = new XmlDocument ();								
		m_taskXML.LoadXml (dataTask);
		m_taskXnl = m_taskXML.GetElementsByTagName("Task");
		InitTaskBox ();																//初始化任务框
	}

    private void OnDisable()
    {

        //解除按钮的点击事件
        ClearInfoPanelBtnEvent();
    }


    void Start()
	{


        string data = Resources.Load("Item").ToString(); 							//读取物品的相关数据
		m_itemXML = new XmlDocument ();								
		m_itemXML.LoadXml (data);
		m_itemXnl = m_itemXML.GetElementsByTagName("Item");

		string dataTask = Resources.Load("Task").ToString(); 						//读取任务相关数据
		m_taskXML = new XmlDocument ();								
		m_taskXML.LoadXml (dataTask);
		m_taskXnl = m_taskXML.GetElementsByTagName("Task");

		string dataAchieve = Resources.Load("Achieve").ToString(); 					//读取外部文件的相关数据
		m_achieveXML = new XmlDocument ();								
		m_achieveXML.LoadXml (dataAchieve);
		m_achieveXnl = m_achieveXML.GetElementsByTagName("achieve");
	}

	void OnMainMenuBtnClick(GameObject _mainMenu)									//点击了返回主界面
	{
        AudioManager.Instance.SoundPlay(Global.GetInstance().audioName_BtnClick2);
        GameManager.Instance.m_gameManagerHero.SetActive(false);
        GameDataManager.Instance.gameData.GameCurrentData = GameManager.Instance. SaveData();
        CloseBackPack();
        GameManager.Instance.m_sceneUIPanel.SetActive(false);
        GameManager.Instance.startSceneController.ShowStartPanel();
        
        int levelIndex = GameDataManager.Instance.gameData.GameCurrentData.levelIndex;
        //if (0 != levelIndex  && levelIndex < 4)            //需要更换当前场景（且是村庄中的场景）
            GameManager.Instance.ChangeCurrScene(0);

        GameManager.Instance.ReadData();
        GameManager.Instance.SetGameStartState(0);									//摄像机回到开始的位置

    }

    void OnHelpBtnClick(GameObject _help)
	{
        AudioManager.Instance.SoundPlay(Global.GetInstance().audioName_BtnClick2);

        GameManager.Instance.SetHelpPanelOpen (true);
		CloseBackPack ();															//关闭背包
	}

	void OnBpMusicClick(GameObject _musicBtn)										//按下音乐按钮
	{
		if(m_bpMusicSlider[0].value>0.01f)
		{
			GameManager.Instance.SetVolume(0, 0f);
			m_bpMusicBtn[0].GetComponent<UISprite>().spriteName = "option_btn_off";	//关闭音乐
			m_bpMusicSlider[0].value=0.01f;											//给定音乐滑动条值
		}
		else
		{
			GameManager.Instance.SetVolume(0, 1f);
			m_bpMusicBtn[0].GetComponent<UISprite>().spriteName = "option_btn_on";	//打开音乐
			m_bpMusicSlider[0].value=1f;											//给定音乐滑动条值
		}
	}
	
	void OnBpSoundClick(GameObject _musicBtn)										//按下音效按钮
	{
		if(m_bpMusicSlider[1].value>0.01f)
		{
			GameManager.Instance.SetVolume(1, 0f);
			m_bpMusicBtn[1].GetComponent<UISprite>().spriteName = "option_btn_off";	//关闭音乐
			m_bpMusicSlider[1].value=0.01f;											//给定音乐滑动条值
		}
		else
		{
			GameManager.Instance.SetVolume(0, 1f);
			m_bpMusicBtn[1].GetComponent<UISprite>().spriteName = "option_btn_on";	//打开音乐
			m_bpMusicSlider[1].value=1f;											//给定音乐滑动条值
		}
	}

	void OnBpAddBtnClick(GameObject _bpAddBtn)										//点击背包系统物品界面加号按钮
	{
        AudioManager.Instance.SoundPlay(Global.GetInstance().audioName_BtnClick3);

        if (m_bpSelectItemIndex!=0)													//当前存在选中物体
		{
			int _currNum = int.Parse (m_bpItemLabel [0].text);						//获取当前数量值
			if(_currNum<m_selectMyItemNumMax)										//如果当前显示的数量少于最大值
				m_bpItemLabel [0].text = (_currNum + 1).ToString ();				//数字可加1
		}
	}

    void OnBpMinusBtnClick(GameObject _bpMinusBtn)									//点击背包系统物品界面减号按钮
	{
        AudioManager.Instance.SoundPlay(Global.GetInstance().audioName_BtnClick3);

        if (m_bpSelectItemIndex!=0)													//当前存在选中物体
		{
			int _currNum = int.Parse (m_bpItemLabel [0].text);						//获取当前数量值
			if(_currNum>=2)															//如果数字大于等于2
				m_bpItemLabel [0].text = (_currNum - 1).ToString ();				//数量可减少
		}
	}

    void OnBpUseBtnClick(GameObject _useBtn)										//点击背包系统物品界面使用按钮
	{
        AudioManager.Instance.SoundPlay(Global.GetInstance().audioName_BtnClick2);

        if (m_bpSelectItemIndex!=0&&m_selectMyItemNumMax!=0)							//当前存在选中物体且选中物品数量不为0
		{
			if(m_bpSelectItemIndex>4&&m_bpSelectItemIndex<9)
			{
		        int _equipedIndex = GameManager.Instance.GetItemEquipIndex();
				if(_equipedIndex  != 0)
				{
					m_bpHeroMask[_equipedIndex-1].SetActive(false);						//隐藏当前装备的
					m_backpackItemSprite[_equipedIndex+3].GetComponent<UISprite>().spriteName = "pack_itemBox_normal";	//物品框恢复正常
				}
				GameManager.Instance.SetItemEquipIndex(m_bpSelectItemIndex-4);			//指定当前装备的道具编号
				m_bpItemBtn[2].SetActive(false);										//关闭使用按钮
				m_bpItemBtn[3].SetActive(true);											//开启解除按钮
				switch(m_bpSelectItemIndex)												//判定背包中当前选定的物品编号
				{
				case 5:                                                                 //成人面具

                        GameManager.Instance.SetBarTaskState(2);
					if(GameManager.Instance.GetTaskIndexState(6)==1)
						GameManager.Instance.SetTaskIndexState(6,2);						//完成酒保任务
					m_bpHeroMask[0].SetActive(true);									//开启背包中主角的面具
					break;
				case 6:																	//德鲁伊之冠
					if (GameManager.Instance.GetAchieveGot(18) == 0)
					{
						AudioManager.Instance.TipsSoundPlay(Global.GetInstance().audioName_AchieveGet);
						GameManager.Instance.SetAchieveGot(18, 1);
						GameManager.Instance.SetMessageType(3, "您获得了成就 【树上开花】");
					}
					m_bpHeroMask[1].SetActive(true);									//开启背包中主角的面具
					break;
				case 7:																	//狐狸头套
					if (GameManager.Instance.GetAchieveGot(3) == 0)
					{
						AudioManager.Instance.TipsSoundPlay(Global.GetInstance().audioName_AchieveGet);
						GameManager.Instance.SetAchieveGot(3, 1);
						GameManager.Instance.SetMessageType(3, "您获得了成就 【反间计】");
					}
					m_bpHeroMask[2].SetActive(true);									//开启背包中主角的面具
					break;
				case 8:																	//机械眼镜
					if (GameManager.Instance.GetAchieveGot(5) == 0)
					{
						AudioManager.Instance.TipsSoundPlay(Global.GetInstance().audioName_AchieveGet);
						GameManager.Instance.SetAchieveGot(5, 1);
						GameManager.Instance.SetMessageType(3, "您获得了成就 【釜底抽薪】");
					}
					m_bpHeroMask[3].SetActive(true);									//开启背包中主角的面具
					break;
				}
				m_bpSelectBox.GetComponent<UISprite>().spriteName = "pack_itemBox_equiped";
			}
			else if(m_bpSelectItemIndex==3 || m_bpSelectItemIndex == 2)												//当前使用的是肉/酒
			{
		        int _bloodNum = int.Parse( m_bpItemLabel[0].text);
                var newIndex = m_bpSelectItemIndex - 1;


                int _num = GameManager.Instance.GetItemNum(newIndex) - _bloodNum;
				m_selectMyItemNumMax = _num;
				if(_num==0)
				{
					m_backpackItemSprite[newIndex].tag = "Untagged";												//改变背包中该方块标签
					m_backpackItemSprite[newIndex].transform.GetChild(0).gameObject.SetActive(false);				//隐藏该物品图标
					if (m_bpItemSelectBox != null) {
						m_bpItemSelectBox.SetActive(false);														//隐藏选中框
					}
					m_bpSelectItemIndex = 0;
					
					m_bpItemLabel[0].text = " ";
					m_bpItemLabel[1].text = " ";
					m_bpItemLabel[2].text = " ";
                    m_bpItemLabel[3].text = "";//指定物品的总数量 --lisong
                    SetDetailItemInfoInvisible(false, false);

                }
                else
				{
                    m_bpItemLabel[0].text = "1";// _num.ToString();
                    m_bpItemLabel[3].text = (_num).ToString();//指定物品的总数量 --lisong

                }
			        GameManager.Instance.SetItemNum(newIndex, _num);
			        GameManager.Instance.SetMessageType(1, (_bloodNum * 10f * newIndex).ToString() + "点体力");
			        GameManager.Instance.SetHeroBloodReduce(-_bloodNum * 0.1f * newIndex);
                
            }
		}

	}

    void OnBpUnEquipBtnClick(GameObject _unEquipBtn)								//点击背包系统物品界面解除按钮
	{
        AudioManager.Instance.SoundPlay(Global.GetInstance().audioName_BtnClick2);

        if (m_bpSelectItemIndex!=0)													//当前存在选中物体
		{
			GameManager.Instance.SetItemEquipIndex(0);//去掉全部的装备
			m_bpItemBtn[2].SetActive(true);											//开启使用按钮	
			m_bpItemBtn[3].SetActive(false);										//关闭解除按钮
			switch(m_bpSelectItemIndex)												//判定当前选中的物品编号
			{
			case 5:                                                                 //成人面具
                    GameManager.Instance.SetBarTaskState(3);						//主角卸掉面具
				m_bpHeroMask[0].SetActive(false);									//隐藏背包中主角的面具
				break;
			case 6:
				m_bpHeroMask[1].SetActive(false);									//隐藏背包中主角的面具
				break;
			case 7:
				m_bpHeroMask[2].SetActive(false);									//隐藏背包中主角的面具
				break;
			case 8:
				m_bpHeroMask[3].SetActive(false);									//隐藏背包中主角的面具
				break;
			}

            m_bpSelectBox.GetComponent<UISprite>().spriteName = "pack_itemBox_normal";
		}
	}



    /// <summary>
    /// 
    /// </summary>
    /// <param name="isBtnVisible">button </param>
    /// <param name="isLabelVisible"> label</param>
    void SetDetailItemInfoInvisible(bool isBtnVisible, bool isLabelVisible) {

        for (int i = 0; i < m_bpItemBtn.Length; i++)
        {
            m_bpItemBtn[i].SetActive(isBtnVisible);
        }
        for (int i = 0; i < m_bpItemLabel.Length; i++)
        {
            m_bpItemLabel[i].gameObject.SetActive(isLabelVisible);
        }

    }

    //  删除物品 --lisong
    void OnBpDeleteBtnClick(GameObject go) {
        int _currNum = int.Parse(m_bpItemLabel[0].text);                        //获取当前数量值

        int residueNum = GameManager.Instance.GetItemNum(m_bpSelectItemIndex - 1) - _currNum;
        GameManager.Instance.SetItemNum(m_bpSelectItemIndex - 1, residueNum);								//更改已拥有的该物品数量
        m_selectMyItemNumMax = residueNum;

        if (residueNum > 0)
        {
            m_bpItemLabel[0].text = "1";//(residueNum).ToString();
            m_bpItemLabel[3].text = (residueNum).ToString();//指定物品的总数量 --lisong
        }
        else if (residueNum == 0)
        {
            // 将物品设置为默认的状态
            m_bpItemLabel[0].text = "";
            m_bpItemLabel[3].text = "";
            _targetPackageItemObj.tag = "Untagged";
            _targetPackageItemObj.transform.GetChild(0).gameObject.SetActive(false); // 隐藏物品的显示
            _targetPackageItemObj.transform.GetChild(1).gameObject.SetActive(true); //  将物品的背景剪影显示出来

            SetDetailItemInfoInvisible(false, false);

            // 对pack中Hero的饰品显示做处理
            if (m_bpSelectItemIndex > 4 && m_bpSelectItemIndex < 9)
            {
                for (int i = 0; i < m_bpHeroMask.Length; i++)
                {
                    m_bpHeroMask[i].SetActive(false);
                }
                GameManager.Instance.SetItemEquipIndex(0); // 取消Hero的饰品显示
            }

        }

    }

    void OnTaskBoxClick(GameObject _taskBoxClick)														//点击了背包中任务栏的某条任务
	{
        AudioManager.Instance.SoundPlay(Global.GetInstance().audioName_BackpageItemClick);

        string _taskName = _taskBoxClick.name;															//获取点击的任务栏名字
		foreach(GameObject _taskBox in m_taskBoxList)													//遍历所有任务
		{
			if(_taskBox.name==_taskName)																//定位点击的任务
			{
				m_taskBoxSelect.SetActive(true);														//开启选中框
				m_taskBoxSelect.transform.position = _taskBox.transform.position;						//指定选中框位置
				m_taskContentBuble.text =  m_taskXnl [int.Parse(_taskName)-1].ChildNodes [1].InnerText;//指定显示的任务内容
				break;
			}
		}
	}


  

    void OnSaveBtnClick(GameObject _saveBtn)															//点击了存档按钮
	{
        AudioManager.Instance.SoundPlay(Global.GetInstance().audioName_BtnClick2);
        UIEventListener.Get(m_saveInfoSureBtn).onClick = OnSaveInfoSureBtnClick;                //点击了存档面板的确定按钮
        UIEventListener.Get(m_saveInfoCancelBtn).onClick = OnSaveInfoCancelBtnClick;                //点击了存档面板的取消按钮
        UIEventListener.Get(m_saveInfoOKBtn).onClick = OnSaveInfoOKBtnClick;                //点击了存档面板的OK按钮

        m_saveInfoLabel.text = "是否需要存档？";                                							//存档提示条文字							
        m_saveInfoOKBtn.gameObject.SetActive(false);
        m_saveInfoPanel.SetActive (true);																//开启存档结束提示面板
		GameManager.Instance.CloseUsualBtn ();															//禁用常用按钮
	}
 

    void OnSaveInfoSureBtnClick(GameObject _okBtn)                                                        //点击了存档提示面板确定按钮
    {
        AudioManager.Instance.SoundPlay(Global.GetInstance().audioName_BtnClick2);
        int _saveIndex = GameManager.Instance.SaveCurrData(false);                                           //存档
        _saveIndex++;
        m_saveInfoLabel.text = "存档完毕，已存至档位" + _saveIndex.ToString();
        //m_usualBtn[2].GetComponent<UISprite>().spriteName = "btn_save02";                               //更改存档按钮图
        m_saveInfoOKBtn.gameObject.SetActive(true);
        m_saveInfoSureBtn.gameObject.SetActive(false);
        m_saveInfoCancelBtn.gameObject.SetActive(false);

    }

    void OnSaveInfoCancelBtnClick(GameObject go)
    {
        AudioManager.Instance.SoundPlay(Global.GetInstance().audioName_BtnClick2);

        m_saveInfoPanel.SetActive(false);                                                                //开启存档结束提示面板
        GameManager.Instance.OpenUsualBtn();															//禁用常用按钮
        ClearInfoPanelBtnEvent();
    }

    void OnSaveInfoOKBtnClick(GameObject go)
    {
        AudioManager.Instance.SoundPlay(Global.GetInstance().audioName_BtnClick2);

        m_saveInfoPanel.SetActive(false);                                                               //隐藏存档信息提示面板
        m_saveInfoOKBtn.gameObject.SetActive(false);
        m_saveInfoSureBtn.gameObject.SetActive(true);
        m_saveInfoCancelBtn.gameObject.SetActive(true);
        GameManager.Instance.OpenUsualBtn();                                                            //开启常用按钮
        ClearInfoPanelBtnEvent();

    }

    void SaveGetSaveData(int _index)																	//点击了档位按钮
	{
        AudioManager.Instance.SoundPlay(Global.GetInstance().audioName_BtnClick2);

        CloseBackPack();																				//关闭背包
		GameManager.Instance.GetSaveData (_index);														//读取存档点1

		_index++;
        m_saveInfoLabel.text = "读档完毕，已读取档位  " + _index.ToString();                                    //读档提示条	
        m_saveInfoOKBtn.gameObject.SetActive(true);
        m_saveInfoSureBtn.gameObject.SetActive(false);
        m_saveInfoCancelBtn.gameObject.SetActive(false);
        UIEventListener.Get(m_saveInfoOKBtn).onClick = OnSaveInfoOKBtnClick;
        m_saveInfoPanel.SetActive (true);																//开启存档结束提示面板


		GameManager.Instance.CloseUsualBtn ();															//禁用常用按钮
	}
	

	void OnBpSaveOneClick(GameObject _saveBtn)															//点击了背包中的存档点1
	{
		SaveGetSaveData (0);
	}

    void OnBpSaveTwoClick(GameObject _saveBtn)															//点击了背包中的存档点2
	{
		SaveGetSaveData (1);
	}
	void OnBpSaveThreeClick(GameObject _saveBtn)														//点击了背包中的存档点3
	{
		SaveGetSaveData (2);
	}

    void OnBpSaveOneDeleteClick(GameObject _saveBtn)													//点击了背包中的存档点1删除按钮
	{
        ShowInfoPanel(0);
	}

    void OnBpSaveTwoDeleteClick(GameObject _saveBtn)													//点击了背包中的存档点2删除按钮
	{
        ShowInfoPanel(1);
	}

    void OnBpSaveThreeDeleteClick(GameObject _saveBtn)													//点击了背包中的存档点3删除按钮
	{
        ShowInfoPanel(2);
	}


    void ShowInfoPanel(int index)
    {
        AudioManager.Instance.SoundPlay(Global.GetInstance().audioName_BtnClick2);
        UIEventListener.Get(m_saveInfoSureBtn).onClick = OnSureDeleteDataBtnClick;
        UIEventListener.Get(m_saveInfoCancelBtn).onClick = OnCancelOrOKDeleteDataBtnClick;
        UIEventListener.Get(m_saveInfoOKBtn).onClick = OnCancelOrOKDeleteDataBtnClick;


        m_saveInfoLabel.text = "确认删除档位 " + (index + 1).ToString() + " 的数据存档？";                  //提示信息文字
        m_saveInfoSureBtn.SetActive(true);
        m_saveInfoCancelBtn.SetActive(true);
        m_saveInfoOKBtn.SetActive(false);
        m_saveInfoPanel.SetActive(true);                                     //开启删除结束提示面板
        currentNeedDeletedataIndex = index;
    }


    void OnSureDeleteDataBtnClick(GameObject go)
    {
        AudioManager.Instance.SoundPlay(Global.GetInstance().audioName_BtnClick2);

        if (currentNeedDeletedataIndex != -1)
        {
            DeleteSaveData(currentNeedDeletedataIndex);
        }
        currentNeedDeletedataIndex = -1;
    }


    void OnCancelOrOKDeleteDataBtnClick(GameObject go)
    {

        AudioManager.Instance.SoundPlay(Global.GetInstance().audioName_BtnClick2);

        m_saveInfoSureBtn.SetActive(true);
        m_saveInfoCancelBtn.SetActive(true);
        m_saveInfoOKBtn.SetActive(false);
        m_saveInfoPanel.SetActive(false);

        ClearInfoPanelBtnEvent();
    }

    void DeleteSaveData(int _index)                                                                 //点击了删档按钮											
    {
        AudioManager.Instance.SoundPlay(Global.GetInstance().audioName_BtnClick2);

        m_bpSaveDeleteBtn[_index].SetActive(false);                     //隐藏删除存档按钮
        m_saveHeroPic[_index].SetActive(false);                                                     //隐藏主角图
        m_saveInfoSureBtn.SetActive(false);
        m_saveInfoCancelBtn.SetActive(false);
        m_saveInfoOKBtn.SetActive(true);
        m_bpSaveBox[_index].GetComponent<BoxCollider>().enabled = false;                           //不能读档
        m_saveTimeLabel[_index].text = " ";                                                          //显示当前存档时系统时间
        m_saveSceneNameLabel[_index].text = " ";                                                     //显示当前场景名称
        m_saveNPCPic[_index].spriteName = " ";                                                      //没有最后对话的NPC
        m_saveDesLabelObj[_index].SetActive(true);

        GameManager.Instance.DeleteSaveData(_index);                                                    //存档点信息清空


        m_saveInfoLabel.text = "已删除数据存档 " + (_index + 1).ToString();                                         //提示信息文字
        m_saveInfoPanel.SetActive(true);                                                                //开启删除结束提示面板
        //GameManager.Instance.CloseUsualBtn();                                                           //禁用常用按钮
    }


    void ClearInfoPanelBtnEvent() {

        if (m_saveInfoSureBtn != null) UIEventListener.Get(m_saveInfoSureBtn).onClick = null;
        if (m_saveInfoCancelBtn != null) UIEventListener.Get(m_saveInfoCancelBtn).onClick = null;
        if (m_saveInfoOKBtn != null) UIEventListener.Get(m_saveInfoOKBtn).onClick = null;
    }

    void OnBackPackBtnClick(GameObject _backPackBtn)													//按下背包按钮 --lisong
	{
        AudioManager.Instance.SoundPlay(Global.GetInstance().audioName_BtnClick);
		if(m_bpTipState==0)
			m_bpTipState = 1;

        m_backPackOpening = true;																		//背包打开 
        GameManager.Instance.CloseUsualBtn();
        m_backPackPage [0].SetActive (true);
		m_backPackPage[7].SetActive(true);																//遮罩打开
		m_openPageCurrIndex = 1;																		//当前为第一页
		m_backPackPage[0].GetComponent<UISprite>().spriteName = "page" + m_openPageCurrIndex.ToString();//背景图更改
		m_backPackPage [m_openPageCurrIndex].SetActive (true);											//打开第一页内容
		m_pageVirtualBtn [9].GetComponent<UISprite> ().depth = 10;
		m_bookMark.depth = 9;
		m_bookMarkMoney.depth = 10;
		m_bookMarkMoney.text = GameManager.Instance.GetCurrMoney().ToString();                          //指定当前拥有的钱数

        for (int i=0; i< 5; i++)																			//设定虚拟按钮样式
			m_pageVirtualBtn[i].SetActive(false);
		for(int i=5; i<9; i++)
			m_pageVirtualBtn[i].SetActive(true);

        for (int i=0; i<3; i++)																			//禁用常用按钮
			m_usualBtn[i].GetComponent<UIButton>().enabled = false;
		m_usualBtn [1].GetComponent<UISprite> ().spriteName = "btn_menu21";								//更改背包按钮图
		m_joyStick.gameObject.SetActive (false);														//关闭摇杆

		m_lifeObj.fillAmount = GameManager.Instance.GetBloodNum ();										//外部获取主角血量值
		m_bpMusicSlider [0].value = GameManager.Instance.GetVolume (0);
		m_bpMusicSlider [1].value = GameManager.Instance.GetVolume (1);

		InitTaskBox ();//获取任务的数据显示任务
	}

	public void CloseBackPack()
	{
        //AudioManager.Instance.SoundPlay(Global.GetInstance().audioName_BtnClick);

        m_backPackOpening = false;																		//背包关闭
		for(int i=0; i<8; i++)																			//关闭所有背包图
			m_backPackPage[i].SetActive(false);			
		for(int i=0; i<9; i++)																			//禁用所有虚拟按钮
			m_pageVirtualBtn[i].SetActive(false);

        m_usualBtn [1].GetComponent<UISprite> ().spriteName = "btn_menu01";								//改变背包背景图
		m_openPageCurrIndex = 0;
		GameManager.Instance.SetBpIndex(m_openPageCurrIndex);											//指定当前打开的背包页数
        GameManager.Instance.OpenUsualBtn();

    }

    void Update()
	{

		if (GameManager.Instance.m_sceneUIPanel.activeSelf && Input.GetKeyDown(KeyCode.Tab) &&(m_backPackOpening || GameManager.Instance.isUsualBtnCanClick))
        {

            if (m_backPackOpening)
                CloseBackPack();
            else
            {

                if (smithyAndbarController.m_smithyState == 1)
                    smithyAndbarController.CloseSmithy();
                if (smithyAndbarController.m_barState == 1)
                    smithyAndbarController.CloseBar();

                OnBackPackBtnClick(this.gameObject);
            }
        }


		//----------------------------------------------------------------------

        if (m_backPackOpening)																			//如果背包打开
		{
			if(m_bpTipState==1)
			{
				m_bpTipTimer -= Time.deltaTime;
				if(m_bpTipTimer<0)
				{
					m_bpTipState = 2;
					m_bpMaskTip.SetActive(false);
				}
			}

			if(Input.GetMouseButtonDown(0))																//如果鼠标按下
			{
				Ray ray = m_nguiCam.GetComponent<Camera>().ScreenPointToRay (Input.mousePosition);
				RaycastHit hit;
				if (Physics.Raycast(ray, out hit)) 														//检测鼠标点击区域
				{
					if(hit.collider.gameObject == m_backPackPage[7])									//如果点击了后边遮罩图 关闭背包
					{
						CloseBackPack();																//关闭背包
						for(int i=0; i<3; i++)															//开启常用按钮
							m_usualBtn[i].GetComponent<UIButton>().enabled = true;
						m_joyStick.gameObject.SetActive (true);											//打开摇杆
					}	
				}
			}

			switch(m_openPageCurrIndex)
			{
			case 1:																						//当前打开的为第一页
				GameManager.Instance.SetVolume(0, m_bpMusicSlider[0].value);
				GameManager.Instance.SetVolume(1, m_bpMusicSlider[1].value);
				if(m_bpMusicSlider[0].value<=0.01f)														//如果滑条过小
				{
					m_bpMusicSlider[0].value=0.01f;														//给定滑条值
					m_bpMusicBtn[0].GetComponent<UISprite>().spriteName = "option_btn_off";				//关闭音量
				}
				else 																					//否则
					m_bpMusicBtn[0].GetComponent<UISprite>().spriteName = "option_btn_on";				//开启音乐
				
				if(m_bpMusicSlider[1].value<=0.01f)
				{
					m_bpMusicSlider[1].value=0.01f;
					m_bpMusicBtn[1].GetComponent<UISprite>().spriteName = "option_btn_off";
				}
				else
					m_bpMusicBtn[1].GetComponent<UISprite>().spriteName = "option_btn_on";
				break;
			case 2:																						//当前打开的为物品页面
				if(Input.GetMouseButtonDown(0))															//如果鼠标按下
				{
					Ray ray = UICamera.mainCamera.ScreenPointToRay (Input.mousePosition);				//获取点击位置
					RaycastHit hit;
					if (Physics.Raycast(ray, out hit)) 													//检测鼠标点击区域
					{
						if(hit.collider.gameObject.tag == "ItemBox")									//按下商品栏中的物品
						{
                                AudioManager.Instance.SoundPlay(Global.GetInstance().audioName_BackpageItemClick);

                                _targetPackageItemObj = hit.collider.gameObject;                        // 保持 玩家点击的物品对象 --lisong
                                itemInfoScrollView.ResetPosition();
                            SetDetailItemInfoInvisible(false, true);//显示出详情面板

							if(m_bpItemSelectBox!= null) m_bpItemSelectBox.SetActive(false);
							m_bpItemSelectBox = _targetPackageItemObj.transform.GetChild(2).gameObject;
                            m_bpItemSelectBox.SetActive(true);											//显示选中框
							//m_bpItemSelectBox.transform.position = hit.collider.gameObject.transform.position;

							m_bpSelectItemIndex = int.Parse(hit.collider.gameObject.name);              //通过名字获取点击的物品索引

                            int itemCount = GameManager.Instance.GetItemNum(m_bpSelectItemIndex - 1);

                                m_bpItemLabel[0].text = "1";//itemCount.ToString();//指定物品数量
							m_bpItemLabel[1].text = m_itemXnl [m_bpSelectItemIndex-1].ChildNodes [1].InnerText;//指定物品名称
                            m_bpItemLabel[2].text = m_itemXnl [m_bpSelectItemIndex-1].ChildNodes [2].InnerText;//指定物品介绍
							m_bpItemLabel[3].text = itemCount.ToString();//指定物品的总数量 --lisong

                            if (m_bpSelectItemIndex>1&&m_bpSelectItemIndex<9)
							{
								
                                for (int i = 0; i < m_bpItemBtn.Length; i++)
                                    m_bpItemBtn[i].SetActive(true);

                                m_bpItemBtn[3].SetActive(false);                                        //关闭解除按钮

                                if (m_bpSelectItemIndex > 4) { //点击的道具未装备
                                    int _isEquiped = GameManager.Instance.GetItemEquipIndex();
                                    if (_isEquiped == m_bpSelectItemIndex - 4)
                                    {
                                        m_bpItemBtn[2].SetActive(false);                                        //关闭使用按钮
                                        m_bpItemBtn[3].SetActive(true);                                        //关闭解除按钮
                                    }

                                }
                            }

                            m_bpItemLabel[0].gameObject.SetActive(m_bpItemBtn[0].activeSelf);
                            m_bpItemLabel[3].gameObject.SetActive(m_bpItemBtn[0].activeSelf);

                            m_selectMyItemNumMax = itemCount;//指定拥有的该物品最大值

							m_bpSelectBox = hit.collider.gameObject;
						}
					}
				}
				break;
			case 3:
				break;
			case 4:																						//当前打开为任务界面
				foreach(GameObject _taskBox in m_taskBoxList)													//遍历所有任务
				{
					int _index = int.Parse(_taskBox.name);														//获取当前任务编号						
					if(GameManager.Instance.GetTaskIndexState(_index-1)==2)										//如果当前任务已完成
						_taskBox.GetComponent<UISprite>().spriteName = "pack_page4_taskBox2";					//改变任务框为完成状态
				}
				break;
			case 5:
				if(Input.GetMouseButtonDown(0))														//如果鼠标按下
				{
					Ray ray = UICamera.mainCamera.ScreenPointToRay (Input.mousePosition);			//获取点击位置
					RaycastHit hit;
					if (Physics.Raycast(ray, out hit)) 												//检测鼠标点击区域
                        {
                            if (hit.collider.gameObject.tag == "36jiBtn")								//按下36计按钮
							{
                            AudioManager.Instance.SoundPlay(Global.GetInstance().audioName_BackpageItemClick);

							if (m_achieveSelectBox != null) m_achieveSelectBox.SetActive (false);
							m_achieveSelectBox = hit.collider.transform.GetChild (1).gameObject; 
							m_achieveSelectBox.SetActive (true);

							int _index = int.Parse(hit.collider.gameObject.name);
							m_achieveText[0].text = m_achieveXnl [_index-1].ChildNodes [1].InnerText;
							m_achieveText[1].text = m_achieveXnl [_index-1].ChildNodes [2].InnerText;
							}
							else if(hit.collider.gameObject.tag == "36jiBtnR")								//按下36计按钮
                            {
                            AudioManager.Instance.SoundPlay(Global.GetInstance().audioName_BackpageItemClick);

							if (m_achieveSelectBox != null) m_achieveSelectBox.SetActive (false);
							m_achieveSelectBox = hit.collider.transform.GetChild (1).gameObject; 
							m_achieveSelectBox.SetActive (true);
								
							int _index = int.Parse(hit.collider.gameObject.name);
								m_achieveText[0].text = m_achieveXnl [_index-1].ChildNodes [1].InnerText;
								m_achieveText[1].text = m_achieveXnl [_index-1].ChildNodes [2].InnerText;
							}
						}
				}
				break;
			case 6:
				if(m_bpMakerName.transform.position.y<=m_bpMakerPos[1].position.y)			//如果制作人员名单未达到最高点
					m_bpMakerName.transform.Translate(0f, Time.deltaTime/4f, 0f);
				else 																		//如果制作人员名单达到最高点
					m_bpMakerName.transform.position = m_bpMakerPos[2].position;
				break;
			}
		}
	}

	void OnBackPackItemBtnClick(GameObject _backPackBtn)													//物品界面
	{
		SetPageBtnActive (2);
		for(int i=0; i<4; i++)																			
		{
			m_bpHeroMask[i].SetActive(false);															//隐藏背包中试带的道具
			m_backpackItemSprite[i+4].GetComponent<UISprite>().spriteName = "pack_itemBox_normal";		//更改背包框
		}
		int _equipIndex = GameManager.Instance.GetItemEquipIndex();
        if (_equipIndex != 0)                                                                               //如果当前存在装备的道具
        {
            m_bpHeroMask[_equipIndex - 1].SetActive(true);                                              //更改背包中当前的道具
            m_backpackItemSprite[_equipIndex + 3].GetComponent<UISprite>().spriteName = "pack_itemBox_equiped";       //更改物品框
        }


        SetDetailItemInfoInvisible(false, false);//


        for (int i=0; i<m_backpackItemSprite.Length; i++)												//遍历所有物品栏											
		{
			if(GameManager.Instance.GetItemNum(i)!=0)													//该物品数量不为0										
			{
				m_backpackItemSprite[i].tag = "ItemBox";												//改变背包中该方块标签
				m_backpackItemSprite[i].transform.GetChild(0).gameObject.SetActive(true);				//开启该物品图标
			}
			else 																						//该物品数量为0
			{
				m_backpackItemSprite[i].tag = "Untagged";												//改变背包中该方块标签
				m_backpackItemSprite[i].transform.GetChild(0).gameObject.SetActive(false);				//隐藏该物品图标
			}
		}
	}
/*
void OnBackPackTeamBtnClick(GameObject _backPackBtn)                                                    //团队界面        
	{
		return;
		SetPageBtnActive (3);
	}
*/
    void OnBackPackTaskBtnClick(GameObject _backPackBtn)                                                    //任务页面
	{
		SetPageBtnActive (4);
		/*for(int i=m_taskBoxList.Count; i<7; i++)														//从已接受的任务向后遍历所有任务
		{
			int _taskIndex = m_taskBoxList.Count;
			if(GameManager.Instance.GetTaskIndexState(i)!=0)											//如果存在接受的任务
			{
				GameObject _taskBox = NGUITools.AddChild (m_taskBoxPanel, m_taskBox);					//背包中新增任务栏
				_taskBox.GetComponent<UISprite>().name = (_taskIndex+1).ToString();						//指定新增的任务栏名称为任务编号
				Vector3 _taskBoxPos = new Vector3 (0f, 299f-115f*_taskIndex, 0f);						//计算新增的任务栏位置
				_taskBox.transform.localPosition = _taskBoxPos;											//指定新增任务栏位置
				GameObject _taskTitle = _taskBox.transform.GetChild (0).gameObject;						//新增的任务栏标题
				_taskTitle.GetComponent<UILabel> ().text = m_taskXnl [_taskIndex].ChildNodes [0].InnerText;	//指定标题
				m_taskBoxList.Add (_taskBox);															//添加到背包任务栏列表
				UIEventListener.Get (_taskBox.gameObject).onClick = OnTaskBoxClick;						//注册背包中任务栏点击事件
			}
		}*/
	}


    void OnBackPackAchieveBtnClick(GameObject _backPackBtn)                                                // 成就页面
	{
		SetPageBtnActive (5);
        int count = 0;
		for(int i=0; i<m_backpackAchieveSprite.Length; i++)												//遍历所有成就栏											
		{
			if(GameManager.Instance.GetAchieveGot(i) != 0)													//当前获得该成就								
			{
                count++;
                if (i<=15)																				//属于左边
					m_backpackAchieveSprite[i].tag = "36jiBtn";				
				else
					m_backpackAchieveSprite[i].tag = "36jiBtnR";                                        //属于右边
                m_backpackAchieveSprite[i].transform.GetChild(0).GetComponent<UISprite>().color = Color.white ;			//开启该成就图标
			}
			else 																						//该物品数量为0
			{
				m_backpackAchieveSprite[i].tag = "Untagged";											//改变背包中该成就方块标签
				m_backpackAchieveSprite[i].transform.GetChild(0).GetComponent<UISprite>().color = Color.gray * 0.5f;          //隐藏该成就图标
            }
		}
        
        //控制“三十六计”标题的显示
        if (GameManager.Instance.GetItemNum(15) == 1)
        {
            achieveTitleObj.SetActive(true);
            jiHaveCountUILabel.gameObject.SetActive(true);
            jiHaveCountUILabel.text = count.ToString() + "/24";
        }
        else {
            jiHaveCountUILabel.gameObject.SetActive(false);
            achieveTitleObj.SetActive(false);

        }

    }

	void OnBackPackSaveDataBtnClick(GameObject _backPackBtn)                                                //存档界面
	{
		SetPageBtnActive (6);
		m_bpMakerName.transform.position = m_bpMakerPos [0].position;									//恢复制作人员名单位置
		for(int i=0; i<m_bpSaveBox.Length; i++)															//遍历3个存档体
		{
			if(GameManager.Instance.GetSaveBoxUsed(i))													//如果该存档点已使用
			{
				m_bpSaveBox [i].GetComponent<BoxCollider> ().enabled = true;                            //可以读档
                m_bpSaveDeleteBtn[i].SetActive(true);						//可以删除存档
				m_saveTimeLabel [i].text = GameManager.Instance.GetSaveDataTime(i).ToString();			//显示当前存档时系统时间
				m_saveSceneNameLabel[i].text = GameManager.Instance.GetSaveSceneName(i);												//显示当前场景名称
				string _picName = "saveNPC" + GameManager.Instance.GetSaveNPCNum(i).ToString();
				m_saveNPCPic[i].spriteName = _picName;													//显示当前最后遇到的NPC图片
				m_saveHeroPic[i].SetActive(true);														//显示主角图
                m_saveDesLabelObj[i].SetActive(false);

            }
			else 
			{
				m_bpSaveBox [i].GetComponent<BoxCollider> ().enabled = false;                           //不能读档
                m_bpSaveDeleteBtn[i].SetActive(false);                       //不能删除存档
                m_saveTimeLabel [i].text = " ";															//显示当前存档时系统时间
				m_saveSceneNameLabel[i].text = " ";														//显示当前场景名称
				m_saveNPCPic[i].spriteName = " ";														//没有最后对话的NPC
				m_saveHeroPic[i].SetActive(false);														//隐藏主角图
                m_saveDesLabelObj[i].SetActive(true);

            }
        }
	}

    void OnBackPackMainBtnClick(GameObject _backPackBtn)									//按下返回首页按钮
	{
		SetPageBtnActive (1);
	}

	void SetPageBtnActive(int _indexNew)												//设置按钮的激活形式
	{
        AudioManager.Instance.SoundPlay(Global.GetInstance().audioName_BpPageDown);

        m_backPackPage[m_openPageCurrIndex].SetActive (false);							//关闭已显示的界面
		if(m_openPageCurrIndex!=_indexNew)												//如果在背包中翻页
		{
			m_bpSelectItemIndex = 0;													//选中物体的编号归0
			if(m_bpItemSelectBox != null) m_bpItemSelectBox.SetActive(false);											//关闭物品选中框
			m_bpItemLabel[0].text = " ";												//指定物品数量复位
			m_bpItemLabel[1].text = " ";												//指定物品名称复位
			m_bpItemLabel[2].text = " ";												//指定物品介绍复位
			m_taskBoxSelect.SetActive(false);											//关闭任务选中框
			if(m_achieveSelectBox != null) m_achieveSelectBox.SetActive(false);										//关闭成就选中框
			m_taskContentBuble.text = "";												//任务介绍文字归零
			m_achieveText[0].text = "";												//成就介绍文字归零
			m_achieveText[1].text = "";
		}
		m_openPageCurrIndex = _indexNew;										
		m_backPackPage[0].GetComponent<UISprite>().spriteName = "page" + m_openPageCurrIndex.ToString();
		m_backPackPage [m_openPageCurrIndex].SetActive (true);							//改变显示的页面
		m_pageVirtualBtn [9].GetComponent<UISprite> ().depth = 7;
		m_bookMark.depth = 6;
		m_bookMarkMoney.depth = 5;
		GameManager.Instance.SetBpIndex(m_openPageCurrIndex);							//翻页时指定当前背包打开的页数
		switch(m_openPageCurrIndex)														//不同页数按钮激活状态
		{
		case 1:																			//需开启第一页
			m_pageVirtualBtn [9].GetComponent<UISprite> ().depth = 10;
			m_bookMark.depth = 9;
			m_bookMarkMoney.depth = 10;
			m_bookMarkMoney.text = GameManager.Instance.GetCurrMoney().ToString();		//指定当前拥有的钱数
			m_lifeObj.fillAmount = GameManager.Instance.GetBloodNum();					//指定当前血量情况跟
			m_bpMusicSlider [0].value = GameManager.Instance.GetVolume (0);				//指定当前音量情况
			m_bpMusicSlider [1].value = GameManager.Instance.GetVolume (1);
			for(int i=0; i<5; i++)
				m_pageVirtualBtn[i].SetActive(false);
			for(int i=5; i<9; i++)
				m_pageVirtualBtn[i].SetActive(true);
			break;


            case 2: //需开启物品界面
                for (int i = 0; i < m_pageVirtualBtn.Length - 1; i++)
                {
                    if (m_pageVirtualBtn[i] == null) return;

                    if (i > 0 && i <= 5)
                        m_pageVirtualBtn[i].SetActive(false);
                    else
                        m_pageVirtualBtn[i].SetActive(true);


                }

                break;
            case 3:     // --lisong
                        /*return;																	//需开启队伍界面 
                        m_pageVirtualBtn[0].SetActive(true);
                        for(int i=1; i<6; i++)
                            m_pageVirtualBtn[i].SetActive(false);
                        for(int i=6; i<9; i++)
                            m_pageVirtualBtn[i].SetActive(true);
                        */
                break;
            case 4:																			//需开启任务界面

                for (int i = 0; i < m_pageVirtualBtn.Length - 1; i++)
                {
                    if (i > 1 && i < 7)
                        m_pageVirtualBtn[i].SetActive(false);
                    else
                        m_pageVirtualBtn[i].SetActive(true);
                }

                break;
            case 5:                                                                         //需开启成就界面
                for (int i = 0; i < m_pageVirtualBtn.Length - 1; i++)
                {
                    if (i > 2 && i < 8)
                        m_pageVirtualBtn[i].SetActive(false);
                    else
                        m_pageVirtualBtn[i].SetActive(true);

                }
                break;
            case 6:                                                                         //需开启存档界面
                for (int i = 0; i < m_pageVirtualBtn.Length - 1; i++)
                {
                    if (i > 3 && i < 9)
                        m_pageVirtualBtn[i].SetActive(false);
                    else
                        m_pageVirtualBtn[i].SetActive(true);
                }
                break;
        }
	}
}
