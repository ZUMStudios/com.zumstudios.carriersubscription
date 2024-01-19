using System;
using System.Collections.Generic;

namespace com.zumstudios.carriersubscription
{
    [Serializable]
    public class KlientoAccount
    {
        public int id;
        public int user_id;
        public int atom_service_id;
        public int from_pack;
        public string ins_date;
        public string upd_date;
        public List<KlientoLogin> logins;
        public List<KlientoCapacities> capacities;

        public KlientoAccount() { }
    }
}
