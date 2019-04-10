namespace MissionPlanner.Wizard
{
    interface IWizard
    {
        /// <summary>
        /// returns the number of pages to progress. ie 0 = none, 2 = 2
        /// </summary>
        /// <returns></returns>
        int WizardValidate();

        /// <summary>
        /// returns if we can leave (back or next)
        /// </summary>
        /// <returns></returns>
        bool WizardBusy();
    }
}