using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class splashScreenScript : MonoBehaviour
{
    public AudioSource music;
    public Animator fadeOut;
    public float musicfadeStrength;
    bool isRunning;
    // Update is called once per frame

    private void Start()
    {
        isRunning = false;
        unlockCursor();
    }
    void Update()
    {
        if (Input.anyKeyDown && !isRunning)
        {
            StartCoroutine(nextScene());
        }
    }

    private IEnumerator nextScene()
    {
        isRunning = true;
        yield return new WaitForSeconds(1);
        fadeOut.Play("FadeOut");
        while(music.volume > 0)
        {
            music.volume -= musicfadeStrength;
            yield return new WaitForSeconds(0.1f);
        }
        yield return new WaitForSeconds(0.65f);

        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public void unlockCursor()
    {
        Cursor.visible = true; // Restores the cursor when leaving the scene
        Cursor.lockState = CursorLockMode.None; // Freely movable again
    }

    public void lockCursor()
    {
        Cursor.visible = false; // Hides the cursor
        Cursor.lockState = CursorLockMode.Locked;
    }
}
