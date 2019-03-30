using UnityEngine;
using System.Collections;

public class SlimeController : MonoBehaviour 
{
	public GameObject m_slimeHero;                                              //主角
    public Sprite[] m_slimeDieSprite;											//史莱姆死亡图
    public int slimeIndex;
	private float m_slimeSpeed = 0.05f;											//史莱姆运移动速度
	private int m_injuryCount = 0;												//史莱姆受伤次数
	private int m_blinkCount = 0;												//受伤闪烁控制										
	private float m_blinkTimer = 0f;											//受伤闪烁计时器
	private bool m_slimeDie = false;											//史莱姆死亡

    private int injuryCount = 5;
    private void OnEnable()
    {
        injuryCount = Random.Range(5,9);
    }

    void SlimeInjuryCheck()														//史莱姆受伤检测
	{
		if(LevelOneGameManager.Instance.GetSlimeInjury(slimeIndex))						//如果史莱姆受伤
		{

				m_slimeSpeed *=0.5f;
				m_injuryCount++;												//受伤次数加1
				if(m_blinkCount==0)												//如果当前不在受伤闪烁阶段
				{
					m_blinkCount = 1;											//开始闪烁
					this.GetComponent<SpriteRenderer>().color = new Color(1,1,1,0.3f);//图片变淡
					m_blinkTimer = 0;											//计时器就位
				}
			LevelOneGameManager.Instance.SetSlimeInjury(slimeIndex, false);				//史莱姆受伤状态恢复
		}


		if(m_injuryCount>= injuryCount-2)                                                    //如果受伤次数大于 3
        {
			m_slimeDie = true;													//开启死亡状态
			m_blinkCount = 1;													//死亡状态闪烁开始
			m_blinkTimer = 0f;													//死亡状态计时器就位
			Destroy(this.GetComponent<Animator>());
			this.GetComponent<SpriteRenderer>().color = new Color(1,1,1,1f);	//恢复史莱姆alpha
			this.GetComponent<SpriteRenderer>().sprite = m_slimeDieSprite[1];	//变为白图
		}
        else                                                                    //如果受伤次数不够 slimeInjuryCounts
        {
			if(m_blinkCount>=1)													//当前正在闪烁
			{
				m_blinkTimer += Time.deltaTime;									//开启计时器
				if(m_blinkTimer>=0.2f)											//需要闪
				{						
					m_blinkCount ++;											//换图次数增加
					m_blinkTimer = 0f;											//计时器归位
					if(m_blinkCount==3)											//根据次数判定要显示的主角图shader
						this.GetComponent<SpriteRenderer>().color = new Color(1,1,1,0.3f);
					else if(m_blinkCount==2||m_blinkCount==4)
						this.GetComponent<SpriteRenderer>().color = new Color(1,1,1,1f);
					else if(m_blinkCount==5)									//闪烁三次后
						m_blinkCount = 0;										//受伤模式结束
				}
			}
		}
	}

	void Update()
	{
        if (LevelOneGameManager.Instance.GetBloodNum() <= 0) return;
        
		if(!m_slimeDie)															//如果史莱姆没有死
		{
			SlimeInjuryCheck();													//受伤检测
			if(this.transform.position.x < -3.8f)									//如果史莱姆碰到左边界
				m_slimeSpeed = -m_slimeSpeed;										//改变方向
			else if(this.transform.position.x > 3.7)								//如果史莱姆移出右边界
				m_slimeSpeed = -m_slimeSpeed;										//改变方向
			this.transform.Translate (new Vector3 (m_slimeSpeed, 0f, 0f));
			
			Bounds rr1 = m_slimeHero.GetComponent<Collider2D>().bounds;								//史莱姆的包围盒
			Rect r1 = new Rect(rr1.center.x - rr1.size.x / 2,
			                   rr1.center.y - rr1.size.y / 2,
			                   rr1.size.x, rr1.size.y);
			Bounds rr2 = this.GetComponent<Collider>().bounds;
			Rect r2 = new Rect(rr2.center.x - rr2.size.x / 2, rr2.center.y - rr2.size.y / 2, rr2.size.x, rr2.size.y);
			if(r1.Overlaps(r2))														//碰到主角	
			{	
				LevelOneGameManager.Instance.SetHeroBloodReduce(0.01f);				//主角生命值减少
			}
		}
		else 																	//如果史莱姆进入死亡阶段
		{
			m_blinkTimer += Time.deltaTime;										//开启死亡闪烁计时器
			if(m_blinkTimer>=0.2f)												//需要闪
			{						
				m_blinkCount ++;												//换图次数增加
				m_blinkTimer = 0f;												//计时器归位
				if(m_blinkCount==3)												//根据次数判定要显示的史莱姆图
					this.GetComponent<SpriteRenderer>().sprite = m_slimeDieSprite[1];
				else if(m_blinkCount==2||m_blinkCount==4)
					this.GetComponent<SpriteRenderer>().sprite = m_slimeDieSprite[0];
				else if(m_blinkCount==5)										//闪烁三次后
				{
					LevelOneGameManager.Instance.SetMessageType(2, injuryCount.ToString()+"金币");		///获得金币
					LevelOneGameManager.Instance.SetCurrAddMoney(injuryCount);			//增加金币数量
					Destroy(this.gameObject);									//销毁这只史莱姆
				}
			}
		}

	}
}
