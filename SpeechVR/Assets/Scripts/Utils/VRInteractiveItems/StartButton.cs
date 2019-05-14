using System;
using UnityEngine;

public class StartButton: MonoBehaviour, VRInteractiveItem
{
    [SerializeField] private StartingActivity activity;
    public void StartInteraction()
    {
        activity.StartBreathing();
    }
}
