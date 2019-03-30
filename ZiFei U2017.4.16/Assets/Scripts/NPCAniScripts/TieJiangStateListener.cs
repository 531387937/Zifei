using UnityEngine;
using System.Collections;

public class TieJiangStateListener : MonoBehaviour 
{
	private Animator m_tieJiangAnimator = null;														//铁匠上的animator组件
	private TieJiangStateController.m_tieJiangStates m_tieJiangCurrState = TieJiangStateController.m_tieJiangStates.idle;

	void Start()
	{
		m_tieJiangAnimator = this.GetComponent<Animator> ();										//获取铁匠的动画组件
	}

	void OnEnable()																					//对象可用时 加入到订阅者列表中
	{
		TieJiangStateController.onStateChange += OnStateChange;
	}
	void OnDisable()																				//不可用时，从订阅者列表中退出
	{
		TieJiangStateController.onStateChange -= OnStateChange;
	}

	bool CheckForValidState(TieJiangStateController.m_tieJiangStates newState)
	{
		bool _returnVal = true;																	//默认不可转
		return _returnVal;
	}

	public void OnStateChange(TieJiangStateController.m_tieJiangStates _newState)						//铁匠状态改变时调用
	{
		if(_newState==m_tieJiangCurrState)
			return;
		if(!CheckForValidState(_newState))
			return;
		switch(_newState)
		{
		case TieJiangStateController.m_tieJiangStates.idle:
			m_tieJiangAnimator.SetBool("tieJiangDia", false);
			break;
		case TieJiangStateController.m_tieJiangStates.dialog:
			m_tieJiangAnimator.SetBool("tieJiangDia", true);
			break;
		}
		m_tieJiangCurrState = _newState;
	}
}
