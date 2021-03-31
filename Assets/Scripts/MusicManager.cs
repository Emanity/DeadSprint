using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MusicManager : MonoBehaviour
{
    public AudioSource track1;
    public AudioSource track2;

    public int musicHistory=1;


    private static readonly string FirstPlay = "FirstPlay";
    private static readonly string BGMusicPref = "BGMusicPref";
    private static readonly string SoundEffectsPref = "SoundEffectsPref";

    private int firstPlayInt;

    public Slider musicSlider, effectsSlider;
    private float musicFloat, effectsFloat;



    void Start()
    {
        track1.Play();
        musicHistory = 1;

        firstPlayInt = PlayerPrefs.GetInt(FirstPlay);

        if(firstPlayInt == 0)
        {
            musicFloat = 0.5f;
            effectsFloat = 0.5f;
            PlayerPrefs.SetFloat(BGMusicPref, musicFloat);
            PlayerPrefs.SetFloat(SoundEffectsPref, effectsFloat);
            PlayerPrefs.SetInt(FirstPlay, -1);
        } 
        else
        {
            musicFloat = PlayerPrefs.GetFloat(BGMusicPref);
            effectsFloat = PlayerPrefs.GetFloat(SoundEffectsPref);
        }

        musicSlider.value = musicFloat;
        effectsSlider.value = effectsFloat;
        track1.volume = musicFloat;
        track2.volume = musicFloat;
    }

    // Update is called once per frame
    void Update()
    {

            if (!track1.isPlaying && !track2.isPlaying)
            {
                if (musicHistory == 2)
                {
                    track1.Play();
                    musicHistory = 1;
                }
                else if (musicHistory == 1)
                {
                    track2.Play();
                    musicHistory = 2;
                }
            }
        


        //musicSlider.onValueChanged.AddListener(delegate { SaveMusicSettings(); });
        //effectsSlider.onValueChanged.AddListener(delegate { SaveEffectsSettings(); });

    }

    public void SaveMusicSettings()
    {
        musicFloat = musicSlider.value;
        PlayerPrefs.SetFloat(BGMusicPref, musicFloat);

        track1.volume = musicFloat;
        track2.volume = musicFloat;
        
    }

    public void SaveEffectsSettings()
    {
        effectsFloat = effectsSlider.value;
        PlayerPrefs.SetFloat(SoundEffectsPref, effectsFloat);
    }
}
