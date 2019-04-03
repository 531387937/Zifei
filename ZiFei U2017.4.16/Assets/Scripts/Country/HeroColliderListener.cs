using UnityEngine;
using System.Collections;

public class HeroColliderListener : MonoBehaviour
{
    public GameObject m_hero;                                       //主角
    public HeroStateListener m_targetStateListener = null;          //主角状态侦听
    Transform m_stageEdge = null;

    void OnTriggerEnter2D(Collider2D colliderObj)                   //进入碰撞检测区域
    {
            switch (colliderObj.tag)
            {
                case "PlatformBottom":                                      //最底层 不能下行区域
                    if (m_stageEdge != null)
                        m_stageEdge.gameObject.SetActive(false);
                    GameManager.Instance.SetDownActive(false);
                    m_targetStateListener.OnVerticalStateChange(HeroStateController.heroVerticalStates.landing);
                    m_targetStateListener.m_invisibleColliderObj = colliderObj.gameObject;
                    break;
                case "Platform":                                            //主角落到地面上时触发landing状态
                    if (m_stageEdge != null)
                        m_stageEdge.gameObject.SetActive(false);
                    m_targetStateListener.OnVerticalStateChange(HeroStateController.heroVerticalStates.landing);
                    m_targetStateListener.m_invisibleColliderObj = colliderObj.gameObject;
                    GameManager.Instance.SetDownActive(true);
                    break;
            }
    }
    void OnTriggerStay2D(Collider2D colliderObj)                   //进入碰撞检测区域
    {
        if(m_targetStateListener.m_invisibleColliderObj != colliderObj.gameObject)
        switch (colliderObj.tag)
        {
            case "PlatformBottom":                                      //最底层 不能下行区域
                if (m_stageEdge != null)
                    m_stageEdge.gameObject.SetActive(false);
                GameManager.Instance.SetDownActive(false);
                m_targetStateListener.OnVerticalStateChange(HeroStateController.heroVerticalStates.landing);
                m_targetStateListener.m_invisibleColliderObj = colliderObj.gameObject;
                break;
            case "Platform":                                            //主角落到地面上时触发landing状态
                if (m_stageEdge != null)
                    m_stageEdge.gameObject.SetActive(false);
                m_targetStateListener.OnVerticalStateChange(HeroStateController.heroVerticalStates.landing);
                m_targetStateListener.m_invisibleColliderObj = colliderObj.gameObject;
                GameManager.Instance.SetDownActive(true);
                break;
        }
    }
   
}
