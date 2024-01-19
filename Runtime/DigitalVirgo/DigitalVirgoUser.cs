using System;

namespace com.zumstudios.carriersubscription
{
    [Serializable]
    public class DigitalVirgoUser
    {
        public string id;
        public string password;
        public string msisdn;
        public string register_date;
        public string termination_date;
        public int credits;
        public string status;
        public string login;
        public int product_id;
        public int mcc;
        public int mnc;
        public string api_key;

        public DigitalVirgoUser() {}
    }
}
