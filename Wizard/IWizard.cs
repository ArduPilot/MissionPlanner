﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ArdupilotMega.Wizard
{
    interface IWizard
    {
        /// <summary>
        /// returns the number of pages to progress. ie 0 = none, 2 = 2
        /// </summary>
        /// <returns></returns>
        int WizardValidate();
    }
}
