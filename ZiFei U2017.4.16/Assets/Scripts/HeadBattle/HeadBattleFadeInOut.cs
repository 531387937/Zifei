using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class HeadBattleFadeInOut : MonoBehaviour
{
	public float fadeSpeed = 1.5f;
	
	private bool sceneStarting = true;
	
	void Awake()
	{
		GetComponent<GUITexture>().pixelInset = new Rect(0f, 0f, Screen.width, Screen.height);
	}
	
	void Update()
	{
		if (sceneStarting)
			StartScene();

		if(HeadBattleGameManager.Instance.GetGameState()==5)				//需要返回村庄
		{
			EndScene();
			//HeadBattleGameManager.Instance.SetGameState(6);
		}
	}
	
	void FadeToClear()
	{
		GetComponent<GUITexture>().color = Color.Lerp(GetComponent<GUITexture>().color, Color.clear, fadeSpeed * Time.deltaTime);
	}
	
	void FadeToBlack()
	{
		GetComponent<GUITexture>().color = Color.Lerp (GetComponent<GUITexture>().color, Color.black, fadeSpeed * Time.deltaTime);
	}
	
	void StartScene()
	{
		FadeToClear();
		
		if(GetComponent<GUITexture>().color.a < 0.05f)
		{
			GetComponent<GUITexture>().color = Color.clear;
			GetComponent<GUITexture>().enabled = false;
			sceneStarting  = false;
		}
	}
	
	public void EndScene()
	{
		GetComponent<GUITexture>().enabled = true;
		FadeToBlack();
		
		if(GetComponent<GUITexture>().color.a >= 0.95f)
		{
			Global.GetInstance().loadName = "SceneOne";
            SceneManager.LoadScene("LoadingScene");
        }
    }
	
}
