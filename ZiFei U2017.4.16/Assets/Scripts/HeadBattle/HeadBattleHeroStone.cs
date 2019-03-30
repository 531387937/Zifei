using UnityEngine;
using System.Collections;

public class HeadBattleHeroStone : MonoBehaviour
{
	private float m_stoneSpeed = 0.3f;							//石头移动速度
	private float m_heroScaleX = 1;								//主角朝向
	
	void Start()
	{
		m_heroScaleX = HeadBattleGameManager.Instance.GetHeroScaleX ();
		
	}

    void OnTriggerEnter2D(Collider2D colliderObj)                   //进入碰撞检测区域
    {
        if (colliderObj.tag == "CountryHead")                                           //打中村长
        {
            Destroy(this.gameObject);
            if (HeadBattleGameManager.Instance.GetAttackType() == 3)
                HeadBattleGameManager.Instance.SetHeadBloodReduce(0.01f);
            else
                HeadBattleGameManager.Instance.SetHeadBloodReduce(0.005f);
        }
		else if (colliderObj.tag=="EdgeLeft"||colliderObj.tag=="EdgeRight")			//打中边界
			Destroy(this.gameObject);
	}
	
	void Update()
	{
		if(m_heroScaleX>0)
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
