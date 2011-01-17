using System;
using System.IO;
using Newtonsoft.Json.Linq;
using MindTouch.Tasking;

namespace LoveSeat.Interfaces
{
	public interface ICouchDocument
	{
		string Id { get; set; }
		string Rev { get; set; }
	}
}