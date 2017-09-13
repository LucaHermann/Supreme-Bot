using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace bot_supreme
{
    [Serializable]
    public class ProductInfo
    {
        public string Name { get; set; }
        public string Color { get; set; }
        public string Category { get; set; }
        public string Size { get; set; }
        public string SizeDefault { get; set; }
}
}
