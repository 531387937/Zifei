using UnityEngine;
using System.Collections;

public class BookBossStateListener : MonoBehaviour 
{
	private Animator m_bookBossAnimator = null;														//书店老板上的animator组件
	private BookBossStateController.m_bookBossStates m_bookBossCurrState = BookBossStateController.m_bookBossStates.idle;

	void Start()
	{
		m_bookBossAnimator = this.GetComponent<Animator> ();										//获取书店老板的动画组件
	}

	void OnEnable()																					//对象可用时 加入到订阅者列表中
	{
		BookBossStateController.onStateChange += OnStateChange;
	}
	void OnDisable()																				//不可用时，从订阅者列表中退出
	{
		BookBossStateController.onStateChange -= OnStateChange;
	}

	bool CheckForValidState(BookBossStateController.m_bookBossStates newState)
	{
		bool _returnVal = true;																		//默认不可转
		return _returnVal;
	}

	public void OnStateChange(BookBossStateController.m_bookBossStates _newState)					//书店老板状态改变时调用
	{
		if(_newState==m_bookBossCurrState)
			return;
		if(!CheckForValidState(_newState))
			return;
		switch(_newState)
		{
		case BookBossStateController.m_bookBossStates.idle:
			m_bookBossAnimator.SetBool("bookBossDia", false);
			break;
		case BookBossStateController.m_bookBossStates.dialog:
			m_bookBossAnimator.SetBool("bookBossDia", true);
			break;
		}
		m_bookBossCurrState = _newState;
	}
}
