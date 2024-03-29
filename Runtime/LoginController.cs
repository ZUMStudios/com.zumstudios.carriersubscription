using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace com.zumstudios.carriersubscription
{
    public class LoginController : MonoBehaviour
    {
        public static LoginMethod CurrentLoginMethod;

        public virtual void RevalidateLogin()
        {
            var userInfo = UserInfo.Load();

            if (userInfo != null)
            {
                if (userInfo.password != null)
                {
                    PasswordLogin(
                        userInfo.phone_number,
                        userInfo.password,
                        OnLoginSuccess,
                        OnLoginError
                    );
                }
                else
                {
                    CascadeLogin(userInfo.phone_number, OnLoginSuccess, OnLoginError);
                }
            }
        }

        public virtual void CascadeLogin(
            string phoneNumber,
            Action<string> onSuccess,
            Action<string> onError
        )
        {
            CurrentLoginMethod = LoginMethod.DigitalVirgo;

            CarrierSubscription.Instance.LoginDigitalVirgo(
                phoneNumber,
                OnLoginSuccess,
                message =>
                {
                    CurrentLoginMethod = LoginMethod.Kliento;

                    CarrierSubscription.Instance.LoginKliento(
                        phoneNumber,
                        null,
                        OnLoginSuccess,
                        OnLoginError
                    );
                }
            );
        }

        public virtual void PasswordLogin(
            string phoneNumber,
            string password,
            Action<string> onSuccess,
            Action<string> onError
        )
        {
            CurrentLoginMethod = LoginMethod.KlientoWithPassword;

            CarrierSubscription.Instance.LoginKliento(
                phoneNumber,
                password,
                OnLoginSuccess,
                OnLoginError
            );
        }

        public virtual void OnLoginSuccess(string message)
        {
            Debug.Log($"OnLoginSuccess: {message}");
        }

        public virtual void OnLoginError(string message)
        {
            Debug.Log($"OnLoginError: {message}");
        }
    }
}
