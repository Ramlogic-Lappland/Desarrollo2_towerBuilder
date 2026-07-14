using UnityEngine;

public class SoundManager : MonoBehaviour
{
    [SerializeField] private SoundLibrary sfxLibrary;
    [SerializeField] private AudioSource sfx2DSource;
    
    public static SoundManager instance;
    
    private float _masterVolume = 0.5f;
    
    private void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
        }
        else
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }
    
    public void PlaySound3D(AudioClip clip, Vector3 position)
    {
        if (clip != null)
        {
            AudioSource.PlayClipAtPoint(clip, position, _masterVolume);
        }
    }

    public void PLaySound3D(string soundName, Vector3 position)
    {
        PlaySound3D(sfxLibrary.GetClipFromName(soundName), position);
    }

    public void PlaySound2D(string soundName)
    {
        AudioClip clip = sfxLibrary.GetClipFromName(soundName);
    
        if (clip != null)
        {
            sfx2DSource.PlayOneShot(clip, _masterVolume);
        }
        else
        {
            Debug.LogWarning($"SoundManager: Could not find sound effect named '{soundName}' in the SoundLibrary!");
        }
    }
}
