using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace bot_supreme
{
    [Serializable]
  public  class UserPreference
    {
     

        public string Name { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Tel { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string Address3 { get; set; }
        public string City { get; set; }
        public string Zip { get; set; }
        public int Country { get; set; }
        public int State { get; set; }
        public int Type { get; set; }   
        public string Number { get; set; }  
        public string ExpYear { get; set; }     
        public string ExpMonth { get; set; }  
        public string Cvv { get; set; }   
        public string Proxy { get; set; }
        public int SleepCheckout { get; set; }
        public string Alarm { get; set; }
        public string ID { get; set; }
        public bool AutoBypassCaptcha { get; set; }
        public List<ProductInfo> ProductList { get; set; }

       

    }
}
