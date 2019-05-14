public interface Activity
{
    void StartActivity();
    
    Activity NextActivity();

    void Activate(bool status);
}
