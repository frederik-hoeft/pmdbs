using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace debugclient
{
    public class ConsoleException : Exception
    {
        private readonly string _message = "NOT_SPECIFIED";
        public ConsoleException(string message)
        {
            _message = message;
        }
        public ConsoleException() { }
        public string message
        {
            get { return _message; }
        }
    }
}
