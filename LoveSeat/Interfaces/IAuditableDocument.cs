using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LoveSeat.Interfaces
{
	public interface IAuditableDocument
	{
		void Creating();
		void Updating();
		void Deleting();

		void Created();
		void Updated();
		void Deleted();
	}
}
