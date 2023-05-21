
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EndFase : MonoBehaviour
{
    public string SceneName;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        SceneManager.LoadScene(SceneName);


    }
}