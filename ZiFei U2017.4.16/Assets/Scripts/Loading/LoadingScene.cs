using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class LoadingScene : MonoBehaviour 
{
    public GameObject hero;
    public float time = 1f;
	public float m_speed = 0.03f;
	public GameObject[] m_backGround;

	private int m_currIndex = 0;
	private AsyncOperation async;
    private float timer = 0f;

    private float moveThreshold = 6f;


    void Start()
	{
        if (Global.GetInstance().loadName == "SceneOne")
        {
            var scale = hero.transform.localScale;
            hero.transform.localScale = new Vector3(-scale.x, scale.y, scale.z);
            m_speed = -m_speed;
            moveThreshold = -moveThreshold;
            m_currIndex = 1;
        }


        StartCoroutine(loadScene());
    }

    IEnumerator loadScene()
	{
        //异步读取场景。
        //Globe.loadName 就是当前场景中需要读取的其他预加载的场景名称。
        //async = Application.LoadLevelAsync(Global.GetInstance().loadName);
        async = SceneManager.LoadSceneAsync(Global.GetInstance().loadName);
        //async = SceneManager.LoadSceneAsync("SceneOne");
        async.allowSceneActivation = false;
        //读取完毕后返回， 系统会自动进入C场景
        yield return async;
		
	}

	void Update()
	{

        BGMove();

        timer += Time.deltaTime;
        if (timer >= time && async != null && async.progress >= 0.9f)
        {
            async.allowSceneActivation = true;

        }


    }

    void BGMove()
    { 

        m_backGround[0].transform.Translate(-m_speed, 0f, 0f);           //云1图向左运动
        m_backGround[1].transform.Translate(-m_speed, 0f, 0f);           //云2图向左运动
        if (m_speed > 0f)
        {
            if (m_backGround[m_currIndex].transform.position.x <= -moveThreshold)          //云当前图移动出场景
            {
                ResetPos();
            }

        }
        else {
            if (m_backGround[m_currIndex].transform.position.x >= -moveThreshold)          //云当前图移动出场景
            {
                ResetPos();
            }

        }

    }

    private void ResetPos()
    {
        if (m_currIndex == 0)                                           //判定当前索引
        {
            m_backGround[m_currIndex].transform.position =          //将移出场景的云图放置最右边
                new Vector3(m_backGround[1].transform.position.x + moveThreshold, m_backGround[1].transform.position.y, m_backGround[1].transform.position.z);
            m_currIndex = 1;                                            //更改当前云编号
        }
        else
        {
            m_backGround[m_currIndex].transform.position =
                new Vector3(m_backGround[0].transform.position.x + moveThreshold, m_backGround[0].transform.position.y, m_backGround[0].transform.position.z);
            m_currIndex = 0;
        }
    }
}

