﻿using UnityEngine;
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
        music.PlayWithFade(owner.GetComponent<ServerController>(), clipSeconds * 0.60f, true);
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

    public void EndGameEffect(){
        MonoBehaviour ownerScript = owner.GetComponent<ServerController>();
        float delay = 1f;
        music.StopAfterDelay(ownerScript, delay+.15f);
        CoroutineHelper.DelayedCall(ownerScript, EndgamePlayShutdown, delay);
    }

    void EndgamePlayShutdown(){
        effect.PlayClip("Shutdown");
    }
}

