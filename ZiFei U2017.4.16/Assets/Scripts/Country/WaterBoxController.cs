using UnityEngine;
using System.Collections;

public class WaterBoxController : MonoBehaviour 
{
	public GameObject m_waterSwitch;									//水阀开关

	private Animator m_waterSwitchAnimator;								//水阀开关动画组件
	private Animator m_waterBoxAnimator;								//水位箱动画组件
	private int m_waterBoxState = 0;										//水位箱状态

	void Awake()
	{
		m_waterSwitchAnimator = m_waterSwitch.GetComponent<Animator>();	//获取水阀开关动画组件
		m_waterBoxAnimator = this.GetComponent<Animator> ();			//获取水位箱动画组件


    }

    private void OnEnable()
    {
        ResetWaterSwitchState();

    }

    public void ResetWaterSwitchState()
    {

        if (GameManager.Instance.GetWaterBoxRepairState() == 2)
        {
            m_waterSwitchAnimator.Play("waterSwitchCloseAnimation");

        }
        else {

            m_waterBoxState = 0;
            m_waterSwitchAnimator.Play("waterSwitchOpenAnimation");
            
        }
    }

    void Update()
	{
        if (m_waterBoxState >= 4) return;


        switch (m_waterBoxState)
		{
		case 0:
			if(GameManager.Instance.GetWaterBoxRepairState()==2)
				m_waterBoxState = 1;
			break;
		case 1:
                m_waterSwitchAnimator.Play("waterSwitchDownAnimation");
			m_waterBoxState = 2;
			break;
		case 2:
            AnimatorStateInfo info = m_waterSwitchAnimator.GetCurrentAnimatorStateInfo(0);
            if (info.IsName ("waterSwitchCloseAnimation"))	
				m_waterBoxState = 3;
			break;
		case 3:
			m_waterBoxAnimator.SetBool("waterBoxDecline", true);
			GameManager.Instance.SetTaskIndexState(4,2);						//完成水手任务
			m_waterBoxState = 4;
			break;
		}
	}
}
