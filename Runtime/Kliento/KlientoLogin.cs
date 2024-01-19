using System;
using System.Collections.Generic;

namespace com.zumstudios.carriersubscription
{
    [Serializable]
    public class KlientoLogin
    {
        public int id;
        public string identifier_type;
        public string identifier;
        public string validator_type;
        public string validator;
        public Dictionary<string, string> extradata;
        public string ins_date;
        public string upd_date;

        public KlientoLogin() { }
    }
}
