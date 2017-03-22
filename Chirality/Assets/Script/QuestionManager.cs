﻿using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using System.IO;
using LitJson;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

// to keep track of the game status
public enum gameStatus
{ InGame, InHelp, InCheck, InFunFact }

public class QuestionManager : MonoBehaviour {

	public static QuestionManager Instance = null;

	private List<Question> questions = new List<Question>();
	private JsonData questionData;
	[SerializeField] GameObject[] questionObjects;
	[SerializeField] GameObject[] questionAnswerObjects;
	[SerializeField] Canvas canvas;
	[SerializeField] Text questionName;
	[SerializeField] Text scoreNumberLabel;
	[SerializeField] Button displayAnswerButton;
	[SerializeField] GameObject helpPanel;
	[SerializeField] GameObject funFactPanel;
	[SerializeField] Text funFactPanelText;

	private int score = 0;
	private GameObject currentQuestion;
	private GameObject currentQuestionAnswer;
	private Question currentQuestionObject;
	private gameStatus currentStatus = gameStatus.InGame;
	public gameStatus CurrentStatus {
		get {
			return currentStatus;
		}
	}
	
	private int Score {
		get{
			return score;
		}
		set{
			score = value;
			scoreNumberLabel.text = score.ToString();
		}
	}

	void Awake() {
		// singleton
		if (Instance == null) {
			Instance = this;
		}else if (Instance != this) {
			Destroy(gameObject);
		}
	}


	void Start () {
		helpPanel.SetActive(false);
		funFactPanel.SetActive(false);
		displayAnswerButton.gameObject.SetActive(false);
		
		string path = "";
		if(Application.platform == RuntimePlatform.Android) {
			string oriPath = System.IO.Path.Combine(Application.streamingAssetsPath, "Questions.json");
			WWW reader = new WWW(oriPath);
			while(!reader.isDone) {}

			string realPath = Application.persistentDataPath + "/Questions";
  			System.IO.File.WriteAllBytes(realPath, reader.bytes);
			path = realPath;
		}else {
			path = System.IO.Path.Combine(Application.streamingAssetsPath, "Questions.json");
		}
		
		questionData = JsonMapper.ToObject(File.ReadAllText(path));
		loadQuestions();
		instantiateRandomQuestionToDisplay();		

	}

	// create the Question objects from the Questions.json and append them to the List<Question>
	void loadQuestions() {
		for (int i = 0; i < questionData.Count; i++){
			questions.Add(new Question((int)questionData[i]["id"],(int)questionData[i]["level"],(string)questionData[i]["code"],(string)questionData[i]["name"],(int)questionData[i]["numberOfCells"],(string)questionData[i]["facts"][Random.Range(0,10)],questionObjects[i],questionAnswerObjects[i],convertArray(questionData,i)));
		}
	}

	// pick a random question from the List<Question> and display it
	void instantiateRandomQuestionToDisplay() {
		int randomNum = Random.Range(0,questions.Count);
		currentQuestionObject = questions[randomNum]; 
		currentQuestion = Instantiate(currentQuestionObject.gameObject,canvas.transform,false);
		currentQuestionAnswer = Instantiate(currentQuestionObject.answerObject,canvas.transform,false);
		questionName.text = currentQuestionObject.name;
		currentQuestionAnswer.SetActive(false);
		
		// change the game status and deactivate the answer button
		currentStatus = gameStatus.InGame;
		displayAnswerButton.gameObject.SetActive(false);
	}

	// configure the functions attached to this button based on different game status
	public void nextButtonPressed() {
		if(currentStatus == gameStatus.InGame) {
			checkAnswer();
		}else if(currentStatus == gameStatus.InCheck) {
			displayFunFact();
		}else if (currentStatus == gameStatus.InFunFact) {

		}
	}




	void checkAnswer() {
		// check for empty slots, return if there is empty one
		for(int i = 0; i < currentQuestion.transform.childCount; i++) {
			if(currentQuestion.transform.GetChild(i).childCount == 0) {
				return;
			}
		}

		// change the game status
		currentStatus = gameStatus.InCheck;
		displayAnswerButton.gameObject.SetActive(true);

		// loop through the slots and check answer
		for(int i = 0; i < currentQuestion.transform.childCount; i++) {
			GameObject elementInCell = currentQuestion.transform.GetChild(i).GetChild(0).gameObject;		
			if(elementInCell.tag == currentQuestionObject.answer[i]) {
				plusScore();
				elementInCell.transform.GetComponent<Image>().color = Color.green;
			}else {
				elementInCell.transform.GetComponent<Image>().color = Color.red;
			}		
		}
	}

	void plusScore() {
		Score++;
	}

	// helper methos to convert json array to normal List
	List<string> convertArray(JsonData ary, int index) {
		List<string> temp = new List<string>();
		for(int i = 0; i < (int)ary[index]["numberOfCells"]; i++) {
			temp.Add((string)ary[index]["answer"][i]);
		}
		return temp;
	}

	// switch to the main scene
	public void homeButtonPressed() {
		SceneManager.LoadScene("MainScene");
	}

	public void toggleHelpPanel() {
		helpPanel.SetActive(!helpPanel.activeInHierarchy);
	}

	public void toggleAnswer() {
		currentQuestion.SetActive(!currentQuestion.activeInHierarchy);
		currentQuestionAnswer.SetActive(!currentQuestionAnswer.activeInHierarchy);		
	}

	public void funFactPanelTouched() {
		if(questions.Count > 1) {
			Destroy(currentQuestion);
			Destroy(currentQuestionAnswer);
			questions.Remove(currentQuestionObject);
			funFactPanel.SetActive(false);
			instantiateRandomQuestionToDisplay();
		}else {
			// go to game over scene 
			Debug.Log("No more questions");
		}
	}

	void displayFunFact() {
		funFactPanelText.text = currentQuestionObject.funFact;
		funFactPanel.SetActive(true);
		currentStatus = gameStatus.InFunFact;
	}

	
	
}











