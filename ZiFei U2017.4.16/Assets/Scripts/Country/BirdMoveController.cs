using UnityEngine;
using System.Collections;

public class BirdMoveController : MonoBehaviour
{
	public GameObject m_heroBird;										//鸟中的主角
	public Transform [] m_jumpPathOne;									//飞行和跳跃的路线
	public Transform [] m_jumpPathTwo;
	public Transform [] m_flyPath;
	public Transform[] m_birdPos;										//麻雀位置相关 初始位置 左边界 右边界

	private Animator m_birdAnimator = null;								//麻雀上的animator组件
	private bool m_jump = false;										//保证只执行一次
	private int m_birdFlyAwayState = 0;									//麻雀飞走状态	

	void Start()
	{
		m_birdAnimator = this.GetComponent<Animator> ();				//获取麻雀的动画组件
	}

	void Update()
	{
		Vector3 _localScale = this.transform.localScale;				//麻雀朝向
		AnimatorStateInfo  info = m_birdAnimator.GetCurrentAnimatorStateInfo(0);
	
		if(m_birdFlyAwayState==0)
		{
			if(info.IsName ("BirdStandAnimation"))							//站立状态
			{
				m_birdAnimator.SetBool("birdReStart", false);
				if(_localScale.x<0)											//保证朝向
				{
					_localScale.x *= -1f;
					this.transform.localScale = _localScale;
				}
			}
			if(info.IsName ("BirdJumpAnimation"))							//跳跃状态
			{
				if(!m_jump)
				{
					m_jump = true;											
					iTween.MoveTo(gameObject, iTween.Hash("path",m_jumpPathOne,"time",0.6f,"onComplete","JumpOneEnd","easyType",iTween.EaseType.easeInCubic));
				}
			}
			if(info.IsName ("BirdPeckRAnimation 0"))						//飞行完后啄地状态
			{
				m_birdAnimator.SetBool ("flyEnd", false);
				m_jump = false;
			}
			if(info.IsName ("BirdPeckRAnimation"))							//跳跃完后啄地状态
			{
				m_birdAnimator.SetBool ("jumpEnd", false);
				m_jump = false;
			}
			if(info.IsName ("BirdFlyAnimation"))							//飞行状态
			{
				if(_localScale.x>0)											//改变朝向
				{
					_localScale.x *= -1f;
					this.transform.localScale = _localScale;
				}
				if(!m_jump)
				{
					m_jump = true;
					iTween.MoveTo(gameObject, iTween.Hash("path",m_flyPath,"time",1f,"onComplete","flyEnd","easyType",iTween.EaseType.easeInCubic));
				}
			}

			Bounds rr1 = m_heroBird.GetComponent<Collider2D>().bounds;						//主角的包围盒
			Rect r1 = new Rect(rr1.center.x - rr1.size.x / 2,
			                   rr1.center.y - rr1.size.y / 2,
			                   rr1.size.x, rr1.size.y);
			Bounds rr2 = this.GetComponent<Collider>().bounds;
			Rect r2 = new Rect(rr2.center.x - rr2.size.x / 2, rr2.center.y - rr2.size.y / 2, rr2.size.x, rr2.size.y);
			if(r1.Overlaps(r2))												//主角碰到麻雀包围盒
			{
				m_birdFlyAwayState = 1;
				//iTween.Stop();
				m_birdAnimator.SetBool("jumpEnd", false);
				m_birdAnimator.SetBool("flyEnd", false);
				m_birdAnimator.SetBool("birdFlyAway", true);
			}
		}
		if(m_birdFlyAwayState==1)
		{
			if(info.IsName ("BirdFlyAwayAnimation"))
			{
				m_birdFlyAwayState = 2;
				if(_localScale.x>0)	
					iTween.MoveTo(this.gameObject,iTween.Hash("x",2f,"y",15,"onComplete","FlyAwayEnd","easyType",iTween.EaseType.linear,"speed",2f));
				else
					iTween.MoveTo(this.gameObject,iTween.Hash("x",-2f,"y",15,"onComplete","FlyAwayEnd","easyType",iTween.EaseType.linear,"speed",2f));

			}				
		}
		if(m_birdFlyAwayState==3)
		{
			if(m_heroBird.transform.position.x<m_birdPos[1].position.x||m_heroBird.transform.position.x>m_birdPos[2].position.x)
			{
				this.transform.position = m_birdPos[0].position;
				m_birdFlyAwayState = 0;
				m_birdAnimator.SetBool("birdFlyAway", false);
				m_birdAnimator.SetBool("birdReStart", true);
				m_jump = false;
			}
		}
	}

	void JumpOneEnd()													//第一次跳跃完成
	{
		iTween.MoveTo(gameObject, iTween.Hash("path",m_jumpPathTwo,"time",0.6f,"onComplete","JumpTwoEnd","easyType",iTween.EaseType.easeInCubic));
	}
	void JumpTwoEnd()													//跌日吃跳跃完成
	{
		m_birdAnimator.SetBool ("jumpEnd", true);
	}
	void flyEnd()														//飞行结束
	{
		m_birdAnimator.SetBool ("flyEnd", true);
	}
	void FlyAwayEnd()
	{
		m_birdFlyAwayState = 3;
	}
}
