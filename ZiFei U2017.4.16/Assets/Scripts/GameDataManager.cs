using UnityEngine; 
using System.Collections; 
 
public class GameSaveData										//GameSaveData类 用于定义存档的数据
{

    public bool newHeroState;                                     //是否是新的数据
    public bool startState;                                     //是否是第一次进入游戏
    public bool used;											//该存档点是否被使用 
	public int  levelIndex;									//存档时所处的场景编号(0村庄 1家 2村长家 3酒馆 4森林 5地牢 6熔洞)
    public string gameSceneName;                              //游戏的scene名称
	public Vector3 heroPosition;								//存档时主角所在的位置
	public float heroLife;									//主角生命值
	public int heroMoney;									//存档时主角的钱数
	public int equipedIndex;								//存档时装备的道具
	public int saveDataIndex;//当前的存档位置
	public int[] items;             						//存档时物品数量情况
	public int[] countryTaskState;                          //存档时村庄中的任务状态
    public int[] achieveState;                                  //36计成就是否获得状态

    public int [] seedState;                                    //种子的状态（0未得到 1得到未用 2种下）
    public int[] keyState;                                    ////钥匙状态（0得不到 1可以得到 2已得到）
    public int [] lightState;
    public int[] flowerState;

    public int getEndItemState;                                        //是否获得通关物品
    public int letterReadState;                                 //信件的状态（0：未收到， 1：收到未读 2：已读）
    public int waterBoxRepairSatate;
    public int jarSatate;
    public int barTaskState;
    public int homeCoinState;//0 not get/ 1:get
    public int stoveState;
    public int lightHouseState;
    public int lightHouseStepState;
    public int headHomeGameState;//村长家游戏状态(0按下喇叭 1开始游戏 2完成游戏 3箱子打开)
    public System.DateTime savedTime;							//存档时的时间
	public int NPCIndex;                                        //最后对话的NPC编号

    public float BGMVolume;                                     //背景音的音量
    public float SoundVolume;                                   //音效的音量

    public GameSaveData()
	{
        newHeroState = true;
        startState = true;
        used = false;										
		levelIndex = 0;
        gameSceneName = "";
        heroPosition = new Vector3(55.14f, -11.565f, -1f);		//主角初始位置在家门口
		heroMoney = 0;
		heroLife = 1f;
		equipedIndex = 0;
		saveDataIndex = -1;


		items = new int[20];
		countryTaskState = new int[8];
		achieveState = new int[36];
		seedState = new int[3];
		keyState = new int[4];
		lightState = new int[3];
		flowerState = new int[3];
        getEndItemState = 0;
        letterReadState = 0;
        waterBoxRepairSatate = 0;
        jarSatate = 0;
        barTaskState = 0;
        homeCoinState = 0;
        stoveState = 0;
        lightHouseState = 0;
        lightHouseStepState = 0;
        headHomeGameState = 0;
        savedTime = System.DateTime.Now;
        NPCIndex = 0;

        BGMVolume = 1f;                                         //背景音音量
        SoundVolume = 1f;                                       //音效音量
    }
}


public class GameData 											//GameData,储存数据的类，把需要储存的数据定义在GameData之内就行
{ 
	public string key;                                          //密钥,用于防止拷贝存档 

    public GameSaveData GameCurrentData = new GameSaveData();
    public GameSaveData GameSaveData1 = new GameSaveData();	
	public GameSaveData GameSaveData2 = new GameSaveData();	
	public GameSaveData GameSaveData3 = new GameSaveData();	


	public GameData() 
	{
        GameCurrentData = new GameSaveData();

	} 
} 











public class GameDataManager:MonoBehaviour 						//管理数据储存的类
{ 
	public static GameDataManager Instance;						//单例化
	private string dataFileName ="GameData.dat";			//存档文件的名称,自己定
	private XmlSaver xs = new XmlSaver(); 
	
	public GameData gameData; 



	public void Awake() 
	{ 
		Instance = this;
		gameData = new GameData(); 								//设定密钥，根据具体平台设定
		gameData.key = SystemInfo.deviceUniqueIdentifier; 
		//Load(); 
	} 

	public void Save() 							    			//存档时调用的函数
	{ 
		string gameDataFile = GetDataPath() + "/"+dataFileName; 
		string dataString= xs.SerializeObject(gameData,typeof(GameData)); 
		xs.CreateXML(gameDataFile,dataString); 
	} 
    
	public void Load() 											//读档时调用的函数
	{ 
		string gameDataFile = GetDataPath() + "/"+dataFileName; 
		if(xs.hasFile(gameDataFile))  							//存档文件存在
		{
            //print("have data");
			string dataString = xs.LoadXML(gameDataFile); 
			GameData gameDataFromXML = xs.DeserializeObject(dataString,typeof(GameData)) as GameData; 

			if(gameDataFromXML.key == gameData.key) 			//是合法存档
			{ 
				gameData = gameDataFromXML;
                //print("gameData.HeroLifeNum :" + gameData.HeroLifeNum);

            } 
			else 												//是非法拷贝存档
			{ 
				//留空：游戏启动后数据清零，存档后作弊档被自动覆盖
			} 
		} 
		else 													//存档文件不存在
		{ 
			if(gameData != null) 								//当前有数据
				Save(); 										//新建存档文件
		} 
	} 

	private static string GetDataPath() 						//获取路径
	{ 
		if(Application.platform == RuntimePlatform.IPhonePlayer)//如果是iphone
		{ 
			string path = Application.persistentDataPath;//.Substring (0, Application.dataPath.Length - 5);
			//path = path.Substring(0, path.LastIndexOf('/'));  
			return path ;//+ "/Documents"; 
		} 
		else if(Application.platform == RuntimePlatform.Android)//如果是android
		{
			return Application.persistentDataPath ;
		}
		else 
			return Application.dataPath;
	} 
} 

