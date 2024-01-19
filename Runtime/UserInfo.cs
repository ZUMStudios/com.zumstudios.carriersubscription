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

        public UserInfo() { }

        public bool isSubscribed()
        {
            var parsedData = DateTime.ParseExact(
                termination_date,
                "yyyy-MM-ddTHH:mm:ss",
                CultureInfo.InvariantCulture
            );
            return (parsedData > DateTime.Now);
        }

        public void Save()
        {
            var jsonString = JsonConvert.SerializeObject(this);
            PlayerPrefs.SetString(Constants.USERINFO_SAVED_DATA, jsonString);
            PlayerPrefs.Save();
        }

        public static UserInfo Load()
        {
            try
            {
                var jsonString = PlayerPrefs.GetString(Constants.USERINFO_SAVED_DATA);
                return JsonConvert.DeserializeObject<UserInfo>(jsonString);
            }
            catch (Exception)
            {
                return null;
            }
        }

        public static void Delete()
        {
            PlayerPrefs.DeleteKey(Constants.USERINFO_SAVED_DATA);
            PlayerPrefs.Save();
        }
    }
}