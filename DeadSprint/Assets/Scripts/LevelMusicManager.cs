using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelMusicManager : MonoBehaviour
{

    public Slider musicSlider, effectsSlider;
    private float musicFloat, effectsFloat;

    private static readonly string BGMusicPref = "BGMusicPref";
    private static readonly string SoundEffectsPref = "SoundEffectsPref";

    public AudioSource igtrack8bit;


    // Start is called before the first frame update
    void Start()
    {
        musicSlider.value = PlayerPrefs.GetFloat(BGMusicPref);
        effectsSlider.value = PlayerPrefs.GetFloat(SoundEffectsPref);
        igtrack8bit.volume = musicSlider.value;
        igtrack8bit.Play();
    }

    // Update is called once per frame
    void Update()
    {

    }


    public void musicUpdate()
    {
        PlayerPrefs.SetFloat(BGMusicPref, musicSlider.value);
        igtrack8bit.volume = musicSlider.value;

    }
}
