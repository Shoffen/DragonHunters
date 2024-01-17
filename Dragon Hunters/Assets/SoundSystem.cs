using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundSystem : MonoBehaviour
{
    private AudioSource releaseArrow;
    private AudioSource loadArrow;
    private AudioSource redGotHit;
    private AudioSource jump;
    private AudioSource land;

    // Start is called before the first frame update
    void Start()
    {
        releaseArrow = GetComponent<AudioSource>();
        loadArrow = GetComponents<AudioSource>()[1];
        redGotHit = GetComponents<AudioSource>()[2];
        jump = GetComponents<AudioSource>()[3];
        land = GetComponents<AudioSource>()[4];
    }

    // Properties to access private variables
    public AudioSource ReleaseArrow
    {
        get { return releaseArrow; }
    }

    public AudioSource LoadArrow
    {
        get { return loadArrow; }
    }

    public AudioSource RedGotHit
    {
        get { return redGotHit; }
    }
    public AudioSource Jump
    {
        get { return jump; }
    }
    public AudioSource Land
    {
        get { return land; }
    }
}
