using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SwitchScene : MonoBehaviour
{
    public void onClickSwitchScene(string nameScene)
    {
        SceneManager.LoadScene(nameScene);
    }
}
