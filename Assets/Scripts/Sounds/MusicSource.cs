using System.Collections;
using System.Collections.Generic;
using UnityEngine;

class MusicSource : AudioSourceWrapper{
    public MusicSource(GameObject sourceOwner) : base(sourceOwner){
        this.SetAudioClip("AmbienceLoop");
    }
}
