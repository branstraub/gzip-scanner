using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace gzip
{
	interface IActor
	{
		Task Act(string content);
	}
}
