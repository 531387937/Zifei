using UnityEngine;
using System.Collections;

public class BarBossInStateListener : MonoBehaviour 
{
	private Animator m_barBossInAnimator = null;													//酒馆内老板上的animator组件
	private BarBossInStateController.m_barBossInStates m_barBossInCurrState = BarBossInStateController.m_barBossInStates.idle;
	
	void Start()
	{
		m_barBossInAnimator = this.GetComponent<Animator> ();										//获取酒馆内老板的动画组件
	}
	
	void OnEnable()																					//对象可用时 加入到订阅者列表中
	{
		BarBossInStateController.onStateChange += OnStateChange;
	}
	void OnDisable()																				//不可用时，从订阅者列表中退出
	{
		BarBossInStateController.onStateChange -= OnStateChange;
	}
	
	bool CheckForValidState(BarBossInStateController.m_barBossInStates newState)
	{
		bool _returnVal = true;																		//默认不可转
		return _returnVal;
	}
	
	public void OnStateChange(BarBossInStateController.m_barBossInStates _newState)					//酒馆内老板状态改变时调用
	{
		if(_newState==m_barBossInCurrState)
			return;
		if(!CheckForValidState(_newState))
			return;
		switch(_newState)
		{
		case BarBossInStateController.m_barBossInStates.idle:
			m_barBossInAnimator.SetBool("barBossInDia", false);
			break;
		case BarBossInStateController.m_barBossInStates.dialog:
			m_barBossInAnimator.SetBool("barBossInDia", true);
			break;
		}
		m_barBossInCurrState = _newState;
	}
}
