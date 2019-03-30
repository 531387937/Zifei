using UnityEngine;
using System.Collections;

public class HeadBattleCountryHeadController : MonoBehaviour 
{
	public GameObject m_gunWave;															//机枪的光线
	public GameObject m_hatWave;															//帽子的光线

	private enum m_countryHeadStates														//村长状态
	{	
		idle,
		gunAttack,
		hatAttack,
		dialog,
		idleDia,
	}
	public UISprite m_headUIEye;
	private delegate void countryHeadStateHandler(m_countryHeadStates _newState);			//定义委托和事件
	private static event countryHeadStateHandler onStateChange; 
	private m_countryHeadStates m_countryHeadCurrState = m_countryHeadStates.idleDia;			//村长初始状态
	private Animator m_countryHeadAnimator = null;											//村长动画器
	private float m_countryHeadTimer = 0f;												//村长攻击方式转换计时器
	private int m_countryHeadStateIndex = 0;
	private int m_headInjuryBlinkCount = 0;
	private float m_headInjuryTimer = 0f;

	void OnEnable()																	//对象可用时 加入到订阅者列表中
	{
		onStateChange += OnStateChange;
	}
	void OnDisable()																//不可用时，从订阅者列表中退出
	{
		onStateChange -= OnStateChange;
	}
	
	void Start()
	{
		m_countryHeadAnimator = this.GetComponent<Animator> ();							//获取村长的动画组件
	}

	void HeadInjuryBlinkCheck()												//受伤闪烁
	{
		if(m_headInjuryBlinkCount==0)											//当前未闪烁
		{
			if(HeadBattleGameManager.Instance.GetHeadInjury())					//如果 受伤
			{
				m_headUIEye.spriteName = "lifeBar_bigBoss_eye2";				// UI面板眼睛改变
				m_headInjuryBlinkCount = 1;									//开始第一次闪
				this.GetComponent<SpriteRenderer>().color = new Color(1,1,1,0.5f);//图片变淡
				m_headInjuryTimer = 0;										//计时器就位
			}
		}
		else 																	//当前  在闪烁
		{
			m_headInjuryTimer += Time.deltaTime;								//开启计时器
			if(m_headInjuryTimer>=0.2f)										//需要闪
			{						
				m_headInjuryBlinkCount ++;									//换图次数增加
				m_headInjuryTimer = 0f;										//计时器归位
				if(m_headInjuryBlinkCount==3)									//根据次数判定要显示的主角图shader
					this.GetComponent<SpriteRenderer>().color =new Color(1,1,1,0.5f);
				else if(m_headInjuryBlinkCount==2||m_headInjuryBlinkCount==4)
					this.GetComponent<SpriteRenderer>().color = new Color(1,1,1,1f);
				else if(m_headInjuryBlinkCount==5)							//闪烁三次后
				{
					m_headInjuryBlinkCount = 0;								//受伤模式结束
					HeadBattleGameManager.Instance.SetHeadInjury(false);		//受伤闪烁结束
					m_headUIEye.spriteName = "lifeBar_bigBoss_eye1";			// UI面板眼睛改变
				}
			}
		}
	}

	void Update()
	{
        if (HeadBattleGameManager.Instance.GetHeroBlood() <= 0) return;

		HeadInjuryBlinkCheck ();

        switch (m_countryHeadCurrState)
        {
            case m_countryHeadStates.gunAttack:
                if (this.GetComponent<SpriteRenderer>().sprite.name == "headBattle15" ||
                   this.GetComponent<SpriteRenderer>().sprite.name == "headBattle16")
                {
                    m_gunWave.SetActive(true);
                }
                else
                    m_gunWave.SetActive(false);
			break;
		case m_countryHeadStates.hatAttack:
                if (this.GetComponent<SpriteRenderer>().sprite.name == "headBattle7" ||
                   this.GetComponent<SpriteRenderer>().sprite.name == "headBattle8")
                {
                    m_hatWave.SetActive(true);
                }
                else
                    m_hatWave.SetActive(false);
			break;
		default:
			m_hatWave.SetActive(false);
			m_gunWave.SetActive(false);
			break;
		}

		int _state = HeadBattleGameManager.Instance.GetGameState ();
		switch(_state)
		{
		case 0:
			break;
		case 1:
			if(onStateChange!=null)
				onStateChange(m_countryHeadStates.dialog);				//转至对话前空闲状态
			break;
		case 2:
			if(onStateChange!=null)
				onStateChange(m_countryHeadStates.idle);				//转至空闲状态
			HeadBattleGameManager.Instance.SetGameState(3);
			break;
		case 3:
			m_countryHeadTimer -= Time.deltaTime;

                if (m_countryHeadTimer<=0)
			{
				if(m_countryHeadStateIndex==0)
				{
					m_countryHeadStateIndex = Random.Range(1,6);
					if(m_countryHeadStateIndex==1)
					{
						m_countryHeadTimer = 1.4f;
						if(onStateChange!=null)
							onStateChange(m_countryHeadStates.hatAttack);		//转至帽子状态
					}
					else
					{
						m_countryHeadTimer = 0.75f;
						if(onStateChange!=null)
							onStateChange(m_countryHeadStates.gunAttack);		//转至机枪状态
					}
				}
				else
				{
					m_countryHeadStateIndex = 0;
					if(onStateChange!=null)
						onStateChange(m_countryHeadStates.idle);				//转至空闲状态
					m_countryHeadTimer = Random.Range(0.8f, 2f);
				}
			}
			break;
		case 4:
			m_headUIEye.spriteName = "lifeBar_bigBoss_eye3";			// UI面板眼睛改变
			Destroy(this.gameObject);
			break;
		}

    }

	void CloseValue()
	{
		m_countryHeadAnimator.SetBool ("HatIdle", false);
		m_countryHeadAnimator.SetBool ("IdleHat", false);
		m_countryHeadAnimator.SetBool ("GunIdle", false);
		m_countryHeadAnimator.SetBool ("IdleGun", false);
	}

	void OnStateChange(m_countryHeadStates _newState)									//村长状态改变时调用
	{
		if(_newState==m_countryHeadCurrState)											//如果状态未发生变化
			return;																		//返回
		switch(_newState)																//判定新状态
		{
		case m_countryHeadStates.idle:													//新状态为空闲
			CloseValue();
			if(m_countryHeadCurrState==m_countryHeadStates.gunAttack)
                {
                    AudioManager.Instance.SoundPlay(Global.GetInstance().audioName_HeadBattle_HeadWeapomLaser1);
                    m_countryHeadAnimator.SetBool ("GunIdle", true);
                }
			else if(m_countryHeadCurrState==m_countryHeadStates.hatAttack)
                {
                    AudioManager.Instance.SoundPlay(Global.GetInstance().audioName_HeadBattle_HeadWeapomLaser2);
                    m_countryHeadAnimator.SetBool ("HatIdle", true);
                }
			else if(m_countryHeadCurrState==m_countryHeadStates.dialog)
				m_countryHeadAnimator.SetInteger ("IdleDia", 2);
			break;
		case m_countryHeadStates.dialog:													//新状态为受伤
			m_countryHeadAnimator.SetInteger ("IdleDia", 1);
			break;
		case m_countryHeadStates.hatAttack:													//新状态为帽子攻击
			CloseValue();
			m_countryHeadAnimator.SetBool ("IdleHat", true);
			break;
		case m_countryHeadStates.gunAttack:													// 当前为机枪攻击
			CloseValue();
			m_countryHeadAnimator.SetBool ("IdleGun", true);
			break;
		}
		m_countryHeadCurrState = _newState;												//更换 当前状态
	}
}

