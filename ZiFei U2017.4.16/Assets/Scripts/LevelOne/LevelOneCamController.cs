using UnityEngine;
using System.Collections;

public class LevelOneCamController : MonoBehaviour 
{
	public GameObject m_heroObj = null;							//主角对象
	public float m_cameraTrackingSpeed = 1f;					//摄像机的追踪速度
	public Transform m_topBorderPos;							//卷屏最高值
	public Transform m_bottomBorderPos;							//卷屏最低值
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
		TrackPlayer ();											//实时更新摄像机和主角的位置
		m_currLerpDis += m_cameraTrackingSpeed;					//将摄像机移动到目标位置
		this.transform.position = Vector3.Lerp (m_lastTargetPos, m_currTargetPos, m_currLerpDis);
	}
	
	void TrackPlayer()
	{
		Vector3 _currCamPos = this.transform.position;			//获取并保存摄像机和主角在世界坐标系中的坐标
		Vector3 _heroPos = m_heroCamPos.transform.position;
		m_lastTargetPos = _currCamPos;

		if(_heroPos.y>m_topBorderPos.position.y)				//如果相机达到最高点
			m_currTargetPos.y = m_topBorderPos.position.y;		//保持最高点不变
		else if(_heroPos.y<m_bottomBorderPos.position.y)		//如果相机达到最低值
			m_currTargetPos.y = m_bottomBorderPos.position.y; 	//保持最低值不变
		else 													//相机要移动
			m_currTargetPos.y = _heroPos.y;						//随主角一起移动
		m_currTargetPos.x = _currCamPos.x;						//摄像机水平不跟随
		m_currTargetPos.z = _currCamPos.z;						//保证摄像机在Z轴方向上的值不变
	}
}

