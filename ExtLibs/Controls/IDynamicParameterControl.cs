using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MissionPlanner.Controls
{
    public delegate void EventValueChanged(object sender, string Name, string Value);

   public interface IDynamicParameterControl
   {
       event EventValueChanged ValueChanged;
      /// <summary>
      /// Gets the name.
      /// </summary>
      /// <value>
      /// The name.
      /// </value>
      string Name { get; set; }

      /// <summary>
      /// Gets the value.
      /// </summary>
      /// <value>
      /// The value.
      /// </value>
      string Value { get; set; }
   }
}
