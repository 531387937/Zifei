using UnityEngine;
using System.Collections;

public class LevelThreeHeroController : MonoBehaviour 
{
	public enum heroHorizontalStates															//主角水平方向的状态
	{	
		idle = 0, 																				//空闲状态
		left,																					//向左状态
		right,																					//向右
	}
	
	public enum heroVerticalStates																//主角竖直方向上的状态
	{	
		idle = 0, 																				//空闲状态
		jump,																					//跳跃状态
		down,																					//下行
		falling,																				//下落
		landing,																				//落地
		
	}
	public delegate void heroHorizontalStateHandler(LevelThreeHeroController.heroHorizontalStates _newState);//定义委托和事件
	public static event heroHorizontalStateHandler onHorizontalStateChange; 
	
	
	public delegate void heroVerticalStateHandler(LevelThreeHeroController.heroVerticalStates _newState);//定义委托和事件
	public static event heroVerticalStateHandler onVerticalStateChange; 
	
	public UISprite[] m_heroMoveBtn;


	private bool m_joyStickDown = false;
	private bool m_leftBtnCheck = false;				//左移按钮是否按下变量
	private bool m_rightBtnCheck = false;				//右移按钮是否按下变量
	private bool m_downBtnCheck = false;				//下行按钮是否按下变量
	private bool m_jumpBtnCheck = false;				//跳跃按钮是否按下变量
	private bool m_downEnable = false;					//是否可以向下走
	private int m_heroBlinkCount = 0;					//主角受伤闪烁次数
	private float m_heroBlinkTimer = 0f;				//主角受伤闪烁计时器


    void OnEnable()										//摇杆可用时注册事件
	{
		EasyJoystick.On_JoystickMove += OnJoystickMove;
		EasyJoystick.On_JoystickMoveEnd += OnJoystickMoveEnd;


    }
    private void Update()
    {
        if (LevelThreeGameManager.Instance.isPlatformType_Mobild) return;

        if (!LevelThreeGameManager.Instance.isUsualBtnCanClick) return;

        var horizontalValue = Input.GetAxis("Horizontal");
        var verticalValue = Input.GetAxis("Vertical");

        if (horizontalValue > 0) m_rightBtnCheck = true; else m_rightBtnCheck = false;

        if (horizontalValue < 0) m_leftBtnCheck = true; else m_leftBtnCheck = false;

        if (verticalValue > 0) m_jumpBtnCheck = true; else m_jumpBtnCheck = false;

        if (verticalValue < 0) m_downBtnCheck = true; else m_downBtnCheck = false;

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
		m_joyStickDown = false;

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
	
	void OnJoystickMove(MovingJoystick move)			//当摇杆处于移动状态，角色开始奔跑
	{
		if(move.joystickName != "EasyJoystick")			//如果不是此摇杆，退出
			return;

		if(!m_joyStickDown)
		{
			m_joyStickDown = true;
            AudioManager.Instance.HeroSoundPlay(Global.GetInstance().audioName_JoyStick);
		}

		float joyPosX = move.joystickAxis.x;			//获取摇杆偏移量
		float joyPosY = move.joystickAxis.y;
		
		if(joyPosX>0&&(joyPosY/joyPosX)>-2.4142f&&(joyPosY/joyPosX)<2.4142f)	//是否方向为右
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
	
	void OnTriggerEnter2D(Collider2D colliderObj)					//进入碰撞检测区域
	{
		
		switch(colliderObj.tag)
		{
		case "PlatformBottom":										//最底层 不能下行区域
			m_downEnable = false;
			onVerticalStateChange(heroVerticalStates.landing);
			LevelThreeHeroListener.m_invisibleColliderObj = colliderObj.gameObject;
			LevelThreeGameManager.Instance.SetHeroOnMovePanel(0, false);				//主角不随平板左右移动
			LevelThreeGameManager.Instance.SetHeroOnMovePanel(1, false);
			break;
		case "Platform":															//主角落到地面上时触发landing状态
			m_downEnable = true;
			onVerticalStateChange(heroVerticalStates.landing);
			LevelThreeHeroListener.m_invisibleColliderObj = colliderObj.gameObject;


            if (colliderObj.name=="groundCollider5")									//如果主角站在移动的平板上
			{
				LevelThreeGameManager.Instance.SetHeroOnMovePanel(0, true);			//主角随平板移动
				LevelThreeGameManager.Instance.SetHeroOnMovePanel(1, false);		//主角随平板移动
			}
			else if(colliderObj.name=="groundCollider3")									//如果主角站在移动的平板上
			{
				LevelThreeGameManager.Instance.SetHeroOnMovePanel(0, false);		//主角随平板移动
				LevelThreeGameManager.Instance.SetHeroOnMovePanel(1, true);			//主角随平板移动
			}
			else 																	//主角接触的不是移动平板
			{
				LevelThreeGameManager.Instance.SetHeroOnMovePanel(0, false);
				LevelThreeGameManager.Instance.SetHeroOnMovePanel(1, false);
			}
			break; 
		}
	}

	void OnTriggerExit2D(Collider2D _colliderObj)									//退出碰撞盒侦听
	{
        LevelThreeHeroListener.m_invisibleColliderObj = null;

        if (_colliderObj.name=="groundCollider5")									//主角离开水平移动的平板
			LevelThreeGameManager.Instance.SetHeroOnMovePanel(0, false);		
		else if(_colliderObj.name=="groundCollider3")								//主角离开竖直移动的平板
			LevelThreeGameManager.Instance.SetHeroOnMovePanel(1, false);			//主角随平板移动
	}

	void HeroInjuryBlinkCheck()													//主角受伤闪烁
	{
		if(m_heroBlinkCount==0)													//当前未闪烁
		{
			if(LevelThreeGameManager.Instance.GetHeroInjury())					//如果主角受伤
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
					LevelThreeGameManager.Instance.SetHeroInjury(false);			//受伤闪烁结束
				}
			}
		}
	}

	
	void LateUpdate()					
	{
		HeroInjuryBlinkCheck ();												//主角受伤闪烁
		//if(LevelThreeGameManager.Instance.GetJoyStickState())
		//{
		//	LevelThreeGameManager.Instance.SetJoyStickState(false);
		//	m_leftBtnCheck = false;
		//	m_rightBtnCheck = false;
		//	m_downBtnCheck = false;
		//	m_jumpBtnCheck = false;
			
		//	m_heroMoveBtn[0].spriteName = "btn_right1";		//恢复静止状态贴图
		//	m_heroMoveBtn[1].spriteName = "btn_uR1";
		//	m_heroMoveBtn[2].spriteName = "btn_up1";
		//	m_heroMoveBtn[3].spriteName = "btn_uL1";
		//	m_heroMoveBtn[4].spriteName = "btn_left1";
		//	m_heroMoveBtn[5].spriteName = "btn_dL1";
		//	m_heroMoveBtn[6].spriteName = "btn_down1";
		//	m_heroMoveBtn[7].spriteName = "btn_dR1";
		//	m_heroMoveBtn[8].spriteName = "btnBkg_dir1";
			
		//	m_heroMoveBtn[1].depth = 2;						//恢复摇杆按钮的深度
		//	m_heroMoveBtn[3].depth = 2;
		//	m_heroMoveBtn[5].depth = 2;
		//	m_heroMoveBtn[7].depth = 2;
		//}
		if(this.GetComponent<Rigidbody2D>().velocity.y<-2f)													//如果主角具有向下的速度
		{
			if(onVerticalStateChange!=null)
				onVerticalStateChange(heroVerticalStates.falling);		//下落状态
		}
		else
		{
			if(m_jumpBtnCheck)																	//按下跳跃键
			{
				if(onVerticalStateChange!=null)
					onVerticalStateChange(heroVerticalStates.jump);			//跳跃状态
			}
			else if(m_downBtnCheck)																//按下下行键
			{
				if(m_downEnable)
					if(onVerticalStateChange!=null)
						onVerticalStateChange(heroVerticalStates.down);		//下行状态
			}
			else
			{
				if(onVerticalStateChange!=null)
					onVerticalStateChange(heroVerticalStates.idle);			//空闲状态
			}
		}
		
		if(m_leftBtnCheck)
		{
			if(onHorizontalStateChange!=null)
				onHorizontalStateChange(heroHorizontalStates.left);
		}
		else if(m_rightBtnCheck)
		{
			if(onHorizontalStateChange!=null)
				onHorizontalStateChange(heroHorizontalStates.right);
		}
		else
		{
			if(onHorizontalStateChange!=null)
				onHorizontalStateChange(heroHorizontalStates.idle);
		}
	}
}

