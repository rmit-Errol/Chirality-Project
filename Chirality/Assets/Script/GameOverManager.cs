﻿using System.Collections;
using System.Collections.Generic;
using Facebook.Unity;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameOverManager : MonoBehaviour {

	[SerializeField] Text gameTitle;
	[SerializeField] Text scoreLabel;
	[SerializeField] Text percentageLabel;
	[SerializeField] Text highPercentageLabel;
	[SerializeField] Image newRecord;
	[SerializeField] Image[] medals;
	[SerializeField] GameObject infoPanel;
	[SerializeField] GameObject infoPanelStandard;
	[SerializeField] GameObject infoPanelExtreme;
	[SerializeField] GameObject infoPanelTrial;
    [SerializeField] GameObject infoPanelLvl4Extreme;
	[SerializeField] Canvas canvas;
	[SerializeField] Text goodEffortLabel;
	public GameObject backgroundMusic;
	private GameObject backgroundMusicObject = null;

	private string title = "";
	private int score = 0;
	private float percentage = 0f;
	private float highPercentage = 0f;
	private string medalString = "";

    private float time = 0f;
    private float timeP = 0f;

    void Start () {
		if(!FB.IsInitialized) {
			FB.Init(FBInitCompletion);
		}else {
			FB.ActivateApp();
		}

		infoPanel.SetActive(false);
		newRecord.gameObject.SetActive(false);

		loadRecords();
		displayRecord();
		displayMedalAndComment();
		updateHighScore();
        updateTime();
		configureBackgroundMusic();
	}

	void Update() {
		if(Input.GetKey(KeyCode.Escape)) {
			goToMenuScene();
		}
	}

	void configureBackgroundMusic() {
		if(PlayerPrefsX.GetBool("Background_Music_Toggle") && !GameObject.Find("BackgroundMusic(Clone)")) {
			backgroundMusicObject = Instantiate(backgroundMusic,Vector3.zero,Quaternion.identity);
		}		
	}

	void loadRecords() {
		title = PlayerPrefs.GetString("Game_Title");
		percentage = PlayerPrefs.GetFloat("Percentage");
		score = PlayerPrefs.GetInt("Score");
		switch (title)
		{
		case "Functional Groups":
			highPercentage = PlayerPrefs.GetFloat("Level_1_High_Percentage");
                time = PlayerPrefs.GetFloat("Level_1_Time");
                break;
		case "Intermolecular Forces":
			highPercentage = PlayerPrefs.GetFloat("Level_3_High_Percentage");
                time = PlayerPrefs.GetFloat("Level_3_Time");
                break;
		case "Chiral Carbons":
			highPercentage = PlayerPrefs.GetFloat ("Level_5_High_Percentage");
                time = PlayerPrefs.GetFloat("Level_5_Time");
                break;
		case "Isomers":
			highPercentage = PlayerPrefs.GetFloat("Level_4_Standard_High_Percentage");
                time = PlayerPrefs.GetFloat("Level_4_Time_Stdd");
                break;
		case "Isomers Extreme":
			highPercentage = PlayerPrefs.GetFloat("Level_4_Extreme_High_Percentage");
                time = PlayerPrefs.GetFloat("Level_4_Time_Ext");
                break;
		case "Structure Classification: Standard":
			highPercentage = PlayerPrefs.GetFloat("Level_2_Standard_High_Percentage");
                time = PlayerPrefs.GetFloat("Level_2_Time_Stdd");
                break;
		case "Structure Classification: Extreme":
			highPercentage = PlayerPrefs.GetFloat("Level_2_Extreme_High_Percentage");
                time = PlayerPrefs.GetFloat("Level_2_Time_Ext");
                break;
		case "Structure Classification: Time Trial":
			highPercentage = PlayerPrefs.GetFloat ("Level_2_Trial_High_Percentage");
                time = PlayerPrefs.GetFloat("Level_2_Time_Time_Trial");
                break;
		case "Naming Molecules":
			highPercentage = PlayerPrefs.GetFloat ("Level_6_High_Percentage");
                time = PlayerPrefs.GetFloat("Level_6_Time");
                Debug.Log("time " + time);
                break;
		default:
			highPercentage = 0f;		
			break;
		}
	}

	void displayRecord() {
		gameTitle.text = title;
		scoreLabel.text = (percentage * 100).ToString() + "%";

		highPercentageLabel.text = "Your previous best was " + (highPercentage * 100).ToString() + "%!";

		if(title =="Structure Classification: Standard" || title =="Structure Classification: Extreme" || title =="Structure Classification: Time Trial") {
			displayRecordForLevel2();
		}else if (title == "Isomers Extreme")
		{
			Debug.Log("Displaying extreme score");
			highPercentageLabel.text = "Your previous best was " + ((int)highPercentage).ToString() + " seconds!";			
		}
	}

	void displayRecordForLevel2() {
		scoreLabel.text = "";
		highPercentageLabel.text = "";

	}

	void displayMedalAndComment() {
		if(title =="Structure Classification: Standard" || title =="Structure Classification: Extreme" || title =="Structure Classification: Time Trial") {
			displayMedalAndCommentForLevel2();
			return;
		}

		if (title == "Isomers Extreme")
		{
			displayMedalAndCommentForLevel4ExtremeMode();
			return;
		}

		if(percentage < 0.5f) {
			instantiateMedal(0);
			goodEffortLabel.text = "Good Effort!";
			percentageLabel.text = "But your chemistry is a little rusty. You got " + (percentage * 100).ToString() + "%.";
		}else if(percentage >= 0.5f && percentage <= 0.69f) {
			instantiateMedal(1);
			goodEffortLabel.text = "Nice try!";
			percentageLabel.text = "You joined tin and copper with " + (percentage * 100).ToString() + "%.";
		}else if(percentage >= 0.7f && percentage <= 0.89f) {
			instantiateMedal(2);
			goodEffortLabel.text = "Great work!";
			percentageLabel.text = "With that sterling effort you got " + (percentage * 100).ToString() + "%.";
		}else if(percentage >= 0.9f && percentage <= 0.99f) {
			instantiateMedal(3);
			goodEffortLabel.text = "Well done!";
			percentageLabel.text = "Your chemistry prowess is gold standard. You got " + (percentage * 100).ToString() + "%.";
		}else {
			instantiateMedal(4);
			goodEffortLabel.text = "Congratulations!";
			percentageLabel.text = "A perfect score! Your knowledge is obviously crystal clear.";
		}
	}


	void displayMedalAndCommentForLevel4ExtremeMode() //level 4 extreme
	{
		scoreLabel.text = "" + score;
		string comment = "";
		comment = "You ended with " + score + " seconds.";
		scoreLabel.text = score + " sec";

		percentageLabel.text = comment;
		//highPercentageLabel.text = "";

		if (score >= 40)
		{
			instantiateMedal(4);
			goodEffortLabel.text = "Congratulations!";
		}
		else if (score >= 30)
		{
			instantiateMedal(3);
			goodEffortLabel.text = "Well done!";
		}
		else if (score >= 20)
		{
			instantiateMedal(2);
			goodEffortLabel.text = "Great work!";
		}
		else if (score >= 10)
		{
			instantiateMedal(1);
			goodEffortLabel.text = "Nice try!";
		}
		else
		{
			instantiateMedal(0);
			goodEffortLabel.text = "Good Effort!";
		}
	}

	void displayMedalAndCommentForLevel2() {
		if (title == "Structure Classification: Standard") {
			displayMedalAndCommentForStandardMode ();
		} else if (title == "Structure Classification: Extreme") {
			displayMedalAndCommentForLevel2ExtremeMode ();
		} else if (title == "Structure Classification: Time Trial") {
			displayMedalAndCommentForTrialMode ();
		}
	}

	void displayMedalAndCommentForStandardMode() { // level 2
		scoreLabel.text = ""+ score;
		int min = score/60;
		string sec = (score % 60).ToString ("00");

		string comment = "";
		if (min == 0) {
			comment = "You took " + sec + " seconds.";
			scoreLabel.text = score + " sec";
		}
		else if (min == 1) {
			comment = "You took " + min + " minute "+ sec +" seconds.";
			scoreLabel.text = min + ":" + sec;
		}
		else if (min > 1) {
			comment = "You took " + min +" minutes " +sec +  " seconds.";
			scoreLabel.text = min + ":" + sec;

		}

		percentageLabel.text = comment;

		if (highPercentage > 0) {

			string bestScoreLabel = "";

			int highMin = (int)highPercentage / 60;
			string highSec = (highPercentage % 60).ToString ("00");


			if (highMin == 0) {
				bestScoreLabel = "Your previous best was " + highSec + " seconds.";
			}
			else if (highMin == 1) {
				bestScoreLabel = "Your previous best was " + highMin + " minute "+ highSec +" seconds.";
			}
			else if (highMin > 1) {
				bestScoreLabel = "Your previous best was " + highMin +" minutes " +highSec +  " seconds.";
			}

			highPercentageLabel.text = bestScoreLabel;

		}


		if (score <= 80) {
			instantiateMedal(4);
			medalString = "Diamond Medal";
			goodEffortLabel.text = "Congratulations!";
		}
		else if (score <= 120) {
			instantiateMedal(3);
			medalString = "Gold Medal";
			goodEffortLabel.text = "Well done!";
		}
		else if (score <= 180) {
			instantiateMedal(2);
			medalString = "Silver Medal";
			goodEffortLabel.text = "Great work!";
		}
		else if (score <= 240) {
			instantiateMedal(1);
			medalString = "Bronze Medal";
			goodEffortLabel.text = "Nice try!";
		}
		else if (score >240) {
			instantiateMedal(0);
			medalString = "Rusty Medal";
			goodEffortLabel.text = "Good Effort!";
		}
	}

	void displayMedalAndCommentForLevel2ExtremeMode() {
		scoreLabel.text = ""+ score;
		int min = score/60;
		string sec = (score % 60).ToString ("00");
		string comment = "";
		if (min == 0) {
			comment = "You beat the clock with " + sec + " seconds remaining.";
			scoreLabel.text = score + " sec";
		}
		else if (min == 1) {
			comment = "You beat the clock with " + min + " minute and "+ sec +" seconds remaining.";
			scoreLabel.text = min + ":" + sec;
		}
		else if (min > 1) {
			comment = "You beat the clock with  " + min +" minutes and " +sec +  " seconds remaining.";
			scoreLabel.text = min + ":" + sec;

		}

		percentageLabel.text = comment;


		if (highPercentage > 0) {

			string bestScoreLabel = "";

			int highMin = (int)highPercentage / 60;
			string highSec = (highPercentage % 60).ToString ("00");

			if (highMin == 0) {
				bestScoreLabel = "Your previous best was " + highSec + " seconds.";
			}
			else if (highMin == 1) {
				bestScoreLabel = "Your previous best was " + highMin + " minute "+ highSec +" seconds.";
			}
			else if (highMin > 1) {
				bestScoreLabel = "Your previous best was " + highMin +" minutes " + highSec +  " seconds.";
			}
			highPercentageLabel.text = bestScoreLabel;
		}


		if (score <= 70) {
			instantiateMedal(0);
			goodEffortLabel.text = "Good Effort!";
			medalString = "Rusty Medal";
		}
		else if (score <= 120) {
			instantiateMedal(1);
			goodEffortLabel.text = "Nice try!";
			medalString = "Bronze Medal";
		}
		else if (score <= 180) {
			instantiateMedal(2);
			goodEffortLabel.text = "Great work!";
			medalString = "Silver Medal";
		}
		else if (score < 240) {
			instantiateMedal(3);
			goodEffortLabel.text = "Well done!";
			medalString = "Gold Medal";
		}
		else if (score >= 240) {
			instantiateMedal(4);
			goodEffortLabel.text = "Congratulations!";
			medalString = "Diamond Medal";
		}
	}

	void displayMedalAndCommentForTrialMode() { //level 2
		scoreLabel.text = ""+ score;
		int min = score/60;
		string sec = (score % 60).ToString ("00");
		string comment = "";
		if (min == 0) {
			comment = "You beat the clock with " + sec + " seconds remaining.";
			scoreLabel.text = score + " sec";
		}
		else if (min == 1) {
			comment = "You beat the clock with " + min + " minute and "+ sec +" seconds remaining.";
			scoreLabel.text = min + ":" + sec;
		}
		else if (min > 1) {
			comment = "You beat the clock with  " + min +" minutes and " +sec +  " seconds remaining.";
			scoreLabel.text = min + ":" + sec;

		}
		percentageLabel.text = comment;

		if (highPercentage > 0) {

			string bestScoreLabel = "";

			int highMin = (int)highPercentage / 60;
			string highSec = (highPercentage % 60).ToString ("00");

			if (highMin == 0) {
				bestScoreLabel = "Your best was " + highSec + " seconds.";
			}
			else if (highMin == 1) {
				bestScoreLabel = "Your best was " + highMin + " minute "+ highSec +" seconds.";
			}
			else if (highMin > 1) {
				bestScoreLabel = "Your best was " + highMin +" minutes " + highSec +  " seconds.";
			}

			highPercentageLabel.text = bestScoreLabel;

		}


		if (score <= 20) {
			instantiateMedal(0);
			goodEffortLabel.text = "Good Effort!";
			medalString = "Rusty Medal";
		}
		else if (score <= 35) {
			instantiateMedal(1);
			goodEffortLabel.text = "Nice try!";
			medalString = "Bronze Medal";
		}
		else if (score <= 50) {
			instantiateMedal(2);
			goodEffortLabel.text = "Great work!";
			medalString = "Silver Medal";
		}
		else if (score < 65) {
			instantiateMedal(3);
			goodEffortLabel.text = "Well done!";
			medalString = "Gold Medal";
		}
		else if (score >= 65) {
			instantiateMedal(4);
			goodEffortLabel.text = "Congratulations!";
			medalString = "Diamond Medal";
		}
	}



	void updateHighScore() {

		if (title == "Structure Classification: Standard") { 

			if (percentage < highPercentage || highPercentage == 0) {
				newRecord.gameObject.SetActive (true);
				PlayerPrefs.SetFloat ("Level_2_Standard_High_Percentage", percentage);
			}

		}else {

			if(percentage > highPercentage) {
				newRecord.gameObject.SetActive(true);

				switch (title)
				{
				case "Functional Groups":
					PlayerPrefs.SetFloat("Level_1_High_Percentage",percentage);
					break;
				case "Intermolecular Forces":
					PlayerPrefs.SetFloat("Level_3_High_Percentage",percentage);
					break;
				case "Isomers":
					PlayerPrefs.SetFloat("Level_4_Standard_High_Percentage", percentage);
					break;
				case "Isomers Extreme":
					PlayerPrefs.SetFloat("Level_4_Extreme_High_Percentage", percentage);
					break;
				case "Structure Classification: Extreme":
					PlayerPrefs.SetFloat("Level_2_Extreme_High_Percentage",percentage);
					break;
				case "Structure Classification: Time Trial":
					PlayerPrefs.SetFloat("Level_2_Trial_High_Percentage",percentage);
					break;
				case "Chiral Carbons":
					PlayerPrefs.SetFloat("Level_5_High_Percentage", percentage);
					break;
				case "Naming Molecules":
					PlayerPrefs.SetFloat("Level_6_High_Percentage", percentage);
					break;
				default:
					break;
				}

			}
		}

	}

    void updateTime()
    {
        timeP = PlayerPrefs.GetFloat("TimeP");
        Debug.Log("timeeeP " + PlayerPrefs.GetFloat("TimeP"));

        if (title == "Functional Groups" || title == "Structure Classification: Standard" || title == "Intermolecular Forces" || title == "Isomers" || title == "Chiral Carbons" || title == "Naming Molecules")
        {
            if (timeP < time)

            {
                switch (title)
                {
                    case "Functional Groups":
                        PlayerPrefs.SetFloat("Level_1_Time", timeP);
                        break;


                    case "Structure Classification: Standard":
                        PlayerPrefs.SetFloat("Level_2_Time_Stdd", timeP);
                        break;
                    case "Intermolecular Forces":
                        PlayerPrefs.SetFloat("Level_3_Time", timeP);
                        break;
                    case "Isomers":
                        PlayerPrefs.SetFloat("Level_4_Time_Stdd", timeP);
                        break;

                    case "Chiral Carbons":
                        PlayerPrefs.SetFloat("Level_5_Time", timeP);
                        break;
                    case "Naming Molecules":
                        PlayerPrefs.SetFloat("Level_6_Time", timeP);
                        break;

                    default:
                        break;
                }
            }
        }
        else
        {
            if (timeP > time)
            {
                switch (title)
                {

                    case "Structure Classification: Extreme":
                        PlayerPrefs.SetFloat("Level_2_Time_Ext", timeP);
                        break;

                    case "Structure Classification: Time Trial":
                        PlayerPrefs.SetFloat("Level_2_Time_Time_Trial", timeP);
                        break;

                    case "Isomers Extreme":
                        PlayerPrefs.SetFloat("Level_4_Time_Ext", timeP);
                        break;

                    default:
                        break;

                }

            }
        }
    }

    public void toggleInfoPanel() {
		if (title == "Structure Classification: Standard") {
			infoPanelStandard.SetActive (!infoPanelStandard.activeInHierarchy);
		} else if (title == "Structure Classification: Extreme") {
			infoPanelExtreme.SetActive (!infoPanelExtreme.activeInHierarchy);
		} else if (title == "Structure Classification: Time Trial") {
			infoPanelTrial.SetActive (!infoPanelTrial.activeInHierarchy);
        }
        else if (title == "Isomers Extreme") {
            infoPanelLvl4Extreme.SetActive(!infoPanelLvl4Extreme.activeInHierarchy);
        }
		else {
			infoPanel.SetActive(!infoPanel.activeInHierarchy);
		}
	}

	void instantiateMedal(int i) {
		Instantiate(medals[i],canvas.transform,false);
	}

	public void goToMenuScene() {
		SceneManager.LoadScene("MainScene");
	}

	public void replay() {
		switch (title)
		{
		case "Functional Groups":
			SceneManager.LoadScene("Level_One_Scene");					
			break;
		case "Intermolecular Forces":
			SceneManager.LoadScene("Level_Three_Scene");	
			break;
		case "Chiral Carbons":
			SceneManager.LoadScene ("Level_Five_Scene");	
			break;
		case "Isomers":
			SceneManager.LoadScene("Level_Four_Scene");
			break;
		case "Isomers Extreme":
			SceneManager.LoadScene("Level_Four_Scene_Extreme");
			break;
		case "Structure Classification: Standard":
			SceneManager.LoadScene("Level_Two_Scene");	
			break;
		case "Structure Classification: Extreme":
			SceneManager.LoadScene("Level_Two_Extreme_Scene");	
			break;
		case "Structure Classification: Time Trial":
			SceneManager.LoadScene("Level_Two_TimeTrial_Scene");	
			break;
		case "Naming Molecules" :
			SceneManager.LoadScene("Level_Six_Scene");	
			break;
		default:
			break;
		}
	}


	public void fbShare() {
		string description;

		if(title =="Structure Classification: Standard" || title =="Structure Classification: Extreme" || title =="Structure Classification: Time Trial") {
			description = "Hey, I got the " + medalString + " in Chirality 2: " + title + ". Come and check it out!";
		}else if(title == "Isomers Extreme"){
			description = "Hey, I got a score of " + (int)percentage + " seconds in Chirality 2: " + title + ". Come and check it out!";
		}else {
			description = "Hey, I got " + (percentage * 100).ToString() + "%" + " in Chirality 2: " + title + ". Come and check it out!";
		}

		FB.ShareLink(contentTitle:"Chirality 2",
			contentURL:new System.Uri("https://spencerandjones2016.blogspot.com.au/"),
			contentDescription: description,
			photoURL: new System.Uri("https://4.bp.blogspot.com/-fm11xRuOIXU/WUeEgPNx4XI/AAAAAAAAAG4/OJphWPx_FLUHE4yXPa1RKucO2NQCEzUNgCLcBGAs/s200/Chirality2-Icon-midblue.png"),
			callback: fbCallBack);
	}

	public void twitterShare() 
	{
		string address = "https://twitter.com/intent/tweet";
		string name = "Chirality 2";
		string description;

        if (title == "Structure Classification: Standard" || title == "Structure Classification: Extreme" || title == "Structure Classification: Time Trial" || title == "Isomers Extreme")
        {
			description = "I got the " + medalString + " in Chirality 2: " + title + ". Check it out on App Store or Google Play!";
		}else {
			description = "I got " + (percentage * 100).ToString() + "%" + " in Chirality 2: " + title + ". Check it out on App Store or Google Play!";
		}
		Application.OpenURL(address + "?text=" + WWW.EscapeURL(name + "\n" + description));
	}

	private void fbCallBack(IShareResult result) {
		// AudioListener.pause = false;	
		if(result.Cancelled || !string.IsNullOrEmpty(result.Error)) {
			Debug.Log(result.Error);
		}else if(!string.IsNullOrEmpty(result.PostId)) {
			Debug.Log(result.PostId);
		}else {
			Debug.Log("Share succeed");
		}
	}


	private void FBInitCompletion() {		
		FB.Mobile.ShareDialogMode = ShareDialogMode.NATIVE;
	}
}
