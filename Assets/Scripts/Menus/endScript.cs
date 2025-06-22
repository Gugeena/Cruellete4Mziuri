using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class endScript : MonoBehaviour
{
    public AudioSource intro, loop;
    public AudioManager am;

    private void Start()
    {
        double nextEventTime = AudioSettings.dspTime + intro.clip.length;
        am.playScheduled(loop, nextEventTime);
    }

}
