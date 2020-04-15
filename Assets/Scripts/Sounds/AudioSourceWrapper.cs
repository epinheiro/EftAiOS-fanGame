using System.Collections;
using UnityEngine;

class AudioSourceWrapper{   
    public AudioSourceWrapper(float maxVolume = 0.05f, float initialVolume = 0){
        SetInnerVolume(maxVolume, initialVolume);
    }

    public AudioSourceWrapper(GameObject newComponentOwner, float maxVolume = 0.05f, float initialVolume = 0){
        SetInnerVolume(maxVolume, initialVolume);
        NewAudioSource(newComponentOwner);
    }

    public AudioSourceWrapper(AudioSource audioSource, float maxVolume = 0.05f, float initialVolume = 0){
        SetInnerVolume(maxVolume, initialVolume);
        SetAudioSource(audioSource);
    }

    ///////////////////////////////////////////////
    /////////////////// Audio source //////////////
    protected AudioSource source;

    /// Public API
    public void SetAudioSource(AudioSource source){
        SetSource(source);
    }

    public void NewAudioSource(GameObject owner){
        SetSource(owner.AddComponent<AudioSource>() as AudioSource);
    }

    void SetSource(AudioSource source){
        source.playOnAwake = false;
        this.source = source;
        SetComponentVolume(Volume);
    }

    ///////////////////////////////////////////////
    /////////////////// Volume ///////////////////
    float _maxVolume;
    float _volume = 0;
    public float Volume{
        get { return _volume; }
        set { 
            _volume = LimitVolume(value);
            source.volume = _volume;
        }
    }
    float LimitVolume(float newvolume){
        if(newvolume > _maxVolume){
            return _maxVolume;
        }else{
            return newvolume;
        }
    }

    void SetInnerVolume(float maxVolume = 0.05f, float currentVolume = 0){
        _maxVolume = maxVolume;
        _volume = LimitVolume(currentVolume);
    }

    void SetComponentVolume(float currentVolume = 0){
        source.volume = currentVolume;
    }

    IEnumerator ChangeVolumeOverTime(float initialVolume, float endVolume, float timePeriod){
        // TODO check - coroutine volume fade is not working
        float finalVolume = LimitVolume(endVolume);
        float difference = _volume - finalVolume;
        float currentVolume = initialVolume;

        int steps = 100;

        float volumeStep = (_volume - finalVolume)/steps;
        float timeStep = timePeriod/steps;

        for( int i=0 ; i<(steps-1) ; i++){
            Volume = Volume + volumeStep;

            yield return new WaitForSeconds(timeStep);
        }

        Volume = finalVolume;
    }

    /// Public API
    public Coroutine ChangeVolumeOverTime(MonoBehaviour owner, float timePeriod, float? endVolume, float? initialVolume){
        if(initialVolume.HasValue) _volume = initialVolume.Value;

        float toVolume;
        if(!endVolume.HasValue) toVolume = _maxVolume;
        else toVolume = endVolume.Value;
        
        return owner.StartCoroutine(ChangeVolumeOverTime(_volume, toVolume, timePeriod));
    }

    public void MaximizeVolume(){
        Volume = _maxVolume;
    }

    ///////////////////////////////////////////////
    /////////////////// Audio clip ////////////////
    protected AudioClip clip;

    AudioClip GetAudioClip(string fileName){
        AudioClip clip;
        clip = (AudioClip) Resources.Load<AudioClip>(GetAudioPath(fileName));
        if(clip == null){
            clip = (AudioClip) Resources.Load<AudioClip>(GetAudioPath(fileName, true));
        }
        if(clip == null){
            throw new System.Exception(string.Format("Audio {0} not found", fileName));
        }
        return clip;
    }

    string GetAudioPath(string fileName, bool isMusic = false){
        if(isMusic){
            return string.Format("Sounds/Effects/{0}", fileName);
        }else{
            return string.Format("Sounds/Music/{0}", fileName);
        }
    }

    void PlayClip(string audioName){
        if(source == null) throw new System.Exception("Cannot play clip - No AudioSource component available, use one of the set options");

        AudioClip clip = GetAudioClip(audioName);
        source.PlayOneShot(clip, _volume);
    }

    void PlayLoop(string audioName){
        AudioClip clip = GetAudioClip(audioName);
        source.clip = clip;
        PlayAssociatedClip(true);
    }

    void PlayAssociatedClip(bool inLoop){
        if(source == null) throw new System.Exception("Cannot play clip - No AudioSource component available, use one of the set options");
        if(source.clip == null) throw new System.Exception("No AudioClip setted, consider using another PlayClip method and give the audio name or use the SetAudioClip method");

        source.loop = inLoop;

        source.Play();
    }

    /// Public API
    public float SetAudioClip(string audioName){
        clip = GetAudioClip(audioName);
        source.clip = clip;
        if(source == null) throw new System.Exception("Cannot set clip - No AudioSource component available, use one of the set options");

        return clip.length;
    }

    public void PlayClip(bool inLoop = false){
        PlayAssociatedClip(inLoop);
    }

    public void PlayClip(string audioName, bool inLoop = false){
        if(inLoop){
            PlayLoop(audioName);
        }else{
            PlayClip(audioName);
        }
    }

    public Coroutine PlayWithFade(MonoBehaviour owner, float timePeriod, bool inLoop = false, float? endVolume = null, float? initialVolume = null){
        PlayClip(inLoop);
        return ChangeVolumeOverTime(owner, timePeriod, endVolume, initialVolume);
    }
}

