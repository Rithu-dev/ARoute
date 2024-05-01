using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ExitManager : MonoBehaviour
{
    [SerializeField] GameObject exitPanel;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if(SceneManager.GetActiveScene().buildIndex != 0)
            {
                SceneManager.LoadScene(0);
            }
            else
            {
                if (exitPanel)
                {
                    exitPanel.SetActive(true);
                }
            }
        }
    }

    public void onUserClickYesNo(int choice) //choice = 0 ==> NO , 1 ==> YES
    {
        if(choice == 1)
        {
            Application.Quit();
        }
        exitPanel.SetActive(false);
    }

    public void onUserClickStart()
    {
        SceneManager.LoadScene(1);//load mainscene
    }
}
