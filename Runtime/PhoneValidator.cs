namespace com.zumstudios.carriersubscription
{
    public static class PhoneValidator
    {
        public static string GetCountryCode(LoginRegion region = LoginRegion.Brasil)
        {
            if (region == LoginRegion.Brasil)
                return "55";

            if (region == LoginRegion.Portugal)
                return "351";

            return "0";
        }

        public static bool IsBrazilAreaCodeLengthCorrect(string areaCode)
        {
            return areaCode.Length == 2;
        }

        public static bool IsPhoneNumberLengthCorrect(
            string phoneNumber,
            LoginRegion region = LoginRegion.Brasil
        )
        {
            if (region == LoginRegion.Brasil || region == LoginRegion.Portugal)
            {
                return phoneNumber.Length == 9;
            }

            return false;
        }
    }
}
