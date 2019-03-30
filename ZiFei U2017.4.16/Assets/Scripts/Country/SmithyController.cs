using UnityEngine;
using System.Collections;
using System.Xml;

public class SmithyController : MonoBehaviour 
{
	public GameObject[] m_smithyPanel;                  //铁匠铺面板（0铁匠铺/酒馆 1铁匠铺物品 2铁匠铺物品介绍 3玩家物品 4玩家物品介绍 5酒馆物品 6遮罩图）
	private GameObject m_SellSmithySelectBox;				//铁匠铺物品选中框 （卖的物品选中框）
	private GameObject m_MySmithySelectBox;				//铁匠铺物品选中框 （自己物品选中框）
	public UILabel[] m_smithySellIntroLabel;			//铁匠铺卖东西介绍界面涉及文字（0数量 1名字 2介绍内容）
	public GameObject[] m_smithySellIntroBtn;			//铁匠铺卖东西界面按钮（0关闭按钮 1增加数目 2减少数目 3购买）
	public UILabel[] m_smithyMyIntroLabel;				//铁匠铺自己东西介绍界面涉及文字（0数量 1名字 2介绍内容）
	public GameObject[] m_smithyMyIntroBtn;				//铁匠铺自己东西界面按钮（0关闭按钮 1增加数目 2减少数目 3卖出）
	public GameObject[] m_sminthySprite;				//铁匠铺自己物品方块
	public GameObject[] m_moneyChangeObj;				//钱数变化所涉及的物品（0当前钱数 1钱数变化的箭头 2变化后的钱数 3变化后钱数单位G）

    public GameObject wrenchObj;//扳手物品
    public GameObject falsebeardObj; //假胡须

    [HideInInspector]
	public int m_smithyState = 0;						//铁匠铺状态（0未打开 1两边均为物品栏 2右为商品展示栏 3左为自己物品展示栏）
    [HideInInspector]
	public int m_barState = 0;							//酒馆状态（0未打开 1两边均为物品栏 2右为商品展示栏 3左为自己物品展示栏）
	private XmlDocument m_smithSellXML;					//铁匠铺xml文件
	private XmlNodeList m_smithySellXnl ;				//读取文件的数组
	private int m_smithyBuyIndex = 0;					//点击的买东西物品栏编号
	private int m_selectMyItemNumMax = 0;				//当前选中的自己物品的数量
	private int m_selectItemMoney = 0;					//当前选中的物品钱数		

	void Awake()
	{
		UIEventListener.Get (m_smithySellIntroBtn[0].gameObject).onClick = OnCloseBuyBtnClick;	//点击买东西界面关闭按钮
		UIEventListener.Get (m_smithySellIntroBtn[1].gameObject).onClick = OnAddBuyBtnClick;	//点击买东西界面数量增加按钮
		UIEventListener.Get (m_smithySellIntroBtn[2].gameObject).onClick = OnMinusBuyBtnClick;	//点击买东西界面数量减少按钮
		UIEventListener.Get (m_smithySellIntroBtn[3].gameObject).onClick = OnBuyBtnClick;		//点击购买按钮

		UIEventListener.Get (m_smithyMyIntroBtn[0].gameObject).onClick = OnCloseMyBtnClick;		//点击卖东西界面关闭按钮
		UIEventListener.Get (m_smithyMyIntroBtn[1].gameObject).onClick = OnAddMyBtnClick;       //点击卖东西界面数量增加按钮
        UIEventListener.Get (m_smithyMyIntroBtn[2].gameObject).onClick = OnMinusMyBtnClick;     //点击卖东西界面数量减少按钮
        UIEventListener.Get (m_smithyMyIntroBtn[3].gameObject).onClick = OnMyBtnClick;			//点击卖出按钮
	}
	
	void OnCloseMyBtnClick(GameObject _closeMyBtn)									//点击关闭自己东西介绍界面按钮
	{
        AudioManager.Instance.SoundPlay(Global.GetInstance().audioName_BtnClick2);

        if (m_smithyState!=0)
		{
			m_smithyPanel [1].SetActive (true);										//开启卖家物品界面
			m_smithyPanel [3].SetActive (true);										//开启买家物品界面
			m_smithyPanel [4].SetActive (false);									//关闭自己东西介绍界面
			if(m_MySmithySelectBox != null) m_MySmithySelectBox.SetActive (false);								//关闭自己东西界面选框
			m_smithyState = 1;														//更改铁匠铺状态
		}
		else if(m_barState!=0)
		{
			m_smithyPanel [5].SetActive (true);										//开启酒馆卖家物品界面
			m_smithyPanel [3].SetActive (true);										//开启买家物品界面
			m_smithyPanel [4].SetActive (false);									//关闭自己东西介绍界面
			if(m_MySmithySelectBox != null) m_MySmithySelectBox.SetActive (false);								//关闭自己东西界面选框
			m_barState = 1;															//更改铁匠铺状态
		}
		m_selectMyItemNumMax = 0;													//所选物品数量最大值归零
		m_moneyChangeObj [1].SetActive (false);										//关闭箭头及计算出的钱数
		m_moneyChangeObj [2].SetActive (false);
		m_moneyChangeObj [3].SetActive (false);
	}
	void OnAddMyBtnClick(GameObject _addMyBtn)										//按下自己物品数量增加按钮
	{
        AudioManager.Instance.SoundPlay(Global.GetInstance().audioName_BtnClick3);

        int _currNum = int.Parse (m_smithyMyIntroLabel [0].text);					//获取当前数量值
		if(_currNum<m_selectMyItemNumMax)											//如果当前显示的数量少于最大值
		{
			m_smithyMyIntroLabel [0].text = (_currNum + 1).ToString ();				//数字可加1
			m_moneyChangeObj[2].GetComponent<UILabel>().text = (GameManager.Instance.GetCurrMoney()+m_selectItemMoney*(_currNum+1)).ToString();
		}
	}

	void OnMinusMyBtnClick(GameObject _minusMyBtn)									//按下数量减少按钮
	{
        AudioManager.Instance.SoundPlay(Global.GetInstance().audioName_BtnClick3);

        int _currNum = int.Parse (m_smithyMyIntroLabel [0].text);					//获取当前数量值
		if(_currNum>=2)																//如果数字大于等于2
		{
			m_smithyMyIntroLabel [0].text = (_currNum - 1).ToString ();				//数量可减少
			m_moneyChangeObj[2].GetComponent<UILabel>().text = (GameManager.Instance.GetCurrMoney()+m_selectItemMoney*(_currNum-1)).ToString();
		}
	}
	void OnMyBtnClick(GameObject _buyBtn)											//按下卖出按钮
	{
        AudioManager.Instance.SoundPlay(Global.GetInstance().audioName_BtnClick2);

        int _goodsNum = m_selectMyItemNumMax - int.Parse(m_smithyMyIntroLabel [0].text);				//减少相应数量	
		GameManager.Instance.SetItemNum (m_smithyBuyIndex - 1, _goodsNum);								//更改已拥有的该物品数量
		if(_goodsNum==0)																				//如果该物品已全部卖出
		{
			int _goodsID = int.Parse (m_smithySellXnl [m_smithyBuyIndex - 1].ChildNodes [0].InnerText);	//获取该物品的编号
			_goodsID -= 1;																				//标注方块索引
			m_sminthySprite[_goodsID].tag = "Untagged";													//改变该方块标签
			m_sminthySprite[_goodsID].transform.GetChild(0).gameObject.SetActive(false);				//隐藏该物品图标
		}
		if(m_smithyState!=0)														//当前为铁匠铺
		{
			m_smithyPanel [1].SetActive (true);										//开启卖家物品界面
			m_smithyPanel [3].SetActive (true);										//开启买家物品界面
			m_smithyPanel [4].SetActive (false);									//关闭自己东西介绍界面
			if(m_MySmithySelectBox != null) m_MySmithySelectBox.SetActive (false);								//关闭自己东西界面选框
			m_smithyState = 1;														//更改铁匠铺状态
		}
		else if(m_barState!=0)														//当前为酒馆
		{
			m_smithyPanel [5].SetActive (true);										//开启酒馆卖家物品界面
			m_smithyPanel [3].SetActive (true);										//开启买家物品界面
			m_smithyPanel [4].SetActive (false);									//关闭自己东西介绍界面
			if(m_MySmithySelectBox != null) m_MySmithySelectBox.SetActive (false);								//关闭自己东西界面选框
			m_barState = 1;															//更改铁匠铺状态
		}
		m_selectMyItemNumMax = 0;													//所选物品数量最大值归零
		m_smithyBuyIndex = 0;														//当前选中的商品名归零
		int _newMoney = int.Parse (m_moneyChangeObj [2].GetComponent<UILabel> ().text);
		GameManager.Instance.SetCurrMoney (_newMoney);							//指定当前全局钱数

		m_moneyChangeObj [0].GetComponent<UILabel> ().text = GameManager.Instance.GetCurrMoney().ToString();
		m_moneyChangeObj [1].SetActive (false);										//关闭箭头及计算出的钱数
		m_moneyChangeObj [2].SetActive (false);
		m_moneyChangeObj [3].SetActive (false);
	}

	void OnCloseBuyBtnClick(GameObject _closeBuyBtn)								//点击关闭买东西界面按钮
	{
        AudioManager.Instance.SoundPlay(Global.GetInstance().audioName_BtnClick2);
		if(m_smithyState!=0)														//如果是铁匠铺
		{
			m_smithyPanel [3].SetActive (true);										//开启自己物品界面
			m_smithyPanel [2].SetActive (false);									//关闭卖家介绍界面
			if(m_SellSmithySelectBox != null) m_SellSmithySelectBox.SetActive (false);								//关闭自己东西界面选框
			m_smithyState = 1;														//更改铁匠铺状态
			m_smithyBuyIndex = 0;													//当前点击的卖家物品索引归零
		}
		else if(m_barState!=0)														//如果是酒馆
		{
			m_smithyPanel [3].SetActive (true);										//开启自己物品界面
			m_smithyPanel [2].SetActive (false);									//关闭卖家介绍界面
			if(m_SellSmithySelectBox != null) m_SellSmithySelectBox.SetActive (false);								//关闭自己东西界面选框
			m_barState = 1;															//更改铁匠铺状态
			m_smithyBuyIndex = 0;													//当前点击的卖家物品索引归零
		}
		m_moneyChangeObj [1].SetActive (false);
		m_moneyChangeObj [2].SetActive (false);
		m_moneyChangeObj [3].SetActive (false);
	}

	void OnAddBuyBtnClick(GameObject _addBuyBtn)									//按下数量增加按钮
	{
        AudioManager.Instance.SoundPlay(Global.GetInstance().audioName_BtnClick3);
		int _currNum = int.Parse (m_smithySellIntroLabel [0].text);					//获取当前数量值
		m_smithySellIntroLabel [0].text = (_currNum + 1).ToString ();				//数字加1
        int newMoney = GameManager.Instance.GetCurrMoney() - m_selectItemMoney * (_currNum + 1);

        m_moneyChangeObj[2].GetComponent<UILabel>().text = newMoney.ToString();

        m_smithySellIntroBtn[3].gameObject.SetActive(newMoney >= 0);
        
	}
	void OnMinusBuyBtnClick(GameObject _minusBuyBtn)								//按下数量减少按钮
	{
        AudioManager.Instance.SoundPlay(Global.GetInstance().audioName_BtnClick3);

        int _currNum = int.Parse (m_smithySellIntroLabel [0].text);					//获取当前数量值
		if(_currNum>=2)																//如果数字大于等于2
		{
			m_smithySellIntroLabel [0].text = (_currNum - 1).ToString ();			//数量可减少
            var newMoney = GameManager.Instance.GetCurrMoney() - m_selectItemMoney * (_currNum - 1);

            m_moneyChangeObj[2].GetComponent<UILabel>().text = (newMoney).ToString();
            m_smithySellIntroBtn[3].gameObject.SetActive(newMoney >= 0);
		}


    }
    void OnBuyBtnClick(GameObject _buyBtn)											//按下购买按钮
	{

        int _newMoney = int.Parse (m_moneyChangeObj [2].GetComponent<UILabel> ().text);//更改当前钱数
		if(_newMoney>=0)
		{
            AudioManager.Instance.SoundPlay(Global.GetInstance().audioName_BtnClick2);

            int _goodsNum = GameManager.Instance.GetItemNum(m_smithyBuyIndex - 1);		//获取该物品当前已拥有数量
			int _goodsID = int.Parse (m_smithySellXnl [m_smithyBuyIndex - 1].ChildNodes [0].InnerText);	//获取该物品的编号
			_goodsID -= 1;																				//标注方块索引
			if(_goodsNum<=0)																			//如果未拥有该物品
			{
				m_sminthySprite[_goodsID].tag = "SmithMyGoods";											//改变该方块标签
				m_sminthySprite[_goodsID].transform.GetChild(0).gameObject.SetActive(true);				//开启该物品图标
				_goodsNum = 0;															//当前商品数量归0
			}
			_goodsNum += int.Parse(m_smithySellIntroLabel [0].text);					//增加相应数量		
			GameManager.Instance.SetItemNum (m_smithyBuyIndex - 1, _goodsNum);			//更改已拥有的该物品数量
			m_smithyPanel [3].SetActive (true);											//开启自己物品界面
			m_smithyPanel [2].SetActive (false);										//关闭卖家介绍界面
			if(m_SellSmithySelectBox != null) m_SellSmithySelectBox.SetActive (false);								//关闭自己东西界面选框
			m_smithyBuyIndex = 0;														//当前点击的卖家物品索引归零
			if(m_smithyState!=0)														//如果当前是铁匠铺
				m_smithyState = 1;														//更改铁匠铺状态
			else if(m_barState!=0)														//如果当前是酒馆
				m_barState = 1;															//更改酒馆状态
			int _originalValue = 1;														//买东西界面数量初始值
			m_smithySellIntroLabel[0].text =_originalValue.ToString();					//买东西界面物品数量归1
			
			
			GameManager.Instance.SetCurrMoney (_newMoney);								//指定当前全局钱数
			m_moneyChangeObj [0].GetComponent<UILabel> ().text = GameManager.Instance.GetCurrMoney().ToString();//更改当前钱数显示值
			m_moneyChangeObj [1].SetActive (false);										//关闭钱数变化箭头
			m_moneyChangeObj [2].SetActive (false);										//关闭计算出的钱数
			m_moneyChangeObj [3].SetActive (false);										//关闭计算出的钱数单位
		}

	}

	void InitMyItem()																				//初始化买卖界面
	{
		for(int i=0; i<4; i++)
		{
			if(GameManager.Instance.GetItemNum(i)!=0)												
			{
				m_sminthySprite[i].tag = "SmithMyGoods";											//改变该方块标签
				m_sminthySprite[i].transform.GetChild(0).gameObject.SetActive(true);				//开启该物品图标
			}
			else 
			{
				m_sminthySprite[i].tag = "Untagged";												//改变该方块标签
				m_sminthySprite[i].transform.GetChild(0).gameObject.SetActive(false);				//隐藏该物品图标
			}
		}
	}

	void Start()
	{
		string data = Resources.Load("Item").ToString(); 				//读取外部文件的相关数据
		m_smithSellXML = new XmlDocument ();								
		m_smithSellXML.LoadXml (data);
		m_smithySellXnl = m_smithSellXML.GetElementsByTagName("Item");
        
    }

	void OnEnable()
	{
		m_moneyChangeObj [1].SetActive (false);										//隐藏钱数变化的箭头
		m_moneyChangeObj [2].SetActive (false);										//隐藏变化后的钱数Label
		m_moneyChangeObj [3].SetActive (false);										//隐藏变化后的钱数LabelG
		InitMyItem ();

	}

	void Update()
	{
        m_moneyChangeObj[0].GetComponent<UILabel>().text = GameManager.Instance.GetCurrMoney().ToString();//当前钱数

        if (GameManager.Instance.GetSmithyState()&&(m_smithyState==0))							//进入铁匠铺				
		{

            // 判断是否接到书店老板的任务，来显示扳手
            if (GameManager.Instance.GetTaskIndexState(2) >= 1)
                wrenchObj.SetActive(true);
            else
                wrenchObj.SetActive(false);


            //判断是否接受到了酒保的任务，来显示胡子
            if (GameManager.Instance.GetTaskIndexState(6) >= 1)
                falsebeardObj.SetActive(true);
            else
                falsebeardObj.SetActive(false);

            for (int i=0; i<m_sminthySprite.Length; i++)											//遍历所有物品栏											
			{
				if(GameManager.Instance.GetItemNum(i)!=0)										//该物品数量不为0										
				{
					m_sminthySprite[i].tag = "SmithMyGoods";									//改变背包中该方块标签
					m_sminthySprite[i].transform.GetChild(0).gameObject.SetActive(true);		//开启该物品图标
				}
				else 																			//该物品数量为0
				{
					m_sminthySprite[i].tag = "Untagged";										//改变背包中该方块标签
					m_sminthySprite[i].transform.GetChild(0).gameObject.SetActive(false);		//隐藏该物品图标
				}
			}

			m_selectMyItemNumMax = 0;															//选中物品的最大值归零
			GameManager.Instance.CloseUsualBtn();												//关闭常用按钮
			m_smithyPanel[1].SetActive(true);													//显示卖家任务栏
			m_smithyPanel[3].SetActive(true);													//显示买家任务栏
			m_smithyPanel[0].gameObject.GetComponent<UISprite>().spriteName = "smithy_bkg";		//更换为铁匠铺背景图
			m_smithyPanel[0].SetActive(true);													//显示背包系统
			m_smithyPanel[6].SetActive(true);													//显示遮罩层
			m_smithyState = 1;																	//铁匠铺状态改变
		}

		if(GameManager.Instance.GetBarState()&&(m_barState==0))									//进入酒馆
		{
			for(int i=0; i<m_sminthySprite.Length; i++)											//遍历所有物品栏											
			{
				if(GameManager.Instance.GetItemNum(i)!=0)										//该物品数量不为0										
				{
					m_sminthySprite[i].tag = "SmithMyGoods";									//改变背包中该方块标签
					m_sminthySprite[i].transform.GetChild(0).gameObject.SetActive(true);		//开启该物品图标
				}
				else 																			//该物品数量为0
				{
					m_sminthySprite[i].tag = "Untagged";										//改变背包中该方块标签
					m_sminthySprite[i].transform.GetChild(0).gameObject.SetActive(false);		//隐藏该物品图标
				}
			}

			m_selectMyItemNumMax = 0;															//选中物品的最大值归零
			GameManager.Instance.CloseUsualBtn();												//关闭常用按钮
			m_smithyPanel[5].SetActive(true);													//显示卖家任务栏
			m_smithyPanel[3].SetActive(true);													//显示买家任务栏
			m_smithyPanel[0].gameObject.GetComponent<UISprite>().spriteName = "pub_bkg";		//更换为酒馆背景图
			m_smithyPanel[0].SetActive(true);													//显示背包系统
			m_smithyPanel[6].SetActive(true);													//显示遮罩层
			m_barState = 1;																		//酒馆状态改变
		}

        // 铁匠铺中的物品信息 
		if(m_smithyState!=0)																	//如果铁匠铺打开
		{
			if(Input.GetMouseButtonDown(0))														//如果鼠标按下
			{
				Ray ray = UICamera.mainCamera.ScreenPointToRay (Input.mousePosition);			//获取点击位置
				RaycastHit hit;
				if (Physics.Raycast(ray, out hit)) 												//检测鼠标点击区域
				{
					int _currMoney = GameManager.Instance.GetCurrMoney();
                   


                    if (hit.collider.gameObject.tag == "SmithSellGoods")							//按下商品栏中的物品
					{
                        AudioManager.Instance.SoundPlay(Global.GetInstance().audioName_BackpageItemClick);

                        m_smithyState = 2;														//改变铁匠铺状态							
						m_smithyPanel[2].SetActive(true);										//开启卖家物品介绍界面
						m_smithyPanel[3].SetActive(false);										//关闭买家物品栏界面
						if(m_SellSmithySelectBox != null) m_SellSmithySelectBox.SetActive (false);		//关闭自己东西界面选框
						m_SellSmithySelectBox = hit.collider.transform.GetChild(1).gameObject;
						m_SellSmithySelectBox.SetActive(true);									//开启卖家物品栏选框
						m_smithyBuyIndex = int.Parse(hit.collider.gameObject.name);             //通过名字获取点击的物品索引


                        m_moneyChangeObj[1].GetComponent<UISprite>().spriteName = "pub_account1";//红色箭头
						m_selectItemMoney = int.Parse(m_smithySellXnl [m_smithyBuyIndex-1].ChildNodes [5].InnerText);	//获取选中物品的钱数
						m_moneyChangeObj[2].GetComponent<UILabel>().text = (_currMoney-m_selectItemMoney).ToString();

                        m_smithySellIntroLabel[0].text = "1";
                        int newMoney = GameManager.Instance.GetCurrMoney() - m_selectItemMoney;
                        m_smithySellIntroBtn[3].gameObject.SetActive(newMoney >= 0);


                        //指定介绍界面物品名称 以及单价
                        m_smithySellIntroLabel[1].text = "[080300FF]" + m_smithySellXnl [m_smithyBuyIndex-1].ChildNodes [1].InnerText
                                                            + "  ([-][FBFCD4FF]" + m_selectItemMoney.ToString() + "[-]" 
                                                            + " [E6B24DFF]G[-]" + "[080300FF])[-]";

                        m_smithySellIntroLabel[2].text = m_smithySellXnl [m_smithyBuyIndex-1].ChildNodes [2].InnerText;//指定介绍界面物品内容

                        m_moneyChangeObj[1].SetActive(true);									//开启箭头
						m_moneyChangeObj[2].SetActive(true);									//开启变化后的钱数
						m_moneyChangeObj[3].SetActive(true);									//开启变化后的钱数单位G

                        m_smithySellIntroBtn[3].gameObject.SetActive((_currMoney - m_selectItemMoney ) >=0);
                        
					}


					else if(hit.collider.gameObject.tag == "SmithMyGoods")						//按下自己物品栏中的物品
					{
                        AudioManager.Instance.SoundPlay(Global.GetInstance().audioName_BackpageItemClick);

                        m_smithyState = 3;														//更改铁匠铺状态
						m_smithyPanel[4].SetActive(true);										//开启自己物品栏介绍界面
						m_smithyPanel[1].SetActive(false);										//关闭卖家物品栏
						if(m_MySmithySelectBox != null) m_MySmithySelectBox.SetActive(false);			//开启自己物品栏选框
						m_MySmithySelectBox = hit.collider.transform.GetChild(2).gameObject;
						m_MySmithySelectBox.SetActive(true);

						m_smithyBuyIndex = int.Parse(hit.collider.gameObject.name);				//通过名字获取点击的物品索引
						m_smithyMyIntroLabel[0].text = GameManager.Instance.GetItemNum(m_smithyBuyIndex-1).ToString();//指定物品数量
						m_selectMyItemNumMax = int.Parse(m_smithyMyIntroLabel[0].text );		//获取该物品目前拥有的数量
					
						m_moneyChangeObj[1].GetComponent<UISprite>().spriteName = "pub_account2";//绿色箭头箭头
						m_selectItemMoney =(int)(int.Parse(m_smithySellXnl [m_smithyBuyIndex-1].ChildNodes [5].InnerText) *0.6f);
						m_moneyChangeObj[2].GetComponent<UILabel>().text = (_currMoney+m_selectItemMoney*m_selectMyItemNumMax).ToString();
                        
                        //指定物品名称 以及物品的单价
                        m_smithyMyIntroLabel[1].text = "[080300FF]" + m_smithySellXnl [m_smithyBuyIndex-1].ChildNodes [1].InnerText
                                                       + "  ([-][FBFCD4FF]" + m_selectItemMoney.ToString() + "[-]" 
                                                       + " [E6B24DFF]G[-]" + "[080300FF])[-]";
                        m_smithyMyIntroLabel[2].text = m_smithySellXnl [m_smithyBuyIndex-1].ChildNodes [2].InnerText;//指定物品介绍


                        if (1 < m_smithyBuyIndex && m_smithyBuyIndex <= 8)
                        {
                            m_smithyMyIntroBtn[3].gameObject.SetActive(true);
                            m_smithyMyIntroLabel[1].text = "[080300FF]" + m_smithySellXnl[m_smithyBuyIndex - 1].ChildNodes[1].InnerText
                                                           + "  ([-][FBFCD4FF]" + m_selectItemMoney.ToString() + "[-]"
                                                           + " [E6B24DFF]G[-]" + "[080300FF])[-]";

						m_moneyChangeObj[1].SetActive(true);									//开启箭头
						m_moneyChangeObj[2].SetActive(true);									//开启变化后的钱数
						m_moneyChangeObj[3].SetActive(true);                                    //开启变化后的钱数单位G
                        }
                        else
                        {
                            m_moneyChangeObj[1].SetActive(false);                                    //close箭头
                            m_moneyChangeObj[2].SetActive(false);                                    //close变化后的钱数
                            m_moneyChangeObj[3].SetActive(false);                                    //close变化后的钱数单位G
                            m_smithyMyIntroBtn[3].gameObject.SetActive(false);
							m_smithyMyIntroLabel[1].text =  "[080300FF]" +m_smithySellXnl[m_smithyBuyIndex - 1].ChildNodes[1].InnerText+"[-]";
                        }

                    }


					else if(hit.collider.gameObject == m_smithyPanel[6])						//如果点击了后边遮罩图 关闭背包
					{
                        CloseSmithy();
                        GameManager.Instance.OpenUsualBtn();									//开启常用按钮
					}
				}
			}

		}

        if (m_barState!=0)																		//如果酒馆打开
		{
			if(Input.GetMouseButtonDown(0))														//如果鼠标按下
			{
				Ray ray = UICamera.mainCamera.ScreenPointToRay (Input.mousePosition);			//获取点击位置
				RaycastHit hit;
				if (Physics.Raycast(ray, out hit)) 												//检测鼠标点击区域
				{
					int _currMoney = GameManager.Instance.GetCurrMoney();
                    

                    if (hit.collider.gameObject.tag == "SmithSellGoods")							//按下商品栏中的物品
					{
                        AudioManager.Instance.SoundPlay(Global.GetInstance().audioName_BackpageItemClick);

                        m_barState = 2;															//改变酒馆状态							
						m_smithyPanel[2].SetActive(true);										//开启卖家物品介绍界面
						m_smithyPanel[3].SetActive(false);										//关闭买家物品栏界面

						if(m_SellSmithySelectBox != null) m_SellSmithySelectBox.SetActive(false);
						m_SellSmithySelectBox = hit.collider.transform.GetChild (1).gameObject;
						m_SellSmithySelectBox.SetActive(true);									//开启卖家物品栏选框

						m_smithyBuyIndex = int.Parse(hit.collider.gameObject.name);				//通过名字获取点击的物品索引
						m_selectItemMoney = int.Parse(m_smithySellXnl [m_smithyBuyIndex-1].ChildNodes [5].InnerText);

                        m_smithySellIntroLabel[0].text = "1";
                        int newMoney = GameManager.Instance.GetCurrMoney() - m_selectItemMoney;
                        m_smithySellIntroBtn[3].gameObject.SetActive(newMoney >= 0);

                        //指定介绍界面物品名称 and pricce
                        m_smithySellIntroLabel[1].text ="[080300FF]" + m_smithySellXnl[m_smithyBuyIndex - 1].ChildNodes[1].InnerText
                                                       + "  ([-][FBFCD4FF]" + m_selectItemMoney.ToString() + "[-]"
                                                       + " [E6B24DFF]G[-]" + "[080300FF])[-]";

                        m_smithySellIntroLabel[2].text = m_smithySellXnl [m_smithyBuyIndex-1].ChildNodes [2].InnerText;//指定介绍界面物品内容
					
						m_moneyChangeObj[1].GetComponent<UISprite>().spriteName = "pub_account1";//红色箭头
						m_moneyChangeObj[2].GetComponent<UILabel>().text = (_currMoney-m_selectItemMoney).ToString();
						m_moneyChangeObj[1].SetActive(true);									//开启箭头
						m_moneyChangeObj[2].SetActive(true);									//开启变化后的钱数
						m_moneyChangeObj[3].SetActive(true);                                    //开启变化后的钱数单位G

                        m_smithySellIntroBtn[3].gameObject.SetActive((_currMoney - m_selectItemMoney) >= 0);
                    }
                    else if(hit.collider.gameObject.tag == "SmithMyGoods")						//按下自己物品栏中的物品
					{
                        AudioManager.Instance.SoundPlay(Global.GetInstance().audioName_BackpageItemClick);

                        m_barState = 3;															//更改酒馆状态
						m_smithyPanel[4].SetActive(true);										//开启自己物品栏介绍界面
						m_smithyPanel[5].SetActive(false);										//关闭卖家物品栏
						if(m_MySmithySelectBox != null) m_MySmithySelectBox.SetActive(false);
						m_MySmithySelectBox = hit.collider.transform.GetChild (2).gameObject;
						m_MySmithySelectBox.SetActive(true);									//开启卖家物品栏选框

						m_smithyBuyIndex = int.Parse(hit.collider.gameObject.name);				//通过名字获取点击的物品索引
						m_smithyMyIntroLabel[0].text = GameManager.Instance.GetItemNum(m_smithyBuyIndex-1).ToString();//指定物品数量
						m_smithyMyIntroLabel[2].text = m_smithySellXnl [m_smithyBuyIndex-1].ChildNodes [2].InnerText;//指定物品介绍
						m_selectMyItemNumMax = int.Parse(m_smithyMyIntroLabel[0].text );		//获取该物品目前拥有的数量
					
						m_moneyChangeObj[1].GetComponent<UISprite>().spriteName = "pub_account2";//绿色箭头箭头
						m_selectItemMoney =(int)( int.Parse(m_smithySellXnl [m_smithyBuyIndex-1].ChildNodes [5].InnerText) *0.6f);


                        m_moneyChangeObj[2].GetComponent<UILabel>().text = (_currMoney+m_selectItemMoney*m_selectMyItemNumMax).ToString();


                        if (1 < m_smithyBuyIndex && m_smithyBuyIndex <= 8)
                        {
                            m_smithyMyIntroBtn[3].gameObject.SetActive(true);
						    m_smithyMyIntroLabel[1].text = "[080300FF]" + m_smithySellXnl[m_smithyBuyIndex - 1].ChildNodes[1].InnerText
                                                           + "  ([-][FBFCD4FF]" + m_selectItemMoney.ToString() + "[-]"
                                                           + " [E6B24DFF]G[-]" + "[080300FF])[-]";

						m_moneyChangeObj[1].SetActive(true);									//开启箭头
						m_moneyChangeObj[2].SetActive(true);									//开启变化后的钱数
						m_moneyChangeObj[3].SetActive(true);                                    //开启变化后的钱数单位
                        }
                        else {

                            m_moneyChangeObj[1].SetActive(false);                                    //close箭头
                            m_moneyChangeObj[2].SetActive(false);                                    //close变化后的钱数
                            m_moneyChangeObj[3].SetActive(false);                                    //close变化后的钱数单位G
                            m_smithyMyIntroBtn[3].gameObject.SetActive(false);
							m_smithyMyIntroLabel[1].text = "[080300FF]" + m_smithySellXnl[m_smithyBuyIndex - 1].ChildNodes[1].InnerText+"[-]";
                        }

                    }
					else if(hit.collider.gameObject == m_smithyPanel[6])						//如果点击了后边遮罩图 关闭背包
					{
                        CloseBar();
                        GameManager.Instance.OpenUsualBtn();									//开启常用按钮
					}
				}
			}
			
		}
	}


    public void CloseSmithy()
    {
        m_smithyState = 0;                                                      //更改铁匠铺状态为关闭
        GameManager.Instance.SetSmithyState(false);                             //改变铁匠铺状态
        for (int i = 0; i < m_smithyPanel.Length; i++)                              //关闭所有铁匠铺界面
            m_smithyPanel[i].SetActive(false);
		if (m_MySmithySelectBox != null) m_MySmithySelectBox.SetActive (false);
		if (m_SellSmithySelectBox != null) m_SellSmithySelectBox.SetActive (false);
		m_moneyChangeObj[1].SetActive(false);                               //关闭箭头
        m_moneyChangeObj[2].SetActive(false);                                   //关闭变化后的钱数
        m_moneyChangeObj[3].SetActive(false);									//关闭变化后的钱数单位
    }

    public void CloseBar()
    {
        m_barState = 0;                                                         //更改酒馆状态为关闭
        GameManager.Instance.SetBarState(false);                                //改变酒馆状态
        for (int i = 0; i < m_smithyPanel.Length; i++)                              //关闭所有酒馆界面
            m_smithyPanel[i].SetActive(false);

		if (m_MySmithySelectBox != null) m_MySmithySelectBox.SetActive (false);
		if (m_SellSmithySelectBox != null) m_SellSmithySelectBox.SetActive (false);
        m_moneyChangeObj[1].SetActive(false);                                   //关闭箭头
        m_moneyChangeObj[2].SetActive(false);                                   //关闭变化后的钱数
        m_moneyChangeObj[3].SetActive(false);									//关闭变化后的钱数单位
    }

}
