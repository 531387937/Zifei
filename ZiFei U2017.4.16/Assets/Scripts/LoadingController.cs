using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class LoadingController : MonoBehaviour 
{
	AsyncOperation async;
	public  float m_time = 2f;

    private bool changeScene = false;
    private float timer = 0f;
	void Start()
	{
        //在这里开启一个异步任务，
        //进入loadScene方法。
        StartCoroutine(loadScene());

        timer = m_time;

    }
	
	//注意这里返回值一定是 IEnumerator
	IEnumerator loadScene()
	{
		//异步读取场景。
		//Globe.loadName 就是A场景中需要读取的C场景名称。
        async = SceneManager.LoadSceneAsync("SceneOne");
        //读取完毕后返回， 系统会自动进入C场景
        async.allowSceneActivation = false;
        yield return async;
		
	}

    private void Update()
    {
        if (!changeScene)
        {
            timer -= Time.deltaTime;
            if (timer <= 0)
            {
                timer = m_time;
                changeScene = true;
                async.allowSceneActivation = true;

            }

        }
    }

}

