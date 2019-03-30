using UnityEngine;
using System.Collections;

public class BpHelpPanelController : MonoBehaviour 
{
	public GameObject m_helpPanel;
	public GameObject m_helpPanelMask;													//帮助面板遮罩
	int m_helpPanelState = 0;
	int m_helpPanelCount = 0;

	void Update()
	{
		switch(m_helpPanelState)
		{
		case 0:
			if(GameManager.Instance.GetHelpPanelOpen())
			{
				m_helpPanelState = 1;
				m_helpPanelMask.SetActive(true);
				m_helpPanel.SetActive(true);
				GameManager.Instance.CloseUsualBtn();
				m_helpPanel.GetComponent<UISprite>().spriteName = "help01";
			}
			break;
		case 1:
			if(Input.GetMouseButtonDown(0))																//如果鼠标按下
			{
				Ray ray = UICamera.mainCamera.ScreenPointToRay(Input.mousePosition);
				RaycastHit hit;
				if (Physics.Raycast(ray, out hit)) 														//检测鼠标点击区域
				{
					if(hit.collider.gameObject == m_helpPanel)										//如果点击了后边遮罩图 关闭背包
					{
						m_helpPanel.GetComponent<UISprite>().spriteName = "help02";
						m_helpPanelState = 2;
					}	
					else if(hit.collider.gameObject == m_helpPanelMask)										//如果点击了后边遮罩图 关闭背包
					{
						m_helpPanel.SetActive(false);
						m_helpPanelMask.SetActive(false);
						GameManager.Instance.OpenUsualBtn();
						GameManager.Instance.SetHelpPanelOpen(false);
						m_helpPanelState = 0;
					}	
				}
			}
			break;
		case 2:
			if(Input.GetMouseButtonDown(0))																//如果鼠标按下
			{
				Ray ray = UICamera.mainCamera.ScreenPointToRay(Input.mousePosition);
				RaycastHit hit;
				if (Physics.Raycast(ray, out hit)) 														//检测鼠标点击区域
				{
					if(hit.collider.gameObject == m_helpPanel||hit.collider.gameObject == m_helpPanelMask)		//如果点击了后边遮罩图 关闭背包
					{
						m_helpPanel.SetActive(false);
						m_helpPanelMask.SetActive(false);
						GameManager.Instance.OpenUsualBtn();
						m_helpPanelState = 0;
						GameManager.Instance.SetHelpPanelOpen(false);
					}
				}
			}
			break;
		}
	}
}
