using UnityEngine;

public class VoiceIndicator : MonoBehaviour
{
    private SpriteRenderer rend;
    private Vector3 baseScale;

    void Start()
    {
        rend = GetComponent<SpriteRenderer>();
        baseScale = transform.localScale;
        rend.enabled = false;
    }

    void Update()
    {
        var state = VoiceInput.Instance.CurrentState;

        if (state == VoiceState.Move)
        {
            rend.enabled = false;
            return;
        }

        rend.enabled = true;

        if (state == VoiceState.Talk)
        {
            rend.color = Color.white;
            transform.localScale = baseScale;
        }
        else if (state == VoiceState.Attack)
        {
            rend.color = Color.red;
            float pulse = 1f + Mathf.Sin(Time.time * 10f) * 0.1f;
            transform.localScale = baseScale * pulse;
        }
    }
}
