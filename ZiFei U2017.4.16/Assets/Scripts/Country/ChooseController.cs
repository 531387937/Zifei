using UnityEngine;
using System.Collections;

public class ChooseController : MonoBehaviour 
{
	public GameObject[] m_chooseUI;										//选项面板（两个选项 ） 后方遮罩
	public GameObject[] m_chooseOptionBtn;								//两个选项按钮
	public UILabel[] m_chooseOptionLabel;
	public GameObject[] m_huoLuObj;										//村庄中的物品 火炉
	public Sprite[] m_huoLuSprite;										//火炉图片

	private int m_thingIndex;

	void Awake()
	{
		UIEventListener.Get (m_chooseOptionBtn[0].gameObject).onClick = OnChooseABtnClick;			//点击A按钮
		UIEventListener.Get (m_chooseOptionBtn[1].gameObject).onClick = OnChooseBBtnClick;			//点击B按钮
	}

	void OnChooseABtnClick(GameObject _Abtn)
	{
        AudioManager.Instance.SoundPlay(Global.GetInstance().audioName_BtnClick2);                    
		m_chooseUI[0].SetActive(false);
		m_chooseUI[1].SetActive(false);															//关闭选项栏	
			GameManager.Instance.OpenUsualBtn();													//开启常用按钮
		if(m_thingIndex==0)
		{
			GameManager.Instance.SetStoveState(2);													//锅炉修好了
			GameManager.Instance.SetTaskIndexState(2,2);											//完成书店老板任务
			m_huoLuObj[0].GetComponent<SpriteRenderer>().sprite = m_huoLuSprite[0];					//更改火炉及火炉选框图
			m_huoLuObj[1].GetComponent<SpriteRenderer>().sprite = m_huoLuSprite[1];

            AudioManager.Instance.SoundPlay(Global.GetInstance().audioName_Country_RepairBoiler);

        }
        else if(m_thingIndex==1)																	//如果给醉鬼酒
		{
			if(GameManager.Instance.GetAchieveGot(28) == 0)
			{
                AudioManager.Instance.TipsSoundPlay(Global.GetInstance().audioName_AchieveGet);

                GameManager.Instance.SetAchieveGot(28, 1);
				GameManager.Instance.SetMessageType(3, "您获得了成就【假痴不癫】");
			}
            GameManager.Instance.SetWaterBoxRepairState(1);									//酒鬼倒下 可点击水阀修理
			GameManager.Instance.SetTaskIndexState(5,2);											//完成酒鬼任务
			
		}
		else if(m_thingIndex==2)
		{
			
			GameManager.Instance.SetSeedState(0, 2);                                                //绿种子种下
        }
		else if(m_thingIndex==3)
		{
			
			GameManager.Instance.SetSeedState(1, 2);												//蓝种子种下


        }
        else if(m_thingIndex==4)
		{
			
			GameManager.Instance.SetSeedState(2, 2);												//红种子种下

        }
    }




	void OnChooseBBtnClick(GameObject _Bbtn)
	{
        AudioManager.Instance.SoundPlay(Global.GetInstance().audioName_BtnClick2);                    
		GameManager.Instance.OpenUsualBtn();													//开启常用按钮
		m_chooseUI[0].SetActive(false);
		m_chooseUI[1].SetActive(false);															//关闭选项栏		
	}

	void Update()
	{
		if(CheckBtnController.Instance.GetThingDialogState()==2)
		{
			CheckBtnController.Instance.SetThingDialogState(0);
			m_chooseUI[0].SetActive(true);
			m_chooseUI[1].SetActive(true);
			GameManager.Instance.CloseUsualBtn();
			m_chooseOptionLabel[0].text = "使用扳手";
			m_chooseOptionLabel[1].text = "不使用扳手";
			m_thingIndex = 0;
		}
		else if(GameManager.Instance.GetChooseState()==1)
		{
			GameManager.Instance.SetChooseState(0);
			m_chooseUI[0].SetActive(true);
			m_chooseUI[1].SetActive(true);
			GameManager.Instance.CloseUsualBtn();
			m_chooseOptionLabel[0].text = "给醉鬼酒";
			m_chooseOptionLabel[1].text = "不给醉鬼酒";
			m_thingIndex = 1;
		}
		else if(GameManager.Instance.GetChooseState()==2)
		{
			GameManager.Instance.SetChooseState(0);
			m_chooseUI[0].SetActive(true);
			m_chooseUI[1].SetActive(true);
			GameManager.Instance.CloseUsualBtn();
			m_chooseOptionLabel[0].text = "种下绿色的种子";
			m_chooseOptionLabel[1].text = "随便看看，不想种";
			m_thingIndex = 2;
		}
		else if(GameManager.Instance.GetChooseState()==3)
		{
			GameManager.Instance.SetChooseState(0);
			m_chooseUI[0].SetActive(true);
			m_chooseUI[1].SetActive(true);
			GameManager.Instance.CloseUsualBtn();
			m_chooseOptionLabel[0].text = "种下蓝色的种子";
			m_chooseOptionLabel[1].text = "随便看看，不想种";
			m_thingIndex = 3;
		}
		else if(GameManager.Instance.GetChooseState()==4)
		{
			GameManager.Instance.SetChooseState(0);
			m_chooseUI[0].SetActive(true);
			m_chooseUI[1].SetActive(true);
			GameManager.Instance.CloseUsualBtn();
			m_chooseOptionLabel[0].text = "种下红色的种子";
			m_chooseOptionLabel[1].text = "随便看看，不想种";
			m_thingIndex = 4;
		}
	}
}
