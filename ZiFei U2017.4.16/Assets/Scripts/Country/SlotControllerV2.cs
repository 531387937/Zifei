using UnityEngine;
using System.Collections;

public class SlotControllerV2 : MonoBehaviour
{
	public GameObject m_slotPanel;													//老虎机面板
	public GameObject m_slotBtn;
	public GameObject[] m_patternLeft;												//左边的两个样式
	public GameObject[] m_patternCenter;											//中间的两个样式
	public GameObject[] m_patternRight;												//右边的两个样式
	public Transform[] m_patternPos;												//样式的三个竖直位置
	public UISprite m_patternBox;													//样式框
	public GameObject[] m_slotArrow;												//老虎机箭头
	public GameObject m_slotMask;													//老虎机遮罩图

	private int m_leftUpIndex = 0;													//左边样式上下方编号
	private int m_leftDownIndex = 1;												
	private int m_centerUpIndex = 1;												//中间样式的上下方编号
	private int m_centerDownIndex = 0;
	private int m_rightUpIndex = 1;													//右边样式的上下方编号
	private int m_rightDownIndex = 0;	
	private float m_moveDis;														//样式竖直距离
	private float m_moveTimer = 0.1f;												//样式移动时间间隔
	private int[] m_patternValue = new int[3];										//三个样式停止时的值
	private int m_slotGameState = 0;												//老虎机游戏状态 （0未开始 1打开老虎机 2第一个正在 3第二个正在 4第三个正在 5游戏结束）

	void Awake()
	{
		UIEventListener.Get (m_slotBtn.gameObject).onClick = OnSlotBtnClick;		//老虎机开关按钮侦听
	}

	void OnSlotBtnClick(GameObject _slotBtn)										//点击了老虎机按钮
	{
		if(m_slotGameState!=0)														//如果当前老虎机面板已启动
		{
			if(m_slotGameState==1)													//如果当前为初始状态
			{
				m_patternBox.spriteName = "slot_foreground1";						//更换样式框图
				m_slotArrow[0].SetActive(true);
				GameManager.Instance.SetCurrAddMoney(-1);
				GameManager.Instance.SetMessageType(3, "你花费了1金币");
			}
			else if(m_slotGameState==2)
			{
				m_slotArrow[0].SetActive(false);
				m_slotArrow[1].SetActive(true);
			}
			else if(m_slotGameState==3)
			{
				m_slotArrow[1].SetActive(false);
				m_slotArrow[2].SetActive(true);
			}
			m_slotGameState ++;														//游戏状态前进
			m_moveTimer = 0.1f;														//计时器归位
		}
	}

	void Start()
	{
		m_moveDis = m_patternPos [1].position.y - m_patternPos [0].position.y;		//样式之间的竖直位移差
		m_patternValue[0] = 1;														//三个样式的初始值
		m_patternValue[1] = 8;
		m_patternValue[2] = 5;
	}

	void Update()
	{
		if(GameManager.Instance.GetPanelState(0))									//如果需要开启老虎机面板
		{
			if(m_slotGameState==0)													//初次启动游戏
			{
				for(int i=0; i<3; i++)
					m_slotArrow[i].SetActive(false);								//隐藏三个箭头
				m_slotGameState = 1;												//老虎机面板已打开
				m_slotPanel.SetActive(true);										//开启老虎机
				m_slotMask.SetActive(true);											//开启老虎机遮罩
				GameManager.Instance.CloseUsualBtn();								//关闭常用按钮
			}
			if(m_slotGameState>=2)													//如果游戏开始了
			{
				if(m_slotGameState<=4)												//如果游戏在状态3之前
				{
					m_moveTimer -= Time.deltaTime;									//倒计时
					if(m_moveTimer<=0)												//样式需要运动
					{
						PatternThreeMove();											//样式3一直运动
						if(m_slotGameState<=3)										//如果游戏在状态2之前
							PatternTwoMove();										//样式2一直运动
						if(m_slotGameState==2)										//如果游戏为状态1
							PatternOneMove();										//样式1一直运动
					}
				}
				else 																//如果达到游戏状态4
				{
					m_slotArrow[2].SetActive(false);								//隐藏箭头3
					m_slotGameState = 1;											//游戏状态归零
					if(ResultCheck())												//如果游戏胜利
						m_patternBox.spriteName = "slot_foreground2";				//更换样式框图
					else
						GameManager.Instance.SetMessageType(3, "运气不好，再来一局吧！");
				}
			}
			if(Input.GetMouseButtonDown(0))											//如果鼠标按下
			{
				Ray ray = UICamera.mainCamera.ScreenPointToRay (Input.mousePosition);
				RaycastHit hit;
				if (Physics.Raycast(ray, out hit)) 									//检测鼠标点击区域
				{
					if(hit.collider.gameObject == m_slotMask)						//如果点击了后边遮罩图 关闭背包
					{
						m_slotPanel.SetActive(false);								//关闭老虎机
						m_slotMask.SetActive(false);								//关闭遮罩图
						GameManager.Instance.SetPanelState(0,false);				//关闭老虎机面板
						GameManager.Instance.OpenUsualBtn();						//开启常用按钮
						m_patternBox.spriteName = "slot_foreground1";				//更换样式框图
						m_slotGameState = 0;										//老虎机游戏未启动
					}
				}
			}
		}
	}

	bool ResultCheck()																//游戏结果判定
	{
		switch(m_patternValue[0])													//如果三个值均为2 5 9中的任意一个
		{
		case 2:
		case 5:
		case 9:
			if(m_patternValue[1]==2||m_patternValue[1]==5||m_patternValue[1]==9)		
			{
				if(m_patternValue[2]==2||m_patternValue[2]==5||m_patternValue[2]==9)	
				{
					GameManager.Instance.SetCurrAddMoney(4);
					GameManager.Instance.SetMessageType(2, "4金币");
					return true;													//胜利
				}
			}
			break;
		case 1:																		//如果三个值均为1 6 8 中的一个
		case 6:
		case 8:
			if(m_patternValue[1]==1||m_patternValue[1]==6||m_patternValue[1]==8)
			{
				if(m_patternValue[2]==1||m_patternValue[2]==6||m_patternValue[2]==8)
				{
					GameManager.Instance.SetCurrAddMoney(4);
					GameManager.Instance.SetMessageType(2, "4金币");
					return true;													//胜利
				}
			}
			break;
		case 3:																		//如果三个值均为3 7中的一个
		case 7:
			if(m_patternValue[1]==3||m_patternValue[1]==7)
			{
				if(m_patternValue[2]==3||m_patternValue[2]==7)
				{
					GameManager.Instance.SetCurrAddMoney(6);
					GameManager.Instance.SetMessageType(2, "6金币");
					return true;													//胜利
				}
					
			}
			break;
		
		}


		if(m_patternValue[0]==m_patternValue[1])									//如果三个值相等
		{
			if(m_patternValue[0]==m_patternValue[2])
			{
				if (m_patternValue[0] == 4) {// "777"
					if (GameManager.Instance.GetAchieveGot (15) == 0) {
						AudioManager.Instance.TipsSoundPlay (Global.GetInstance ().audioName_AchieveGet);
						GameManager.Instance.SetAchieveGot (15, 1);
						GameManager.Instance.SetMessageType (3, "您获得了成就【抛砖引玉】");
					}
					GameManager.Instance.SetCurrAddMoney(20);
					GameManager.Instance.SetMessageType(2, "20金币");
					return true;														//胜利
				} else {
				
					GameManager.Instance.SetCurrAddMoney(10);
					GameManager.Instance.SetMessageType(2, "10金币");
					return true;														//胜利
				}
			}
		}
		return false;
	}

	void PatternOneMove()															//样式1移动
	{
		if(m_patternValue[0]==1)													//如果当前值为1 
		{
			m_patternLeft[m_leftDownIndex].transform.position = new Vector3(m_patternLeft[m_leftDownIndex].transform.position.x,m_patternPos[0].position.y,0f);
			int _tempIndex;															//将下方样式移至上方 交换上下样式编号
			_tempIndex = m_leftUpIndex;
			m_leftUpIndex = m_leftDownIndex;
			m_leftDownIndex = _tempIndex;
		}
		m_patternLeft [m_leftUpIndex].transform.Translate (0f, m_moveDis/10f, 0f);	//样式下移
		m_patternLeft [m_leftDownIndex].transform.Translate (0f, m_moveDis/10f, 0f);
		m_moveTimer = 0.1f;															//计时器恢复
		if(m_patternValue[0]==0)													//如果样式值为0
			m_patternValue[0] = 9;													//变为最后一个
		else 																		//样式值不为0
			m_patternValue[0] --;													//值递减
	}
	void PatternTwoMove()
	{
		if(m_patternValue[1]==1)													//如果当前值为1 
		{
			m_patternCenter[m_centerDownIndex].transform.position = new Vector3(m_patternCenter[m_centerDownIndex].transform.position.x,m_patternPos[0].position.y,0f);
			int _tempIndex;															//将下方样式移至上方 交换上下样式编号
			_tempIndex = m_centerUpIndex;
			m_centerUpIndex = m_centerDownIndex;
			m_centerDownIndex = _tempIndex;
		}
		m_patternCenter [m_centerUpIndex].transform.Translate (0f, m_moveDis/10f, 0f);//样式下移
		m_patternCenter [m_centerDownIndex].transform.Translate (0f, m_moveDis/10f, 0f);
		m_moveTimer = 0.1f;															//计时器恢复
		if(m_patternValue[1]==0)													//如果样式值为0
			m_patternValue[1] = 9;													//变为最后一个
		else 																		//样式值不为0
			m_patternValue[1] --;													//值递减
	}
	void PatternThreeMove()
	{
		if(m_patternValue[2]==1)													//如果当前值为1 
		{
			m_patternRight[m_rightDownIndex].transform.position = new Vector3(m_patternRight[m_rightDownIndex].transform.position.x,m_patternPos[0].position.y,0f);
			int _tempIndex;															//将下方样式移至上方 交换上下样式编号
			_tempIndex = m_rightUpIndex;
			m_rightUpIndex = m_rightDownIndex;
			m_rightDownIndex = _tempIndex;
		}
		m_patternRight [m_rightUpIndex].transform.Translate (0f, m_moveDis/10f, 0f);//样式下移
		m_patternRight [m_rightDownIndex].transform.Translate (0f, m_moveDis/10f, 0f);
		m_moveTimer = 0.1f;															//计时器恢复
		if(m_patternValue[2]==0)													//如果样式值为0
			m_patternValue[2] = 9;													//变为最后一个
		else 																		//样式值不为0
			m_patternValue[2] --;													//值递减
	}
}
