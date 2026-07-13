using UnityEngine;

[System.Serializable]
public struct SoundEffect
{
    public string groupID;
    public AudioClip[] clips;
}

public class SoundLibrary : MonoBehaviour
{
    public SoundEffect[] soundEffects;

    public AudioClip GetClipFromName(string name)
    {
        foreach (var soundEffect in soundEffects)
        {
            if (soundEffect.groupID == name)
            {
                if (soundEffect.clips == null || soundEffect.clips.Length == 0)
                {
                    Debug.LogWarning($"SoundLibrary: Group '{name}' exists, but the clips array is empty!");
                    return null;
                }
                return soundEffect.clips[Random.Range(0, soundEffect.clips.Length)];
            }
        }
    
        Debug.LogWarning($"SoundLibrary: No group found with the name '{name}'");
        return null;
    }

}
