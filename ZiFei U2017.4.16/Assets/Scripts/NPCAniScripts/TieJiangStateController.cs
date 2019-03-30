using UnityEngine;
using System.Collections;

public class TieJiangStateController : MonoBehaviour 
{
	public enum m_tieJiangStates																			//铁匠状态
	{	
		idle = 0, 																							//空闲状态
		dialog,																								//对话状态
	}

	public delegate void tieJiangStateHandler(TieJiangStateController.m_tieJiangStates _newState);			//定义委托和事件
	public static event tieJiangStateHandler onStateChange; 

	void LateUpdate()		
	{
		if(CheckBtnController.Instance.GetDialogIndex()==2)
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
					onStateChange(TieJiangStateController.m_tieJiangStates.dialog);								//对话状态
			}
			else
			{
				if(onStateChange!=null)
					onStateChange(TieJiangStateController.m_tieJiangStates.idle);								//空闲状态
			}
		}
		else
		{
			if(onStateChange!=null)
				onStateChange(TieJiangStateController.m_tieJiangStates.idle);								//空闲状态
		}
	}
}
