using UnityEngine;
using System.Collections;

public class DrunkardStateController : MonoBehaviour 
{
    private DrunkardStateListener drunkardStateListener;
	public enum m_drunkardStates																	//酒鬼状态
	{	
		idle = 0, 																					//空闲状态
		dialog,																						//对话状态
		down,																						//倒地睡觉状态
	}
	
	//public delegate void drunkardStateHandler(m_drunkardStates _newState);	//定义委托和事件
	//public static event drunkardStateHandler onStateChange;


    private void Awake()
    {
        drunkardStateListener = GetComponent<DrunkardStateListener>();
    }

    private void OnEnable()
    {
        InitDrunkardState();
    }

    public void InitDrunkardState()
    {

        if (GameManager.Instance.GetWaterBoxRepairState() >= 1)
        {
            drunkardStateListener.PlayDrunkAnimationClip("DrunkardDownAnimation");//酒鬼倒下状态
        }
        else
        {
            drunkardStateListener.PlayDrunkAnimationClip("DrunkardIdleAnimation");

        }

    }


    void LateUpdate()		
	{
        //InitDrunkardState();

        if (GameManager.Instance.GetWaterBoxRepairState() < 1)
		{
			if(CheckBtnController.Instance.GetDialogIndex()==4)											//如果主角和酒鬼说话
			{
				int _diaState = CheckBtnController.Instance.GetDialogState ();							//获取对话状态
				if(_diaState==-1)
				{
					Vector3 _localScale = this.transform.localScale;									//NPC朝向
					if(CheckBtnController.Instance.GetNPCDirection())									//NPC应该朝左
					{
						if(_localScale.x>0)
						{
							_localScale.x *= -1f;
							this.transform.localScale = _localScale;
						}
					}
					else 																				//NPC应该朝右
					{
						if(_localScale.x<0)
						{
							_localScale.x *= -1f;
							this.transform.localScale = _localScale;
						}
					}
					CheckBtnController.Instance.SetDialogState(1);										//转向完毕 弹出对话框
				}
				else if((_diaState==2||_diaState==4))													//如果正在弹出对话框
				{
                        drunkardStateListener.OnStateChange(m_drunkardStates.dialog);					//酒鬼处于对话状态
				}
				else 																					//否则
				{
                        drunkardStateListener.OnStateChange(m_drunkardStates.idle);					//酒鬼空闲状态
				}
			}
			//else 
			//{
   //                 drunkardStateListener.OnStateChange(m_drunkardStates.idle);						//酒鬼空闲状态
			//}
		}else
            drunkardStateListener.OnStateChange(m_drunkardStates.down);                       //酒鬼倒下状态


    }
}

