using UnityEngine;
using System.Collections;
using System.Xml;

public class DialogController : MonoBehaviour 
{
	private XmlDocument m_npcTalkXML;									//对话xml文件
	private XmlNodeList xnl ;											//读取文件的数组
	private XmlNodeList xnlCountryThing ;								//读取文件中村庄物品对话的数组
	
	public UILabel m_dialogLabel;										//对话框
	public GameObject m_dialogTail;										//对话框的下标
	public GameObject m_diaLogPoint;									//对话框下方指示物
	
	private float m_diaLogTimer = -1f;									//对话框显示时长
	private int m_dialogNPCNum = 0;										//对话的人物编号
	private string m_dialogText;										//对话内容
	private Vector3 m_thingDiaPos;										//物品对话的位置
	
	void Start()														
	{
		string data = Resources.Load("NPCTalk").ToString(); 							//读取对话相关数据
		m_npcTalkXML = new XmlDocument ();								
		m_npcTalkXML.LoadXml (data);
		xnl = m_npcTalkXML.GetElementsByTagName("countryDia");
		xnlCountryThing = m_npcTalkXML.GetElementsByTagName("countryThing");
	}

	void Update()
	{
		int _dialogState = CheckBtnController.Instance.GetDialogState ();                   //获取是否按下对话按钮

        if (_dialogState > 0)                                                                   //按下对话按钮
        {
            Vector3 _screenPos = Camera.main.WorldToScreenPoint(CheckBtnController.Instance.GetDialogPos());    //世界坐标转为屏幕坐标	
            Vector3 _uiScreenPos = UICamera.mainCamera.ScreenToWorldPoint(_screenPos);      //转为NGUI世界坐标
            _uiScreenPos.z = 0f;                                                            //NGUI坐标中去掉Z轴
            m_dialogLabel.gameObject.transform.position = _uiScreenPos;                     //指定对话框位置

            if (_dialogState == 1)                                                              //如果按下对话按钮需弹出对话框
            {
                AudioManager.Instance.SoundPlay(Global.GetInstance().audioName_Country_DialogSpeak);
                if (GameManager.Instance.GetTaskIndexState(3) == 1 && GameManager.Instance.GetTaskIndexState(7) == 10) //如果可以接受铁匠的支线任务
                {
                    m_dialogNPCNum = CheckBtnController.Instance.GetDialogIndex();
                    m_dialogText = xnl[m_dialogNPCNum].ChildNodes[4].InnerText;     //获取对话内容
                    GameManager.Instance.SetDialogText(m_dialogText);                           //指定对话内容（为打字机）
                   
                        CheckBtnController.Instance.SetDialogState(2);                          //开启需弹出任务框变量
                    m_dialogLabel.text = "";                                                    //置空对话内容
                    m_dialogLabel.gameObject.AddComponent<TypewriterEffect>();                  //添加打印机效果
                    SetDiaPointDir();                                                           //摆正对话框下方箭头朝向
                    m_dialogLabel.gameObject.SetActive(true);                                   //开启对话
                    m_dialogTail.SetActive(true);
                }
                else
                {
                    m_dialogNPCNum = CheckBtnController.Instance.GetDialogIndex();              //获取对话人物
                GameManager.Instance.SetSaveNPCNum(m_dialogNPCNum + 1);                     //改变最后遇到的NPC编号
                int _diaIndex = GetDiaIndex(m_dialogNPCNum + 1);                              //获取对话编号

                m_dialogText = xnl[m_dialogNPCNum].ChildNodes[_diaIndex].InnerText;     //获取对话内容
                GameManager.Instance.SetDialogText(m_dialogText);                           //指定对话内容（为打字机）
                if (_diaIndex == 1)                                                         //如果需要弹出任务框
                {
                    m_dialogTail.SetActive(true);                                           //显示未完待续的小尾巴
                    CheckBtnController.Instance.SetDialogState(2);                          //开启需弹出任务框变量
                }
                else if (GameManager.Instance.GetStoveState() != 2 && _diaIndex == 2 && m_dialogNPCNum == 2)//如果已接受铁匠任务但炉子未修理
                {
                    m_dialogTail.SetActive(true);
                    CheckBtnController.Instance.SetDialogState(2);
                }
                else                                                                        //无需弹出任务框
                    CheckBtnController.Instance.SetDialogState(4);
                m_dialogLabel.text = "";                                                    //置空对话内容
                m_dialogLabel.gameObject.AddComponent<TypewriterEffect>();                  //添加打印机效果
                SetDiaPointDir();                                                           //摆正对话框下方箭头朝向
                m_dialogLabel.gameObject.SetActive(true);                                   //开启对话
            }
        }
        else if (_dialogState == 3)                                                     //对话显示完毕需要弹出任务框
        {
            m_dialogLabel.gameObject.SetActive(false);                                  //关闭对话框
            m_dialogTail.SetActive(false);                                              //隐藏小尾巴
            Destroy(m_dialogLabel.gameObject.GetComponent("TypewriterEffect"));         //移除打印机效果
        }
        else if (_dialogState == 5)                                                     //对话完毕需弹出铁匠铺
        {
            GameManager.Instance.SetSmithyState(true);                                  //打开铁匠铺
            CheckBtnController.Instance.SetDialogState(0);                              //对话状态变量归零
        }
        else if (_dialogState == 6)                                                        //对话完毕需弹出酒馆
        {
            GameManager.Instance.SetBarState(true);                                     //打开酒馆买卖界面
            CheckBtnController.Instance.SetDialogState(0);                              //对话状态变量归零
        }
		}
		
		else if(CheckBtnController.Instance.GetThingDialogState()==1)						//按下调查村庄中的物品
		{
			if(m_diaLogTimer==-1f)															//显示对话内容
			{
				m_thingDiaPos = CheckBtnController.Instance.GetThingDialogPos();			//获取主角的对话位置
				Vector3 _screenPos = Camera.main.WorldToScreenPoint (m_thingDiaPos);		//将位置由世界坐标转为屏幕坐标
				Vector3 _uiScreenPos = UICamera.mainCamera.ScreenToWorldPoint(_screenPos);	//转化为NGUI坐标
				_uiScreenPos.z = 0f;														//去掉Z轴
				m_dialogLabel.gameObject.transform.position = _uiScreenPos;					//指定对话框位置

				int _diaIndex = CheckBtnController.Instance.GetThingDialogIndex();			//获取对话索引
				if(_diaIndex==16)															//当前碰到的是罐子
					_diaIndex = 11;															//指定罐子的对话索引
				if(_diaIndex==1&&GameManager.Instance.GetTaskIndexState(7)==1)
                {
                    m_dialogText = xnlCountryThing[_diaIndex].ChildNodes[1].InnerText;
                    GameManager.Instance.SetTaskIndexState(7, 2);
                }
                else
				m_dialogText =  xnlCountryThing[_diaIndex].ChildNodes[GetThingDiaIndex(_diaIndex)].InnerText;			//获取对话内容
				GameManager.Instance.SetDialogText(m_dialogText);							//为打字机指定对话内容
				m_diaLogTimer = -3f;														//对话已显示（）保证只执行一次
				m_dialogLabel.text = "";													//置空对话内容
				 m_dialogLabel.gameObject.AddComponent<TypewriterEffect>();					//添加打印机脚本
				SetDiaPointDir();															//摆正对话框下方箭头朝向
				m_dialogLabel.gameObject.SetActive(true);									//开启对话
			}
		}
		else  																				//主角离开或关闭对话框
		{
			m_diaLogTimer = -1f;
			if(_dialogState!=-1)
				CheckBtnController.Instance.SetDialogState(0);
			m_dialogLabel.gameObject.SetActive(false);										//关闭对话框
			m_dialogTail.SetActive(false);													//关闭对话框小尾巴
			if(m_dialogLabel.gameObject.GetComponent("TypewriterEffect"))					//如果存在打印机脚本
				Destroy(m_dialogLabel.gameObject.GetComponent("TypewriterEffect"));			//移除打印机脚本

		}
	}

	void SetDiaPointDir()																	//指定对话框下方箭头的方向
	{
		if(CheckBtnController.Instance.GetNPCDirection())							//NPC面向左
		{
			Vector3 _localscale = m_diaLogPoint.transform.localScale;				//保证指示箭头朝右
			if(_localscale.x>0)
			{
				_localscale.x *= -1;
				m_diaLogPoint.transform.localScale = _localscale;
			}
		}
		else
		{
			Vector3 _localscale = m_diaLogPoint.transform.localScale;				//保证指示箭头朝左
			if(_localscale.x<0)
			{
				_localscale.x *= -1;
				m_diaLogPoint.transform.localScale = _localscale;
			}
		}
	}

	int GetThingDiaIndex(int _thingIndex)
	{
		int _index = 0;																//返回的对话索引 默认为第一条
 		if (_thingIndex == 0)														//如果碰到锅炉
			_index = GameManager.Instance.GetStoveState ();							//返回的对话内容和锅炉状态一致
		return _index;
	}
	
	int GetDiaIndex(int _NPCnum)													//计算当前应显示的对话编号函数
	{
		int _index = 0;																//对话索引

		switch(_NPCnum)																//获取NPC编号
		{
		case 1:																		//和村长对话
			if(GameManager.Instance.GetTaskIndexState(0)==0)						//未得到信
				_index = 0;
			else
				_index = GameManager.Instance.GetTaskIndexState(1) + 1;				//根据村长任务状态确定对话编号
			break;
		case 2:																		//和书店老板对话
			if(GameManager.Instance.GetTaskIndexState(1)==0)						//未接受村长任务
				_index = 0;
			else 																	//已接受村长任务
				_index = GameManager.Instance.GetTaskIndexState(2) + 1;				//根据书店老板任务状态确定对话编号
			if(_index==3)
			{
                    if (GameManager.Instance.GetItemNum(15) == 0)
                    {
				        GameManager.Instance.SetMessageType(1, "勇者秘笈");
					GameManager.Instance.SetItemNum(15,1);								//得到一本勇者秘笈书

                    }
                    GameManager.Instance.SetTaskIndexState(2, 3);

            }
			break;
		case 3:																		//和铁匠对话
			if(GameManager.Instance.GetTaskIndexState(2)==0)						//未接受铁匠任务
				_index = 0;
            if(GameManager.Instance.GetTaskIndexState(7)==1)
                {
                    _index = 5;
                }
			else 																	//接受铁匠任务
				_index = GameManager.Instance.GetTaskIndexState(3) + 1;				//根据铁匠任务状态确定对话编号
			break;
		case 4:                                                                     //和水手对话
                if (GameManager.Instance.GetTaskIndexState(2) < 2)                          //未完成书店老板任务
                    _index = 0;
                else
                {
                    if (GameManager.Instance.GetItemNum(8) == 1||GameManager.Instance.GetItemNum(9) == 1
					||GameManager.Instance.GetItemNum(10) == 1||GameManager.Instance.GetItemNum(11) == 1)
                    {
                        _index = 4;
                    }
                    else {
                    _index = GameManager.Instance.GetTaskIndexState(4) + 1;             //根据水手任务状态确定对话编号
                    if (_index == 3)
                        GameManager.Instance.SetTaskIndexState(4, 3);
                    }
                }
                break;
		case 5:																		//和酒鬼对话
			if(GameManager.Instance.GetTaskIndexState(4)==0)						//未接受水手任务
				_index = 0;
			else 																	//接受水手任务
				_index = GameManager.Instance.GetTaskIndexState(5) + 1;		
			break;
		case 6:																		//和酒保对话
			if(GameManager.Instance.GetTaskIndexState(5)==0)						//未接受酒鬼任务
				_index = 0;
			else 																	//接受酒鬼任务
				_index = GameManager.Instance.GetTaskIndexState(6) + 1;	
			break;
		}
		return _index;																//返回对话索引
	}
}
