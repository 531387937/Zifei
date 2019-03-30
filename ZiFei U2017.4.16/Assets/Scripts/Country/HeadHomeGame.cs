using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class HeadHomeGame : MonoBehaviour
{
	public GameObject[] m_musicPanel;									//四个音乐板
	public GameObject m_panelSelectBox;
	public GameObject m_cabinet; 										//柜子
	private int m_headState = 0;
	private float m_panelTimer = 0.3f;									
	private int m_panelIndex = 0;
	private int m_panelCount = 0;											//板子亮起的计数
	private int m_correctCount = 0;


    List<int> resourceData;
    //排序之后的List newDaata
    List<int> newData = new List<int>();

    private void OnEnable()
    {
        resourceData = new List<int>(new int[] { 0, 1, 2, 3, 4 });

       // ResetCabinetState();


    }

    public void ResetCabinetState()
    {

        if (GameManager.Instance.GetHeadHomeState() >= 3)
        {
            m_cabinet.transform.position = new Vector3(3.1f, -12.30152f, -0.7f);
            GameManager.Instance.SetHeadHomeState(4);
            m_headState = 4;
        }
        else {
            m_cabinet.transform.position = new Vector3(0.9f, -12.30152f, -0.7f);
            m_panelSelectBox.transform.position = new Vector3(100, 0, 0);
            m_headState = 0;
        }

    }

    void ClickPanelCheck()
	{
		if(Input.GetMouseButtonDown(0))
		{
			Ray _ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			RaycastHit _hit;
            if (Physics.Raycast(_ray, out _hit))
            {
                if (_hit.transform.gameObject == m_musicPanel[newData[m_panelIndex]].gameObject)
					m_correctCount++;
			}
		}
	}
	
	void Update()
	{
		switch(m_headState)
		{
		case 0:
			if(GameManager.Instance.GetHeadHomeState()==1)
			{
                    RandomIndex();
                m_headState = 1;
				GameManager.Instance.SetHeadHomeState(2);				//音乐正在播放
				GetComponent<AudioSource>().Play();
				m_panelSelectBox.SetActive(true);
				m_panelTimer = 0.2f;
				m_panelIndex = 0;
				m_panelCount = 0;
				m_correctCount = 0;
			}
			break;
		case 1:
			m_panelTimer -= Time.deltaTime;
			if(m_panelTimer<=0)
			{
				if(m_panelCount==5)
					m_headState = 2;
				else
				{
					m_panelTimer = 0.8f;
					m_panelIndex++;
					if(m_panelIndex==4)
						m_panelIndex = 0;
					m_panelSelectBox.transform.position = m_musicPanel[newData[m_panelIndex]].transform.position;
					m_panelCount++;
				}
			}
			ClickPanelCheck();
			break;
		case 2:
			if(m_correctCount==5)
			{
                    AudioManager.Instance.ThingSoundPlay(Global.GetInstance().audioName_Penetralium);

                    GameManager.Instance.SetHeadHomeState(3);
                    m_panelSelectBox.transform.position = new Vector3(100,0,0);
                    m_panelSelectBox.SetActive(false);
                    m_headState = 3;

				if (GameManager.Instance.GetAchieveGot(0) == 0)
				{
					AudioManager.Instance.TipsSoundPlay(Global.GetInstance().audioName_AchieveGet);
					GameManager.Instance.SetAchieveGot(0, 1);
					GameManager.Instance.SetMessageType(3, "您获得了成就 【暗度陈仓】");
				}

			}
			else
			{
				GameManager.Instance.SetHeadHomeState(0);
                    m_panelSelectBox.transform.position = new Vector3(100,0,0);
                    m_panelSelectBox.SetActive(false);
				m_headState = 0;
			}
			break;
		case 3:
			if(m_cabinet.transform.position.x<=3.53f)
				m_cabinet.transform.Translate(0.1f, 0f, 0f);
			else
			{
				GameManager.Instance.SetHeadHomeState(4);
				m_headState = 4;
			}
			break;
		}
	}




    public void RandomIndex() {
        newData.Clear();
        resourceData = new List<int>(new int[] { 0, 1, 2, 3, 4 });
    //为了降低运算的数量级，当执行完一个元素时，就需要把此元素从原List中移除
    int countNum = resourceData.Count;
        //使用while循环，保证将resourceData中的全部元素转移到newData中而不产生遗漏
        while (newData.Count<countNum)
    {
            //随机将resourceData中序号为index的元素作为b中的第一个元素放入b中
            int index = Random.Range(0, resourceData.Count - 1);
      //检测是否重复，保险起见
         if (!newData.Contains(resourceData[index])) {
                //若newData中还没有此元素，添加到b中
                newData.Add(resourceData[index]);
                //成功添加后，将此元素从resourceData中移除，避免重复取值
                resourceData.Remove(resourceData[index]);
         }
    }
    }

}
