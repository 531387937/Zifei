using UnityEngine;
using System.Collections;
using System.Xml;

public class TaskController : MonoBehaviour
{
    public GameObject m_taskUI;                                         //任务对话框
    public UILabel m_taskTitle;                                         //任务标题
    public UILabel m_taskContent;                                       //任务内容
    public GameObject m_acceptBtn;                                      //接受按钮
    public GameObject m_refuseBtn;                                      //拒绝按钮
    public GameObject[] m_chooseUI;                                     //选项栏
    public GameObject[] m_chooseOptionBtn;                              //三个选项按钮	

    private XmlDocument m_npcTalkXML;                                   //对话xml文件
    private XmlNodeList xnl;                                            //读取文件的数组
    private int m_dialogNPCNum = 0;                                     //对话的人物编号

    void Awake()
    {
        UIEventListener.Get(m_acceptBtn.gameObject).onClick = OnAcceptTaskBtnClick;             //点击接受任务按钮
        UIEventListener.Get(m_refuseBtn.gameObject).onClick = OnRefuseTaskBtn0Click;                //点击拒绝任务按钮

        UIEventListener.Get(m_chooseOptionBtn[0].gameObject).onClick = OnChooseABtnClick;           //点击A按钮
        UIEventListener.Get(m_chooseOptionBtn[1].gameObject).onClick = OnChooseBBtnClick;           //点击B按钮
        UIEventListener.Get(m_chooseOptionBtn[2].gameObject).onClick = OnChooseCBtnClick;           //点击C按钮
    }

    void OnChooseABtnClick(GameObject _aBtn)                            //点击选项A
    {
        AudioManager.Instance.SoundPlay(Global.GetInstance().audioName_BtnClick2);
        m_chooseUI[0].SetActive(false);                                 //关闭选项栏
        m_chooseUI[1].SetActive(false);                                 //关闭选项栏
        GameManager.Instance.SetStoveState(1);
        GameManager.Instance.OpenUsualBtn();                            //开启常用按钮
        GameManager.Instance.SetTaskIndexState(3, 2);                   //完成铁匠任务
    }
    void OnChooseBBtnClick(GameObject _bBtn)                            //点击选项B
    {
        AudioManager.Instance.SoundPlay(Global.GetInstance().audioName_BtnClick2);
        m_chooseUI[0].SetActive(false);                                 //关闭选项栏
        m_chooseUI[1].SetActive(false);                                 //关闭选项栏
        GameManager.Instance.SetStoveState(1);
        GameManager.Instance.OpenUsualBtn();                            //开启常用按钮
        GameManager.Instance.SetSmithyState(true);                      //开启铁匠铺
        GameManager.Instance.SetTaskIndexState(3, 2);                   //完成铁匠任务
    }
    void OnChooseCBtnClick(GameObject _vBtn)                            //点击选项C
    {
        AudioManager.Instance.SoundPlay(Global.GetInstance().audioName_BtnClick2);
        m_chooseUI[0].SetActive(false);                                 //关闭选项栏
        m_chooseUI[1].SetActive(false);                                 //关闭选项栏
        GameManager.Instance.OpenUsualBtn();                            //开启常用按钮
    }

    void OnAcceptTaskBtnClick(GameObject _acceptBtn)                    //点击接受按钮
    {
        AudioManager.Instance.SoundPlay(Global.GetInstance().audioName_Country_TaskGet);
        if (GameManager.Instance.GetReadLetterState() == 1)             //如果正在读信
        {
            GameManager.Instance.SetMessageType(1, "一封信");          //获取信消息提示
            GameManager.Instance.SetItemNum(0, 1);                      //物品栏增加一封信
            GameManager.Instance.SetReadLetterState(2);                 //信件已接受
            GameManager.Instance.SetTaskIndexState(0, 1);               //改变任务进度
            m_taskUI.SetActive(false);                                  //关闭任务框
            GameManager.Instance.OpenUsualBtn();                        //开启常用按钮
        }
        else
        {
            switch (m_dialogNPCNum - 1)
            {
                case 0:                                                     //对话人为村长
                    GameManager.Instance.SetTaskIndexState(0, 2);           //完成邮箱中信的任务
                    GameManager.Instance.SetTaskIndexState(1, 1);           //接受村长任务
                    GameManager.Instance.SetCurrAddMoney(100);
                    GameManager.Instance.SetMessageType(2, "100金币");        //获取村长给的100金币
                    break;
                case 1:                                                     //对话人为书店老板
                    GameManager.Instance.SetTaskIndexState(1, 2);           //完成村长任务
                    GameManager.Instance.SetTaskIndexState(2, 1);           //接受书店老板任务
                    break;
                case 2:                                                     //对话人为铁匠
                    GameManager.Instance.SetTaskIndexState(3, 1);           //接受铁匠任务
                    break;
                case 3:                                                     //对话人为水手
                    GameManager.Instance.SetTaskIndexState(4, 1);           //接受水手任务
                    break;
                case 4:                                                     //对话人为酒鬼
                    GameManager.Instance.SetTaskIndexState(5, 1);            //接受酒鬼任务
                    break;
                case 5:                                                     //对话人为酒保
                    GameManager.Instance.SetTaskIndexState(6, 1);           //接受酒保任务
                    break;
            }
            m_taskUI.SetActive(false);                                  //关闭任务框
            if (m_dialogNPCNum == 3)                                        //如果接受的是铁匠任务 弹出选项栏
            {
                m_chooseUI[0].SetActive(true);                              //弹出选项栏
                m_chooseUI[1].SetActive(true);                              //弹出选项栏
            }
            else                                                        //接受的非铁匠任务
                GameManager.Instance.OpenUsualBtn();                    //开启常用按钮
        }
    }

    void OnRefuseTaskBtn0Click(GameObject _refuseBtn)                                   //点击拒绝按钮
    {
        AudioManager.Instance.SoundPlay(Global.GetInstance().audioName_BtnClick2);

        m_taskUI.SetActive(false);                                                      //关闭任务框
        GameManager.Instance.OpenUsualBtn();                                            //开启常用按钮
        GameManager.Instance.SetReadLetterState(0);                                 //没有接受
        m_dialogNPCNum = 0;                                                             //NPC编号归零
    }

    void Start()
    {
        string data = Resources.Load("Task").ToString();                                //读取任务相关数据
        m_npcTalkXML = new XmlDocument();
        m_npcTalkXML.LoadXml(data);
        xnl = m_npcTalkXML.GetElementsByTagName("Task");
    }

    void Update()
    {
        if (GameManager.Instance.GetReadLetterState() == 1)                             //如果当前点击了读信
        {
            GameManager.Instance.CloseUsualBtn();

            m_taskUI.SetActive(true);                                                   //开启任务框
            m_taskTitle.text = xnl[0].ChildNodes[0].InnerText;                      //显示任务内容
            m_taskContent.text = xnl[0].ChildNodes[1].InnerText;
        }

        int _dialogState = CheckBtnController.Instance.GetDialogState();               //获取对话状态
        if (_dialogState == 3)                                                              //对话显示完毕需要弹出任务框
        {
            if (GameManager.Instance.GetTaskIndexState(3) == 1 && (GameManager.Instance.GetStoveState() != 2))          //如果已接受铁匠任务但炉子未修理
            {
                m_chooseUI[0].SetActive(true);                                              //弹出选项框
                m_chooseUI[1].SetActive(true);                                              //弹出选项框
            }
            else                                                                        //如果非铁匠任务
            {
                AudioManager.Instance.SoundPlay(Global.GetInstance().audioName_BtnClick2);

                m_dialogNPCNum = CheckBtnController.Instance.GetDialogIndex() + 1;      //编号加1（因为有信）
                m_taskUI.SetActive(true);                                               //开启任务框
                m_taskTitle.text = xnl[m_dialogNPCNum].ChildNodes[0].InnerText;     //显示任务标题
                m_taskContent.text = xnl[m_dialogNPCNum].ChildNodes[1].InnerText;       //显示任务内容

            }
            GameManager.Instance.CloseUsualBtn();                                       //关闭常用按钮
            CheckBtnController.Instance.SetDialogState(0);                              //对话状态变量归零
        }
    }
}
