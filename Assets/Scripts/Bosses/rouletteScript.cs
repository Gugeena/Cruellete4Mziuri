using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class rouletteScript : MonoBehaviour
{
    public GameObject spin, roulette, rouletteTop, toast;
    Animator spinAnimator, rouletteTopAnim, toastAnim;
    public bool green;
    Image toastImage;
    public SlotBossAI slotAI;
    public dBossScript dBoss;
    public PlinkoBossScript plinkoBoss;

    public int spinCount;

    public AudioManager am;

    public AudioSource spinSound;

    void Start()
    {
        spinAnimator = spin.GetComponent<Animator>();
        rouletteTopAnim = rouletteTop.GetComponent<Animator>();
        toastImage = toast.GetComponent<Image>();
        toastAnim = toast.GetComponent<Animator>();
    }

    public IEnumerator spinCRT(Sprite[] toastSprites, int buffAmount)
    {
        if (am != null) am.playAudio(spinSound);
        spinCount++;
        int chosenBuff = Random.Range(1, buffAmount);
        roulette.transform.eulerAngles = new Vector3(0, 0, Random.Range(0f, 360f));
        roulette.transform.parent = spin.transform;
        rouletteTopAnim.Play("rouletteSlideOut");
        yield return new WaitForSeconds(0.2f);
        spinAnimator.Play("spin");
        yield return new WaitForSeconds(2f);
        roulette.transform.parent = rouletteTop.transform;
        yield return new WaitForSeconds(1f);
        if (green) chosenBuff = 0;
        toastImage.sprite = toastSprites[chosenBuff];

        if (slotAI != null)slotAI.currBuff = chosenBuff;
        if (dBoss != null) dBoss.currBuff = chosenBuff;
        if (plinkoBoss != null) plinkoBoss.currBuff = chosenBuff;

        toastAnim.Play("toastSlide");
        rouletteTopAnim.Play("rouletteSlideBack");
        yield return new WaitForSeconds(1.2f);
        if (slotAI != null){ 
            slotAI.isInvulnerable = false;
            slotAI.handleBuffs();
        }
        else if (dBoss != null) {
            dBoss.isInvulnerable = false;
            dBoss.handleBuffs();
        }
        else if (plinkoBoss != null)
        {
            plinkoBoss.handleBuffs();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("greenRoulette"))
        {
            green = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("greenRoulette"))
        {
            green = false;
        }
    }
}
