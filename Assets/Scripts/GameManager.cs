using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public GameObject gameOverPanel;
    public GameObject player;
    [SerializeField]
    private Image _timerImg;
    [SerializeField]
    private Text _timerText;
    private float _currentTime;
    [SerializeField]
    private float _duration;
    public GameObject Count;
    public GameObject mom;
    float bittiGecikmesi = 5f;
    AudioSource back;

    void Start()
    {
        _currentTime = _duration;
        _timerText.text = _currentTime.ToString();
        StartCoroutine(UpdateTime());
        back = GameObject.FindGameObjectWithTag("Canvas").GetComponent<AudioSource>();
        back.enabled = true;
    }

    private IEnumerator UpdateTime()
    {
        while(_currentTime >= 0)
        {
            _timerImg.fillAmount = Mathf.InverseLerp(0, _duration, _currentTime);
            _timerText.text = _currentTime.ToString();
            yield return new WaitForSeconds(1f);
            _currentTime--;        
        }
        yield return null;
        if (_currentTime <= 0)
        {
            Invoke("Bitti", bittiGecikmesi);
            Anne();
            back.enabled = false;
        }
    }


    void Bitti()
    {
        player.SetActive(false);
        gameOverPanel.SetActive(true);
        Count.SetActive(false);
        mom.SetActive(false);
    }
    void Anne()
    {
        mom.SetActive(true);
    }

    public void Restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
