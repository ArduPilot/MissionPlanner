namespace MissionPlanner.Utilities
{
    public interface ISpeech
    {
        bool IsReady { get; }
        void SpeakAsync(string text);
        void SpeakAsyncCancelAll();
    }
}