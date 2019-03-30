using UnityEngine;
using System.Collections;

public class SailorStateListener : MonoBehaviour 
{
	private Animator m_sailorAnimator = null;														//水手上的animator组件
	private SailorStateController.m_sailorStates m_sailorCurrState = SailorStateController.m_sailorStates.idle;

	void Start()
	{
		m_sailorAnimator = this.GetComponent<Animator> ();											//获取水手的动画组件
	}

	void OnEnable()																					//对象可用时 加入到订阅者列表中
	{
		SailorStateController.onStateChange += OnStateChange;
	}
	void OnDisable()																				//不可用时，从订阅者列表中退出
	{
		SailorStateController.onStateChange -= OnStateChange;
	}

	bool CheckForValidState(SailorStateController.m_sailorStates newState)
	{
		bool _returnVal = true;																		//默认不可转
		return _returnVal;
	}

	public void OnStateChange(SailorStateController.m_sailorStates _newState)						//水手状态改变时调用
	{
		if(_newState==m_sailorCurrState)
			return;
		if(!CheckForValidState(_newState))
			return;
		switch(_newState)
		{
		case SailorStateController.m_sailorStates.idle:
			m_sailorAnimator.SetBool("sailorDia", false);
			break;
		case SailorStateController.m_sailorStates.dialog:
			m_sailorAnimator.SetBool("sailorDia", true);
			break;
		}
		m_sailorCurrState = _newState;
	}
}
