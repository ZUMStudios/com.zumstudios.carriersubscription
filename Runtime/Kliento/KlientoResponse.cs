using System;
using System.Collections.Generic;

namespace com.zumstudios.carriersubscription
{
    [Serializable]
    public class KlientoResponse
    {
        public bool success;
        public string auth_token;
        public List<KlientoData> data;
        public List<string> errors;

        public KlientoResponse() { }
    }
}
