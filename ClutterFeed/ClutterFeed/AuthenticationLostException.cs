using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClutterFeed
{
    class AuthenticationLostException : Exception
    {
        public AuthenticationLostException() : base()
        {

        }

        public override string Message
        {
            get
            {
                return "ClutterFeed requires reauthentication";
            }
        }
    }
}
