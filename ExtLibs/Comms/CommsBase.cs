using System.Collections;

namespace MissionPlanner.Comms
{
    public delegate string SettingsOption(string name, string value, bool set = false);

    public delegate void ApplyThemeTo(object control);

    public enum inputboxreturn
    {
        OK,
        Cancel,
        NotSet
    }

    public delegate inputboxreturn InputBoxShow(string title, string prompttext, ref string text);

    public abstract class CommsBase
    {
        private readonly Hashtable cache = new Hashtable();

        public static event InputBoxShow InputBoxShow;

        public static event SettingsOption Settings;

        public static event ApplyThemeTo ApplyTheme;

        protected virtual void ApplyThemeTo(object control)
        {
            if (ApplyTheme != null) ApplyTheme(control);
        }

        protected virtual inputboxreturn OnInputBoxShow(string title, string prompttext, ref string text)
        {
            if (InputBoxShow == null)
                return inputboxreturn.NotSet;

            return InputBoxShow(title, prompttext, ref text);
        }

        protected virtual string OnSettings(string name, string value, bool set = false)
        {
            // answer using external function
            if (Settings != null)
            {
                // get the external saved value
                var answer = Settings(name, value, set);

                // return value if its a bad answer
                if (answer == "")
                    return value;

                // return external value
                return answer;
            }

            // save it if we dont have a config
            if (set)
                cache[name] = value;

            // return it if we have seen it
            if (cache.ContainsKey(name))
                return cache[name].ToString();

            // return what was passed in if no answer
            return value;
        }
    }
}