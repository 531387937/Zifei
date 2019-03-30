using UnityEngine;
using System.Collections;

public class DrunkardStateListener : MonoBehaviour 
{
	public Animator m_drunkardAnimator ;													//酒鬼上的animator组件
	private DrunkardStateController.m_drunkardStates m_drunkardCurrState = DrunkardStateController.m_drunkardStates.idle;
	

	//void OnEnable()																				//对象可用时 加入到订阅者列表中
	//{
	//	DrunkardStateController.onStateChange += OnStateChange;
	//}
	//void OnDisable()																			//不可用时，从订阅者列表中退出
	//{
	//	DrunkardStateController.onStateChange -= OnStateChange;
	//}
	


	//bool CheckForValidState(DrunkardStateController.m_drunkardStates newState)					//检测状态是否可以转变
	//{
	//	bool _returnVal = true;																	//默认不可转
	//	return _returnVal;
	//}
	
	public void OnStateChange(DrunkardStateController.m_drunkardStates _newState)				//酒鬼状态改变时调用
	{
        if (_newState == m_drunkardCurrState)                                                       //酒鬼状态未发生改变
            return;
        //if (!CheckForValidState(_newState))                                                     //酒鬼状态不可转
        //    return;
        switch (_newState)																		//判断新状态
		{
		case DrunkardStateController.m_drunkardStates.idle:										//如果是空闲
			m_drunkardAnimator.SetBool("drunkardDia", false);
            if (m_drunkardCurrState == DrunkardStateController.m_drunkardStates.down)
                m_drunkardAnimator.SetTrigger("drunkardInit");

             break;
		case DrunkardStateController.m_drunkardStates.dialog:									//如果是对话
			m_drunkardAnimator.SetBool("drunkardDia", true);
			break;
		case DrunkardStateController.m_drunkardStates.down:										//如果是倒地
			m_drunkardAnimator.SetTrigger("drunkardDown");
			break;
		}
		m_drunkardCurrState = _newState;														//更新酒鬼状态
	}

    public void PlayDrunkAnimationClip(string animationClip) {
        m_drunkardAnimator.Play(animationClip);

    }
}

