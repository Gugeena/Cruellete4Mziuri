using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VideoInstintatorScript : MonoBehaviour
{
    public GameObject VideoPlayer;
    public GameObject VideoPlayer1;
    public GameObject Square;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(VideoInstantiator());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private IEnumerator VideoInstantiator()
    {
        Square.SetActive(false);
        yield return new WaitForSeconds(0.1f);
        VideoPlayer.SetActive(true);
        VideoPlayer1.SetActive(true);
        yield return null;
    }    
}
