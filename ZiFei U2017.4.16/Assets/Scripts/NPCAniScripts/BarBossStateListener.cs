using UnityEngine;
using System.Collections;

public class BarBossStateListener : MonoBehaviour 
{
	private Animator m_barBossAnimator = null;														//铁匠上的animator组件
	private BarBossStateController.m_barBossStates m_barBossCurrState = BarBossStateController.m_barBossStates.head;
	
	void Start()
	{
		m_barBossAnimator = this.GetComponent<Animator> ();										//获取铁匠的动画组件
	}
	
	void OnEnable()																					//对象可用时 加入到订阅者列表中
	{
		BarBossStateController.onStateChange += OnStateChange;
	}
	void OnDisable()																				//不可用时，从订阅者列表中退出
	{
		BarBossStateController.onStateChange -= OnStateChange;
	}
	
	bool CheckForValidState(BarBossStateController.m_barBossStates newState)
	{
		bool _returnVal = true;																	//默认不可转
		return _returnVal;
	}
	
	public void OnStateChange(BarBossStateController.m_barBossStates _newState)						//铁匠状态改变时调用
	{
		if(_newState==m_barBossCurrState)
			return;
		if(!CheckForValidState(_newState))
			return;
		switch(_newState)
		{
		case BarBossStateController.m_barBossStates.head:
			m_barBossAnimator.SetBool("barBossDia", false);
			break;
		case BarBossStateController.m_barBossStates.dialog:
			m_barBossAnimator.SetBool("barBossDia", true);
			break;
		case BarBossStateController.m_barBossStates.coin:
			m_barBossAnimator.SetBool("barBossCoin", true);
			break;
		}
		m_barBossCurrState = _newState;
	}
}
