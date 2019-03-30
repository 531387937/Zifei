using UnityEngine;
using System.Collections;

public class LevelThreeSwordWallController : MonoBehaviour 
{
	public GameObject m_sordWallHero;										//主角
	public GameObject m_swordDownCollider;									//剑墙下落的包围盒检测区域
	public GameObject m_launchCollider;										//剑墙发射剑的开关检测区域
	public GameObject m_sword;												//剑墙发射的剑
	public Transform m_launchPos;											//发射剑的位置

	public Vector3 checkSwordStatePos;
	private float m_downSpeed = 0.03f;										//剑墙下落速度
	private int m_swordWallState = 0;										//剑墙状态
	private float m_launchTimer = 0f;										//发射剑的时间间隔		

	private bool isSwordLaunch = false;//SWORD  launch state.

	private Transform heroTrans;

	void Start(){
		heroTrans = m_sordWallHero.transform;

	}

	void Update()
	{
        if (LevelThreeGameManager.Instance.GetBloodNum() <= 0) return;

		if (!isSwordLaunch && heroTrans.position.x >= checkSwordStatePos.x) {

			if(LevelThreeGameManager.Instance.GetAchieveGot(23) == 0)
			{
				AudioManager.Instance.TipsSoundPlay(Global.GetInstance().audioName_AchieveGet);

				LevelThreeGameManager.Instance.SetAchieveGot(23, 1);
				LevelThreeGameManager.Instance.SetMessageType(3, "您获得了成就【以逸待劳】");
			}
		}

		Bounds _heroRR1 = m_sordWallHero.GetComponent<Collider2D>().bounds;                 //主角的包围盒
        
            //Rect _heroR1 = new Rect(_heroRR1.center.x - _heroRR1.size.x / 2,
            //                    _heroRR1.center.y - _heroRR1.size.y / 2,
            //                    _heroRR1.size.x, _heroRR1.size.y);


        Rect _heroR1 = new Rect(_heroRR1.center.x - _heroRR1.size.x / 2,
                                _heroRR1.center.y + 1, _heroRR1.size.x, _heroRR1.size.y);                  //主角的包围盒

        Bounds _swordDownCollider = m_swordDownCollider.GetComponent<Collider>().bounds;//剑墙下落检测区域
		switch(m_swordWallState)
		{
		case 0:													
			Rect _swordDownColliderRect = new Rect(_swordDownCollider.center.x - _swordDownCollider.size.x / 2, _swordDownCollider.center.y - _swordDownCollider.size.y / 2, _swordDownCollider.size.x, _swordDownCollider.size.y);
			if(_heroR1.Overlaps(_swordDownColliderRect))					//如果主角触发剑墙下落机关
			{
				m_swordWallState = 1;                                       //进入剑墙下落阶段
                    AudioManager.Instance.SoundPlay(Global.GetInstance().audioName_LevelThree_LevelThreeStoneDown);

                }

                break;
		case 1:
			Bounds _swordWallCollider = this.GetComponent<Collider>().bounds;				//剑墙包围盒
			Rect _swordWallColliderRect = new Rect(_swordWallCollider.center.x - _swordWallCollider.size.x / 2
													,_swordWallCollider.center.y - _swordWallCollider.size.y / 2
													,_swordDownCollider.size.x, _swordDownCollider.size.y);

                if (_heroR1.Overlaps(_swordWallColliderRect))					//如果主角碰到正在下落的剑墙
				LevelThreeGameManager.Instance.SetHeroBloodReduce(0.02f);	//主角生命值减少
			
			if(this.transform.position.y>5.4f)								//下落未结束
			{
				m_downSpeed += 0.01f;										//速度不断增加
				transform.Translate (0f, -m_downSpeed, 0f);			//剑墙下落
			}
			else 															//下落结束
			{
				m_swordWallState = 2;										//剑墙进入下一个阶段
				transform.position = new Vector3(transform.position.x, 5.4f, transform.position.z);//指定剑墙位置
			}
			break;
		case 2:
			Bounds _swordWallLaunchCollider = m_launchCollider.GetComponent<Collider>().bounds; //剑墙发射剑的包围盒

                Rect _swordWallLaunchColliderRect = new Rect(_swordWallLaunchCollider.center.x - _swordWallLaunchCollider.size.x / 2 
                    ,_swordWallLaunchCollider.center.y - _swordWallLaunchCollider.size.y / 2
                    , _swordWallLaunchCollider.size.x
                    , _swordWallLaunchCollider.size.y);

                if (_heroR1.Overlaps(_swordWallLaunchColliderRect))				//如果主角触发发剑开关
				{
					if(LevelThreeGameManager.Instance.GetAchieveGot(1) == 0)
					{
	                        AudioManager.Instance.TipsSoundPlay(Global.GetInstance().audioName_AchieveGet);

	                        LevelThreeGameManager.Instance.SetAchieveGot(1, 1);
						LevelThreeGameManager.Instance.SetMessageType(3, "您获得了成就【打草惊蛇】");
					}
					
					isSwordLaunch = true;
					
					m_launchTimer -= Time.deltaTime;							//保证发射间隔为1s
					if(m_launchTimer<=0)
					{
	                        AudioManager.Instance.SoundPlay(Global.GetInstance().audioName_LevelThree_Sword);

	                        GameObject cloneSword = (GameObject)Instantiate(m_sword, m_launchPos.position, transform.rotation);	//初始化剑
						m_launchTimer = 1f;										//计时器复位
					}
				}
				else 															//如果主角未触发机关
					m_launchTimer = 0;											//计时器归零
				break;
		}
	}
	
}
