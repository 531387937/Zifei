using UnityEngine;
using System.Collections;

public class LevelThreeWhaleController : MonoBehaviour
{
	public GameObject m_whaleHero;										//主角
	private float m_whaleSpeed = -0.02f;								//熔岩鲸游动的速度
    private Bounds whaleBounds;
    private Rect whaleRect;

    private Bounds heroBounds;
    private Rect heroRect;

        

    private void Start()
    {
            whaleBounds = this.GetComponent<Collider>().bounds;
        whaleRect = new Rect(whaleBounds.center.x - whaleBounds.size.x / 2
                                , whaleBounds.center.y - whaleBounds.size.y / 2
                                , whaleBounds.size.x, whaleBounds.size.y);

        heroBounds = m_whaleHero.GetComponent<Collider2D>().bounds;                     //主角的包围盒
        heroRect = new Rect(heroBounds.center.x - heroBounds.size.x / 2,
                           heroBounds.center.y - heroBounds.size.y / 2,
                           heroBounds.size.x, heroBounds.size.y);

        //heroRect = new Rect(rr1.center.x - rr1.size.x / 2,
        //                    rr1.center.y + 1, rr1.size.x, rr1.size.y);                  //主角的包围盒


    }

    void Update()
	{
        if (LevelThreeGameManager.Instance.GetBloodNum() <= 0) return;
        
        Vector3 _localScale = this.transform.localScale;				//获取鲸朝向
		if(this.transform.position.x<=-13f)								//碰到左边界 需向右移动
		{
			_localScale.x *= -1f;										//改变鲸朝向
			this.transform.localScale = _localScale;
			m_whaleSpeed = 0.02f;										//改变速度方向
		}
		else if(this.transform.position.x>=-9.3f)						//碰到右边界 需左移动
		{
			_localScale.x *= -1f;										//改变鲸朝向
			this.transform.localScale = _localScale;
			m_whaleSpeed = -0.02f;										//改变速度方向
		}
		this.transform.Translate (m_whaleSpeed, 0f, 0f);				//鲸移动

		if(this.GetComponent<SpriteRenderer>().sprite.name=="Whale5"||	//如果当前鲸正处于喷射状态
		   this.GetComponent<SpriteRenderer>().sprite.name=="Whale6"||
		   this.GetComponent<SpriteRenderer>().sprite.name=="Whale7")
        {

            whaleBounds = this.GetComponent<Collider>().bounds;
            heroBounds = m_whaleHero.GetComponent<Collider2D>().bounds;                     //主角的包围盒

            heroRect = new Rect(heroBounds.center.x - heroBounds.size.x / 2,
                           heroBounds.center.y - heroBounds.size.y / 2,
                           heroBounds.size.x, heroBounds.size.y);
            whaleRect = new Rect(whaleBounds.center.x - whaleBounds.size.x / 2
                                , whaleBounds.center.y - whaleBounds.size.y / 2
                                , whaleBounds.size.x, whaleBounds.size.y);


            if (heroRect.Overlaps(whaleRect))                                       //如果鲸碰到主角
            {
				LevelThreeGameManager.Instance.SetHeroBloodReduce(0.0075f);	//主角血量减少
            }
		}
	}
}
