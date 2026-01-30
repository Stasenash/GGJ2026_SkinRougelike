using UnityEngine;

public enum VoiceState
{
    Idle,
    Move,
    Attack
}

public class VoiceInput : MonoBehaviour
{
    public static VoiceInput Instance;

    public VoiceState CurrentState { get; private set; }

    private AudioClip micClip;
    private const int sampleWindow = 128;
    private float[] samples = new float[sampleWindow];

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        micClip = Microphone.Start(null, true, 1, 44100);
    }

    void Update()
    {
        float volume = GetVolume();

        if (volume < 0.1f)
            CurrentState = VoiceState.Idle;
        else if (volume < 0.2f)
            CurrentState = VoiceState.Move;
        else
            CurrentState = VoiceState.Attack;
    }

    float GetVolume()
    {
        int micPos = Microphone.GetPosition(null) - sampleWindow;
        if (micPos < 0) return 0;

        micClip.GetData(samples, micPos);

        float sum = 0;
        for (int i = 0; i < sampleWindow; i++)
            sum += Mathf.Abs(samples[i]);

        return sum / sampleWindow;
    }
}
