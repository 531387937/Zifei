using UnityEngine;
using System.Collections;

public class LevelOneMonkeyListener : MonoBehaviour 
{
	public GameObject m_stone;
	public Transform m_stoneStartPos;
	private bool m_stoneMake = false;


	private Animator m_monkeyAnimator = null;										//猴子上的animator组件
	private LevelOneMonkeyController.m_monkeyStates m_monkeyCurrState = LevelOneMonkeyController.m_monkeyStates.idle;
	public Animator m_monkeyChairAnimator;											//猴子底座的animator组件
	private LevelOneMonkeyController.m_monkeyChairStates m_monkeyChairCurrState = LevelOneMonkeyController.m_monkeyChairStates.smoke;

	void Start()
	{
		m_monkeyAnimator = this.GetComponent<Animator> ();							//获取猴子的动画组件
	}

	void Update()
	{
        if (LevelOneGameManager.Instance.GetTurnToCountry() != 0) return;//hero 返村庄，不进行攻击
        
		if(m_monkeyCurrState==LevelOneMonkeyController.m_monkeyStates.attack)
		{
			if(this.GetComponent<SpriteRenderer>().sprite.name=="24")				//猴子攻击动画播放到最后一帧
			{
				if(!m_stoneMake)													//是否生成了石头
				{
					GameObject cloneBullet = Instantiate(m_stone, m_stoneStartPos.position, transform.rotation);	//初始化子弹
					m_stoneMake = true;
				}
			}
			else
				m_stoneMake = false;
		}
	}

	void OnEnable()																	//对象可用时 加入到订阅者列表中
	{
		LevelOneMonkeyController.onMonkeyStateChange += OnStateChange;
		LevelOneMonkeyController.onMonkeyChairStateChange += OnMonkeyChairStateChange;
	}
	void OnDisable()																//不可用时 从订阅者列表中退出
	{
		LevelOneMonkeyController.onMonkeyStateChange -= OnStateChange;
		LevelOneMonkeyController.onMonkeyChairStateChange -= OnMonkeyChairStateChange;
	}
	
	bool CheckForValidState(LevelOneMonkeyController.m_monkeyStates newState)		//判断动画之间是否可以切换
	{
		bool _returnVal = true;														//默认不可转
		return _returnVal;
	}

	public void OnMonkeyChairStateChange(LevelOneMonkeyController.m_monkeyChairStates _chairNewState)
	{
		if(_chairNewState==m_monkeyChairCurrState)
			return;
		switch(_chairNewState)
		{
		case LevelOneMonkeyController.m_monkeyChairStates.smoke:
			m_monkeyChairAnimator.SetBool("monkeyChairFire", false);
			break;
		case LevelOneMonkeyController.m_monkeyChairStates.fire:
			m_monkeyChairAnimator.SetBool("monkeyChairFire", true);
			break;
		}
		m_monkeyChairCurrState = _chairNewState;
	}
	
	public void OnStateChange(LevelOneMonkeyController.m_monkeyStates _newState)	//猴子状态改变时调用
	{
		if(_newState==m_monkeyCurrState)
			return;
		if(!CheckForValidState(_newState))
			return;
		switch(_newState)
		{
		case LevelOneMonkeyController.m_monkeyStates.idle:
			m_monkeyAnimator.SetBool("monkeyAttack", false);
			break;
		case LevelOneMonkeyController.m_monkeyStates.attack:
			m_monkeyAnimator.SetBool("monkeyAttack", true);
			break;
		case LevelOneMonkeyController.m_monkeyStates.die:
			m_monkeyAnimator.SetBool("monkeyDie", true);
			break;
		}
		m_monkeyCurrState = _newState;
	}
}
