using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UI_MainMenu : MonoBehaviour
{
    public void ChooseSawyer() {
        SceneManager.LoadScene("Sawyer");
    }

    public void ChooseUR5() {
        SceneManager.LoadScene("UR5");
    }
}
