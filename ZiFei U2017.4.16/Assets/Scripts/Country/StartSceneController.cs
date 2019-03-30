using UnityEngine;
using System.Collections;
using System;

public class StartSceneController : MonoBehaviour 
{
	public GameObject m_startScenePanel;											//开始界面面板
	public GameObject m_sceneUIPanel;                                            //游戏界面面板
	public GameObject m_mainPanel;													//主界面面板
    public GameObject[] m_startSceneSparrow;										//开始界面的麻雀们
	public GameObject[] m_mainBtn;													//主界面三个按钮


	public GameObject m_settingScenePanel;											//设置界面面板
	public GameObject[] m_settingSceneBtn;											//设置界面按钮
	public UISlider[] m_settingMusicSlider;											//设置界面两个音乐相关滑动条
	public GameObject[] m_settingMusicBtn;											//设置界面两个音乐相关图标	

	public GameObject m_loadScenePanel;												//读档界面面板
	public GameObject[] m_loadSceneBtn;												//读档界面按钮
	public GameObject[] m_loadSaveBox;												//读档页面存档条
	public GameObject[] m_loadSaveDeleteBtn;										//读档页面存档栏的删除按钮
	public UILabel[] m_loadSaveTimeLabel;											//读档时间点 
	public UILabel[] m_loadaveSceneNameLabel;										//读档场景名
	public UISprite[] m_saveNPCPic;													//存档最后对话NPC图
	public GameObject[] m_saveHeroPic;												//存档界面的主角图
	public GameObject[] m_saveDesLabel;                                              //存档界面描述信息

    public GameObject m_loadaveInfoPanel;											//读档信息提示面板
	public UILabel m_loadaveInfoLabel;												//读档信息提示文字
	public GameObject m_loadaveInfoSureBtn;											//读档信息提示面板确定按钮
	public GameObject m_loadaveInfoCancelBtn;											//读档信息提示面板确定按钮
	public GameObject m_loadaveInfoOKBtn;											//读档信息提示面板确定按钮
	public Transform[] m_panelTransPos;												//面板移动的起始和终止位置

    private float[] m_sparrowSpeedX;												//麻雀飞行水平速度
	private float[] m_sparrowSpeedY;												//麻雀飞行竖直速度
	private bool m_loadPanelState = false;											//关闭状态
	private TweenTransform m_transAni;
    private int currentNeedDeletedataIndex;// 保存当前需要删除档位的index

    private bool isSliderChangeValue = false;

    void Awake()
	{
		UIEventListener.Get (m_mainBtn[3]).onClick = OnContinueBtnClick;             //单击继续按钮
		UIEventListener.Get (m_mainBtn[0]).onClick = OnStartBtnClick;				//单击开始游戏按钮
		UIEventListener.Get (m_mainBtn[1]).onClick = OnSettingBtnClick;				//单击设置按钮
		UIEventListener.Get (m_mainBtn[2]).onClick = OnLoadingBtnClick;				//单击读档按钮

        UIEventListener.Get (m_settingSceneBtn[0]).onClick = OnSettingReturnClick;	//单击设置界面返回按钮
		UIEventListener.Get (m_settingMusicBtn[0]).onClick = OnSettingMusicClick;	//单击设置界面音乐按钮
		UIEventListener.Get (m_settingMusicBtn[1]).onClick = OnSettingSoundClick;	//单击设置界面音效按钮

		UIEventListener.Get (m_loadSceneBtn[0]).onClick = OnLoadReturnClick;		//单击读档界面返回按钮

		UIEventListener.Get (m_loadSaveBox[0].gameObject).onClick = OnLoadSaveOneClick;						//点击背包系统存档点1
		UIEventListener.Get (m_loadSaveBox[1].gameObject).onClick = OnLoadSaveTwoClick;						//点击背包系统存档点2
		UIEventListener.Get (m_loadSaveBox[2].gameObject).onClick = OnLoadSaveThreeClick;					//点击背包系统存档点3
		UIEventListener.Get (m_loadSaveDeleteBtn[0].gameObject).onClick = OnLoadSaveOneDeleteClick;			//点击背包系统存档点1删除按钮
		UIEventListener.Get (m_loadSaveDeleteBtn[1].gameObject).onClick = OnLoadSaveTwoDeleteClick;			//点击背包系统存档点2删除按钮
		UIEventListener.Get (m_loadSaveDeleteBtn[2].gameObject).onClick = OnLoadSaveThreeDeleteClick;       //点击背包系统存档点3删除按钮

        currentNeedDeletedataIndex = -1;

    }
    private void OnEnable()
    {
        m_mainBtn[3].SetActive(!GameDataManager.Instance.gameData.GameCurrentData.newHeroState);
    }


    void Start()
    {
		ShowStartPanel();
    }

    public void ShowStartPanel()
    {
        m_mainBtn[3].SetActive(!GameDataManager.Instance.gameData.GameCurrentData.newHeroState);

        m_startScenePanel.SetActive(true);                                          //开启开始界面面板
        m_mainPanel.SetActive(true);                                                //开启主界面面板
        m_sparrowSpeedX = new float[3];                                             //建立麻雀的飞行速度数组
        m_sparrowSpeedY = new float[3];
        for (int i = 0; i < m_sparrowSpeedX.Length; i++)                                    //遍历所有麻雀
        {
            m_sparrowSpeedX[i] = UnityEngine.Random.Range(0.03f, 0.05f);                        //随机水平速度
            m_sparrowSpeedX[i] = -m_sparrowSpeedX[i];                               //水平速度反向
            m_sparrowSpeedY[i] = UnityEngine.Random.Range(0.03f, 0.05f);                        //随机竖直速度
        }
        m_sparrowSpeedX[0] = -m_sparrowSpeedX[0];                                   //保证已反向的麻雀速度正确
    }



    void Update()
	{
		if(GameManager.Instance.GetGameStartState()==0)								//当前处于开始界面
		{
			for(int i=0; i<m_startSceneSparrow.Length; i++)							//遍历所有麻雀
			{
				m_startSceneSparrow[i].SetActive(true);								//显示麻雀
				if(m_startSceneSparrow[i].transform.position.x<29f||m_startSceneSparrow[i].transform.position.x>38f) //超出水平边界
				{
					m_startSceneSparrow[i].transform.localScale 					//水平反向麻雀
						= new Vector3(-m_startSceneSparrow[i].transform.localScale.x, m_startSceneSparrow[i].transform.localScale.y, m_startSceneSparrow[i].transform.localScale.z);
					m_sparrowSpeedX[i] = UnityEngine.Random.Range(0.03f, 0.05f);				//更新水平速度
					if(m_startSceneSparrow[i].transform.localScale.x>0)				//如果麻雀向左飞
						m_sparrowSpeedX[i] = -m_sparrowSpeedX[i];					//反向水平速度
				}
				if(m_startSceneSparrow[i].transform.position.y>1f)					//如果麻雀飞出上边界
				{
					m_sparrowSpeedY[i] = UnityEngine.Random.Range(0.03f, 0.05f);				//更新竖直速度
					m_sparrowSpeedY[i] = -m_sparrowSpeedY[i];						//麻雀向下飞
				}
				else if(m_startSceneSparrow[i].transform.position.y<-12f)			//如果麻雀飞出下边界
					m_sparrowSpeedY[i] = UnityEngine.Random.Range(0.03f, 0.05f);				//更新竖直速度
				m_startSceneSparrow[i].transform.Translate(m_sparrowSpeedX[i], m_sparrowSpeedY[i], 0f);		//麻雀一直在飞
			}
		}

		if(m_loadPanelState)							
		{
			GameManager.Instance.CloseUsualBtn();
			if(!m_loadaveInfoPanel.activeSelf)												//按下了确定按钮
			{
				for(int i=0; i<m_loadSaveBox.Length; i++)									//遍历三个档位
				{
					if(GameManager.Instance.GetSaveBoxUsed(i))								//如果该存档点已使用
					{
						m_loadSaveBox [i].GetComponent<BoxCollider> ().enabled = true;		//禁用读档
						m_loadSaveDeleteBtn [i].SetActive(true);//禁用删除存档
						
					}
				}
				m_loadPanelState = false;
			}
		}


		if(isSliderChangeValue && m_settingScenePanel.activeSelf)							//设置界面打开
		{
			if(m_settingMusicSlider[0].value<=0.01f)										//如果滑条过小
			{
				m_settingMusicSlider[0].value=0.01f;										//给定滑条值
				m_settingMusicBtn[0].GetComponent<UISprite>().spriteName = "option_btn_off";//关闭音量
			}
			else 																			//否则
				m_settingMusicBtn[0].GetComponent<UISprite>().spriteName = "option_btn_on";	//开启音乐

			if(m_settingMusicSlider[1].value<=0.01f)
			{
				m_settingMusicSlider[1].value=0.01f;
				m_settingMusicBtn[1].GetComponent<UISprite>().spriteName = "option_btn_off";
			}
			else
				m_settingMusicBtn[1].GetComponent<UISprite>().spriteName = "option_btn_on";


			GameManager.Instance.SetVolume(0, m_settingMusicSlider[0].value);
			GameManager.Instance.SetVolume(1, m_settingMusicSlider[1].value);
            isSliderChangeValue = false;
        }
	}

    public void SliderChangeValue() {
        isSliderChangeValue = true;
    }

	void OnSettingMusicClick(GameObject _musicBtn)											//按下音乐按钮
	{
        AudioManager.Instance.SoundPlay(Global.GetInstance().audioName_BtnClick2);

        if (m_settingMusicSlider[0].value>0.01f)
		{
			GameManager.Instance.SetVolume(0, 0f);
			m_settingMusicBtn[0].GetComponent<UISprite>().spriteName = "option_btn_off";	//关闭音乐
			m_settingMusicSlider[0].value=0.01f;											//给定音乐滑动条值
		}
		else
		{
			GameManager.Instance.SetVolume(0, 1f);
			m_settingMusicBtn[0].GetComponent<UISprite>().spriteName = "option_btn_on";		//打开音乐
			m_settingMusicSlider[0].value=1f;												//给定音乐滑动条值
		}
	}

	void OnSettingSoundClick(GameObject _musicBtn)											//按下音效按钮
	{
        AudioManager.Instance.SoundPlay(Global.GetInstance().audioName_BtnClick2);

        if (m_settingMusicSlider[1].value>0.01f)
		{
			GameManager.Instance.SetVolume(1, 0f);
			m_settingMusicBtn[1].GetComponent<UISprite>().spriteName = "option_btn_off";	//关闭音乐
			m_settingMusicSlider[1].value=0.01f;											//给定音乐滑动条值
		}
		else
		{
			GameManager.Instance.SetVolume(1, 1f);
			m_settingMusicBtn[1].GetComponent<UISprite>().spriteName = "option_btn_on";		//打开音乐
			m_settingMusicSlider[1].value=1f;												//给定音乐滑动条值
		}
	}

	void OnStartBtnClick(GameObject _startBtn)										//单击开始游戏按钮
	{
        AudioManager.Instance.SoundPlay(Global.GetInstance().audioName_BtnClick2);

        GameManager.Instance.InitData ();											//初始化数据 新游戏
        GameManager.Instance.m_gameManagerHero.SetActive(true);
		GameManager.Instance.SetGameStartState (1);                                 //按下开始游戏变量

        m_mainPanel.SetActive (false);												//关闭主界面面板
		m_startScenePanel.SetActive (false);										//关闭开始界面面板
		for(int i=0; i<m_startSceneSparrow.Length; i++)								//遍历所有麻雀
			m_startSceneSparrow[i].SetActive(false);                                //隐藏麻雀

        GameManager.Instance.isUsualBtnCanClick = false;

    }


    private void OnContinueBtnClick(GameObject go)
    {
        AudioManager.Instance.SoundPlay(Global.GetInstance().audioName_BtnClick2);

        m_mainPanel.SetActive(false);                                               //关闭主界面面板
        m_startScenePanel.SetActive(false);                                     //关闭开始界面面板
        for (int i = 0; i < m_startSceneSparrow.Length; i++)                                //遍历所有麻雀
            m_startSceneSparrow[i].SetActive(false);								//隐藏麻雀

        m_sceneUIPanel.SetActive(true);
        GameManager.Instance.GetSaveData(3);									//读取当前的存档数据
        GameManager.Instance.m_gameManagerHero.SetActive(true);
        GameManager.Instance.SetGameStartState(3);                                 //相机跟随主角
        GameManager.Instance.OpenUsualBtn();                                                            //开启常用按钮

    }

    void OnSettingBtnClick(GameObject _settingBtn)									//单击设置按钮
	{
        AudioManager.Instance.SoundPlay(Global.GetInstance().audioName_BtnClick);
        m_settingMusicSlider[0].value = GameManager.Instance.GetVolume(0);
        m_settingMusicSlider[1].value = GameManager.Instance.GetVolume(1);
        m_mainPanel.SetActive (false);												//关闭主界面面板
		m_settingScenePanel.SetActive (true);										//开启设置界面面板
		m_transAni = m_settingScenePanel.AddComponent<TweenTransform> ();			//给设置面板开启移动组件
		m_transAni.from = m_panelTransPos [0];										//移动的初始位置
		m_transAni.to = m_panelTransPos [1];										//移动的终止位置
		m_transAni.duration = 0.5f;													//移动时长
	}


	void OnLoadingBtnClick(GameObject _loadingBtn)									//单击读档按钮
	{
        AudioManager.Instance.SoundPlay(Global.GetInstance().audioName_BtnClick);

        m_mainPanel.SetActive (false);												//关闭主界面面板
		m_loadScenePanel.SetActive (true);											//开启读档界面面板
		m_transAni = m_loadScenePanel.AddComponent<TweenTransform> ();				//给设置界面添加移动组件
		m_transAni.from = m_panelTransPos [0];										//移动的初始位置
		m_transAni.to = m_panelTransPos [1];										//移动的终止位置
		m_transAni.duration = 0.5f;													//移动时长
		for(int i=0; i<m_loadSaveBox.Length; i++)															//遍历3个存档体
		{
			if(GameManager.Instance.GetSaveBoxUsed(i))													//如果该存档点已使用
			{
				m_loadSaveBox [i].GetComponent<BoxCollider> ().enabled = true;                          //可以读档

                m_loadSaveDeleteBtn[i].SetActive(true);

                m_loadSaveTimeLabel[i].text = GameManager.Instance.GetSaveDataTime(i).ToString();			//显示当前存档时系统时间
				m_loadaveSceneNameLabel[i].text = GameManager.Instance.GetSaveSceneName(i);												//显示当前场景名称
				string _picName = "saveNPC" + GameManager.Instance.GetSaveNPCNum(i).ToString();
				m_saveNPCPic[i].spriteName = _picName;													//显示当前最后遇到的NPC图片
				m_saveHeroPic[i].SetActive(true);														//显示主角图
                m_saveDesLabel[i].SetActive(false);


            }
			else 
			{
				m_loadSaveBox [i].GetComponent<BoxCollider> ().enabled = false;                         //不能读档
                m_loadSaveDeleteBtn[i].SetActive(false);                      //不能删除存档
                m_loadSaveTimeLabel [i].text = " ";															//显示当前存档时系统时间
				m_loadaveSceneNameLabel[i].text = " ";														//显示当前场景名称
				m_saveNPCPic[i].spriteName = " ";														//没有最后对话的NPC
				m_saveHeroPic[i].SetActive(false);														//隐藏主角图
                m_saveDesLabel[i].SetActive(true);

            }
        }
	}
	
	void OnSettingReturnClick(GameObject _settingReturn)							//单击设置界面的返回按钮
	{
        AudioManager.Instance.SoundPlay(Global.GetInstance().audioName_BtnClick3);

        m_settingScenePanel.SetActive (false);										//关闭设置界面面板
		m_mainPanel.SetActive (true);												//开启主界面面板
		Destroy (m_settingScenePanel.GetComponent<TweenTransform>());				//移除设置面板的移动组件
	}

	void OnLoadReturnClick(GameObject _loadReturn)									//单击读档界面的返回按钮
	{
        AudioManager.Instance.SoundPlay(Global.GetInstance().audioName_BtnClick3);
		m_loadScenePanel.SetActive (false);											//关闭读档界面面板
		m_mainPanel.SetActive (true);												//开启主界面面板
		Destroy (m_loadScenePanel.GetComponent<TweenTransform>());					//移除读档面板的移动组件
	}

	void SaveGetSaveData(int _index)												//点击了档位按钮
	{
        AudioManager.Instance.SoundPlay(Global.GetInstance().audioName_BtnClick2);
        for (int i = 0; i < m_startSceneSparrow.Length; i++)                            //遍历所有麻雀
            m_startSceneSparrow[i].SetActive(false);                             //hide all 麻雀

            GameManager.Instance.GetSaveData (_index);                                  //读取存档点
        GameManager.Instance.m_gameManagerHero.SetActive(true);

        m_loadaveInfoLabel.text = "读档完毕，已读取档位" + (_index+1).ToString();         //读档提示条
        GameManager.Instance.cameraCtr.UiPanelHelp = false;//不显示帮助界面
        GameManager.Instance.SetGameStartState (4);                                 //相机跟随主角

        UIEventListener.Get(m_loadaveInfoOKBtn).onClick = OnCancelOrOKDeleteDataBtnClick;

        m_loadaveInfoSureBtn.SetActive(false);
        m_loadaveInfoCancelBtn.SetActive(false);
        m_loadaveInfoOKBtn.SetActive(true);

        m_loadaveInfoPanel.SetActive (true);										//开启存档结束提示面板
		m_loadScenePanel.SetActive (false);
		m_startScenePanel.SetActive (false);
	}

	
	public void OnLoadSaveOneClick(GameObject _btn)
	{
		SaveGetSaveData (0);
	}


    void OnLoadSaveTwoClick(GameObject _btn)
	{
		SaveGetSaveData (1);
	}
	void OnLoadSaveThreeClick(GameObject _btn)
	{
		SaveGetSaveData (2);
	}

    void OnLoadSaveOneDeleteClick(GameObject _btn)
	{
        ShowInfoPanel(0);
	}
	void OnLoadSaveTwoDeleteClick(GameObject _btn)
	{
        ShowInfoPanel(1);
	}
	void OnLoadSaveThreeDeleteClick(GameObject _btn)
	{
        ShowInfoPanel(2);
	}

    void ShowInfoPanel(int index) {
        AudioManager.Instance.SoundPlay(Global.GetInstance().audioName_BtnClick3);

        UIEventListener.Get(m_loadaveInfoSureBtn).onClick = OnSureDeleteDataBtnClick;       //
        UIEventListener.Get(m_loadaveInfoCancelBtn).onClick = OnCancelOrOKDeleteDataBtnClick;       //
        UIEventListener.Get(m_loadaveInfoOKBtn).onClick = OnCancelOrOKDeleteDataBtnClick;       //

        m_loadaveInfoLabel.text = "确认删除档位 " + (index + 1).ToString() +" 的数据存档？";                  //提示信息文字
        m_loadaveInfoSureBtn.SetActive(true);
        m_loadaveInfoCancelBtn.SetActive(true);
        m_loadaveInfoOKBtn.SetActive(false);
        m_loadaveInfoPanel.SetActive(true);                                     //开启删除结束提示面板
        currentNeedDeletedataIndex = index;
    }


    void OnSureDeleteDataBtnClick(GameObject go) {
        AudioManager.Instance.SoundPlay(Global.GetInstance().audioName_BtnClick3);

        if (currentNeedDeletedataIndex != -1)
        {
            DeleteSaveData(currentNeedDeletedataIndex);
        }
        currentNeedDeletedataIndex = -1;
    }


    void OnCancelOrOKDeleteDataBtnClick(GameObject go) {

        AudioManager.Instance.SoundPlay(Global.GetInstance().audioName_BtnClick3);

        m_loadaveInfoSureBtn.SetActive(true);
        m_loadaveInfoCancelBtn.SetActive(true);
        m_loadaveInfoOKBtn.SetActive(false);
        m_loadaveInfoPanel.SetActive(false);

        GameManager.Instance.OpenUsualBtn();                                                            //开启常用按钮
        
		for (int i = 0; i < m_loadSaveBox.Length; i++)                                  //遍历三个档位
		{
			if (GameManager.Instance.GetSaveBoxUsed(i))                             //如果该存档点已使用
			{
				m_loadSaveBox[i].GetComponent<BoxCollider>().enabled = true;       //打开读档按钮
				m_loadSaveDeleteBtn[i].GetComponent<BoxCollider>().enabled = true;//打开删除存档
			}
		}

		ClearInfoPanelBtnEvent();
    }

    void DeleteSaveData(int _index)                                             //点击了删档按钮											
    {
        m_loadPanelState = true;
        GameManager.Instance.DeleteSaveData(_index);                                //存档点信息清空
        m_loadSaveBox[_index].GetComponent<BoxCollider>().enabled = false;      //不能读档
        m_loadSaveDeleteBtn[_index].SetActive(false);

        m_loadSaveTimeLabel[_index].text = " ";                                 //当前存档时系统时间为空
        m_loadaveSceneNameLabel[_index].text = " ";                                 //设置当前场景名称为空

        m_loadaveInfoLabel.text = "已删除数据档位 " + (_index + 1).ToString();                  //提示信息文字
        m_saveDesLabel[_index].SetActive(true);
        m_loadaveInfoSureBtn.SetActive(false);
        m_loadaveInfoCancelBtn.SetActive(false);
        m_loadaveInfoOKBtn.SetActive(true);
        m_saveHeroPic[_index].SetActive(false);                                                     //隐藏主角图
        m_saveNPCPic[_index].spriteName = " ";														//没有最后对话的NPC

        UIEventListener.Get(m_loadaveInfoOKBtn).onClick = OnCancelOrOKDeleteDataBtnClick;

        m_loadaveInfoPanel.SetActive(true);                                     //开启删除结束提示面板
        for (int i = 0; i < m_loadSaveBox.Length; i++)                                  //遍历三个档位
        {
            if (GameManager.Instance.GetSaveBoxUsed(i))                             //如果该存档点已使用
            {
                m_loadSaveBox[i].GetComponent<BoxCollider>().enabled = false;       //禁用读档
				m_loadSaveDeleteBtn[i].GetComponent<BoxCollider>().enabled = false;//禁用删除存档
            }
        }
        m_loadPanelState = true;
    }

    void ClearInfoPanelBtnEvent() {

        UIEventListener.Get(m_loadaveInfoSureBtn).onClick = null;   //
        UIEventListener.Get(m_loadaveInfoCancelBtn).onClick = null;
        UIEventListener.Get(m_loadaveInfoOKBtn).onClick = null;

    }
}
