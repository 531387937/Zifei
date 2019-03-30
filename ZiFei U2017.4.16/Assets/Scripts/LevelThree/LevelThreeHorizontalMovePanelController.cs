using UnityEngine;
using System.Collections;

public class LevelThreeHorizontalMovePanelController : MonoBehaviour 
{
	public Transform[] m_moveEdge;												//移动的边界
	private float m_moveSpeed = 0.03f; 

	void Update()
	{
		if(this.transform.position.x<=m_moveEdge[0].position.x)
			m_moveSpeed = 0.03f;
		else if(this.transform.position.x>=m_moveEdge[1].position.x)
			m_moveSpeed = -0.03f;
		this.transform.Translate (m_moveSpeed, 0f, 0f);							//平板左右移动
		if(LevelThreeGameManager.Instance.GetHeroOnMovePanel(0))				//如果主角站在移动的平板上
			LevelThreeGameManager.Instance.SetMovePanelSpeed (m_moveSpeed);		//获取平板当前的速度
	}
}
