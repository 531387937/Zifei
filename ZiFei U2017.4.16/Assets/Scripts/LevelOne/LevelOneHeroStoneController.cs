using UnityEngine;
using System.Collections;

public class LevelOneHeroStoneController : MonoBehaviour 
{
	private float m_stoneSpeed = 0.7f;							//石头下落速度
	private int m_dir;

    private SpriteRenderer _renderer;
    private Camera mainCamera;

    void Start()
	{
		m_dir = LevelOneGameManager.Instance.GetHeroStoneDir ();
        mainCamera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
        _renderer = GetComponent<SpriteRenderer>();

    }

	void OnTriggerEnter2D(Collider2D colliderObj)					//进入碰撞检测区域
	{
		if(colliderObj.tag=="Monkey")
		{
			Destroy(this.gameObject);								//销毁石头
			LevelOneGameManager.Instance.SetMonkeyBloodReduce(0.02f);
		}
		else if(colliderObj.tag=="Slime1")
		{
			Destroy(this.gameObject);								//销毁石头
			LevelOneGameManager.Instance.SetSlimeInjury(0, true);
		}
		else if(colliderObj.tag=="Slime2")
		{
			Destroy(this.gameObject);								//销毁石头
			LevelOneGameManager.Instance.SetSlimeInjury(1, true);
		}
	}
	
	void Update()
	{

        if (!GameObjectUtil.IsVisibleFrom(_renderer, mainCamera)) Destroy(this.gameObject);

        switch (m_dir)
		{
		case 0:															//朝上飞
			if(this.transform.position.y<49f)								//如果石头还没落出底边界
				this.transform.Translate(0f, m_stoneSpeed, 0f);		//石头下落
			else 														//石头落出底边界
				Destroy(this.gameObject);								//销毁石头
			break;
		case 1:															//朝左飞
			if(this.transform.position.x>-4.9f)								//如果石头还没落出底边界
				this.transform.Translate(-m_stoneSpeed, 0f, 0f);		//石头下落
			else 														//石头落出底边界
				Destroy(this.gameObject);								//销毁石头
			break;
		case 2:															//朝右飞
			if(this.transform.position.x<4.9f)								//如果石头还没落出底边界
				this.transform.Translate(m_stoneSpeed, 0f, 0f);		//石头下落
			else 														//石头落出底边界
				Destroy(this.gameObject);								//销毁石头
			break;
		}

	}
}
