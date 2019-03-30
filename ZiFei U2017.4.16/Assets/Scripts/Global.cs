public class Global 
{
	public string loadName;
    public string audioName_AchieveGet = "Music/AchieveGet";
    public string audioName_BtnClick = "Music/BtnClick";
    public string audioName_BtnClick2 = "Music/BtnClick2";
    public string audioName_BtnClick3 = "Music/BtnClick3";
    public string audioName_BpPageDown = "Music/BpPageDown";
    public string audioName_Down  = "Music/Down";
    public string audioName_GlassWave = "Music/GlassWave";
    public string audioName_GoldCoin = "Music/GoldCoin";
    public string audioName_HeroBloodReduce = "Music/HeroBloodReduce";
    public string audioName_JoyStick = "Music/JoyStick";
    public string audioName_Stone = "Music/Stone";
    public string audioName_BtnDownClick = "Music/BtnDownClick";
    public string audioName_Penetralium = "Music/Penetralium";
    public string audioName_DeadSound = "Music/DeadSound";
    public string audioName_SuccessSound = "Music/SuccessSound";
    public string audioName_HouseLightLaser = "Music/HouseLightLaser";
    public string audioName_MessageTips = "Music/MessageTips";
    public string audioName_BackpageItemClick = "Music/BackpageItemClick";
    public string audioName_Boomerang = "Music/Boomerang";
    

    public string audioName_HeadBattle_HeadWeapomLaser1 = "Music/HeadBattle/HeadWeapomLaser1";
    public string audioName_HeadBattle_HeadWeapomLaser2 = "Music/HeadBattle/HeadWeapomLaser2";

    public string audioName_Country_BarDoor = "Music/Country/BarDoor";
    public string audioName_Country_LevelDoorOpen = "Music/Country/LevelDoorOpen";
    public string audioName_Country_HeroHomeDoorr = "Music/Country/HeroHomeDoor";
    public string audioName_Country_CountryHeadDoor = "Music/Country/CountryHeadDoor";
    public string audioName_Country_JarBreak = "Music/Country/JarBreak";
    public string audioName_Country_Sad = "Music/Country/Sad";
    public string audioName_Country_RepairBoiler = "Music/Country/RepairBoiler";
    public string audioName_Country_SoundSpeaker = "Music/Country/SoundSpeaker";
    public string audioName_Country_TaskGet = "Music/Country/TaskGet";
    public string audioName_Country_Valve = "Music/Country/Valve";
    public string audioName_Country_DialogSpeak = "Music/Country/DialogSpeak";

    public string audioName_LevelOne_MonkeyInjury = "Music/LevelOne/MonkeyInjury";

    public string audioName_LevelTwo_IronDoorDown = "Music/LevelTwo/IronDoorDown";
    public string audioName_LevelTwo_LevelTwoIronDoor = "Music/LevelTwo/LevelTwoIronDoor";
    public string audioName_LevelTwo_LevelTwoTentacle = "Music/LevelTwo/LevelTwoTentacle";
    public string audioName_LevelTwo_SpiderBirth = "Music/LevelTwo/SpiderBirth";
    public string audioName_LevelTwo_SpiderDown = "Music/LevelTwo/SpiderDown";
    public string audioName_LevelTwo_SpiderInjury = "Music/LevelTwo/SpiderInjury"; 
    public string audioName_LevelTwo_Swamp = "Music/LevelTwo/Swamp"; 

    public string audioName_LevelThree_LevelThreeLightWave = "Music/LevelThree/LevelThreeLightWave";
    public string audioName_LevelThree_LevelThreeStoneDown = "Music/LevelThree/LevelThreeStoneDown";
    public string audioName_LevelThree_Sword = "Music/LevelThree/Sword";



    private static Global instance;
    private Global()
    {

    }

    public static Global GetInstance()
	{
		if (instance == null)
			instance = new Global();
		
		return instance;
	}
}