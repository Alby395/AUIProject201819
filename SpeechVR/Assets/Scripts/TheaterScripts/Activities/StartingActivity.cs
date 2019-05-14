using UnityEngine;

public class StartingActivity: MonoBehaviour, Activity
{
    [SerializeField] private GameObject ball;
    [SerializeField] private Canvas message;
    [SerializeField] private SpeechActivity nextActivity;
    private BoxCollider _canvasCollider;
    private Animator _animator;
    
    
    private void Start()
    {
        _animator = ball.GetComponent<Animator>();
    }

    public void StartActivity()
    {
        message.enabled = true;

        FirebaseManager.Instance.StartReading();
    }

    public Activity NextActivity()
    {
        gameObject.SetActive(false);
        return nextActivity;
    }

    public void Activate(bool status)
    {
        gameObject.SetActive(status);
    }
    public void StartBreathing()
    {
        ball.SetActive(true);
        message.enabled = false;
        message.GetComponent<BoxCollider>().enabled = false;
        _animator.SetTrigger("Breathing");
        
        FirebaseManager.Instance.SearchBaseValue();
    }
}