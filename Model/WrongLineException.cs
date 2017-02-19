using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model
{
    public class WrongLineException : Exception
    {
        public WrongLineException(string message) : base(message)
        {

        }
    }
}
