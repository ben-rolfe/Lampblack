using UnityEngine;
using System.Collections;

public class Music : MonoBehaviour {
    [SerializeField]
    AudioClip[] clips;
    [SerializeField]
    float duration;
    public int clip = 0;
    AudioSource source;

	void Awake () {
        source = GetComponent<AudioSource>();
        InvokeRepeating("PlayClip", 0f, duration);
	}
	
    public void PlayClip()
    {
        if (clip > -1 && clip < clips.Length)
        {
            source.PlayOneShot(clips[clip]);
        }
    }

}
