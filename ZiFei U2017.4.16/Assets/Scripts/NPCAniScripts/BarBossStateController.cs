using UnityEngine;
using System.Collections;

public class BarBossStateController : MonoBehaviour 
{
	public enum m_barBossStates																			//酒保状态
	{	
		head = 0, 																							//摇头晃脑状态
		dialog,																								//对话状态
		coin,																								//扔硬币状态
	}
	
	public delegate void tieJiangStateHandler(BarBossStateController.m_barBossStates _newState);			//定义委托和事件
	public static event tieJiangStateHandler onStateChange; 
	private int m_barBossCheck = 0;																			//酒保阶段
	
	void LateUpdate()		
	{
		if(CheckBtnController.Instance.GetDialogIndex()==5)											//对话的对象是酒保
		{
			int _diaState = CheckBtnController.Instance.GetDialogState ();
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
				CheckBtnController.Instance.SetDialogState(1);
			}
			else if((_diaState==2||_diaState==4))
			{
				if(onStateChange!=null)
					onStateChange(BarBossStateController.m_barBossStates.dialog);								//对话状态
				if(m_barBossCheck==0)
					m_barBossCheck = 1;
			}
			else
			{
				if(m_barBossCheck==0)
				{
					if(onStateChange!=null)
						onStateChange(BarBossStateController.m_barBossStates.head);								//空闲状态
				}
				else
				{
					if(onStateChange!=null)
						onStateChange(BarBossStateController.m_barBossStates.coin);								//空闲状态
				}
				
			}
		}
		else
		{
			if(m_barBossCheck==0)
			{
				if(onStateChange!=null)
					onStateChange(BarBossStateController.m_barBossStates.head);								//空闲状态
			}
			else
			{
				if(onStateChange!=null)
					onStateChange(BarBossStateController.m_barBossStates.coin);								//空闲状态
			}
		}
	}
}
