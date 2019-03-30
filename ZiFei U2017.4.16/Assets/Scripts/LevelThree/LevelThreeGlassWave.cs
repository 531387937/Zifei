using UnityEngine;
using System.Collections;

public class LevelThreeGlassWave : MonoBehaviour
{
	private int m_glassWaveState = 0;
	private float m_addSpeed = 0.5f;
	private float m_waveTimer = 0.3f;

	void Update()
	{
		switch(m_glassWaveState)
		{
		case 0:
			if(LevelThreeGameManager.Instance.GetGlassWaveEmit())
			{
				m_glassWaveState = 1;
			}
			break;
		case 1:
			Vector3 _scale = this.transform.localScale;
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
			break;
		case 2:
			m_waveTimer -= Time.deltaTime;
			if(m_waveTimer<0)
			{
				m_glassWaveState = 0;
				LevelThreeGameManager.Instance.SetGlassWaveEmit(false);
				this.transform.localScale = new Vector3(1f, 0f, 1f);
				m_addSpeed = 0.5f;
			}
			break;
		}
	}
}
