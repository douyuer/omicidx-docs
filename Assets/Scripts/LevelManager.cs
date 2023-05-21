using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    public string[] levels;
    private int l = 0;
    // Start is called before the first frame update
    void Start()
    {
        
        DontDestroyOnLoad(this.gameObject);
    }

    public void NextLevel()
    {
        l = l + 1;

        Debug.Log(this.levels[l]);
    }
}
