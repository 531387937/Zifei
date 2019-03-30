using UnityEngine;
using System.Collections;

public class CameraController : MonoBehaviour 
{
	public GameObject m_heroObj;								//主角对象
	public float m_cameraTrackingSpeed = 1f;					//摄像机的追踪速度
	public Transform m_RTborderPos;								//右上卷屏右边界位置
	public Transform m_LBborderPos;								//左下卷屏左边界位置
	public Transform m_heroCamPos;								//主角决定的相机位置
	public GameObject m_sceneUIPanel;							//场景中的UI面板
	public Transform[] m_camInRoomPos;							//主角在场景内时摄像机的位置
	public Transform m_lightHouseCamPos;						//灯塔特写时相机的位置
	public Transform m_startCamPos;								//开始界面相机位置
	public GameObject m_uiPanelMask;							//UI面板遮罩

	private Vector3 m_lastTargetPos = Vector3.zero;				//上一个目标位置
	private Vector3 m_currTargetPos = Vector3.zero;				//下一个目标位置
	private float m_currLerpDis = 0.0f;
	private int m_UIPanelState = 0;								//UI界面遮罩提示条状态
	private bool m_uiPanelHelp = false;


	private bool isLikeIpadScreenAspect = false;

    public bool UiPanelHelp
    {
        get
        {
            return m_uiPanelHelp;
        }

        set
        {
            m_uiPanelHelp = value;
        }
    }

	void Awake(){

		if ((float)Screen.width / Screen.height < 1.7f)
			isLikeIpadScreenAspect = true;	

	}

    void Start () 
	{
		if(GameManager.Instance.GetGameStartState ()!=0)
		{
			m_sceneUIPanel.SetActive(true);						//开启界面UI按钮

		}
		Vector3 _heroPos = m_heroObj.transform.position;		//记录主角位置
		Vector3 _startTargetPos = _heroPos;
		m_lastTargetPos = _startTargetPos;						//上一个和下一个目标位置均为主角所在位置
		m_currTargetPos = _startTargetPos;
	}

	void LateUpdate()
	{
		int _gameStartState = GameManager.Instance.GetGameStartState ();
		switch(_gameStartState)
		{
		case 0:
			this.transform.position = m_startCamPos.position;	//指定相机位置
			break;
		case 1:
			iTween.MoveTo(gameObject, iTween.Hash("position",new Vector3(m_heroCamPos.transform.position.x,m_heroCamPos.transform.position.y,this.transform.position.z),
			                                      "time",2f,"onComplete","CamStartMoveEnd","easyType",iTween.EaseType.easeInCubic));
			GameManager.Instance.SetGameStartState(2);
			break;
		case 2:
			break;
		case 3:
			int _currScene = GameManager.Instance.GetCurrentScene();
			if(_currScene==0)									//保证只在村庄场景下卷屏
			{
				TrackPlayer ();									//实时更新摄像机和主角的位置
				m_currLerpDis += m_cameraTrackingSpeed;			//将摄像机移动到目标位置
				this.transform.position = Vector3.Lerp (m_lastTargetPos, m_currTargetPos, m_currLerpDis);
			}
			else 												//如果主角在村庄中的室内
				this.transform.position = m_camInRoomPos[_currScene-1].position;	//指定摄像机的位置
			break;
		case 4:													//点击了读档按钮
			m_UIPanelState = 1;									//需显示UI面板
            GameManager.Instance.SetGameStartState (3);			//游戏开始了
			break;
		case 5:													//主角胜利回村 灯塔特写
			this.transform.position = m_lightHouseCamPos.position;	//指定相机位置
			break;
		case 6:													//灯塔播放结束
			iTween.MoveTo(gameObject, iTween.Hash("position",new Vector3(m_heroCamPos.transform.position.x,m_heroCamPos.transform.position.y,this.transform.position.z),
			                                      "time",2f,"onComplete","CamLightHouseMoveEnd","easyType",iTween.EaseType.easeInCubic));
			GameManager.Instance.SetGameStartState(2);

			break;
		}



		if(m_UIPanelState!=0)														//如果需要启动UI面板
		{
			switch(m_UIPanelState)													//判定当前UI面板启动状态
			{
			case 1:																	//第一步
				m_sceneUIPanel.GetComponent<UIWidget>().alpha = 0;					//设定面板alpha为0
				m_sceneUIPanel.SetActive (true);									//开启界面UI
				m_UIPanelState = 2;													//进入下一步
				break;
			case 2:																	//第二步
				m_sceneUIPanel.GetComponent<UIWidget>().alpha += Time.deltaTime*3f;	//开启alpha
				if(m_sceneUIPanel.GetComponent<UIWidget>().alpha>=1)				//如果面板显示完全
				{
					m_sceneUIPanel.GetComponent<UIWidget>().alpha = 1f;				//保证其alpha为1
					if(m_uiPanelHelp)
					{
						m_UIPanelState = 3;												//面板显示完毕

						if(isLikeIpadScreenAspect)
							m_uiPanelMask.GetComponent<UISprite>().spriteName = "tips01_4by3";
						else
							m_uiPanelMask.GetComponent<UISprite>().spriteName = "tips01";
						m_uiPanelMask.SetActive (true);

						GameManager.Instance.CloseUsualBtn();
					}
					else
					{
						m_UIPanelState = 0;
					}
				}
				break;
			case 3:																	//UI面板遮罩计时器
				if(Input.GetMouseButtonDown(0))																//如果鼠标按下
				{
					Ray ray = UICamera.mainCamera.ScreenPointToRay(Input.mousePosition);
					RaycastHit hit;
					if (Physics.Raycast(ray, out hit)) 														//检测鼠标点击区域
					{
						if(hit.collider.gameObject == m_uiPanelMask)									//如果点击了后边遮罩图 关闭背包
						{
							if(isLikeIpadScreenAspect)
								m_uiPanelMask.GetComponent<UISprite>().spriteName = "tips03_4by3";
							else
								m_uiPanelMask.GetComponent<UISprite>().spriteName = "tips03";
							m_UIPanelState = 4;	
						}	
					}
				}
				break;
			case 4:
				if(Input.GetMouseButtonDown(0))																//如果鼠标按下
				{
					Ray ray = UICamera.mainCamera.ScreenPointToRay(Input.mousePosition);
					RaycastHit hit;
					if (Physics.Raycast(ray, out hit)) 														//检测鼠标点击区域
					{
						if(hit.collider.gameObject == m_uiPanelMask)									//如果点击了后边遮罩图 关闭背包
						{
							m_UIPanelState = 0;	
							m_uiPanelMask.SetActive(false);

							if(isLikeIpadScreenAspect)
								m_uiPanelMask.GetComponent<UISprite>().spriteName = "tips01_4by3";
							else
                            	m_uiPanelMask.GetComponent<UISprite>().spriteName = "tips01";
                                
							GameManager.Instance.OpenUsualBtn();
							GameManager.Instance.SetMessageType(3, "家门旁的邮筒收到了新邮件！");
						}
					}
				}
				break;
			}
		}
	}

	void CamStartMoveEnd()										//相机从主界面移动到村庄完毕
	{
		m_uiPanelHelp = true;
		m_UIPanelState = 1;										//需显示UI面板
		GameManager.Instance.SetGameStartState (3);				//游戏开始了
	}

	void CamLightHouseMoveEnd()
	{
		GameManager.Instance.OpenUsualBtn ();					//开启常用按钮（顺序不能变）
		GameManager.Instance.SetGameStartState (3);				//游戏开始了
	}

	void TrackPlayer()
	{
		Vector3 _currCamPos = this.transform.position;			//获取并保存摄像机和主角在世界坐标系中的坐标
		Vector3 _heroPos = m_heroCamPos.transform.position;
		m_lastTargetPos = _currCamPos;
		if(_heroPos.x>m_RTborderPos.position.x)					//主角超过右边界
		{
			m_currTargetPos.x = m_RTborderPos.position.x;		//摄像机水平不跟随
			if(_heroPos.y>m_RTborderPos.position.y)				//主角超过上边界
				m_currTargetPos.y = m_RTborderPos.position.y;
			else if(_heroPos.y<m_LBborderPos.position.y)		//主角低于下边界
				m_currTargetPos.y = m_LBborderPos.position.y; 
			else 												//主角y轴正常
				m_currTargetPos.y = _heroPos.y;
		}
		else if(_heroPos.x<m_LBborderPos.position.x)			//主角小于左边界
		{
			m_currTargetPos.x = m_LBborderPos.position.x;		//摄像机水平不跟随
			if(_heroPos.y>m_RTborderPos.position.y)
				m_currTargetPos.y = m_RTborderPos.position.y;
			else if(_heroPos.y<m_LBborderPos.position.y)
				m_currTargetPos.y = m_LBborderPos.position.y; 
			else
				m_currTargetPos.y = _heroPos.y;
		}
		else 													//主角x轴正常
		{
			m_currTargetPos.x = _heroPos.x;
			if(_heroPos.y>m_RTborderPos.position.y)
				m_currTargetPos.y = m_RTborderPos.position.y;
			else if(_heroPos.y<m_LBborderPos.position.y)
				m_currTargetPos.y = m_LBborderPos.position.y; 
			else
				m_currTargetPos.y = _heroPos.y;
		}
		m_currTargetPos.z = _currCamPos.z;						//保证摄像机在Z轴方向上的值不变
	}
}
