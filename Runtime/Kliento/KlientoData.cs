using System;
using System.Collections.Generic;

namespace com.zumstudios.carriersubscription
{
    [Serializable]
    public class KlientoData
    {
        public int id;
        public int user_type_id;
        public string nickname;
        public string ins_date;
        public string upd_date;
        public List<KlientoAccount> accounts;

        public KlientoData() { }
    }
}
