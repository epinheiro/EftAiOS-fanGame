using UnityEngine;
using System.Collections;

public class AudioController
{
    GameObject owner;

    MusicSource music;
    EffectSource effect;

    ServerController serverController;

    public AudioController(GameObject owner){
        this.owner = owner;

        music = new MusicSource(owner);
        effect = new EffectSource(owner);

        serverController = owner.GetComponent<ServerController>();
    }

    ////// Sound events
    public void MatchStart(){
        float clipSeconds = effect.SetAudioClip("Door");
        music.PlayWithFade(serverController, clipSeconds * 0.60f, true);
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
        float delay = 1f;
        music.StopAfterDelay(serverController, delay+.15f);
        CoroutineHelper.DelayedCall(serverController, EndgamePlayShutdown, delay);
    }

    void EndgamePlayShutdown(){
        effect.PlayClip("Shutdown");
    }
}

