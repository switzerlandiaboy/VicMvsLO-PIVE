using System.Collections.Generic;
using FluidMidi;
using UnityEngine;

[CreateAssetMenu(fileName = "MIDISong", menuName = "ScriptableObjects/MIDISong")]
public class MIDISong : ScriptableObject
{
    [SerializeField] public StreamingAsset song;
    [SerializeField] public bool autoSoundfont = true;
    [SerializeField] public StreamingAsset soundfont;
    [SerializeField] public int startTicks;
    [SerializeField] public int startLoopTicks;
    [SerializeField] public int endTicks;
    [SerializeField] [Range(1, 16)] public int bahChannel;
    [SerializeField] public int[] bahTicks;
    [SerializeField] [BitField] public int mutedChannelsNormal;
    [SerializeField] [BitField] public int mutedChannelsSpectating;
    [SerializeField] [Range(0.1f, 2.0f)] public float playbackSpeedNormal = 1.0f;
    [SerializeField] [Range(0.1f, 2.0f)] public float playbackSpeedFast = 1.25f;
    
    public bool hasBahs => bahTicks != null && bahTicks.Length != 0;
}
