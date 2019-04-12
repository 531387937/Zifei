using UnityEngine;
using System.Collections;

public class LevelOneHeroController : MonoBehaviour 
{
	public enum heroHorizontalStates															//主角水平方向的状态
	{	
		idle = 0, 																				//空闲状态
		left,																					//向左状态
		right,																					//向右
	}
	
	public delegate void heroHorizontalStateHandler(LevelOneHeroController.heroHorizontalStates _newState);//定义委托和事件
	public static event heroHorizontalStateHandler onHorizontalStateChange; 
	
	public enum heroVerticalStates																//主角竖直方向上的状态
	{	
		idle = 0, 																				//空闲状态
		jump,																					//跳跃状态
		down,																					//下行
		falling,																				//下落
		landing,																				//落地
	}
	
	public delegate void heroVerticalStateHandler(LevelOneHeroController.heroVerticalStates _newState);//定义委托和事件
	public static event heroVerticalStateHandler onVerticalStateChange; 

	public enum heroClimbHorizontalStates														//主角爬行时的水平状态
	{	
		idle,																					//未转身时的静止状态
		idleClimb,																				//爬行静止状态
		leftClimb,																				//向左爬
		rightClimb,																				//向右爬
	}
	
	public delegate void heroClimbHorizontalStateHandler(LevelOneHeroController.heroClimbHorizontalStates _newState);//定义委托和事件
	public static event heroClimbHorizontalStateHandler onClimbHorizontalStateChange; 

	public enum heroClimbVerticalStates															//主角爬行时的竖直状态
	{	
		idle,																					//未转身时的静止状态
		idleClimb,																				//爬行静止状态
		upClimb,																				//向上爬
		downClimb,																				//向下爬
	}
	
	public delegate void heroClimbVerticalStateHandler(LevelOneHeroController.heroClimbVerticalStates _newState);//定义委托和事件
	public static event heroClimbVerticalStateHandler onClimbVerticalStateChange; 
	
	public UISprite[] m_heroMoveBtn;					//摇杆切图
	public GameObject m_heroCollider;					//主角脚部碰撞盒（爬行用）
	public GameObject[] m_mapCollider;					//爬行地图包围盒
	public static int m_climbState = 0;                 //0主角未碰到爬行块 1主角碰到爬行块但未按下向上按钮 2主角处于爬行状态 

    private bool m_joyStickSound = false;										//摇杆音效是否播放
	private bool m_leftBtnCheck = false;				//左移按钮是否按下变量
	private bool m_rightBtnCheck = false;				//右移按钮是否按下变量
	private bool m_downBtnCheck = false;				//下行按钮是否按下变量
	private bool m_jumpBtnCheck = false;				//跳跃按钮是否按下变量

	private int m_heroBlinkCount = 0;					//主角闪烁次数
	private float m_heroBlinkTimer = 0f;				//主角闪烁计时器


    void OnEnable()										//摇杆可用时注册事件
	{
		EasyJoystick.On_JoystickMove += OnJoystickMove;
		EasyJoystick.On_JoystickMoveEnd += OnJoystickMoveEnd;



    }
    private void Start()
    {
        m_mapCollider = GameObject.FindGameObjectsWithTag("climbCollider");
    }
    private void Update()
    {
        if (LevelOneGameManager.Instance.isPlatformType_Mobild) return;
        
        if (!LevelOneGameManager.Instance.isUsualBtnCanClick) return;
        var horizontalValue = Input.GetAxis("Horizontal");
        var verticalValue = Input.GetAxis("Vertical");




        if (horizontalValue > 0) m_rightBtnCheck = true; else m_rightBtnCheck = false;

        if (horizontalValue < 0) m_leftBtnCheck = true; else m_leftBtnCheck = false;

        if (verticalValue > 0) m_jumpBtnCheck = true; else m_jumpBtnCheck = false;

        if (verticalValue < 0) m_downBtnCheck = true; else m_downBtnCheck = false;

        {
            m_heroMoveBtn[0].spriteName = "btn_right1";                         //恢复静止状态贴图
            m_heroMoveBtn[1].spriteName = "btn_uR1";
            m_heroMoveBtn[2].spriteName = "btn_up1";
            m_heroMoveBtn[3].spriteName = "btn_uL1";
            m_heroMoveBtn[4].spriteName = "btn_left1";
            m_heroMoveBtn[5].spriteName = "btn_dL1";
            m_heroMoveBtn[6].spriteName = "btn_down1";
            m_heroMoveBtn[7].spriteName = "btn_dR1";
            m_heroMoveBtn[8].spriteName = "btnBkg_dir1";

            m_heroMoveBtn[1].depth = 2;                     //恢复摇杆按钮的深度
            m_heroMoveBtn[3].depth = 2;
            m_heroMoveBtn[5].depth = 2;
            m_heroMoveBtn[7].depth = 2;
        }

        if (verticalValue == 0 && horizontalValue == 0) return;


        if ((horizontalValue > 0) && (verticalValue > 0))
        {
            m_heroMoveBtn[1].spriteName = "btn_uR2";
            m_heroMoveBtn[1].depth = 5;

        }
        else if ((horizontalValue < 0) && (verticalValue > 0))
        {

            m_heroMoveBtn[3].spriteName = "btn_uL2";
            m_heroMoveBtn[3].depth = 5;

        }
        else if ((horizontalValue < 0) && (verticalValue < 0))
        {
            m_heroMoveBtn[5].spriteName = "btn_dL2";
            m_heroMoveBtn[5].depth = 5;
        }
        else if ((horizontalValue > 0) && (verticalValue < 0))
        {
            m_heroMoveBtn[7].spriteName = "btn_dR2";
            m_heroMoveBtn[7].depth = 5;
        }
        else
        {

            if (horizontalValue > 0)
                m_heroMoveBtn[0].spriteName = "btn_right2";
            if (horizontalValue < 0)
                m_heroMoveBtn[4].spriteName = "btn_left2";


            if (verticalValue > 0)
                m_heroMoveBtn[2].spriteName = "btn_up2";
            if (verticalValue < 0)
                m_heroMoveBtn[6].spriteName = "btn_down2";


        }

        m_heroMoveBtn[8].spriteName = "btnBkg_dir2";

    }

    void OnDisable()									//摇杆不可用时移除事件
	{
		EasyJoystick.On_JoystickMove -= OnJoystickMove;
		EasyJoystick.On_JoystickMoveEnd -= OnJoystickMoveEnd;
	}
	
	void OnDestroy()									//摇杆销毁时移除事件
	{
		EasyJoystick.On_JoystickMove -= OnJoystickMove;
		EasyJoystick.On_JoystickMoveEnd -= OnJoystickMoveEnd;
	}


	void OnJoystickMoveEnd(MovingJoystick move)			//摇杆处于停止状态，角色进入等待动画
	{

		m_joyStickSound = false;
		m_leftBtnCheck = false;
		m_rightBtnCheck = false;
		m_downBtnCheck = false;
		m_jumpBtnCheck = false;
		
		m_heroMoveBtn[0].spriteName = "btn_right1";		//恢复静止状态贴图
		m_heroMoveBtn[1].spriteName = "btn_uR1";
		m_heroMoveBtn[2].spriteName = "btn_up1";
		m_heroMoveBtn[3].spriteName = "btn_uL1";
		m_heroMoveBtn[4].spriteName = "btn_left1";
		m_heroMoveBtn[5].spriteName = "btn_dL1";
		m_heroMoveBtn[6].spriteName = "btn_down1";
		m_heroMoveBtn[7].spriteName = "btn_dR1";
		m_heroMoveBtn[8].spriteName = "btnBkg_dir1";
		
		m_heroMoveBtn[1].depth = 2;						//恢复摇杆按钮的深度
		m_heroMoveBtn[3].depth = 2;
		m_heroMoveBtn[5].depth = 2;
		m_heroMoveBtn[7].depth = 2;
		
	}
	
	void OnJoystickMove(MovingJoystick move)									//当摇杆处于移动状态，角色开始奔跑
	{
        if (move.joystickName != "EasyJoystick")                                    //如果不是此摇杆，退出
        {
            print("No");
            return;
        }

        if (!m_joyStickSound)
		{
			m_joyStickSound = true;
            AudioManager.Instance.HeroSoundPlay(Global.GetInstance().audioName_JoyStick);
		}
		float joyPosX = move.joystickAxis.x;									//获取摇杆偏移量
		float joyPosY = move.joystickAxis.y;
		
        if (joyPosX>0&&(joyPosY/joyPosX)>-2.4142f&&(joyPosY/joyPosX)<2.4142f)	//是否方向为右
			m_rightBtnCheck = true;
		else
			m_rightBtnCheck = false;
		if(joyPosX<0&&(joyPosY/joyPosX)>-2.4142f&&(joyPosY/joyPosX)<2.4142f)	//是否方向为左
			m_leftBtnCheck = true;
		else
			m_leftBtnCheck = false;
		if(joyPosY>0&&((joyPosY/joyPosX)<-0.4142f||(joyPosY/joyPosX)>0.4142f))	//是否方向为上
			m_jumpBtnCheck = true;
		else
			m_jumpBtnCheck = false;
		if(joyPosY<0&&((joyPosY/joyPosX)<-0.4142f||(joyPosY/joyPosX)>0.4142f))	//是否方向为下
			m_downBtnCheck = true;
		else
			m_downBtnCheck = false;
		
		if(joyPosX>0&&(joyPosY/joyPosX)>=-0.4142f&&(joyPosY/joyPosX)<=0.4142f)	//按下向右按钮
		{
			m_heroMoveBtn[0].spriteName = "btn_right2";
		}
		else
		{
			m_heroMoveBtn[0].spriteName = "btn_right1";
		}
		if(joyPosX>0&&(joyPosY/joyPosX)>0.4142f&&(joyPosY/joyPosX)<2.4142f)		//按下右上按钮
		{
			m_heroMoveBtn[1].spriteName = "btn_uR2";
			m_heroMoveBtn[1].depth = 5;
		}
		else
		{
			m_heroMoveBtn[1].spriteName = "btn_uR1";
			m_heroMoveBtn[1].depth = 2;
		}
		if(joyPosY>0&&((joyPosY/joyPosX)<-2.4142f||(joyPosY/joyPosX)>2.4142f))	//按下向上按钮
		{
			m_heroMoveBtn[2].spriteName = "btn_up2";
		}
		else
		{
			m_heroMoveBtn[2].spriteName = "btn_up1";
		}
		if(joyPosX<0&&(joyPosY/joyPosX)<=-0.4142f&&(joyPosY/joyPosX)>=-2.4142f)	//按下左上按钮
		{
			m_heroMoveBtn[3].spriteName = "btn_uL2";
			m_heroMoveBtn[3].depth = 5;
		}
		else
		{
			m_heroMoveBtn[3].spriteName = "btn_uL1";
			m_heroMoveBtn[3].depth = 2;
		}
		if(joyPosX<0&&(joyPosY/joyPosX)>=-0.4142f&&(joyPosY/joyPosX)<=0.4142f)	//按下向左按钮
		{
			m_heroMoveBtn[4].spriteName = "btn_left2";
		}
		else
		{
			m_heroMoveBtn[4].spriteName = "btn_left1";
		}
		if(joyPosX<0&&(joyPosY/joyPosX)>0.4142f&&(joyPosY/joyPosX)<2.4142f)		//按下左下按钮
		{
			m_heroMoveBtn[5].spriteName = "btn_dL2";
			m_heroMoveBtn[5].depth = 5;
		}
		else
		{
			m_heroMoveBtn[5].spriteName = "btn_dL1";
			m_heroMoveBtn[5].depth = 2;
		}
		if(joyPosY<0&&((joyPosY/joyPosX)<-2.4142f||(joyPosY/joyPosX)>2.4142f))	//按下向下按钮
		{
			m_heroMoveBtn[6].spriteName = "btn_down2";
		}
		else
		{
			m_heroMoveBtn[6].spriteName = "btn_down1";
		}
		if(joyPosX>0&&(joyPosY/joyPosX)<=-0.4142f&&(joyPosY/joyPosX)>=-2.4142f)	//按下右下按钮
		{
			m_heroMoveBtn[7].spriteName = "btn_dR2";
			m_heroMoveBtn[7].depth = 5;
		}
		else
		{
			m_heroMoveBtn[7].spriteName = "btn_dR1";
			m_heroMoveBtn[7].depth = 2;
		}
		m_heroMoveBtn[8].spriteName = "btnBkg_dir2";
	}

	void HeroInjuryBlinkCheck()
	{
		if(m_heroBlinkCount==0)													//当前未闪烁
		{
			if(LevelOneGameManager.Instance.GetHeroInjury())					//如果主角受伤
			{
                m_heroBlinkCount = 1;											//开始第一次闪
				this.GetComponent<SpriteRenderer>().color = new Color(0,0,0);	//图片变黑
				m_heroBlinkTimer = 0;											//计时器就位
			}
		}
		else 																	//当前主角在闪烁
		{
			m_heroBlinkTimer += Time.deltaTime;									//开启计时器
			if(m_heroBlinkTimer>=0.2f)											//需要闪
			{						
				m_heroBlinkCount ++;											//换图次数增加
				m_heroBlinkTimer = 0f;											//计时器归位
				if(m_heroBlinkCount==3)											//根据次数判定要显示的主角图shader
					this.GetComponent<SpriteRenderer>().color = new Color(0,0,0);
				else if(m_heroBlinkCount==2||m_heroBlinkCount==4)
					this.GetComponent<SpriteRenderer>().color = new Color(1,1,1);
				else if(m_heroBlinkCount==5)									//闪烁三次后
				{
					m_heroBlinkCount = 0;										//受伤模式结束
					LevelOneGameManager.Instance.SetHeroInjury(false);			//受伤闪烁结束
				}
			}
		}
	}

	void LateUpdate()					
	{
		HeroInjuryBlinkCheck();													//检测主角此时是否需要受伤闪烁

		Bounds rr1 = m_heroCollider.GetComponent<Collider2D>().bounds;						//主角的包围盒
		Rect r1 = new Rect(rr1.center.x - rr1.size.x / 2,
		                   rr1.center.y - rr1.size.y / 2,
		                   rr1.size.x, rr1.size.y);

        for (int i=0; i<m_mapCollider.Length; i++)
		{
			Bounds rr2 = m_mapCollider[i].GetComponent<Collider>().bounds;
			Rect r2 = new Rect(rr2.center.x - rr2.size.x / 2, rr2.center.y - rr2.size.y / 2, rr2.size.x, rr2.size.y);
			if(r1.Overlaps(r2))												
			{
				if(m_climbState==0||m_climbState==1)					//当时未处于爬行状态
					m_climbState = 1;									//处于可爬行状态
				break;
			}
			else if(i==m_mapCollider.Length-1)								//未碰到需要爬行的地块
			{
				if(m_climbState==1)											//之前有碰过
					m_climbState = 0;										//归零
				else if(m_climbState==2)									//目前正在爬行
					m_climbState = 3;										//爬行结束
			}
		}

		if(m_climbState<2)													//如果不是爬行状态
		{
			if(this.GetComponent<Rigidbody2D>().velocity.y<-2f)								//如果主角具有向下的速度
			{
				if(onVerticalStateChange!=null)								//转为下落状态
					onVerticalStateChange(LevelOneHeroController.heroVerticalStates.falling);		
				/*if(m_climbState==1)											//如果主角碰到爬行图块
					m_climbState = 4;										//准备爬行*/
			}
			else 															//当前不处于下落状态
			{
				if(m_jumpBtnCheck)											//按下跳跃键
				{
					if(m_climbState==0)										//当前未碰到爬行图块
					{
						if(onVerticalStateChange!=null)						//跳跃状态
							onVerticalStateChange(LevelOneHeroController.heroVerticalStates.jump);			
					}
					else if(m_climbState==1)								//当前碰到爬行图块
						m_climbState = 4;									//准备爬行
				}
				else if(m_downBtnCheck)										//按下下行键
				{

				}
				else 														//竖直方向按键均未按下
				{
					if(onVerticalStateChange!=null)							//竖直方向上为空闲状态
						onVerticalStateChange(LevelOneHeroController.heroVerticalStates.idle);			
				}
			}
			if(m_leftBtnCheck)											//按下左移方向键
			{
				if(onHorizontalStateChange!=null)						//向左移动
					onHorizontalStateChange(heroHorizontalStates.left);
			}
			else if(m_rightBtnCheck)									//按下右移方向键
			{	
				if(onHorizontalStateChange!=null)						//向右移动
					onHorizontalStateChange(heroHorizontalStates.right);
			}
			else 														//水平按键未按下
			{
				if(onHorizontalStateChange!=null)						//水平方向上为空闲状态
					onHorizontalStateChange(heroHorizontalStates.idle);
			}
		}
		else if(m_climbState==2)											//当前处于爬行状态
		{
			if(m_leftBtnCheck)												//按下左移按钮
			{
				if(onClimbHorizontalStateChange!=null)						//向左爬
					onClimbHorizontalStateChange(heroClimbHorizontalStates.leftClimb);
			}
			else if(m_rightBtnCheck)										//按下右移按钮
			{
				if(onClimbHorizontalStateChange!=null)						//向右爬
					onClimbHorizontalStateChange(heroClimbHorizontalStates.rightClimb);
			}
			else 															//水平按键未按下
			{
				if(onClimbHorizontalStateChange!=null)						//水平方向为空闲状态
					onClimbHorizontalStateChange(heroClimbHorizontalStates.idleClimb);
			}

			if(m_jumpBtnCheck)												//按下向上按钮
			{
				if(onClimbVerticalStateChange!=null)						//向上爬
					onClimbVerticalStateChange(heroClimbVerticalStates.upClimb);
			}
			else if(m_downBtnCheck)											//按下向下按钮
			{
				if(onClimbVerticalStateChange!=null)						//向下爬
					onClimbVerticalStateChange(heroClimbVerticalStates.downClimb);
			}
			else 															//竖直方向未按下
			{
				if(onClimbVerticalStateChange!=null)						//竖直方向为空闲状态
					onClimbVerticalStateChange(heroClimbVerticalStates.idleClimb);
			}
		}
		if(m_climbState==3) 												//爬行时脱离可爬行图块
		{
			this.GetComponent<Rigidbody2D>().gravityScale = 5f;				//恢复重力影响
			if(onClimbHorizontalStateChange!=null)							//爬行水平方向转为空闲状态
				onClimbHorizontalStateChange(LevelOneHeroController.heroClimbHorizontalStates.idle);
			if(onClimbVerticalStateChange!=null)							//爬行竖直方向转为空闲状态
				onClimbVerticalStateChange(LevelOneHeroController.heroClimbVerticalStates.idle);
		}
		else if(m_climbState==5)											//爬行准备过程结束 平时动画归于空闲
		{
			m_climbState = 2;												//进入爬行状态	
			this.GetComponent<Rigidbody2D>().gravityScale = 0f;				//取消重力影响
			if(onVerticalStateChange!=null)									//竖直方向转为空闲
				onVerticalStateChange(LevelOneHeroController.heroVerticalStates.idle);	
			if(onClimbHorizontalStateChange!=null)							//水平转为爬行水平空闲状态
				onClimbHorizontalStateChange(LevelOneHeroController.heroClimbHorizontalStates.idleClimb);
			if(onClimbVerticalStateChange!=null)							//竖直转为爬行竖直空闲状态
				onClimbVerticalStateChange(LevelOneHeroController.heroClimbVerticalStates.idleClimb);
		}
		else if(m_climbState==6)											//爬行过程中遇到平台
		{
			this.GetComponent<Rigidbody2D>().gravityScale = 5f;				//恢复重力影响
			if(onClimbHorizontalStateChange!=null)							//爬行水平归于空闲状态
				onClimbHorizontalStateChange(LevelOneHeroController.heroClimbHorizontalStates.idle);
			if(onClimbVerticalStateChange!=null)							//爬行竖直归于空闲状态
				onClimbVerticalStateChange(LevelOneHeroController.heroClimbVerticalStates.idle);
			m_climbState = 0;												//爬行状态结束归零
			
		}
	}
}
