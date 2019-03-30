using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Animator))]
public class HeroStateListener : MonoBehaviour
{
	[HideInInspector]
    public GameObject m_invisibleColliderObj = null;                    //需要隐藏的碰撞盒
    public GameObject[] m_colliderObj;                                  //村子里的碰撞体

    public GameObject[] m_moveBgPic;                                    //移动的背景图
    public GameObject[] m_moveCloudPic;                                 //移动的云图		
    public  float m_heroWalkSpeed = 7f;                                 //主角移动速度
    public float m_jumpForceVertical = 800f;                           //跳跃时垂直方向上向上的力



    private Animator m_heroAnimator = null;                             //主角上的animator组件
    private bool m_hideCollider = false;                                //是否需要隐藏碰撞盒
    private bool m_collideLeftEdge = false;                             //是否碰到左边界
    private bool m_collideRightEdge = false;                            //是否碰到右边界

    private Rigidbody2D heroRigidBody2d;

    private float m_cloudOneMoveSpeed = 0.007f;                         //云1飘行速度
    private float m_cloudTwoMoveSpeed = 0.003f;                         //云2飘行速度
    private int m_cloudOneCurrIndex = 0;                                //云1当前显示的图索引号
    private int m_cloudTwoCurrIndex = 0;                                //云2当前显示的图索引号

    private HeroStateController.heroHorizontalStates m_horizontalCurrentState = HeroStateController.heroHorizontalStates.idle;
    private HeroStateController.heroVerticalStates m_verticalCurrentState = HeroStateController.heroVerticalStates.idle;

    //void OnEnable()                                                     //对象可用时 加入到订阅者列表中
    //{
    //    HeroStateController.onVerticalStateChange += OnVerticalStateChange;
    //    HeroStateController.onHorizontalStateChange += OnHorizontalStateChange;

    //}
    //void OnDisable()                                                    //不可用时，从订阅者列表中退出
    //{
    //    HeroStateController.onVerticalStateChange -= OnVerticalStateChange;
    //    HeroStateController.onHorizontalStateChange -= OnHorizontalStateChange;

    //}

    void Awake()
    {
        m_heroAnimator = GetComponent<Animator>();             //获取主角的动画组件
        heroRigidBody2d = GetComponent<Rigidbody2D>();

}

void OnCollisionEnter2D(Collision2D coll)                           //检测主角边界碰撞	
    {
        if (coll.gameObject.tag == "EdgeLeft")                              //碰到左边界
            m_collideLeftEdge = true;
        else if (coll.gameObject.tag == "EdgeRight")                        //碰到右边界
            m_collideRightEdge = true;
    }

    void CloudMove()                                                                //云运动函数
    {
        m_moveCloudPic[0].transform.Translate(m_cloudOneMoveSpeed, 0f, 0f);     //云1图向右运动
        m_moveCloudPic[1].transform.Translate(m_cloudOneMoveSpeed, 0f, 0f);     //云2图向右运动
        if (m_moveCloudPic[m_cloudOneCurrIndex].transform.position.x >= 71.6f)          //云当前图移动出场景
        {
            if (m_cloudOneCurrIndex == 0)                                               //判定当前索引
            {
                m_moveCloudPic[m_cloudOneCurrIndex].transform.position =            //将移出场景的云图放置最右边
                    new Vector3(m_moveCloudPic[1].transform.position.x - 71.6f, m_moveCloudPic[1].transform.position.y, m_moveCloudPic[1].transform.position.z);
                m_cloudOneCurrIndex = 1;                                            //更改当前云编号
            }
            else
            {
                m_moveCloudPic[m_cloudOneCurrIndex].transform.position =
                    new Vector3(m_moveCloudPic[0].transform.position.x - 71.6f, m_moveCloudPic[0].transform.position.y, m_moveCloudPic[0].transform.position.z);
                m_cloudOneCurrIndex = 0;
            }
        }

        m_moveCloudPic[2].transform.Translate(m_cloudTwoMoveSpeed, 0f, 0f);
        m_moveCloudPic[3].transform.Translate(m_cloudTwoMoveSpeed, 0f, 0f);
        if (m_moveCloudPic[m_cloudTwoCurrIndex].transform.position.x >= 71.6f)
        {
            if (m_cloudTwoCurrIndex == 2)
            {
                m_moveCloudPic[m_cloudTwoCurrIndex].transform.position =
                    new Vector3(m_moveCloudPic[3].transform.position.x - 71.6f, m_moveCloudPic[3].transform.position.y, m_moveCloudPic[3].transform.position.z);
                m_cloudTwoCurrIndex = 3;
            }
            else
            {
                m_moveCloudPic[m_cloudTwoCurrIndex].transform.position =
                    new Vector3(m_moveCloudPic[2].transform.position.x - 71.6f, m_moveCloudPic[2].transform.position.y, m_moveCloudPic[2].transform.position.z);
                m_cloudTwoCurrIndex = 2;
            }
        }
    }

    void LateUpdate()
    {
        CloudMove();
        OnHorizontalStateCycle();                                           //左右移动相关
      	//ResetCollider();
    }

    private void ResetCollider()
    {
        if (GameManager.Instance.GetCurrentScene() == 0)                        //当前主角在村庄里
        {

            for (int i = 0; i < m_colliderObj.Length; i++)
            {
                if (m_hideCollider)
                {
                    if (m_invisibleColliderObj.name == m_colliderObj[i].name)   //只隐藏主角所站立的地面
                    {
                        m_colliderObj[i].SetActive(false);
                        
                    }
                }
                else {

                    if (m_colliderObj[i].transform.position.y < transform.position.y)
                    {
                        if (heroRigidBody2d.velocity.y > 0f)
                            m_colliderObj[i].SetActive(false);
                        else
                            m_colliderObj[i].SetActive(true);                   //显示所有低于主角的地面碰撞盒
                    }
                    else
                        m_colliderObj[i].SetActive(false);

                }

            }
        }
    }

    void AnimationNumClose()                                            //关闭所有动画改变的变量
    {
        m_heroAnimator.SetInteger("idleRun", 2);
        m_heroAnimator.SetInteger("idleJump", 2);
        m_heroAnimator.SetInteger("idleFall", 2);
        m_heroAnimator.SetInteger("runFall", 2);
        m_heroAnimator.SetInteger("runJump", 2);
        m_heroAnimator.SetInteger("jumpFall", 2);
    }

    void OnHorizontalStateCycle()                                       //左右移动位移相关
    {
        Vector3 _localScale = this.transform.localScale;                //主角朝向
        switch (m_horizontalCurrentState)                               //判断主角当前水平状态
        {
            case HeroStateController.heroHorizontalStates.left:             //当前为向左走状态
                if (!m_collideLeftEdge)                                     //当前未碰到左边界
                {
                    m_moveBgPic[0].transform.Translate(new Vector3(m_heroWalkSpeed * 0.7f * 0.002f, 0f, 0f));
                    m_moveBgPic[1].transform.Translate(new Vector3(m_heroWalkSpeed * 0.35f * 0.002f, 0f, 0f));
                    m_moveBgPic[2].transform.Translate(new Vector3(m_heroWalkSpeed * 0.1f * 0.002f, 0f, 0f));

                    this.transform.Translate(new Vector3((m_heroWalkSpeed * -1f) * 0.02f, 0f, 0f));             //向左移动
                    if (m_collideRightEdge)                                 //右边界变量归位
                        m_collideRightEdge = false;
                }
                if (_localScale.x > 0f)                                     //主角朝向需更改
                {
                    _localScale.x *= -1f;
                    this.transform.localScale = _localScale;
                }
                break;
            case HeroStateController.heroHorizontalStates.right:            //当前为向右走状态
                if (!m_collideRightEdge)                                        //当前未碰到右边界
                {
                    m_moveBgPic[0].transform.Translate(new Vector3((m_heroWalkSpeed * -1f) * 0.7f * 0.002f, 0f, 0f));
                    m_moveBgPic[1].transform.Translate(new Vector3((m_heroWalkSpeed * -1f) * 0.5f * 0.002f, 0f, 0f));
                    m_moveBgPic[2].transform.Translate(new Vector3((m_heroWalkSpeed * -1f) * 0.3f * 0.002f, 0f, 0f));

                    this.transform.Translate(new Vector3(m_heroWalkSpeed * 0.02f, 0f, 0f));                 //向右移动
                    if (m_collideLeftEdge)                                  //左边界变量归位
                        m_collideLeftEdge = false;
                }
                if (_localScale.x < 0f)                                     //主角朝向需更改
                {
                    _localScale.x *= -1f;
                    this.transform.localScale = _localScale;
                }
                break;
        }
    }

    public void OnHorizontalStateChange(HeroStateController.heroHorizontalStates _horizontalNewState)   //主角状态改变时调用以改变动画
    {
		if (_horizontalNewState != HeroStateController.heroHorizontalStates.idle) {
			if (heroRigidBody2d.velocity.y >= 0f) {
				m_hideCollider = false;
				ResetCollider ();
			}
		}

        if (_horizontalNewState == m_horizontalCurrentState)                //当前水平状态未发生变化
            return;


        switch (_horizontalNewState)                                        //新水平状态类型
        {
            case HeroStateController.heroHorizontalStates.idle:             //需转为静止状态	
                if (m_verticalCurrentState != HeroStateController.heroVerticalStates.falling &&                 //当前状态不是跳跃或下落
                   m_verticalCurrentState != HeroStateController.heroVerticalStates.jump)
                    m_heroAnimator.SetInteger("idleRun", 0);
                break;
            case HeroStateController.heroHorizontalStates.left:             //需转为跑步状态
            case HeroStateController.heroHorizontalStates.right:

                if (m_verticalCurrentState != HeroStateController.heroVerticalStates.falling &&
                   m_verticalCurrentState != HeroStateController.heroVerticalStates.jump)

                    m_heroAnimator.SetInteger("idleRun", 1);

                break;
        }
        
        m_horizontalCurrentState = _horizontalNewState;                 //更新当前状态

        
    }

    public void OnVerticalStateChange(HeroStateController.heroVerticalStates _verticalNewState) //主角状态改变时调用以改变动画
    {
        //print("old: "+ m_verticalCurrentState +"     ->            new :   "+_verticalNewState);
        if (_verticalNewState == m_verticalCurrentState)                    //当前竖直状态未发生变化
            return;

        if (!CheckForValidVerticalState(_verticalNewState))             //当前状态不能发生转变
            return;

        switch (_verticalNewState)                                      //新竖直状态类型
        {
            case HeroStateController.heroVerticalStates.idle:               //需转为静止状态
                if (m_verticalCurrentState == HeroStateController.heroVerticalStates.jump)
                {
                    AnimationNumClose();                                    //关闭所有变量
                    m_heroAnimator.SetInteger("idleJump", 0);               //idle <- jump

                }
                break;
            case HeroStateController.heroVerticalStates.jump:               //需转为跳跃状态
                if (m_horizontalCurrentState == HeroStateController.heroHorizontalStates.idle)          //如果当前水平未动
                {
                    AnimationNumClose();                                    //关闭所有变量
                    m_heroAnimator.SetInteger("idleJump", 1);               //idle→jump
                }
                else                                                        //如果当前为跑步状态
                {
                    AnimationNumClose();                                    //关闭所有变量
                    m_heroAnimator.SetInteger("runJump", 1);                //run→jump

                }
                if (heroRigidBody2d.velocity.y <= 0)
                    heroRigidBody2d.AddForce(new Vector2(0, m_jumpForceVertical));                     //给主角添加向上力
                break;
            case HeroStateController.heroVerticalStates.landing:            //需转为落地状态
                if (m_verticalCurrentState == HeroStateController.heroVerticalStates.falling)           //当前为下落状态
                {
                    if (m_horizontalCurrentState == HeroStateController.heroHorizontalStates.idle)      //水平为空闲
                    {
                        if (m_heroAnimator.GetInteger("jumpFall") == 1)
                        {
                            m_heroAnimator.SetInteger("jumpFall", 2);           //fall→idle
                            m_heroAnimator.SetInteger("idleJump", 0);           //fall→idle
                        }
                        else
                            AnimationNumClose();                                //关闭所有变量
                        m_heroAnimator.SetInteger("idleFall", 0);           //fall→idle
                    }
                    else                                                    //水平为跑步
                    {
                        if (m_heroAnimator.GetInteger("jumpFall") == 1)
                        {
                            m_heroAnimator.SetInteger("jumpFall", 2);           //fall→idle
                            m_heroAnimator.SetInteger("runJump", 0);            //fall→idle
                        }
                        else
                            AnimationNumClose();                                //关闭所有变量
                        m_heroAnimator.SetInteger("runFall", 0);            //fall→run
                    }

                }
                else if (m_verticalCurrentState == HeroStateController.heroVerticalStates.jump)     //当前为跳跃状态
                {
                    if (m_horizontalCurrentState == HeroStateController.heroHorizontalStates.idle)      //水平为空闲
                    {
                        AnimationNumClose();                                //关闭所有变量
                        m_heroAnimator.SetInteger("idleJump", 0);           //jump→idle
                    }
                    else                                                    //水平为跑步
                    {
                        AnimationNumClose();                                //关闭所有变量
                        m_heroAnimator.SetInteger("runJump", 0);            //jump→run
                    }
                }
                break;
            case HeroStateController.heroVerticalStates.down:               //需转为下行状态
                m_hideCollider = true;                                      //需要隐藏下方碰撞块
                ResetCollider();

                break;
            case HeroStateController.heroVerticalStates.falling:            //需转为下落状态	
                if (m_verticalCurrentState == HeroStateController.heroVerticalStates.jump)              //当前为跳跃状态
                {
                    AnimationNumClose();                                    //关闭速游变量
                    m_heroAnimator.SetInteger("jumpFall", 1);               //jump→fall
                }
                else                                                        //当前非跳跃状态
                {
                    if (m_horizontalCurrentState == HeroStateController.heroHorizontalStates.idle)      //水平为空闲状态
                    {
                        AnimationNumClose();                                //关闭所有变量
                        m_heroAnimator.SetInteger("idleFall", 1);           //idle→fall
                    }
                    else                                                    //水平为跑步状态
                    {
                        AnimationNumClose();                                //关闭所有变量
                        m_heroAnimator.SetInteger("runFall", 1);            //run→fall
                    }
                }
                m_hideCollider = false;                                     //关闭隐藏按钮
                ResetCollider();
                if(m_horizontalCurrentState ==HeroStateController.heroHorizontalStates.left ||    //防止跳跃后滑行
                   m_horizontalCurrentState == HeroStateController.heroHorizontalStates.right)
                {
                    m_heroAnimator.SetInteger("idleRun", 1);
                }
                break;
        }
        m_verticalCurrentState = _verticalNewState;                     //更新竖直状态
        //ResetCollider();

    }

    bool CheckForValidVerticalState(HeroStateController.heroVerticalStates _newState)       //检测是否可以发生状态转变
    {
        bool _returnVal = false;                                            //默认不能
        switch (m_verticalCurrentState)                                     //判断当前竖直状态
        {
            case HeroStateController.heroVerticalStates.idle:                   //
                    _returnVal = true;

                break;
            case HeroStateController.heroVerticalStates.landing:
			if ((_newState != HeroStateController.heroVerticalStates.falling)
				&& (_newState != HeroStateController.heroVerticalStates.down))    //不能转为下落状态
                    _returnVal = true;

                break;
            case HeroStateController.heroVerticalStates.jump:                   //当前为跳跃状态
			if (_newState == HeroStateController.heroVerticalStates.falling  //只能转为下落和落地状态
				|| _newState == HeroStateController.heroVerticalStates.landing
				|| _newState == HeroStateController.heroVerticalStates.idle)
                    _returnVal = true;

                break;

            case HeroStateController.heroVerticalStates.down:                   //当前为下行状态
			if (_newState == HeroStateController.heroVerticalStates.falling
				|| _newState == HeroStateController.heroVerticalStates.idle)    //可转为下落状态
                    _returnVal = true;

                break;
            case HeroStateController.heroVerticalStates.falling:                //当前为下落状态
                if (_newState == HeroStateController.heroVerticalStates.landing)    //可转为落地状态
                    _returnVal = true;

                break;
        }
        return _returnVal;                                                  //返回是否可转化变量
    }
}
