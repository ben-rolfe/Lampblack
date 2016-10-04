using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using XInputDotNetPure;

public class PlayerSound : MonoBehaviour {
	[SerializeField]
	AudioClip[] musicClips;
	[SerializeField]
	AudioClip[] helpClips;

	CharacterController cc;
    int _checkpoint = 0;
	Checkpoint[] checkpoints = new Checkpoint[9];
	float checkpointTime = 0;
	int currentSource = 0;
	List<int> fadingIn = new List<int>();
	List<int> fadingOut = new List<int>();
	bool helpGiven = false;
	AudioSource helpSource;
	float maxVolume = 0.3f;
	AudioSource[] sources;
	float thunk;
	bool won = false;

	MeshRenderer splashRenderer;

    void Start () {
        cc = GetComponent<CharacterController>();
		splashRenderer = GetComponentInChildren<MeshRenderer>();

		sources = new AudioSource[musicClips.Length];
        for (int i = 0; i < musicClips.Length; i++)
        {
            sources[i] = gameObject.AddComponent<AudioSource>();
            sources[i].clip = musicClips[i];
			if (i == musicClips.Length - 1) // Don't queue finale sound
			{
				sources[i].volume = 1;
				sources[i].loop = false;
			}
			else
			{
				sources[i].volume = 0;
				sources[i].loop = true;
				sources[i].Play();
			}
		}
		FadeClip(_checkpoint, true);

		foreach (Checkpoint cp in FindObjectsOfType<Checkpoint>())
		{
			if (cp.checkpoint < 10) //Checkpoints>9 are the animals
			{
				checkpoints[cp.checkpoint - 1] = cp;
			}
		}

		helpSource = gameObject.AddComponent<AudioSource>();
		helpSource.PlayOneShot(helpClips[10]); //Play intro clip
	}

	void Update()
	{
		if (Input.GetButtonDown("Cancel"))
		{
			Quit();
		}
	}

	void FixedUpdate()
    {
        GamePad.SetVibration(0, thunk, thunk);
        thunk = 0f;
        float fadeAmount = Time.fixedDeltaTime;
        List<int> remove = new List<int>();
        for (int i = 0; i < fadingIn.Count; i++)
        {
            if (sources[fadingIn[i]].volume > maxVolume - fadeAmount)
            {
                sources[fadingIn[i]].volume = maxVolume;
                fadingIn.RemoveAt(i);
            }
            else
            {
                sources[fadingIn[i]].volume += fadeAmount;
            }

        }
        for (int i = 0; i < fadingOut.Count; i++)
        {
            if (sources[fadingOut[i]].volume < fadeAmount)
            {
                sources[fadingOut[i]].volume = 0;
				if (fadingOut[i] == 0)
				{
					Destroy(splashRenderer.gameObject);
				}
                fadingOut.RemoveAt(i);
            }
            else
            {
                sources[fadingOut[i]].volume -= fadeAmount;
				if (fadingOut[i] == 0)
				{
					splashRenderer.material.color = new Color(1f, 1f, 1f, sources[0].volume / maxVolume);
				}
			}

		}

		//if it's been over a certain amount of time since the last checkpoint was hit, and help hasn't been given, give it now.
		if (!helpGiven && Time.time - checkpointTime > 40f)
		{
			helpSource.volume = 1f + 0.1f * checkpoint;

			AudioClip helpClip = helpClips[checkpoint];
			//Exceptions to normal help clip:
			//If you've already gone through the waterfall, but are still on checkpoint 4, play the help for checkpoint 5, instead.
			if (checkpoint == 4 && transform.position.x < -99f)
			{
				helpClip = helpClips[5];
			}
			//If you're on checkpoint 7 or 8, and already in the throne room, play the throne room help, instead.
			else if (checkpoint > 6 && transform.position.x < -199f)
			{
				helpClip = helpClips[9];
			}

			helpSource.PlayOneShot(helpClip);
			helpGiven = true;
		}
    }

    void OnControllerColliderHit(ControllerColliderHit col)
    {
        if (col.collider.tag == "Wall")
        {
            Vector3 colDirection = col.point - transform.position;
            colDirection.y = 0;
            thunk = Mathf.Pow(Mathf.Clamp01(1f - Vector3.Angle(col.moveDirection, colDirection.normalized) / 90f), 3f);
        }
    }

    public int checkpoint
    {
        get
        {
            return _checkpoint;
        }
        set
        {
			checkpoints[checkpoint].source.Stop();
            FadeClip(checkpoint, false);
            _checkpoint = value;
			if (checkpoint < checkpoints.Length)
			{
				Debug.Log("Checkpoint " + checkpoint);
				FadeClip(checkpoint, true);
				checkpoints[checkpoint].source.Play();
				helpGiven = false;
			}
			else
			{
				//GAME OVER
				// Disable help
				helpGiven = true;

				//Fade out all optional clips
				for (int i = 10; i < 18; i++)
				{
					FadeClip(i, false);
				}

				//Play the finale
				sources[18].Play();


				// Quit once final clip is done
				Invoke("Quit", sources[18].clip.length + 1f);
			}
		}
	}
    	
    public void FadeClip(int clip, bool fadeIn)
    {
        if (fadeIn)
        {
			checkpointTime = Time.time;
            if (fadingOut.Contains(clip))
            {
                fadingOut.Remove(clip);
            }
            fadingIn.Add(clip);
        }
        else
        {
            if (fadingIn.Contains(clip))
            {
                fadingIn.Remove(clip);
            }
            fadingOut.Add(clip);
        }
    }

	void UnloopFinale()
	{
		sources[18].loop = false;
	}

	void PlayOutro()
	{
		helpSource.volume = 1f;
		helpSource.PlayOneShot(helpClips[11]);
	}

	void Quit()
	{
#if UNITY_EDITOR
		UnityEditor.EditorApplication.isPlaying = false;
#else
		Application.Quit();
#endif
	}
}

