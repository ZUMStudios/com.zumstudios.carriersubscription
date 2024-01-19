using System;

namespace com.zumstudios.carriersubscription
{
    [Serializable]
    public class DigitalVirgoError
    {
        public string result_code;
        public string message;

        public DigitalVirgoError() { }
    }
}
