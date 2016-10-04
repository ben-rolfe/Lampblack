using UnityEngine;
using System.Collections;
using Locogame.Propagate;

public class Checkpoint : MonoBehaviour {
    public int checkpoint;
    [SerializeField]
    AudioClip successClip;
    public AudioSource source;

	void Awake () {
        source = GetComponentInChildren<AudioSource>();
    }

    void OnTriggerEnter(Collider col)
    {
        if (col.tag == "Player" && col.GetComponent<PlayerSound>().checkpoint == checkpoint - 1)
        {
			col.GetComponent<PlayerSound>().checkpoint = checkpoint;
			Destroy(GetComponent<SphereCollider>());
            source.loop = false;
            source.clip = successClip;
            source.Play();
//            sound.masterVolume = 0;
        }
    }
}
