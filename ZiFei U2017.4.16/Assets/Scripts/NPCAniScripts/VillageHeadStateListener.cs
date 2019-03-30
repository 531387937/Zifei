using UnityEngine;
using System.Collections;

public class VillageHeadStateListener : MonoBehaviour 
{
	private Animator m_villageHeadAnimator = null;													//村长的animator组件
	private VillageHeadStateController.m_villageHeadStates m_villageHeadCurrState = VillageHeadStateController.m_villageHeadStates.idle;

	void Start()
	{
		m_villageHeadAnimator = this.GetComponent<Animator> ();										//获取村长的动画组件
	}

	void OnEnable()																					//对象可用时 加入到订阅者列表中
	{
		VillageHeadStateController.onStateChange += OnStateChange;
	}
	void OnDisable()																				//不可用时，从订阅者列表中退出
	{
		VillageHeadStateController.onStateChange -= OnStateChange;
	}

	bool CheckForValidState(VillageHeadStateController.m_villageHeadStates newState)
	{
		bool _returnVal = true;																		//默认不可转
		return _returnVal;
	}

	public void OnStateChange(VillageHeadStateController.m_villageHeadStates _newState)				//村长状态改变时调用
	{
		if(_newState==m_villageHeadCurrState)
			return;
		if(!CheckForValidState(_newState))
			return;
		switch(_newState)
		{
		case VillageHeadStateController.m_villageHeadStates.idle:
			m_villageHeadAnimator.SetBool("villageHeadDia", false);
			break;
		case VillageHeadStateController.m_villageHeadStates.dialog:
			m_villageHeadAnimator.SetBool("villageHeadDia", true);
			break;
		}
		m_villageHeadCurrState = _newState;
	}
}
