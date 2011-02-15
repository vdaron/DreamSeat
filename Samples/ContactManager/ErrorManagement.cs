using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace ContactManager
{
	static class ErrorManagement
	{
		public static void ProcessException(Exception exception)
		{
			MessageBox.Show("Error while communicating with Couchdb :" + exception.Message);
		}
	}
}
