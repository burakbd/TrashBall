using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ScoreColl : MonoBehaviour
{
    private int currentScore;
    public Text scoreText;
    public GameObject yes;
    AudioSource basket;
    AudioSource yesSesi;
    void Start()
    {
        currentScore = 0;
        basket = GetComponent<AudioSource>();
        yesSesi = GameObject.FindGameObjectWithTag("YesSesi").GetComponent<AudioSource>();
    }

    private void HandleScore()
    {
        scoreText.text = "Score: " + currentScore + "/3";

        if(currentScore == 3)
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        }

    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            yes.SetActive(true);
            yesSesi.enabled = true;
            basket.enabled = true;
            currentScore++;
            HandleScore();
        }

    }
    
}
