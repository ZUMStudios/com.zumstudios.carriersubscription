namespace com.zumstudios.carriersubscription
{
    public static class Constants
    {
        public const string DIGITAL_VIRGO_URL =
            "https://opm.jmeservicios.com/public/mcm/api/account/{MCM_ID}?msisdn={PHONE_NUMBER}";
        public const string KLIENTO_DEFAULT_URL =
            "https://kliento.dv-content.io/18000/login/all?identifier={PHONE_NUMBER}&atom_service_id={ATOM_SERVICE_ID}";
        public const string KLIENTO_VALIDATOR_URL =
            "https://kliento.dv-content.io/18000/login/all?identifier={PHONE_NUMBER}&validator={VALIDATOR}&atom_service_id={ATOM_SERVICE_ID}";

        public const string DIGITAL_VIRGO_ACTIVE_STATUS_LABEL = "active";
        public const string KLIENTO_SUBSCRIBED_STATUS_LABEL = "subscribed";
        public const string USERINFO_SAVED_DATA = "USERINFO_SAVED_DATA";
    }
}
