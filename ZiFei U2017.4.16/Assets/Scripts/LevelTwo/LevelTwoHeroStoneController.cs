using UnityEngine;
using System.Collections;

public class LevelTwoHeroStoneController : MonoBehaviour
{

	private float m_stoneSpeed = 0.3f;							//石头移动速度
	private float m_heroScaleX = 1;                             //主角朝向
    private SpriteRenderer _renderer;
    private Camera mainCamera;
    void Start()
	{
		m_heroScaleX = LevelTwoGameManager.Instance.GetHeroScaleX ();
        mainCamera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
        _renderer = GetComponent<SpriteRenderer>();
    }

	void OnTriggerEnter2D(Collider2D colliderObj)					//进入碰撞检测区域
	{
		if(colliderObj.tag=="LevelTwoSpider")						//打中蜘蛛
		{
			Destroy(this.gameObject);								//销毁石头
			LevelTwoGameManager.Instance.SetSpiderHit(true);		//打中蜘蛛
			LevelTwoGameManager.Instance.SetSpiderBloodReduce(0.005f);	//蜘蛛血量减少
		}
	}

	void Update()
	{

        if (!GameObjectUtil.IsVisibleFrom(_renderer, mainCamera)) Destroy(this.gameObject);

        if (m_heroScaleX>0)
		{
			if(this.transform.position.x<30f)							//如果石头超出边界
				this.transform.Translate(m_stoneSpeed, 0f, 0f);			//石头飞出去
			else 														//石头落出底边界
				Destroy(this.gameObject);								//销毁石头
		}
		else
		{
			if(this.transform.position.x>-30f)							//如果石头超出边界
				this.transform.Translate( -m_stoneSpeed, 0f, 0f);		//石头飞出去
			else 														//石头落出底边界
				Destroy(this.gameObject);								//销毁石头
		}
	}
}
