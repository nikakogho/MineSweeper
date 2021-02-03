using UnityEngine;

public class PauseGame : MonoBehaviour {
    public GameObject tiles;
    public GameObject pauseUI;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            bool active = !pauseUI.activeSelf;
            pauseUI.SetActive(active);
            
            if (active)
            {
                Pause();
            }
            else
            {
                Proceed();
            }
        }
    }

    void Pause()
    {
        Time.timeScale = 0;
        tiles.SetActive(false);
    }

    public void Proceed()
    {
        Time.timeScale = 1;
        tiles.SetActive(true);
    }
}
