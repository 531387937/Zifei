using UnityEngine;
using System.Collections;

public class JarController : MonoBehaviour 
{
	public GameObject[] m_jarObj;											//两个罐子物品

	private float m_oldBoxX = 0f;											//重力感应X检测
	private float m_oldBoxY = 0f;											//重力感应Y检测
	private int m_jarBreakState = 0;										//罐子破碎动画

    private Animator jarAnimator;

	private SpriteRenderer jar0SpriteRenderer;







    private void Awake()
    {
        jarAnimator = m_jarObj[0].GetComponent<Animator>();

		jar0SpriteRenderer = m_jarObj[0].GetComponent<SpriteRenderer> ();

        ResetJarState();
    }

    void Update () 
	{
		
		
		switch(m_jarBreakState)
		{
		case 0:
			if(GameManager.Instance.GetJarState()!=2)							//如果罐子未碎
			{
				if(GameManager.Instance.GetMeetJar())							//如果主角碰到罐子
				{
					float _newX = Input.acceleration.x;
					float _newY = -Input.acceleration.y;
					
					float _disX = Mathf.Abs (m_oldBoxX - _newX);
					float _disY = Mathf.Abs (m_oldBoxY - _newY);
					if((_disX>1||_disY>1))
					{
                        AudioManager.Instance.SoundPlay(Global.GetInstance().audioName_Country_JarBreak);

						jarAnimator.SetTrigger("JarBreakState");	//开启罐子摔碎动画
						GameManager.Instance.SetJarState(2);					//罐子已碎
						GameManager.Instance.SetMessageType(2, "20金币");		//主角获得十金币
						GameManager.Instance.SetCurrAddMoney(20);
						m_jarBreakState = 1;

						if (GameManager.Instance.GetAchieveGot(21) == 0)
						{
							AudioManager.Instance.TipsSoundPlay(Global.GetInstance().audioName_AchieveGet);
							GameManager.Instance.SetAchieveGot(21, 1);
							GameManager.Instance.SetMessageType(3, "您获得了成就【无中生有】");
						}
					}
					m_oldBoxX = _newX;
					m_oldBoxY = _newY;
				}
			}
			break;
		case 1:
			if(jar0SpriteRenderer.sprite.name == "jar08")
			{
				m_jarObj[0].SetActive(false);
				m_jarObj[1].SetActive(true);
				m_jarBreakState = 2;
			}
			break;
		case 2:
			m_jarObj [0].SetActive (false);
			m_jarObj [1].SetActive (true);
			m_jarBreakState = 3;

			break;
		}

	}

    public void ResetJarState() {
        if (GameManager.Instance.GetJarState() == 2)//罐子已碎
        {
            m_jarObj[0].SetActive(false);
            m_jarObj[1].SetActive(true);
			m_jarBreakState = 3;
        }
        else {
			m_jarObj[1].SetActive(false);
            m_jarObj[0].SetActive(true);
			m_jarBreakState = 0;
        }


    }


}
