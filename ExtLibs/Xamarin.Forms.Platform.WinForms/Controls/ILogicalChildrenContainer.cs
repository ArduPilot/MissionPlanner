using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WForms = System.Windows.Forms;

namespace Xamarin.Forms.Platform.WinForms
{
	public interface ILogicalChildrenContainer
	{
		IList<WForms.Control> LogicalChildren { get; }
	}
}
