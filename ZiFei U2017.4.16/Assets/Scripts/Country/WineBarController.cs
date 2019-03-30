using UnityEngine;
using System.Collections;

public class WineBarController : MonoBehaviour 
{
	public GameObject m_wineBarMaskPic; 									//酒馆遮罩图
	private float m_maskPicAlpha = 1f;
	private float m_maskPicAlphaTimer = 0.1f;								//遮罩alpha变化速率
	private int m_maskPicAlphaCount = 0;									//闪烁次数

	void MaskPicMove()														//遮罩图移动函数
	{
		m_maskPicAlphaTimer -= Time.deltaTime;
		if(m_maskPicAlphaTimer<0)
		{
			m_maskPicAlpha = Random.Range (0.7f, 1f);
			m_maskPicAlphaCount ++;
			if(m_maskPicAlphaCount==10)
			{
				m_maskPicAlphaTimer = 3f;
				m_maskPicAlphaCount = 0;
				m_maskPicAlpha = 1;
			}
			else 
				m_maskPicAlphaTimer = 0.1f;

			m_wineBarMaskPic.GetComponent<SpriteRenderer> ().color 
				= new Color(m_wineBarMaskPic.GetComponent<SpriteRenderer> ().color.r, 
				            m_wineBarMaskPic.GetComponent<SpriteRenderer> ().color.g,
				            m_wineBarMaskPic.GetComponent<SpriteRenderer> ().color.b,
				            m_maskPicAlpha	
				            );												//遮罩alpha渐变
		}
	}
		
	void Update()
	{
		MaskPicMove ();
	}

}
