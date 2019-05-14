
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Audio;

public class MicrophoneManager: MonoBehaviour
{
    public static MicrophoneManager Instance { get; private set; }
    private enum State {Increasing, Decreasing, None}

    [SerializeField] private AudioMixerGroup micMixer;
    [SerializeField] private AudioMixerGroup masterMixer;
    
    private AudioSource _source;
    
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(this);
        }
    }

    private void Start()
    {
        _source = GetComponent<AudioSource>();
        _source.loop = true;
        _source.outputAudioMixerGroup = micMixer;
        AndroidRuntimePermissions.Permission permission;

        do
        {
            permission = AndroidRuntimePermissions.CheckPermission("android.permission.RECORD_AUDIO");
            
            if (permission != AndroidRuntimePermissions.Permission.Granted)
            {
                permission = AndroidRuntimePermissions.RequestPermission("android.permission.RECORD_AUDIO");
            }
        } while (permission != AndroidRuntimePermissions.Permission.Granted);

    }

    /// <summary>
    /// Starts tracking the microphone activity
    /// </summary>
    public void StartTracking()
    {
        StartCoroutine(StartRecording());
    }
    
    /// <summary>
    /// Coroutine that tracks the microphone values
    /// </summary>
    /// <returns></returns>
    private IEnumerator StartRecording()
    {
        string device = Microphone.devices[0];
        
        State state = State.None;
       
        int min, max;

        Microphone.GetDeviceCaps(device, out min, out max);

        _source.clip = Microphone.Start(device, true, 1200, max);

        while (Microphone.GetPosition(device) < 1)
        {
            yield return null;
        }
        
        _source.Play();

        float[] clipSampleData = new float[256];
        int count = 0;
        
        while (Microphone.IsRecording(null))
        {
            float clipLoudness = 0f;

            _source.clip.GetData(clipSampleData, _source.timeSamples);

            foreach (float sample in clipSampleData)
            {
                clipLoudness += Math.Abs(sample);
            }

            clipLoudness /= 256;
            
            Debug.Log("Loudness: " + clipLoudness);
            if (clipLoudness >= 0.01f && !state.Equals(State.Decreasing))
            {
                count++;
                state = State.Increasing;
            }
            else if(clipLoudness < 0.01f && !state.Equals(State.Increasing))
            {
                count--;
                state = State.Decreasing;
            }
            else
            {
                state = State.None;
            }
            Debug.Log("Mic counter " + count);

            if(count > 8)
            {
                TheaterManager.Instance.PickAudienceAnimate();
                count = 0;
            }
            else if (count < -6)
            {
                TheaterManager.Instance.PickIndifferentAnimate();
                count = 0;
            }

            yield return new WaitForSeconds(1);
        }
    }
    
    /// <summary>
    /// Stops the recording
    /// </summary>
    public void Stop()
    {
        Microphone.End(null);
        _source.Stop();
        _source.loop = false;
        _source.outputAudioMixerGroup = masterMixer;       
    }

    /// <summary>
    /// Play the recording
    /// </summary>
    public void Play()
    {
        if(!_source.isPlaying)
           _source.Play();
    }
}
