namespace MissionPlanner.Utilities
{
    public interface ISpeech
    {
        bool speechEnable { get; set; }
        bool IsReady { get; }
        void SpeakAsync(string text);
        void SpeakAsyncCancelAll();
    }
}