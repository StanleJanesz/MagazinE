﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedLibrary.Base_Classes___Database
{
	public abstract class UnbanRequestAbstract
	{
		public int Id { get; set; }
		public string Reason { get; set; }
		public int BanId { get; set; }
		public UnbanRequestState State { get; set; }
	}
}
