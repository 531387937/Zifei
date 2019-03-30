using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Animator))]
public class LevelOneHeroListener : MonoBehaviour 
{
	public GameObject[] m_colliderObj;									//第一关里的碰撞体(平台)

	public float m_heroClimbSpeed = 30f;								//主角爬行速度
	public float m_heroWalkSpeed = 30f;									//主角移动速度
	public float m_jumpForceVertical = 3000f;							//跳跃时垂直方向上向上的力
	public float m_jumpForceHorizontal = 2000f;							//跳跃时水平方向上的力
	private Animator m_heroAnimator = null;								//主角上的animator组件
	private bool m_collideLeftEdge = false;								//是否碰到左边界
	private bool m_collideRightEdge = false;							//是否碰到右边界
	private bool m_turnCheck = false;									//检测转身
	
	private LevelOneHeroController.heroHorizontalStates m_horizontalCurrentState = LevelOneHeroController.heroHorizontalStates.idle;
	private LevelOneHeroController.heroVerticalStates m_verticalCurrentState = LevelOneHeroController.heroVerticalStates.idle;
	private LevelOneHeroController.heroClimbHorizontalStates m_climbHorizontalCurrentState = LevelOneHeroController.heroClimbHorizontalStates.idle;
	private LevelOneHeroController.heroClimbVerticalStates m_climbVerticalCurrentState = LevelOneHeroController.heroClimbVerticalStates.idle;

	void OnEnable()														//对象可用时 加入到订阅者列表中
	{
		LevelOneHeroController.onVerticalStateChange += OnVerticalStateChange;
		LevelOneHeroController.onHorizontalStateChange += OnHorizontalStateChange;
		LevelOneHeroController.onClimbHorizontalStateChange += OnClimbHorizontalStateChange;
		LevelOneHeroController.onClimbVerticalStateChange += OnClimbVerticalStateChange;
	}
	void OnDisable()													//不可用时，从订阅者列表中退出
	{
		LevelOneHeroController.onVerticalStateChange -= OnVerticalStateChange;
		LevelOneHeroController.onHorizontalStateChange -= OnHorizontalStateChange;
		LevelOneHeroController.onClimbHorizontalStateChange -= OnClimbHorizontalStateChange;
		LevelOneHeroController.onClimbVerticalStateChange -= OnClimbVerticalStateChange;
	}
	
	void Start()
	{
		m_heroAnimator = this.GetComponent<Animator> ();				//获取主角的动画组件
	}
	
	void OnCollisionEnter2D(Collision2D coll) 							//检测主角边界碰撞	
	{
		if(coll.gameObject.tag=="EdgeLeft")								//碰到左边界
			m_collideLeftEdge = true;
		else if(coll.gameObject.tag=="EdgeRight")						//碰到右边界
			m_collideRightEdge = true;
	}
	
	void LateUpdate()
	{
		AnimatorStateInfo  info = m_heroAnimator.GetCurrentAnimatorStateInfo(0);
		if(m_turnCheck)													//正在转身
		{
			if(!info.IsName("HeroTurnAnimation"))						//转身动画完成
			{
				m_turnCheck = false;									//转身结束
				m_heroAnimator.SetBool("turnToClimb", false);			//开启爬行动画
			}
		}

		if(LevelOneHeroController.m_climbState==2)						//当前处于爬行状态
		{
			OnClimbMoveStateCycle();									//爬行状态位移相关
			LevelOneGameManager.Instance.SetHeroStoneDir(0);			//只能向上扔
		}
		else 															//当前处于普通状态
		{
			OnHorizontalStateCycle ();									//左右移动相关
			if(this.transform.localScale.x>0)
				LevelOneGameManager.Instance.SetHeroStoneDir(2);			//只能向右扔
			else
				LevelOneGameManager.Instance.SetHeroStoneDir(1);			//只能向左扔
		}

		if(LevelOneHeroController.m_climbState==4)						//如果需要爬行
		{
			if(info.IsName("HeroIdleAnimation"))						//当前处于空闲动画
			{
				LevelOneHeroController.m_climbState=5;					//准备爬行
				AnimationNumClose();									//关闭所有动画控制变量
				this.GetComponent<Rigidbody2D>().velocity = new Vector2(0f, 0f);//刚体速度归零

			}
			else
			{
				LevelOneHeroController.m_climbState=0;					//准备爬行
			}
		}

		for(int i=0; i<m_colliderObj.Length; i++)						//遍历所有的地面(判断是否设置为碰撞体)
		{
			if(m_colliderObj[i].transform.position.y<this.transform.position.y)
			{
				m_colliderObj[i].SetActive(true);						//显示所有低于主角的地面碰撞盒
			}
			else
				m_colliderObj[i].SetActive(false);						//隐藏所有高于主角的地面碰撞盒
		}
	}
	
	void AnimationNumClose()											//关闭所有动画改变的变量
	{
		m_heroAnimator.SetInteger("idleRun", 2);
		m_heroAnimator.SetInteger("idleJump", 2);
		m_heroAnimator.SetInteger("idleFall", 2);
		m_heroAnimator.SetInteger("runFall", 2);
		m_heroAnimator.SetInteger("runJump", 2);
		m_heroAnimator.SetInteger("jumpFall", 2);
	}

	void OnClimbMoveStateCycle()										//爬行状态位移相关
	{
		switch(m_climbHorizontalCurrentState)							//判断当前水平状态
		{
		case LevelOneHeroController.heroClimbHorizontalStates.leftClimb://当前为左爬
			if(!m_collideLeftEdge)										//未碰到左边界
			{
				this.transform.Translate(new Vector3((m_heroClimbSpeed*-1f)*0.02f, 0f, 0f));//向左移动
				if(m_collideRightEdge)									//右边界变量归位
					m_collideRightEdge = false;
			}
			break;
		case LevelOneHeroController.heroClimbHorizontalStates.rightClimb://当前为右爬
			if(!m_collideRightEdge)										//当前未碰到右边界
			{
				this.transform.Translate(new Vector3(m_heroClimbSpeed*0.02f, 0f, 0f));//向右移动
				if(m_collideLeftEdge)									//左边界变量归位
					m_collideLeftEdge = false;
			}
			break;
		}
		switch(m_climbVerticalCurrentState)								//判断当前竖直爬行状态
		{
		case LevelOneHeroController.heroClimbVerticalStates.upClimb:	//当前为上爬
			this.transform.Translate(new Vector3(0f, m_heroClimbSpeed*0.02f, 0f));//向上移动
			break;
		case LevelOneHeroController.heroClimbVerticalStates.downClimb:	//当前为下爬
			this.transform.Translate(new Vector3(0f, -m_heroClimbSpeed*0.02f, 0f));//向下移动
			break;
		}
	}
	
	void OnHorizontalStateCycle()										//左右移动位移相关
	{
		Vector3 _localScale = this.transform.localScale;				//主角朝向
		switch(m_horizontalCurrentState)								//判断主角当前水平状态
		{
		case LevelOneHeroController.heroHorizontalStates.left:			//当前为向左走状态
			if(!m_collideLeftEdge)										//当前未碰到左边界
			{
				this.transform.Translate(new Vector3((m_heroWalkSpeed*-1f)*0.02f, 0f, 0f));//向左移动
				if(m_collideRightEdge)									//右边界变量归位
					m_collideRightEdge = false;
			}
			if(_localScale.x > 0f)										//主角朝向需更改
			{
				_localScale.x *= -1f;
				this.transform.localScale = _localScale;
			}
			break;
		case LevelOneHeroController.heroHorizontalStates.right:			//当前为向右走状态
			if(!m_collideRightEdge)										//当前未碰到右边界
			{
				this.transform.Translate(new Vector3(m_heroWalkSpeed*0.02f, 0f, 0f));//向右移动
				if(m_collideLeftEdge)									//左边界变量归位
					m_collideLeftEdge = false;
			}
			if(_localScale.x<0f)										//主角朝向需更改
			{
				_localScale.x *= -1f;
				this.transform.localScale = _localScale;
			}
			break;
		}
	}

	public void OnClimbHorizontalStateChange(LevelOneHeroController.heroClimbHorizontalStates _climbHNewState)//爬行水平状态改变
	{
		if(_climbHNewState == m_climbHorizontalCurrentState)				//当前水平状态未发生变化
			return;
		switch(_climbHNewState)												//判断当前新状态
		{
		case LevelOneHeroController.heroClimbHorizontalStates.idle:			//需退出爬行状态
			m_heroAnimator.SetInteger("climbState", 4);
			m_heroAnimator.SetBool("climbToTurn", true);
			m_heroAnimator.SetBool("turnToIdle", true);
			break;
		case LevelOneHeroController.heroClimbHorizontalStates.idleClimb:	//进入爬行未动状态
			if(m_climbHorizontalCurrentState==LevelOneHeroController.heroClimbHorizontalStates.idle)//如果还未转身
			{
				m_heroAnimator.SetInteger("climbState", 1);
				m_heroAnimator.SetBool("climbToTurn", false);
				m_heroAnimator.SetBool("turnToIdle", false);
				m_heroAnimator.SetBool("turnToClimb", true);
				m_turnCheck = true;											//转身动画完毕检测
			}
			else 															//如果停止爬行
			{
				if(m_climbVerticalCurrentState==LevelOneHeroController.heroClimbVerticalStates.idleClimb)
					m_heroAnimator.SetInteger("climbState", 4);		
			}
			break;
		case LevelOneHeroController.heroClimbHorizontalStates.leftClimb:	//向左向右爬行
		case LevelOneHeroController.heroClimbHorizontalStates.rightClimb:
			m_heroAnimator.SetInteger("climbState", 3);
			break;
		}
		m_climbHorizontalCurrentState = _climbHNewState;					//更新爬行状态
	}

	public void OnClimbVerticalStateChange(LevelOneHeroController.heroClimbVerticalStates _climbVNewState)
	{
		if(_climbVNewState == m_climbVerticalCurrentState)					//当前水平状态未发生变化
			return;
		switch(_climbVNewState)												//获取当前竖直新状态
		{
		case LevelOneHeroController.heroClimbVerticalStates.idle:			//退出爬行状态
			break;
		case LevelOneHeroController.heroClimbVerticalStates.idleClimb:		//进入爬行未动状态
			if(m_climbVerticalCurrentState!=LevelOneHeroController.heroClimbVerticalStates.idle)	//如果已转身
			{
				if(m_climbHorizontalCurrentState==LevelOneHeroController.heroClimbHorizontalStates.idleClimb)
					m_heroAnimator.SetInteger("climbState", 4);	
			}
			break;
		case LevelOneHeroController.heroClimbVerticalStates.downClimb:		//向上或向下爬
		case LevelOneHeroController.heroClimbVerticalStates.upClimb:
			m_heroAnimator.SetInteger("climbState", 3);
			break;
		}
		m_climbVerticalCurrentState = _climbVNewState;						//更新爬行竖直状态
	}
	
	public void OnHorizontalStateChange(LevelOneHeroController.heroHorizontalStates _horizontalNewState)	//主角状态改变时调用以改变动画
	{
		if(_horizontalNewState==m_horizontalCurrentState)					//当前水平状态未发生变化
			return;
		switch(_horizontalNewState)											//新水平状态类型
		{
		case LevelOneHeroController.heroHorizontalStates.idle:				//需转为静止状态	
			if(m_verticalCurrentState!=LevelOneHeroController.heroVerticalStates.falling&&					//当前状态不是跳跃或下落
			   m_verticalCurrentState!=LevelOneHeroController.heroVerticalStates.jump)
				m_heroAnimator.SetInteger("idleRun", 0);
			break;
		case LevelOneHeroController.heroHorizontalStates.left:				//需转为跑步状态
		case LevelOneHeroController.heroHorizontalStates.right:	
			
			if(m_verticalCurrentState!=LevelOneHeroController.heroVerticalStates.falling&&	//如果当前正在下降或是跳跃
			   m_verticalCurrentState!=LevelOneHeroController.heroVerticalStates.jump)
				m_heroAnimator.SetInteger("idleRun", 1);
			break;
		} 
		m_horizontalCurrentState = _horizontalNewState;						//更新当前状态
	}
	
	public void OnVerticalStateChange(LevelOneHeroController.heroVerticalStates _verticalNewState)	//主角状态改变时调用以改变动画
	{
		if(_verticalNewState==m_verticalCurrentState)						//当前竖直状态未发生变化
			return;
		if(!CheckForValidVerticalState(_verticalNewState))					//当前树枝状态不能发生转变
			return;
		switch(_verticalNewState)											//新竖直状态类型
		{
		case LevelOneHeroController.heroVerticalStates.idle:				//需转为静止状态
                if (m_verticalCurrentState == LevelOneHeroController.heroVerticalStates.jump)
                {
                    AnimationNumClose();                                    //关闭所有变量
                    m_heroAnimator.SetInteger("idleJump", 0);               //idle <- jump

                }
                break;
		case LevelOneHeroController.heroVerticalStates.jump:				//需转为跳跃状态
			if(m_horizontalCurrentState==LevelOneHeroController.heroHorizontalStates.idle)			//如果当前水平未动
			{
				AnimationNumClose();										//关闭所有变量
				m_heroAnimator.SetInteger("idleJump", 1);					//idle→jump
			}	
			else 															//如果当前为跑步状态
			{
				AnimationNumClose();										//关闭所有变量
				m_heroAnimator.SetInteger("runJump", 1);					//run→jump
				
			}
                if (GetComponent<Rigidbody2D>().velocity.y <= 0)
                    this.GetComponent<Rigidbody2D>().AddForce(new Vector2(0, m_jumpForceVertical));						//给主角添加向上力
			break;
		case LevelOneHeroController.heroVerticalStates.landing:				//需转为落地状态
			if(m_verticalCurrentState==LevelOneHeroController.heroVerticalStates.falling)			//当前为下落状态
			{
				if(m_horizontalCurrentState==LevelOneHeroController.heroHorizontalStates.idle)		//水平为空闲
				{
					if(m_heroAnimator.GetInteger("jumpFall")==1)
					{
						m_heroAnimator.SetInteger("jumpFall", 2);			//fall→idle
						m_heroAnimator.SetInteger("idleJump", 0);			//fall→idle
					}
					else
						AnimationNumClose();								//关闭所有变量
					m_heroAnimator.SetInteger("idleFall", 0);				//fall→idle
				}
				else 														//水平为跑步
				{
					if(m_heroAnimator.GetInteger("jumpFall")==1)
					{
						m_heroAnimator.SetInteger("jumpFall", 2);			//fall→idle
						m_heroAnimator.SetInteger("runJump", 0);			//fall→idle
					}
					else
						AnimationNumClose();								//关闭所有变量
					m_heroAnimator.SetInteger("runFall", 0);				//fall→run
				}
			}
			else if(m_verticalCurrentState==LevelOneHeroController.heroVerticalStates.jump)		//当前为跳跃状态
			{
				if(m_horizontalCurrentState==LevelOneHeroController.heroHorizontalStates.idle)		//水平为空闲
				{
					AnimationNumClose();									//关闭所有变量
					m_heroAnimator.SetInteger("idleJump", 0);				//jump→idle
				}
				else 														//水平为跑步
				{
					AnimationNumClose();									//关闭所有变量
					m_heroAnimator.SetInteger("runJump", 0);				//jump→run
				}
			}
			break;
		case LevelOneHeroController.heroVerticalStates.down:				//需转为下行状态

			break;
		case LevelOneHeroController.heroVerticalStates.falling:				//需转为下落状态	
			if(m_verticalCurrentState==LevelOneHeroController.heroVerticalStates.jump)				//当前为跳跃状态
			{
				AnimationNumClose();										//关闭速游变量
				m_heroAnimator.SetInteger("jumpFall", 1);					//jump→fall
			}
			else 															//当前非跳跃状态
			{
				if(m_horizontalCurrentState==LevelOneHeroController.heroHorizontalStates.idle)		//水平为空闲状态
				{
					AnimationNumClose();									//关闭所有变量
					m_heroAnimator.SetInteger("idleFall", 1);				//idle→fall
				}
				else 														//水平为跑步状态
				{
					AnimationNumClose();									//关闭所有变量
					m_heroAnimator.SetInteger("runFall", 1);				//run→fall
				}
			}
                if (m_horizontalCurrentState == LevelOneHeroController.heroHorizontalStates.left ||          //防止跳跃后滑行
                    m_horizontalCurrentState == LevelOneHeroController.heroHorizontalStates.right)
                {
                    m_heroAnimator.SetInteger("idleRun", 1);
                }
                break;
		} 
		m_verticalCurrentState = _verticalNewState;							//更新竖直状态
	}
	
	bool CheckForValidVerticalState(LevelOneHeroController.heroVerticalStates _newState)		//检测是否可以发生状态转变
	{
		bool _returnVal = false;											//默认不能
		switch(m_verticalCurrentState)										//判断当前竖直状态
		{
		case LevelOneHeroController.heroVerticalStates.idle:					//当前为空闲或落地状态
			_returnVal = true;
			break;
		case LevelOneHeroController.heroVerticalStates.landing:				
			if(_newState!=LevelOneHeroController.heroVerticalStates.falling)	//不能转为下落状态
				_returnVal = true;
			else
				_returnVal = false;
			break;
		case LevelOneHeroController.heroVerticalStates.jump:					//当前为跳跃状态
			if(_newState==LevelOneHeroController.heroVerticalStates.falling||	//只能转为下落和落地状态
			   _newState==LevelOneHeroController.heroVerticalStates.landing
               || _newState == LevelOneHeroController.heroVerticalStates.idle)
				_returnVal = true;
			else
				_returnVal = false;
			break;
			
		case LevelOneHeroController.heroVerticalStates.down:					//当前为下行状态
			if(_newState==LevelOneHeroController.heroVerticalStates.falling)	//可转为下落状态
				_returnVal = true;
			else
				_returnVal = false;
			break;
		case LevelOneHeroController.heroVerticalStates.falling:					//当前为下落状态
			if(_newState==LevelOneHeroController.heroVerticalStates.landing)	//可转为落地状态
				_returnVal = true;
			else 
				_returnVal = false;
			break;
		}
		return _returnVal;														//返回是否可转化变量
	}
}
