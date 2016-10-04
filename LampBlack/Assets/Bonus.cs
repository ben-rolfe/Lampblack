using UnityEngine;
using System.Collections;
using Locogame.Propagate;

public class Bonus : MonoBehaviour {
    [SerializeField]
    int checkpoint;
    [SerializeField]
    AudioClip successClip;
    AudioSource[] sources;

    void Awake () {
        sources = GetComponentsInChildren<AudioSource>();
	}

    void OnTriggerEnter(Collider col)
    {
        if (col.tag == "Player")
        {
            col.GetComponent<PlayerSound>().FadeClip(checkpoint, true);
            Destroy(GetComponent<SphereCollider>());
            sources[0].loop = false;
            sources[1].loop = false;
            sources[0].clip = successClip;
            sources[1].clip = null;
            sources[0].Play();
        }
    }
}
