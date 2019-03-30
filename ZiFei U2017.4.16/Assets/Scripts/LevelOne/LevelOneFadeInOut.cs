using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class LevelOneFadeInOut : MonoBehaviour
{

    public GameObject m_UIRoot;
    public GameObject m_uiMonsterPanel;
    public float turnSceneTime = 3f;
    public float fadeSpeed = 1.5f;

    private bool sceneStarting = true;
    private bool m_hideUI = false;

    private bool isChangeScene = false;

    private GUITexture texture;
    private float timer = 0f;

    void Awake()
    {
        texture = GetComponent<GUITexture>();
        texture.pixelInset = new Rect(0f, 0f, Screen.width, Screen.height);
    }

    private void Start()
    {
        isChangeScene = false;
        timer = turnSceneTime;

    }


    void Update()
    {
        if (sceneStarting)
            StartScene();

        if (!isChangeScene && LevelOneGameManager.Instance.GetTurnToCountry() != 0)               //需要返回村庄
        {
            m_UIRoot.SetActive(false);

            if (!m_hideUI)
            {
                m_uiMonsterPanel.SetActive(false);
                m_hideUI = true;
            }
            timer = turnSceneTime;
            texture.enabled = true;
            isChangeScene = true;
        }

        if (isChangeScene)
        {
            timer -= Time.deltaTime;
            texture.color = Color.Lerp(texture.color, Color.black, fadeSpeed * Time.deltaTime);

            if (timer <= 0)
            {
                EndScene();

            }


        }



    }


    void StartScene()
    {
        texture.color = Color.Lerp(texture.color, Color.clear, fadeSpeed * Time.deltaTime);

        if (texture.color.a < 0.05f)
        {
            texture.color = Color.clear;
            texture.enabled = false;
            sceneStarting = false;
        }
    }

    public void EndScene()
    {

        timer = turnSceneTime;
        isChangeScene = false;
        Global.GetInstance().loadName = "SceneOne";
        SceneManager.LoadScene("LoadingScene");
    }

}
