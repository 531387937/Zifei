using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeadBattleMessageController : MonoBehaviour {

	public Transform m_coinBirthPos;														//金币诞生位置 主角头顶
	public GameObject m_coin;																//主角头顶的金币
	public GameObject m_messageBox;															//获取金币后提示框
	public GameObject m_goldBling;															//金币闪光
	public UILabel[] m_messageLabel;														//获取金币后提示文字

	private UISpriteAnimation m_coinAni;													//动画控制器
	private UISpriteAnimation m_messageAni;													//动画控制器
	private int m_coinGetState = 0;															//金币获取阶段
	private int m_messageState = 0;															//消息提示阶段
	private float m_coinTimer =0;															//金币计时器
	private string m_messageContent;
	private bool m_allWords = false;                                                        //是不是需要正局提示





	private bool isCanShowMessage = false;
	private float showDelayTime = 0.75f;
	private float showDelayTimer = 0f;

	void Update()
	{


		var messageType = HeadBattleGameManager.Instance.GetMessageType();

		if (messageType == 2)
		{
			m_messageContent = HeadBattleGameManager.Instance.GetMessageContent ();
			m_messageState = 1;
			m_coinGetState = 1;
			m_messageLabel[1].GetComponent<UILabel>().text = m_messageContent;
			m_allWords = false;
			HeadBattleGameManager.Instance.ClearMessage();
		}
		else if(messageType == 1)
		{
			m_messageContent =HeadBattleGameManager.Instance.GetMessageContent();
			m_messageState = 1;
			m_messageLabel[1].GetComponent<UILabel>().text = m_messageContent;
			m_allWords = false;
			HeadBattleGameManager.Instance.ClearMessage();

		}
		else if(messageType == 3)
		{
			m_messageContent = HeadBattleGameManager.Instance.GetMessageContent ();
			m_messageState = 1;
			m_messageLabel[2].GetComponent<UILabel>().text = m_messageContent;
			m_allWords = true;
			HeadBattleGameManager.Instance.ClearMessage();

		}


		if(m_coinGetState == 1 || m_coinGetState == 2 || m_coinGetState == 3)
			CoinAni ();

		if(m_messageState == 1 || m_messageState == 2 )
			MessageAni ();


		if (isCanShowMessage) {
			showDelayTimer += Time.deltaTime;
			if (showDelayTimer >= showDelayTime) {
				showDelayTimer = 0;
				isCanShowMessage = false;
				HeadBattleGameManager.Instance.isCanShowMsg = true;//

			}
		}
	}







	void CoinAni()
	{
		switch(m_coinGetState)
		{
		case 1:
			Vector3 _screenPos = Camera.main.WorldToScreenPoint (m_coinBirthPos.position);	//将位置由世界坐标转为屏幕坐标
			Vector3 _uiScreenPos = UICamera.mainCamera.ScreenToWorldPoint(_screenPos);		//转化为NGUI坐标
			_uiScreenPos.z = 0f;															//z轴归零
			m_coin.transform.position = _uiScreenPos;										//指定金币位置
			m_coin.gameObject.SetActive(true);												//金币出现
			m_coinAni = m_coin.gameObject.AddComponent<UISpriteAnimation>();				//添加金币动画
			m_coinAni.loop = false;															//动画不循环
			m_coinAni.namePrefix = "coin";													//指定动画图
			m_coinAni.framesPerSecond = 10;													//帧率 
			m_coinGetState = 2;																//跳转至金币上移阶段
			m_coinTimer = 0.3f;
			break;
		case 2:
			m_coinTimer -= Time.deltaTime;														//开启计时器
			if(m_coinTimer>0)																	
				m_coin.transform.Translate(new Vector3(0f, 0.001f, 0f));					//规定时间内金币一直上移
			else if(m_coinTimer<0)																//上移结束
			{
				m_coinGetState = 3;															//跳转至金币闪光阶段
				Destroy(m_coin.GetComponent<UISpriteAnimation>());							//销毁金币动画
				m_coinTimer = 0.5f;																//计时器归位
				m_goldBling.transform.position = m_coin.transform.position;					//指定闪光位置
				m_goldBling.SetActive(true);												//开启闪光
				m_coinAni = m_goldBling.gameObject.AddComponent<UISpriteAnimation>();		//加载闪光动画
				m_coinAni.loop = false;														//闪光动画不循环
				m_coinAni.namePrefix = "ani_goldBling";										//指定闪光动画图
				m_coinAni.framesPerSecond = 10;												//闪光帧率
			}
			break;
		case 3:
			if(!m_coinAni.isPlaying)														//如果闪光结束
			{	
				m_coinGetState = 0;															//跳转至消息闪现阶段
				Destroy(m_goldBling.GetComponent<UISpriteAnimation>());						//销毁闪光动画
				m_goldBling.SetActive(false);												//隐藏闪光控件
				m_coin.SetActive(false);													//隐藏金币
				//HeadBattleGameManager.Instance.SetMessageType(1, m_messageContent);
			}
			break;
		}
	}

	void MessageAni()
	{
		switch(m_messageState)
		{
		case 1:
			m_messageBox.SetActive(true);													//开启消息栏
			m_messageAni = m_messageBox.gameObject.AddComponent<UISpriteAnimation>();		//加载消息动画
			m_messageAni.loop = false;														//消息动画不循环
			m_messageAni.namePrefix = "ani_message";										//指定消息动画图
			m_messageAni.framesPerSecond = 10;                                              //消息动画帧率
			m_messageState = 2;
			break;

		case 2:
			if(!m_messageAni.isPlaying)														//消息动画已播放
			{
				m_messageState = 0;															//跳转至空闲状态
				Destroy(m_messageBox.GetComponent<UISpriteAnimation>());					//销毁消息动画
				m_messageBox.SetActive(false);												//隐藏消息栏		
			}
			else 																			//消息动画正在播放
			{
				if (m_messageBox.GetComponent<UISprite>().spriteName == "ani_message03")       //播放到第三帧
				{
					if (!m_allWords)
					{
						m_messageLabel[0].gameObject.SetActive(true);                           //显示消息内容
						m_messageLabel[1].gameObject.SetActive(true);                           //显示消息内容
					}
					else
						m_messageLabel[2].gameObject.SetActive(true);                           //显示消息内容

				}
				else if (m_messageBox.GetComponent<UISprite>().spriteName == "ani_message10")   //播放到第十帧
				{
					if (!m_allWords)
					{
						m_messageLabel[0].gameObject.SetActive(false);                          //隐藏消息内容
						m_messageLabel[1].gameObject.SetActive(false);                          //隐藏消息内容
					}
					else
						m_messageLabel[2].gameObject.SetActive(false);

					isCanShowMessage = true;

				}
			}
			break;
		}
	}
}
