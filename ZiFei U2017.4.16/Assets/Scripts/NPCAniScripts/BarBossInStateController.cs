using UnityEngine;
using System.Collections;

public class BarBossInStateController : MonoBehaviour 
{
	public enum m_barBossInStates																	//酒吧内老板状态
	{	
		idle = 0, 																					//空闲状态
		dialog,																						//对话状态
	}
	
	public delegate void barBossInStateHandler(BarBossInStateController.m_barBossInStates _newState);//定义委托和事件
	public static event barBossInStateHandler onStateChange; 
	
	void LateUpdate()		
	{
		if(CheckBtnController.Instance.GetDialogIndex()==6)											//需要对话
		{
			int _diaState = CheckBtnController.Instance.GetDialogState ();		
			if(_diaState==-1)
			{
				Vector3 _localScale = this.transform.localScale;									//NPC朝向
				if(CheckBtnController.Instance.GetNPCDirection())									//NPC应该朝左
				{
					if(_localScale.x<0)
					{
						_localScale.x *= -1f;
						this.transform.localScale = _localScale;
					}
				}
				else 																				//NPC应该朝右
				{
					if(_localScale.x>0)
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
					onStateChange(BarBossInStateController.m_barBossInStates.dialog);				//对话状态
			}
			else
			{
				if(onStateChange!=null)
					onStateChange(BarBossInStateController.m_barBossInStates.idle);					//空闲状态
			}
		}
		else
		{
			if(onStateChange!=null)
				onStateChange(BarBossInStateController.m_barBossInStates.idle);						//空闲状态
		}
	}
}
