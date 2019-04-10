using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class SceneFadeInOut : MonoBehaviour
{
	public float fadeSpeed = 1.5f;
	public GameObject[] m_doorObj;															//三个大门
	public GameObject[] m_doorMask;															//三个大门遮罩

	private bool sceneStarting = true;														//是否是场景第一次开启
	private int m_doorState = 0;															//当前开门动画阶段
	private AnimatorStateInfo m_doorAniInfo;												//门动画变量
	private int m_doorIndex = 0;															//当前相关的门编号
	private Animator m_doorAnimator;														//门动画组件

	void Awake()
	{
		GetComponent<GUITexture>().pixelInset = new Rect(0f, 0f, Screen.width, Screen.height);
	}
	
	void Update()
	{
		if (sceneStarting)																	//场景开启
			StartScene();																	//调用打开函数

		switch(m_doorState)																	//判断当前场景切换阶段
		{
		case 0:																				//初始阶段														
			if(GameManager.Instance.GetLevelIndex()!=0)										//需要切换
			{
				m_doorState = 1;															//进行下一阶段
				m_doorIndex = GameManager.Instance.GetLevelIndex();							//获取要进的关卡编号
				m_doorAnimator = m_doorObj[m_doorIndex-1].GetComponent<Animator>();			//获取门的动画组件
				m_doorAnimator.GetComponent<Animator>().SetBool("OpenFirstDoor", true);		//转至开门动画
				m_doorMask[m_doorIndex-1].SetActive(true);									//开启相应遮罩
				SetDoorMaskAlpha(0.2f);														//设置遮罩的alpha
			}
			break;
		case 1:																				//第二阶段 开门动画
			m_doorAniInfo = m_doorAnimator.GetCurrentAnimatorStateInfo(0);					//动画
			if(m_doorAniInfo.IsName("DoorIsOpenAnimation"))									//开门动画播完
				m_doorState = 2;
			if(m_doorIndex==1)																//需进入第一关关卡
			{
				if(m_doorObj[m_doorIndex-1].GetComponent<SpriteRenderer>().sprite.name=="ani_doorG_03")
					SetDoorMaskAlpha(0.5f);													//设置遮罩的alpha
			}
			else if(m_doorIndex==2)															//需进入第二关卡
			{
				if(m_doorObj[m_doorIndex-1].GetComponent<SpriteRenderer>().sprite.name=="ani_doorB_03")
					SetDoorMaskAlpha(0.5f);													//设置遮罩的alpha
			}
			else if(m_doorIndex==3)															//需进入第三关卡
			{
				if(m_doorObj[m_doorIndex-1].GetComponent<SpriteRenderer>().sprite.name=="ani_doorR_03")
					SetDoorMaskAlpha(0.5f);													//设置遮罩的alpha
			}
			break;
		case 2:
			EndScene();																		//淡出
			break;
		}
	}

	void SetDoorMaskAlpha(float _alpha)
	{
		m_doorMask[m_doorIndex-1].GetComponent<SpriteRenderer> ().color 
			= new Color(m_doorMask[m_doorIndex-1].GetComponent<SpriteRenderer> ().color.r, 
			            m_doorMask[m_doorIndex-1].GetComponent<SpriteRenderer> ().color.g,
			            m_doorMask[m_doorIndex-1].GetComponent<SpriteRenderer> ().color.b,
			            _alpha	
			            );		
	}
	
	void FadeToClear()
	{
		GetComponent<GUITexture>().color = Color.Lerp(GetComponent<GUITexture>().color, Color.clear, fadeSpeed * Time.deltaTime);
	}
	
	void FadeToBlack()
	{
		GetComponent<GUITexture>().color = Color.Lerp (GetComponent<GUITexture>().color, Color.black, fadeSpeed * Time.deltaTime);
	}
	
	void StartScene()																		//首次开启场景函数
	{
		FadeToClear();																		//淡入
		if(GetComponent<GUITexture>().color.a < 0.05f)														//如果淡入图的alpha足够小
		{
			GetComponent<GUITexture>().color = Color.clear;													//清除
			GetComponent<GUITexture>().enabled = false;														//关闭淡入淡出图
			sceneStarting  = false;															//第一次开启已结束
		}
	}
	
	public void EndScene()
	{
		GetComponent<GUITexture>().enabled = true;
		FadeToBlack();
		if(GetComponent<GUITexture>().color.a >= 0.95f)
		{
			switch(m_doorIndex)
			{
			case 1:
				Global.GetInstance().loadName = "LevelOne";
				break;
			case 2:
				Global.GetInstance().loadName = "LevelTwo";
				break;
			case 3:
				Global.GetInstance().loadName = "LevelThree";
				break;
			}
            //xy 19.3.30
 GameManager.Instance.SaveCurrData(true);                                           //存档
            //
            SceneManager.LoadScene("LoadingScene");
		}
			
	}
	
}