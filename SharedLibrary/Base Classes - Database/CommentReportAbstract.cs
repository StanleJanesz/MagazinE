using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedLibrary.Base_Classes___Database
{
	public abstract class CommentReportAbstract
	{
		public int Id { get; set; }
		public int CommentId { get; set; }
		public int ReportAuthorId { get; set; }
		public string Reason { get; set; }
		public DateTime Date { get; set; }
		public CommentReportState State { get; set; }
	}
}
