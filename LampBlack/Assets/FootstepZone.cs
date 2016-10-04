using UnityEngine;
using System.Collections;
using UnityStandardAssets.Characters.FirstPerson;

public class FootstepZone : MonoBehaviour {
	[SerializeField]
	AudioClip[] insideClips;
	[SerializeField]
	AudioClip[] forwardClips;
	[SerializeField]
	AudioClip[] backClips;

	[SerializeField]
	float insideVolume;
	[SerializeField]
	float forwardVolume;
	[SerializeField]
	float backVolume;


	FirstPersonController fpc;

	void Start()
	{
		fpc = FindObjectOfType<FirstPersonController>();
	}

	void OnTriggerEnter(Collider col)
	{
		if (col.tag == "Player")
		{
			fpc.m_FootstepSounds = insideClips;
			fpc.m_AudioSource.volume = insideVolume;
		}
	}
	void OnTriggerExit(Collider col)
	{
		if (col.tag == "Player")
		{
			if (col.transform.position.x > transform.position.x)
			{
				fpc.m_FootstepSounds = backClips;
				fpc.m_AudioSource.volume = backVolume;
			}
			else
			{
				fpc.m_FootstepSounds = forwardClips;
				fpc.m_AudioSource.volume = forwardVolume;
			}
		}
	}

}
