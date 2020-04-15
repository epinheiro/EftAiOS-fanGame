using System.Collections;
using System.Collections.Generic;
using UnityEngine;

class MusicSource : AudioSourceWrapper{
    public MusicSource(GameObject sourceOwner) : base(sourceOwner, 0.15f){
        this.SetAudioClip("AmbienceLoop");
    }
}
