using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClutterFeed
{
    class Profile
    {
        public bool Active { get; set; } = false;
        public string Name { get; set; }
        public string UserKey { get; set; }
        public string UserSecret { get; set; }
        public bool Default { get; set; } = false;
    }
}
