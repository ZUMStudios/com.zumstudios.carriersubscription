using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using Newtonsoft.Json;
using UnityEngine;

namespace com.zumstudios.carriersubscription
{
    [Serializable]
    public class UserInfo
    {
        public string phone_number;
        public string password;
        public string termination_date;
        public string status;
        public string api_key;

        public UserInfo() { }

        public void Save()
        {
            var jsonString = JsonConvert.SerializeObject(this);
            PlayerPrefs.SetString(CarrierSubscriptionConstants.USERINFO_SAVED_DATA, jsonString);
            PlayerPrefs.Save();
        }

        public static UserInfo Load()
        {
            try
            {
                var jsonString = PlayerPrefs.GetString(
                    CarrierSubscriptionConstants.USERINFO_SAVED_DATA
                );
                return JsonConvert.DeserializeObject<UserInfo>(jsonString);
            }
            catch (Exception)
            {
                return null;
            }
        }

        public static void Delete()
        {
            PlayerPrefs.DeleteKey(CarrierSubscriptionConstants.USERINFO_SAVED_DATA);
            PlayerPrefs.Save();
        }

        public static bool IsSubscribed()
        {
            var user = UserInfo.Load();

            if (user != null)
            {
                if (
                    string.Equals(user.status, "active") || string.Equals(user.status, "subscribed")
                )
                    return true;
            }

            return false;
        }

        public static bool IsSubscribedToProduct(string api_key)
        {
            var user = UserInfo.Load();

            if (user != null)
            {
                if (
                    string.Equals(user.status, "active") || string.Equals(user.status, "subscribed")
                )
                {
                    if (string.Equals(user.api_key, api_key))
                        return true;
                }
            }

            return false;
        }
    }
}
