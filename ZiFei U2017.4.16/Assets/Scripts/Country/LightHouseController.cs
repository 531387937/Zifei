using UnityEngine;
using System.Collections;

public class LightHouseController : MonoBehaviour 
{
	public GameObject[] m_light;													//三个灯
	public GameObject m_lightHouseBtn;												//灯塔按钮
	public Sprite m_buttonDown;														//灯塔按钮按下图
	public GameObject m_lightBox;													//灯塔盒子
	public Sprite m_lightBoxSprite;													//灯塔盒子图
	public GameObject m_lightHouseWave;												//灯塔盒子射出来的光

	public GameObject[] m_flower;													//三朵花			
	public Sprite[] m_flowerSprite;													//三朵花花生长后的样子

	private float m_lightHouseTimer;												//灯塔计时器
	private int m_lightHouseBlinkCount = 0;											//灯塔闪烁次数
	private int m_lightHouseStepState = 0;												//灯塔阶段
	private int m_lightIndex = 0;													//需要亮的灯编号
	private bool m_lightStateOpen = true;											//灯塔灯是否亮										
	private bool[] m_fuelGot = new bool[3];											//三个燃料获得情况
	private int m_flowerState = 0;													//花开的状态
	private float m_flowerTimer = 0f;												//花开的计时器


    private Sprite m_buttonDownOrigin;
    private Sprite m_lightBoxSpriteOrigin;


    void Start()
    {
        m_buttonDownOrigin = m_lightHouseBtn.GetComponent<SpriteRenderer>().sprite;
        m_lightBoxSpriteOrigin = m_lightBox.GetComponent<SpriteRenderer>().sprite;
        //InitLightData();    
    }

    public void InitLightData()
    {
        for (int i = 0; i < 3; i++)
        {
            var seedState = GameManager.Instance.GetSeedState(i) == 2;//种子已经种下

            m_light[i].GetComponent<SpriteRenderer>().enabled = seedState;
            m_flower[i].SetActive(seedState);                                //开启相应的花图

            if (seedState)
                m_flower[i].GetComponent<SpriteRenderer>().sprite = m_flowerSprite[i];//花为生长后的图

            m_fuelGot[i] = seedState;
        }

        m_lightHouseStepState = GameManager.Instance.GetLightHouseStepState();


        var lightHouseState = GameManager.Instance.GetLightHouseState() == 6;//已经按下灯塔的按钮

        if (lightHouseState)
        {
            m_lightHouseBtn.GetComponent<SpriteRenderer>().sprite = m_buttonDown;//更换按钮图为按下
            m_lightBox.GetComponent<SpriteRenderer>().sprite = m_lightBoxSprite;//更换灯塔盒子
            m_lightHouseStepState = 6;
            //GameManager.Instance.SetLightHouseStepState(m_lightHouseStepState);

        }
        else {
            m_lightHouseBtn.GetComponent<SpriteRenderer>().sprite = m_buttonDownOrigin;//更换按钮图为原来的
            m_lightBox.GetComponent<SpriteRenderer>().sprite = m_lightBoxSpriteOrigin;//更换灯塔盒子原来的
        }
        m_lightBox.SetActive(lightHouseState);                                         //是否开启灯塔盒子
        m_lightHouseWave.SetActive(lightHouseState);//发射激光
         
    }



    void Update()
	{

		switch (m_lightHouseStepState)                                                  //判定当前灯塔状态
        {
		case 0:
                if (GameManager.Instance.GetGameStartState() == 5)                          //如果当前有灯需要亮
                {
                    for (int i = 0; i < 3; i++)                                             //如果得到燃料 相应的灯要亮起
                    {
                        if (GameManager.Instance.GetSeedState(i) == 2)                      //如果该编号燃料已经获得
                        {
                            m_light[i].GetComponent<SpriteRenderer>().enabled = true;
                            m_flower[i].SetActive(true);                                //开启相应的花图
                            m_flower[i].GetComponent<SpriteRenderer>().sprite = m_flowerSprite[i];//花为生长后的图
                            m_fuelGot[i] = true;
                        }
                    }
                    m_lightHouseStepState = 1;                                              //下一阶段
                    GameManager.Instance.SetLightHouseStepState(m_lightHouseStepState);
                    m_lightIndex = GameManager.Instance.GetEndItemState() - 1;       //获取当前需要闪的灯的编号
                }
                break;
		case 1:
                if (m_lightHouseBlinkCount == 0)                                            //当前未闪烁
                {
                    m_lightHouseBlinkCount = 1;                                         //开始第一次闪
                    m_light[m_lightIndex].GetComponent<SpriteRenderer>().enabled = true;//打开灯
                    m_lightHouseTimer = 0;
                }
                else                                                                    //当前灯在闪烁
                {
                    m_lightHouseTimer += Time.deltaTime;                                //开启计时器
                    if (m_lightHouseTimer >= 0.05f)                                     //需要闪
                    {
                        m_lightStateOpen = !m_lightStateOpen;                           //改变灯的状态
                        m_light[m_lightIndex].GetComponent<SpriteRenderer>().enabled = m_lightStateOpen;
                        m_lightHouseBlinkCount++;                                       //换图次数增加
                        m_lightHouseTimer = 0f;                                         //计时器归位
                        if (m_lightHouseBlinkCount == 20)                                   //闪烁足够多
                        {
                            m_lightHouseStepState = 2;
                            GameManager.Instance.SetLightHouseStepState(m_lightHouseStepState);

                            GameManager.Instance.SetGameStartState(6);                  //摄像机恢复
                            m_light[m_lightIndex].GetComponent<SpriteRenderer>().enabled = false;//关闭当前正在闪的灯
                        }
                    }
                }
                break;
            case 2:

                if (m_fuelGot[0] && m_fuelGot[1] && m_fuelGot[2])                           //如果三个灯都亮了
                {
                    GameManager.Instance.SetLightHouseState(5);                         //可以开按钮
                    m_lightHouseStepState = 3;
                    GameManager.Instance.SetLightHouseStepState(m_lightHouseStepState);

                }
                else
                {
                    for (int i = 0; i < 3; i++)                                                 //如果得到燃料 相应的灯要亮起
                    {
                        if (!m_fuelGot[i] && GameManager.Instance.GetSeedState(i) == 2)
                        {

                            m_fuelGot[i] = true;
                            m_lightIndex = i;                                           //获取要开的花编号
                            m_flowerState = 1;
                            m_light[i].GetComponent<SpriteRenderer>().enabled = true;
                        }
                    }
                }
                break;
            case 3:                                                                     //检测按钮是否打开
                if (GameManager.Instance.GetLightHouseState() == 6)                     //按下灯塔按钮
                {
                    m_lightHouseStepState = 4;
                    GameManager.Instance.SetLightHouseStepState(m_lightHouseStepState);

                    m_lightHouseTimer = 0.5f;
                }
                break;
            case 4:
                m_lightHouseTimer -= Time.deltaTime;
                if (m_lightHouseTimer < 0)
                {
                    m_lightHouseBtn.GetComponent<SpriteRenderer>().sprite = m_buttonDown;//更换按钮图为按下
                    m_lightBox.SetActive(true);                                         //开启灯塔盒子
                    m_lightHouseTimer = 0.3f;
                    m_lightHouseStepState = 5;
                    GameManager.Instance.SetLightHouseStepState(m_lightHouseStepState);

                }
                break;
            case 5:                                                                     //灯塔盒子打开
                m_lightHouseTimer -= Time.deltaTime;
                if (m_lightHouseTimer < 0)
                {
                    m_lightBox.GetComponent<SpriteRenderer>().sprite = m_lightBoxSprite;//更换灯塔盒子
                    m_lightHouseStepState = 6;
                    GameManager.Instance.SetLightHouseStepState(m_lightHouseStepState);

                    m_lightHouseWave.SetActive(true);                                   //射出光
                }
                break;
            case 6:
                break;
        }
    


		FlowerCheck ();																//花状态检测
	}

	void FlowerCheck()
	{
		if (m_flowerState <= 0)
			return;
		
		switch(m_flowerState)
		{
		case 1:
			m_flowerTimer += Time.deltaTime;
			if(m_flowerTimer>=0.5f)
			{
				m_flower[m_lightIndex].SetActive(true);
				m_flowerState = 2;
				m_flowerTimer = 0.2f;
			}
			break;
		case 2:
			m_flowerTimer -= Time.deltaTime;
			if(m_flowerTimer<0)
			{
				m_flower[m_lightIndex].GetComponent<SpriteRenderer>().sprite = m_flowerSprite[m_lightIndex];
				m_flowerState = 3;
				switch(m_lightIndex)
				{
				case 0:
					GameManager.Instance.SetMessageType(1, "绿色灯塔燃料");
					GameManager.Instance.SetItemNum(12, 1);	
					break;
				case 1:
					GameManager.Instance.SetMessageType(1, "蓝色灯塔燃料");
					GameManager.Instance.SetItemNum(13, 1);	
					break;
				case 2:
					GameManager.Instance.SetMessageType(1, "红色灯塔燃料");
					GameManager.Instance.SetItemNum(14, 1);	
					break;
				}
				m_flowerState = 0;
			}
			break;
		}
	}
}
