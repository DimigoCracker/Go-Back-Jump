using System.Threading;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    private string[] gbj = { "go", "back", "jump" };
    public GameObject[] say;

    public AudioSource start;
    public AudioSource[] go;
    public AudioSource[] back;
    public AudioSource[] jump;

    public int turn;
    public int direction;
    public int player;
    public int numberOfPlayers;

    public float wrongChance;

    public Canvas canvas;

    public Text[] uiTexts;

    private float timer;
    public float maxTime = 5;
    public Text timerText;

    private float answertime;
    public float answerRootMax = 2;
    public float late;

    public InputField textInput;

    private char OnValidate(string text, int charIndex, char addedChar)
    {
        if (addedChar == '\n')
        {
            if (CheckText(text))
            {
                turn++;
                timer = maxTime;
            }
            else GameOver(false);
            textInput.text = "";
            return (char)0;
        }
        return addedChar;
    }

    void Start()
    {
        Time.timeScale = 0;
        timer = maxTime;
        PopupText(0);

        textInput.lineType = InputField.LineType.MultiLineNewline;
        textInput.onValidateInput = OnValidate;
    } 

    void Update()
    {
        ShowTimer();

        if (Input.GetKeyDown(KeyCode.Space))
        {
            Restart();
        }

        if (player < 0) player += numberOfPlayers;
        if (player >= numberOfPlayers) player -= numberOfPlayers;

        if (timer <= 0)
        {
            timer = 0;
            if (player == 0)
                GameOver(false);
            else
                GameOver(true);
        }
        if (timer < answertime)
        {
            if (Random.Range(0, 100.0f) < wrongChance)
                SayWrong();
            else
                SayCorrect(player);
        }
    }

    void Restart()
    {
        textInput.text = "";

        PopupText(1);

        turn = 1;
        direction = 1;
        player = 1;

        timer = maxTime;

        SetAnswertime();

        Time.timeScale = 1;

        start.Play();
    }

    void PopupText(int n)
    {
        for (int i = 0; i < 4; i++)
            uiTexts[i].enabled = false;
        uiTexts[n].enabled = true;
    }
        
    bool CheckText(string text) {
        if (player == 0) {
            if (turn % 3 == 0) {
                if (text.Equals("go"))
                {
                    player += direction;
                    return true;
                } else if (textInput.text == "back")
                {
                    direction *= -1;
                    player += direction;
                    return true;
                } else if (textInput.text == "jump")
                {
                    player += direction * 2;
                    return true;
                } else {
                    return false;
                }
            } else {
                if (textInput.text == turn.ToString())
                {
                    player += direction;
                    return true;
                } else {
                    return false;
                }
            }
        } else {
            return false;
        }
    }

    void SaySomething(string what, int hwo)
    {
        GameObject bubble = Instantiate(say[hwo - 1], new Vector3(0, 0, 0), Quaternion.identity, canvas.transform);
        bubble.transform.localPosition = new Vector3(0, 0, 0);
        bubble.GetComponentInChildren<Text>().text = what;
        switch (what)
        {
            case "go":
                go[hwo - 1].Play();
                break;
            case "back":
                back[hwo - 1].Play();
                break;
            case "jump":
                jump[hwo - 1].Play();
                break;
        }
        timer = maxTime;
        
        Destroy(bubble, 1);
    }
    
    void SayWrong()
    {
        int hwo = Random.Range(1, 5);

        string what;
        if (player == hwo)
        {
            if (turn % 3 == 0)
            {
                what = turn.ToString();
            }
            else
            {
                what = gbj[Random.Range(0, 3)];
            }
        }
        else
        {
            if (turn % 3 == 0)
            {
                what = gbj[Random.Range(0, 3)];
            }
            else
            {
                what = turn.ToString();
            }
        }

        SaySomething(what , hwo);
        GameOver(true);
    }

    void SayCorrect(int hwo)
    {
        if (hwo == 0)
            return;
        string what;
        if (turn % 3 == 0)
        {
            what = gbj[Random.Range(0, 3)];
            switch (what)
            {
                case "go":
                    player += direction;
                    break;
                case "back":
                    direction *= -1;
                    player += direction;
                    break;
                case "jump":
                    player += direction * 2;
                    break;
            }
        }
        else
        {
            what = turn.ToString();
            player += direction;
        }

        SaySomething(what, hwo);

        turn++;
        timer = maxTime;

        SetAnswertime();
    }

    void ShowTimer()
    {
        timer -= Time.deltaTime;
        timerText.text = timer.ToString();
    }

    void GameOver(bool win)
    {
        Time.timeScale = 0;
        timer = maxTime;

        if (win)
        {
            PopupText(2);
        }
        else
        {
            PopupText(3);
        }
    }

    void SetAnswertime()
    {
        answertime = Random.Range(-late, answerRootMax);
        answertime *= answertime;
    }
}