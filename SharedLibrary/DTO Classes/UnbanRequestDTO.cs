using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharedLibrary.Base_Classes___Database;

namespace SharedLibrary.DTO_Classes
{
	public class UnbanRequestDTO : UnbanRequestAbstract
	{
		public int SolvedById { get; set; } //jakies invalid id jak nie jest rozwiazane
	}
}
