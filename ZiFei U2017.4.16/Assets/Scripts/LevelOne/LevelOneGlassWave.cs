using UnityEngine;
using System.Collections;

public class LevelOneGlassWave : MonoBehaviour 
{
	private int m_currDir;														//判断发射的方向
	private int m_glassWaveState = 0;											//眼镜波的状态
	private float m_addSpeed = 0.1f;
	private float m_waveTimer = 0.8f;
	private bool m_stopAdd = false;	

	void OnTriggerEnter2D(Collider2D colliderObj)								//进入碰撞检测区域
	{
		if(colliderObj.tag=="Monkey")
		{
			LevelOneGameManager.Instance.SetMonkeyBloodReduce(0.05f);
		}
		else if(colliderObj.tag=="Slime1")
		{
			LevelOneGameManager.Instance.SetSlimeInjury(0, true);
		}
		else if(colliderObj.tag=="Slime2")
		{
			LevelOneGameManager.Instance.SetSlimeInjury(1, true);
		}
		else if(colliderObj.tag=="EdgeRight"||colliderObj.tag=="EdgeLeft")		//碰到左右边界
		{
			m_stopAdd = true;
		}
	}

	void Update()
	{
		switch(m_glassWaveState)												//判定眼镜波的状态
		{
		case 0:																		
			if(LevelOneGameManager.Instance.GetGlassWaveEmit())					//需要发射
			{
				int _newDir = LevelOneGameManager.Instance.GetHeroStoneDir ();	//获取主角当前方向
				if(m_currDir==0)
				{
					if(_newDir!=0)
						transform.Rotate (0f, 0f, -90f);
				}
				else 
				{
					if(_newDir==0)
						transform.Rotate (0f, 0f, 90f);
				}
				m_currDir = _newDir;
				m_glassWaveState = 1;
			}
			break;
		case 1:
			Vector3 _scale = this.transform.localScale;
			if(!m_stopAdd)
			{
				if(_scale.y<=10f)
				{
					_scale.y += m_addSpeed;
					m_addSpeed += 0.5f;
					this.transform.localScale = _scale;
				}
				else
				{
					m_glassWaveState = 2;
					m_waveTimer = 0.3f;
				}
			}
			else
			{
				m_glassWaveState = 2;
				m_waveTimer = 0.3f;
			}
			break;
		case 2:
			m_waveTimer -= Time.deltaTime;
			if(m_waveTimer<0)
			{
				m_glassWaveState = 0;
				LevelOneGameManager.Instance.SetGlassWaveEmit(false);
				this.transform.localScale = new Vector3(1f, 0f, 1f);
				m_addSpeed = 0.5f;
				m_stopAdd = false;
			}
			break;
		}
	}
}
