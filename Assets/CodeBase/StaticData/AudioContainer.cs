using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Match3 / Audio Container", fileName = "Audio Container", order = 1)]
public class AudioContainer : ScriptableObject
{
    public List<AudioItem> AudioItems; 
}