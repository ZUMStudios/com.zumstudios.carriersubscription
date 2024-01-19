using System;
using System.Collections.Generic;

namespace com.zumstudios.carriersubscription
{
    [Serializable]
    public class KlientoCapacities
    {
        public int id;
        public int account_id;
        public int billing_type;
        public string capacity_type;
        public string status_label;
        public int atom_offer_id;
        public int unlimited;
        public string ins_date;
        public string upd_date;
        public int atom_product_id;
        public int consume_access;
        public Dictionary<string, string> extradata;

        public KlientoCapacities() { }
    }
}
