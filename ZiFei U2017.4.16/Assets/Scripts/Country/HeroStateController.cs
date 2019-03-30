using UnityEngine;
using System.Collections;

public class HeroStateController : MonoBehaviour
{

    public enum heroHorizontalStates                                                            //主角水平方向的状态
    {
        idle = 0,                                                                               //空闲状态
        left,                                                                                   //向左状态
        right,                                                                                  //向右
    }

    public enum heroVerticalStates                                                              //主角竖直方向上的状态
    {
        idle = 0,                                                                               //空闲状态
        jump,                                                                                   //跳跃状态
        down,                                                                                   //下行
        falling,                                                                                //下落
        landing,                                                                                //落地

    }

    //public delegate void heroHorizontalStateHandler(HeroStateController.heroHorizontalStates _newState);//定义委托和事件
    //public static event heroHorizontalStateHandler onHorizontalStateChange;

    //public delegate void heroVerticalStateHandler(HeroStateController.heroVerticalStates _newState);//定义委托和事件
    //public static event heroVerticalStateHandler onVerticalStateChange;


    public UISprite[] m_heroMoveBtn;

    private bool m_joyStickSound = false;                                       //摇杆音效是否播放
    private bool m_leftBtnCheck = false;                                        //左移按钮是否按下变量
    private bool m_rightBtnCheck = false;                                       //右移按钮是否按下变量
    private bool m_downBtnCheck = false;                                        //下行按钮是否按下变量
    private bool m_jumpBtnCheck = false;                                        //跳跃按钮是否按下变量

    private HeroStateListener heroStateListener;


    void OnEnable()                                                             //注册摇杆事件
    {
        EasyJoystick.On_JoystickMove += OnJoystickMove;
        EasyJoystick.On_JoystickMoveEnd += OnJoystickMoveEnd;

        heroStateListener = GetComponent<HeroStateListener>();


    }

    void OnDisable()                                                            //移除摇杆事件
    {
        EasyJoystick.On_JoystickMove -= OnJoystickMove;
        EasyJoystick.On_JoystickMoveEnd -= OnJoystickMoveEnd;
    }

    void OnDistroy()                                                            //移除摇杆事件
    {
        EasyJoystick.On_JoystickMove -= OnJoystickMove;
        EasyJoystick.On_JoystickMoveEnd -= OnJoystickMoveEnd;
    }

    void OnJoystickMoveEnd(MovingJoystick move)                                 //摇杆处于停止状态，角色进入等待动画
    {
        m_joyStickSound = false;
        m_leftBtnCheck = false;
        m_rightBtnCheck = false;
        m_downBtnCheck = false;
        m_jumpBtnCheck = false;

        m_heroMoveBtn[0].spriteName = "btn_right1";                             //恢复静止状态贴图
        m_heroMoveBtn[1].spriteName = "btn_uR1";
        m_heroMoveBtn[2].spriteName = "btn_up1";
        m_heroMoveBtn[3].spriteName = "btn_uL1";
        m_heroMoveBtn[4].spriteName = "btn_left1";
        m_heroMoveBtn[5].spriteName = "btn_dL1";
        m_heroMoveBtn[6].spriteName = "btn_down1";
        m_heroMoveBtn[7].spriteName = "btn_dR1";
        m_heroMoveBtn[8].spriteName = "btnBkg_dir1";

        m_heroMoveBtn[1].depth = 2;                                             //恢复摇杆按钮的深度
        m_heroMoveBtn[3].depth = 2;
        m_heroMoveBtn[5].depth = 2;
        m_heroMoveBtn[7].depth = 2;

    }


    private void Update()
    {
        if (GameManager.Instance.isPlatformType_Mobild) return;

        if (!GameManager.Instance.isUsualBtnCanClick) return;

        var horizontalValue = Input.GetAxis("Horizontal");
        var verticalValue = Input.GetAxis("Vertical");


        m_heroMoveBtn[0].spriteName = "btn_right1";                             //恢复静止状态贴图
        m_heroMoveBtn[1].spriteName = "btn_uR1";
        m_heroMoveBtn[2].spriteName = "btn_up1";
        m_heroMoveBtn[3].spriteName = "btn_uL1";
        m_heroMoveBtn[4].spriteName = "btn_left1";
        m_heroMoveBtn[5].spriteName = "btn_dL1";
        m_heroMoveBtn[6].spriteName = "btn_down1";
        m_heroMoveBtn[7].spriteName = "btn_dR1";
        m_heroMoveBtn[8].spriteName = "btnBkg_dir1";

        m_heroMoveBtn[1].depth = 2;                                             //恢复摇杆按钮的深度
        m_heroMoveBtn[3].depth = 2;
        m_heroMoveBtn[5].depth = 2;
        m_heroMoveBtn[7].depth = 2;

        if (horizontalValue != 0 || verticalValue != 0)
            AudioManager.Instance.HeroSoundPlay(Global.GetInstance().audioName_JoyStick);
      

        if (horizontalValue > 0) m_rightBtnCheck = true; else m_rightBtnCheck = false;

        if (horizontalValue < 0) m_leftBtnCheck = true; else m_leftBtnCheck = false;

        if (verticalValue > 0) m_jumpBtnCheck = true; else m_jumpBtnCheck = false;

        if (verticalValue < 0) m_downBtnCheck = true; else m_downBtnCheck = false;



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

    void OnJoystickMove(MovingJoystick move)                                    //当摇杆处于移动状态，角色开始奔跑
    {
        if (move.joystickName != "EasyJoystick")                                    //如果不是此摇杆，退出
            return;
		
        if (!m_joyStickSound)
        {
            m_joyStickSound = true;
            AudioManager.Instance.HeroSoundPlay(Global.GetInstance().audioName_JoyStick);

        }


        float joyPosX = move.joystickAxis.x;                                    //获取摇杆偏移量
        float joyPosY = move.joystickAxis.y;

        if (joyPosX > 0 && (joyPosY / joyPosX) > -2.4142f && (joyPosY / joyPosX) < 2.4142f) //是否方向为右
            m_rightBtnCheck = true;
        else
            m_rightBtnCheck = false;
        if (joyPosX < 0 && (joyPosY / joyPosX) > -2.4142f && (joyPosY / joyPosX) < 2.4142f) //是否方向为左
            m_leftBtnCheck = true;
        else
            m_leftBtnCheck = false;
        if (joyPosY > 0 && ((joyPosY / joyPosX) < -0.4142f || (joyPosY / joyPosX) > 0.4142f))   //是否方向为上
            m_jumpBtnCheck = true;
        else
            m_jumpBtnCheck = false;
        if (joyPosY < 0 && ((joyPosY / joyPosX) < -0.4142f || (joyPosY / joyPosX) > 0.4142f))   //是否方向为下
            m_downBtnCheck = true;
        else
            m_downBtnCheck = false;

        if (joyPosX > 0 && (joyPosY / joyPosX) >= -0.4142f && (joyPosY / joyPosX) <= 0.4142f)   //按下向右按钮
        {
            m_heroMoveBtn[0].spriteName = "btn_right2";
        }
        else
        {
            m_heroMoveBtn[0].spriteName = "btn_right1";
        }
        if (joyPosX > 0 && (joyPosY / joyPosX) > 0.4142f && (joyPosY / joyPosX) < 2.4142f)      //按下右上按钮
        {
            m_heroMoveBtn[1].spriteName = "btn_uR2";
            m_heroMoveBtn[1].depth = 5;
        }
        else
        {
            m_heroMoveBtn[1].spriteName = "btn_uR1";
            m_heroMoveBtn[1].depth = 2;
        }
        if (joyPosY > 0 && ((joyPosY / joyPosX) < -2.4142f || (joyPosY / joyPosX) > 2.4142f))   //按下向上按钮
        {
            m_heroMoveBtn[2].spriteName = "btn_up2";
        }
        else
        {
            m_heroMoveBtn[2].spriteName = "btn_up1";
        }
        if (joyPosX < 0 && (joyPosY / joyPosX) <= -0.4142f && (joyPosY / joyPosX) >= -2.4142f)  //按下左上按钮
        {
            m_heroMoveBtn[3].spriteName = "btn_uL2";
            m_heroMoveBtn[3].depth = 5;
        }
        else
        {
            m_heroMoveBtn[3].spriteName = "btn_uL1";
            m_heroMoveBtn[3].depth = 2;
        }
        if (joyPosX < 0 && (joyPosY / joyPosX) >= -0.4142f && (joyPosY / joyPosX) <= 0.4142f)   //按下向左按钮
        {
            m_heroMoveBtn[4].spriteName = "btn_left2";
        }
        else
        {
            m_heroMoveBtn[4].spriteName = "btn_left1";
        }
        if (joyPosX < 0 && (joyPosY / joyPosX) > 0.4142f && (joyPosY / joyPosX) < 2.4142f)      //按下左下按钮
        {
            m_heroMoveBtn[5].spriteName = "btn_dL2";
            m_heroMoveBtn[5].depth = 5;
        }
        else
        {
            m_heroMoveBtn[5].spriteName = "btn_dL1";
            m_heroMoveBtn[5].depth = 2;
        }
        if (joyPosY < 0 && ((joyPosY / joyPosX) < -2.4142f || (joyPosY / joyPosX) > 2.4142f))   //按下向下按钮
        {
            m_heroMoveBtn[6].spriteName = "btn_down2";
        }
        else
        {
            m_heroMoveBtn[6].spriteName = "btn_down1";
        }
        if (joyPosX > 0 && (joyPosY / joyPosX) <= -0.4142f && (joyPosY / joyPosX) >= -2.4142f)  //按下右下按钮
        {
            m_heroMoveBtn[7].spriteName = "btn_dR2";
            m_heroMoveBtn[7].depth = 5;
        }
        else
        {
            m_heroMoveBtn[7].spriteName = "btn_dR1";
            m_heroMoveBtn[7].depth = 2;
        }
        m_heroMoveBtn[8].spriteName = "btnBkg_dir2";                            //改变摇杆中心图片 标注为按下状态
    }

    void LateUpdate()
    {
        if (GameManager.Instance.GetJoyStickState())
        {
            GameManager.Instance.SetJoyStickState(false);
            m_leftBtnCheck = false;
            m_rightBtnCheck = false;
            m_downBtnCheck = false;
            m_jumpBtnCheck = false;

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

	
		//---------------------------------------------------------------------------
        
		if (this.GetComponent<Rigidbody2D>().velocity.y < - 2f)                       //如果主角具有向下的速度
		{
			heroStateListener.OnVerticalStateChange(heroVerticalStates.falling);      //下落状态
		}
		else
        {
            if (m_jumpBtnCheck)                                                                 //按下跳跃键
            {
                heroStateListener.OnVerticalStateChange(heroVerticalStates.jump);         //跳跃状态
            }
            else if (m_downBtnCheck && GameManager.Instance.GetDownActive())                                       //如果此时可以向下走
            {
               	heroStateListener.OnVerticalStateChange(heroVerticalStates.down);     //下行状态
               
            }
			else 
            {
               heroStateListener.OnVerticalStateChange(heroVerticalStates.idle);         //空闲状态
            }
        }


		//--------------------------------------------------
		if (m_leftBtnCheck)
		{
			heroStateListener.OnHorizontalStateChange(heroHorizontalStates.left);
		}
		else if (m_rightBtnCheck)
		{
			heroStateListener.OnHorizontalStateChange(heroHorizontalStates.right);
		}
		else
		{
			heroStateListener.OnHorizontalStateChange(heroHorizontalStates.idle);
		}

    }
}
