using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class camShakerScript : MonoBehaviour
{
    [SerializeField] private GameObject shakeCam;


    public float amplitude, frequency, time;

    public bool onCollide;

    private void Awake()
    {
        if (shakeCam == null) shakeCam = GameObject.Find("ShakeCam");
    }
    public IEnumerator shake()
    {
        CinemachineVirtualCamera cam = shakeCam.GetComponent<CinemachineVirtualCamera>();
        CinemachineBasicMultiChannelPerlin cbmcp = cam.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
        float oldAmp = cbmcp.m_AmplitudeGain;
        float oldFreq = cbmcp.m_FrequencyGain;
        cbmcp.m_AmplitudeGain = amplitude;
        cbmcp.m_FrequencyGain = frequency;

        print("Shaking!");
        cam.enabled = true; 
        yield return new WaitForSeconds(time);
        cam.enabled = false;
        cbmcp.m_AmplitudeGain = oldAmp;
        cbmcp.m_FrequencyGain = oldFreq;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (onCollide && collision.gameObject.layer != 9) {
            StartCoroutine(shake());
        }
    }

}
