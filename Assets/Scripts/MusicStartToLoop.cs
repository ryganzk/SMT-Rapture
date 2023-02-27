using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class MusicStartToLoop : MonoBehaviour
{
    void OnEnable()
    {
        AudioSource[] sources = GetComponents<AudioSource>();
        AudioSource musicStart = sources[0];
        AudioSource musicLoop = sources[1];
        musicStart.Play();
        musicLoop.PlayDelayed(musicStart.clip.length);
    }

    // void OnEnable()
    // {
    //     GetComponent<AudioSource> ().loop = true;
    //     StartCoroutine(playBGM());
    // }
 
    // IEnumerator playBGM()
    // {

    //     GetComponent<AudioSource>().clip = startClip;
    //     GetComponent<AudioSource>().Play();
    //     yield return new WaitForSeconds(GetComponent<AudioSource>().clip.length);
    //     GetComponent<AudioSource>().clip = loopClip;
    //     GetComponent<AudioSource>().Play();
    // }
}