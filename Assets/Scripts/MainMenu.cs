using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Audio;
using UnityEngine.UI;


public class MainMenu : MonoBehaviour
{
    public AudioMixer audioMixer;

    public Slider musicSlider;
    public Slider sfxSlider;

    public void Start()
    {
        LoadVolume();
        musicSlider.value = PlayerPrefs.GetFloat("SFXVolume", 0.75f);
        sfxSlider.value = PlayerPrefs.GetFloat("MusicVolume", 0.75f);
        MusicManager.instance.PlayMusic("menusong");
    }

    public void PlayGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        MusicManager.instance.PlayMusic("gamesong");
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void UpdateMusicVolume(float volume)
    {
        audioMixer.SetFloat("MusicVolume",Mathf.Log10 (volume) *20);
        Debug.Log(volume);
    }

    public void UpdateSFXVolume(float volume)
    {
        audioMixer.SetFloat("SFXVolume",Mathf.Log10 (volume) *20);
    }
    
    public void SetLevel (float sliderValue)
    {
        audioMixer.SetFloat("MusicVol", Mathf.Log10(sliderValue) * 20);
        PlayerPrefs.SetFloat("MusicVolume", sliderValue);
    }

    public void LoadVolume()
    {
        musicSlider.value = PlayerPrefs.GetFloat("MusicVolume");
        sfxSlider.value = PlayerPrefs.GetFloat("SFXVolume");
    }
}
