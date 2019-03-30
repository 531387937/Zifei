using UnityEngine;
using System.Collections;

public class LevelTwoSpiderEggController : MonoBehaviour
{
	public GameObject m_smallSpiderHero;														//主角
	public GameObject m_smallSpider;															//小蜘蛛
    public int stampCount  = 2;


    private int m_spiderEggState = 0;															//蜘蛛卵的状态（0初始动 1破碎 2碎完小蜘蛛诞生）
	private int m_smallSpiderState = 0;															//小蜘蛛状态（0未出生 1出生下落 2来回走 3被踩 4销毁 ）
	private float m_smallSpiderDownSpeed = 0.1f;												//小蜘蛛下落速度
	private float m_smallSpiderMoveSpeed = 0.01f;												//小蜘蛛水平移动速度
	private float m_smallSpiderTimer = 1.5f;													//小蜘蛛死亡计时器

    private int tempStampCount;

	[HideInInspector]
	public bool isLittleSpiderLive = false;//小蜘蛛是否死亡

    private void OnEnable()
    {
        tempStampCount = 5 * stampCount;
    }

    void Update()
	{
        if (LevelTwoGameManager.Instance.GetBloodNum() <= 0) return;
        if (m_smallSpider == null) return;
        
        if (m_spiderEggState!=2)																	//蜘蛛卵状态
			SpiderEggState();
		if(m_smallSpiderState!=0)																//小蜘蛛出生了
			SmallSpiderState();
	}

	void SpiderEggState()																		//蜘蛛卵状态
	{

        if (m_spiderEggState==0)																	//蜘蛛卵初始状态
		{
			Bounds rr1 = m_smallSpiderHero.GetComponent<Collider2D>().bounds;									//主角的包围盒
			Rect r1 = new Rect(rr1.center.x - rr1.size.x / 2,
			                   rr1.center.y - rr1.size.y / 2,
			                   rr1.size.x, rr1.size.y);
			Bounds rr2 = this.GetComponent<Collider>().bounds;													//获取蜘蛛卵包围盒
			Rect r2 = new Rect(rr2.center.x - rr2.size.x / 2, rr2.center.y - rr2.size.y / 2, rr2.size.x, rr2.size.y);
			if(r1.Overlaps(r2))																	//主角碰到蜘蛛卵
			{
				m_spiderEggState = 1;															//蜘蛛卵进入下一状态
				this.GetComponent<Animator>().SetBool("eggBreaking", true);						//蜘蛛卵破碎动画
				Destroy(this.GetComponent<BoxCollider>());										//移除蜘蛛卵包围盒
			}
		}
		else if(m_spiderEggState==1)															//蜘蛛卵第二阶段
		{
			if(this.GetComponent<SpriteRenderer>().sprite.name=="spiderEgg6")					//如果蜘蛛卵破碎动画播放到最后一帧
			{
				m_spiderEggState = 2;															//蜘蛛卵状态结束
				this.GetComponent<Animator>().SetBool("eggBroken", true);						//蜘蛛卵处于破碎状态
				m_smallSpider.SetActive(true);													//开启小蜘蛛
				m_smallSpiderState = 1;															//小蜘蛛出生
				isLittleSpiderLive = true;//小蜘蛛出生

			}
		}
	}

	void SmallSpiderState()																	//小蜘蛛状态
	{
		switch(m_smallSpiderState)															//判定小蜘蛛状态
		{
		case 1:																				//小蜘蛛下落状态
			if(m_smallSpider.transform.position.y>=2.095f)									//正在下落
			{
				m_smallSpider.transform.Translate(0f, -m_smallSpiderDownSpeed, 0f);			//加速下落
				m_smallSpiderDownSpeed += 0.003f;
			}
			else 																			//下落完成
			{
				m_smallSpider.transform.position = new Vector3(m_smallSpider.transform.position.x, 2.095f, m_smallSpider.transform.position.z);
				m_smallSpiderState = 2;														//指定小蜘蛛位置 并 进入下一阶段
				m_smallSpider.GetComponent<Animator>().SetBool("SmallSpiderRun", true);		//小蜘蛛转为左右移动状态
			}
			Bounds _rr1 = m_smallSpiderHero.GetComponent<Collider2D>().bounds;								//主角的包围盒
			Rect _r1 = new Rect(_rr1.center.x - _rr1.size.x / 2,
			                    _rr1.center.y - _rr1.size.y / 2,
			                    _rr1.size.x, _rr1.size.y);
			Bounds _rr2 = m_smallSpider.GetComponent<Collider>().bounds;										//获取小蜘蛛包围盒
			Rect _r2 = new Rect(_rr2.center.x - _rr2.size.x / 2, _rr2.center.y - _rr2.size.y / 2, _rr2.size.x, _rr2.size.y);
			if(_r1.Overlaps(_r2))																//主角碰到小蜘蛛
				LevelTwoGameManager.Instance.SetHeroBloodReduce(0.001f);							//主角血量减少
			break;
		case 2:																				//小蜘蛛左右移动状态
			Vector3 _localScale = m_smallSpider.transform.localScale;						//获取小蜘蛛朝向
			if(m_smallSpider.transform.position.x<=-12.58f)									//碰到左边界 需向右移动
			{
				_localScale.x *= -1f;														//改变小蜘蛛朝向
				m_smallSpider.transform.localScale = _localScale;
				m_smallSpiderMoveSpeed = 0.03f;												//改变速度方向
			}
			else if(m_smallSpider.transform.position.x>=-6.53f)								//碰到右边界 需左移动
			{
				_localScale.x *= -1f;														//改变小蜘蛛朝向
				m_smallSpider.transform.localScale = _localScale;
				m_smallSpiderMoveSpeed = -0.03f;											//改变速度方向
			}
			m_smallSpider.transform.Translate (m_smallSpiderMoveSpeed, 0f, 0f);				//小蜘蛛移动
			
			Bounds rr1 = m_smallSpiderHero.GetComponent<Collider2D>().bounds;								//主角的包围盒
			Rect r1 = new Rect(rr1.center.x - rr1.size.x / 2,
			                   rr1.center.y - rr1.size.y / 2,
			                   rr1.size.x, rr1.size.y);
			Bounds rr2 = m_smallSpider.GetComponent<Collider>().bounds;										//获取小蜘蛛包围盒
			Rect r2 = new Rect(rr2.center.x - rr2.size.x / 2, rr2.center.y - rr2.size.y / 2, rr2.size.x, rr2.size.y);
			if(r1.Overlaps(r2))																//主角碰到小蜘蛛
			{
				if(m_smallSpiderHero.GetComponent<Rigidbody2D>().velocity.y<-7f)							//主角在下落
				{
                    tempStampCount--;
                    if (tempStampCount <= 0)
                    {
                        m_smallSpiderState = 3;                                                 //进入下一阶段 踩到小蜘蛛
                        m_smallSpider.GetComponent<Animator>().SetBool("SmallSpiderDie", true);	//小蜘蛛死亡动画

						isLittleSpiderLive = false;//小蜘蛛死了
                    }
                }
                else 																		//主角接触到小蜘蛛
				LevelTwoGameManager.Instance.SetHeroBloodReduce(0.001f);						//主角受伤开始闪
			}
			break;
		case 3:
			m_smallSpiderTimer -= Time.deltaTime;											//开启死亡动画倒计时
			if(m_smallSpiderTimer<0)														//如果死亡动画结束
			{
				LevelTwoGameManager.Instance.SetMessageType(2, "5金币");					//提示消息
				LevelTwoGameManager.Instance.SetCurrAddMoney(5);
				m_smallSpiderState = 0;														//状态归零											
				Destroy(m_smallSpider);														//销毁小蜘蛛
			}
			break;
		}
	}


	public void SetSmallSpiderDead(){
		m_smallSpiderTimer = 0f;
		m_smallSpiderState = 3;                                                 //

	}
}
