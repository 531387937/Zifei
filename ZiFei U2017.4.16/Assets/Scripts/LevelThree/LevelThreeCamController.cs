using UnityEngine;
using System.Collections;

public class LevelThreeCamController : MonoBehaviour 
{
	public GameObject m_heroObj = null;							//主角对象
	public float m_cameraTrackingSpeed = 1f;					//摄像机的追踪速度
	public Transform m_RborderPos;								//右卷屏右边界位置
	public Transform m_LborderPos;								//左卷屏左边界位置
	public Transform m_heroCamPos;								//主角决定的相机位置
	private Vector3 m_lastTargetPos = Vector3.zero;				//上一个目标位置
	private Vector3 m_currTargetPos = Vector3.zero;				//下一个目标位置
	private float m_currLerpDis = 0.0f;
	
	void Start () 
	{
		Vector3 _heroPos = m_heroObj.transform.position;		//记录主角位置
		Vector3 _startTargetPos = _heroPos;
		m_lastTargetPos = _startTargetPos;						//上一个和下一个目标位置均为主角所在位置
		m_currTargetPos = _startTargetPos;
	}
	
	void LateUpdate()
	{
		if(LevelThreeGameManager.Instance.GetCamTrackingValue())
		{
			TrackPlayer ();											//实时更新摄像机和主角的位置
			m_currLerpDis += m_cameraTrackingSpeed;					//将摄像机移动到目标位置
			this.transform.position = Vector3.Lerp (m_lastTargetPos, m_currTargetPos, m_currLerpDis);
		}

	}
	
	
	void TrackPlayer()
	{
		Vector3 _currCamPos = this.transform.position;			//获取并保存摄像机和主角在世界坐标系中的坐标
		Vector3 _heroPos = m_heroCamPos.transform.position;
		m_lastTargetPos = _currCamPos;
		if(_heroPos.x>m_RborderPos.position.x)					//主角超过右边界
			m_currTargetPos.x = m_RborderPos.position.x;		//摄像机水平不跟随
		else if(_heroPos.x<m_LborderPos.position.x)			//主角小于左边界
			m_currTargetPos.x = m_LborderPos.position.x;		//摄像机水平不跟随
		else 													//主角x轴正常
			m_currTargetPos.x = _heroPos.x;
		m_currTargetPos.y = _currCamPos.y;						//保证摄像机在Z轴方向上的值不变
		m_currTargetPos.z = _currCamPos.z;						//保证摄像机在Z轴方向上的值不变
	}
}

