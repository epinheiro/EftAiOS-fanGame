using UnityEngine;
using System.Collections;

public class AudioController
{
    GameObject owner;

    MusicSource music;
    EffectSource effect;

    public AudioController(GameObject owner){
        this.owner = owner;

        music = new MusicSource(owner);
        effect = new EffectSource(owner);
    }

    ////// Sound events
    public void MatchStart(){
        float clipSeconds = effect.SetAudioClip("Door");
        music.PlayWithFade(owner.GetComponent<ServerController>(), clipSeconds * 0.75f, true);
        effect.PlayClip();
    }

    public void PlayerDiedEffect(){
        effect.PlayClip("Attacked");
    }

    public void PlayerEscapedEffect(){
        effect.PlayClip("EscapePod");
    }

    public void AlienOverrunEffect(){
        effect.PlayClip("AlienOverrun");
    }
}

