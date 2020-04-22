using System.Collections;
using System.Collections.Generic;
using UnityEngine;

class EffectSource : AudioSourceWrapper{
    public EffectSource(GameObject sourceOwner, float volume = 0.7f) : base(sourceOwner, volume, volume){
    }
}
