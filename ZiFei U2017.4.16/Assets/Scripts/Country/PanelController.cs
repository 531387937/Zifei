using UnityEngine;
using System.Collections;
using System.Xml;

public class PanelController : MonoBehaviour 
{
	public GameObject[] m_countryPanel;										//村庄中的展板图
	public GameObject m_countryPanelMask;									//村庄中展板图遮罩
	public UILabel[] m_tipPanelLabel;										//提示面板文字

	private int m_currPanelIndex = 0;										//当前打开的展板编号
	private XmlDocument m_tipXML;											//小提示xml文件
	private XmlNodeList m_tipXnl ;											//读取文件的数组

	void Start()														
	{
		string data = Resources.Load("TipPanelContent").ToString(); 		//读取任务相关数据
		m_tipXML = new XmlDocument ();								
		m_tipXML.LoadXml (data);
		m_tipXnl = m_tipXML.GetElementsByTagName("Tip");
	}

	void Update()
	{
		if(m_currPanelIndex==0)												//当前没有展板打开
		{
			if (GameManager.Instance.GetPanelState (1))						//如果需打开地图展板
				m_currPanelIndex = 1;
			else if (GameManager.Instance.GetPanelState (2))				//如果需打开任务展板
			{
				m_currPanelIndex = 2;
				int _index = Random.Range(0, 4);							//随机一条提示显示
				m_tipPanelLabel[0].text = m_tipXnl[_index].ChildNodes[0].InnerText;
				m_tipPanelLabel[1].text = m_tipXnl[_index].ChildNodes[1].InnerText;
			}
			else if (GameManager.Instance.GetPanelState (3))				//如果需打开水箱告示展板
				m_currPanelIndex = 3;
			else if (GameManager.Instance.GetPanelState (4))				//如果需打开主角家展板
				m_currPanelIndex = 4;

			if(m_currPanelIndex!=0)											//如果当前需要打开展板
			{
				m_countryPanel[0].SetActive(true);							//开启展板面板
				m_countryPanel[m_currPanelIndex].SetActive(true);			//开启相应展板
				m_countryPanelMask.SetActive(true);							//开启展板遮罩
				GameManager.Instance.CloseUsualBtn();						//关闭常用按钮
			}
		}
		else 																//如果当前有展板打开
		{
			if(Input.GetMouseButtonDown(0))									//如果鼠标按下
			{
				Ray ray = UICamera.mainCamera.ScreenPointToRay (Input.mousePosition);
				RaycastHit hit;
				if (Physics.Raycast(ray, out hit)) 							//检测鼠标点击区域
				{
					if(hit.collider.gameObject == m_countryPanelMask)		//如果点击了后边遮罩图 关闭背包
					{
						m_countryPanel[0].SetActive(false);					//关闭展板面板
						m_countryPanel[m_currPanelIndex].SetActive(false);	//关闭指定展板
						m_countryPanelMask.SetActive(false);				//关闭遮罩图
						GameManager.Instance.SetPanelState(m_currPanelIndex,false);	
						GameManager.Instance.OpenUsualBtn();				//开启常用按钮
						m_currPanelIndex = 0;
					}
				}
			}
		}
	}
}
