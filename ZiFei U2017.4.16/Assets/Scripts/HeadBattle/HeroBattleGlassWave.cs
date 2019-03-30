using UnityEngine;
using System.Collections;

public class HeroBattleGlassWave : MonoBehaviour 
{
	public GameObject m_waveObj;
	private int m_glassWaveState = 0;
	private float m_addSpeed = 0.5f;
	private float m_waveTimer = 0.3f;
	private bool m_stopAdd = false;	

	void OnTriggerEnter2D(Collider2D colliderObj)										//进入碰撞检测区域
	{
		if(m_glassWaveState!=0)
		{
			if(colliderObj.tag=="CountryHead")										//打中
			{
				if(HeadBattleGameManager.Instance.GetAttackType()==1||HeadBattleGameManager.Instance.GetAttackType()==3)
					HeadBattleGameManager.Instance.SetHeadBloodReduce(0.05f);
				else
					HeadBattleGameManager.Instance.SetHeadBloodReduce(0.025f);
			}
		}
	}
	
	void Update()
	{
		switch(m_glassWaveState)
		{
		case 0:
			if(HeadBattleGameManager.Instance.GetGlassWaveEmit())
				m_glassWaveState = 1;
			break;
		case 1:
			Vector3 _scale = m_waveObj.transform.localScale;
			if(!m_stopAdd)
			{
				if(_scale.y<=0.5f)
				{
					_scale.y += m_addSpeed;
					m_addSpeed += 0.5f;
					m_waveObj.transform.localScale = _scale;
				}
				else
				{
					m_glassWaveState = 2;
					m_waveTimer = 0.3f;
					this.GetComponent<BoxCollider2D>().enabled = true;
				}
			}
			else
			{
				m_glassWaveState = 2;
				m_waveTimer = 0.3f;
				this.GetComponent<BoxCollider2D>().enabled = true;
			}
			break;
		case 2:
			m_waveTimer -= Time.deltaTime;
			if(m_waveTimer<0)
			{
				m_glassWaveState = 0;
				HeadBattleGameManager.Instance.SetGlassWaveEmit(false);
				m_waveObj.transform.localScale = new Vector3(1f, 0f, 1f);
				m_addSpeed = 0.5f;
				m_stopAdd = false;
				this.GetComponent<BoxCollider2D>().enabled = false;
			}
			break;
		}
	}
}
