using UnityEngine;
using System.Collections;

public class LevelOneHeroCollider : MonoBehaviour
{
	public LevelOneHeroListener m_targetStateListener = null;			//主角状态侦听
	
	void OnTriggerEnter2D(Collider2D colliderObj)					//进入碰撞检测区域
	{
		
		switch(colliderObj.tag)
		{
		case "PlatformBottom":										//最底层 不能下行区域

			m_targetStateListener.OnVerticalStateChange(LevelOneHeroController.heroVerticalStates.landing);
			if(LevelOneHeroController.m_climbState==3)
				LevelOneHeroController.m_climbState = 0;
			else if(LevelOneHeroController.m_climbState==2)
				LevelOneHeroController.m_climbState = 6;
			break;
		case "Platform":											//主角落到地面上时触发landing状态
			m_targetStateListener.OnVerticalStateChange(LevelOneHeroController.heroVerticalStates.landing);
			break; 
		}
	}
}
