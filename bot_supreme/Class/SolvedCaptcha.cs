using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bot_supreme
{
    class SolvedCaptcha
    {
        public string Response { get; private set; }
        public DateTime CreateTime { get; private set; }
        public bool isValid()
        {

            return (DateTime.Now - CreateTime).TotalSeconds < 120;
        }
        public SolvedCaptcha(string response)
        {
            Response = response;
            CreateTime = DateTime.Now;
        }

    }
}
