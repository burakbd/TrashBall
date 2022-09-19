using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerYer : MonoBehaviour
{
    public GameObject Bir;
    public GameObject Iki;
    public GameObject Uc;
    public GameObject Dort;
    public GameObject Bes;
    public GameObject Alti;
    public GameObject Yedi;
    public GameObject Ouch;
    AudioSource hata;

    private void Start()
    {
        hata = GameObject.FindGameObjectWithTag("Hata").GetComponent<AudioSource>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            StartCoroutine(CopBirikti());
            Ouch.SetActive(true);
            hata.enabled = true;
        }

    }

    IEnumerator CopBirikti()
    {
        for (int i = 0; i < 7; i++)
        {
            yield return new WaitForSeconds(0f);
            Bir.SetActive(true);
            yield return new WaitForSeconds(14f);
            Iki.SetActive(true);
            yield return new WaitForSeconds(14f);
            Uc.SetActive(true);
            yield return new WaitForSeconds(16f);
            Dort.SetActive(true);
            yield return new WaitForSeconds(16f);
            Bes.SetActive(true);
            yield return new WaitForSeconds(14f);
            Alti.SetActive(true);
            yield return new WaitForSeconds(16f);
            Yedi.SetActive(true);
        }
        
    }

}
