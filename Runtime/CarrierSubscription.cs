using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Networking;

namespace com.zumstudios.carriersubscription
{
    public class CarrierSubscription : MonoBehaviour
    {
        #region Variables
        private bool _isInitialized;

        private int _mcmId;
        private string _digitalVirgoUser;
        private string _digitalVirgoPassword;
        private int _atomService_Id;

        public static bool EnableLogs;

        #endregion

        #region Public methods
        public bool IsInitialized() => _isInitialized;

        public void Initialize(
            int mcmId,
            string digitalVirgoUser,
            string digitalVirgoPassword,
            int atomServiceId,
            Action onInitialized = null
        )
        {
            _instance._mcmId = mcmId;
            _instance._digitalVirgoUser = digitalVirgoUser;
            _instance._digitalVirgoPassword = digitalVirgoPassword;
            _instance._atomService_Id = atomServiceId;

            _isInitialized = true;

            onInitialized?.Invoke();
        }

        public void LoginDigitalVirgo(
            string phoneNumber,
            Action<string> onSuccess,
            Action<string> onError
        )
        {
            if (IsInitialized() == false)
            {
                throw new System.Exception(
                    "CarrierSubscription não foi inicializado corretamente."
                );
            }

            StartCoroutine(LoginDigitalVirgoCoroutine(phoneNumber, onSuccess, onError));
        }

        public void LoginKliento(
            string phoneNumber,
            string password,
            Action<string> onSuccess,
            Action<string> onError
        )
        {
            if (IsInitialized() == false)
            {
                throw new System.Exception(
                    "CarrierSubscription não foi inicializado corretamente."
                );
            }

            StartCoroutine(LoginKlientoDefaultCoroutine(phoneNumber, password, onSuccess, onError));
        }
        #endregion

        #region Private methods
        private IEnumerator LoginDigitalVirgoCoroutine(
            string phoneNumber,
            Action<string> onSuccess,
            Action<string> onError
        )
        {
            var numberWithCountryCode = $"%2b{phoneNumber}";
            var url = new StringBuilder().Append(CarrierSubscriptionConstants.DIGITAL_VIRGO_URL);
            url.Replace("{PHONE_NUMBER}", numberWithCountryCode);
            url.Replace("{MCM_ID}", _mcmId.ToString());

            var userPass = $"{_digitalVirgoUser}:{_digitalVirgoPassword}";
            var bytes = ASCIIEncoding.ASCII.GetBytes(userPass);
            var base64userPass = Convert.ToBase64String(bytes);

            var request = UnityWebRequest.Get(url.ToString());
            request.method = "GET";
            request.SetRequestHeader("Authorization", $"Basic {base64userPass}");
            request.SetRequestHeader("Content-Type", "application/json");
            request.timeout = 10;

            yield return request.SendWebRequest();

            HandleDigitalVirgoLoginRequest(request, onSuccess, onError);
        }

        private void HandleDigitalVirgoLoginRequest(
            UnityWebRequest request,
            Action<string> onSuccess,
            Action<string> onError
        )
        {
            switch (request.result)
            {
                case UnityWebRequest.Result.Success:
                    try
                    {
                        var jsonString = request.downloadHandler.text;
                        PrintLog(jsonString);

                        if (jsonString.Contains("result_code") || jsonString.Contains("message"))
                        {
                            var errorObj = JsonConvert.DeserializeObject<DigitalVirgoError>(
                                jsonString
                            );
                            onError?.Invoke($"Erro: {errorObj.message}");
                        }
                        else
                        {
                            var user = JsonConvert.DeserializeObject<DigitalVirgoUser[]>(
                                jsonString
                            );

                            if (user.Length == 0)
                                onError?.Invoke($"Erro: Usuário inválido");
                            else
                                HandleLoggedDigitalVirgoUser(user[0], onSuccess, onError);
                        }
                    }
                    catch (Exception)
                    {
                        onError?.Invoke("Erro: Não foi possível realizar o login no momento.");
                    }
                    break;
                case UnityWebRequest.Result.ConnectionError:
                    onError?.Invoke($"Erro de conexão: {request.responseCode}");
                    break;
                case UnityWebRequest.Result.DataProcessingError:
                    onError?.Invoke("Erro ao obter os dados do servidor");
                    break;
                case UnityWebRequest.Result.ProtocolError:
                    onError?.Invoke("Erro de conexão");
                    break;
                default:
                    break;
            }
        }

        private void HandleLoggedDigitalVirgoUser(
            DigitalVirgoUser user,
            Action<string> onSuccess,
            Action<string> onError
        )
        {
            if (
                string.Equals(
                    user.status,
                    CarrierSubscriptionConstants.DIGITAL_VIRGO_ACTIVE_STATUS_LABEL
                )
            )
            {
                if (string.IsNullOrEmpty(user.termination_date))
                {
                    var now = DateTime.Now;
                    now = now.AddDays(7);
                    user.termination_date = now.ToString("yyyy-MM-ddTHH:mm:ss");
                }

                var userInfo = new UserInfo();
                userInfo.phone_number = user.msisdn.Replace("+", "");
                userInfo.termination_date = user.termination_date;
                userInfo.api_key = user.api_key;
                userInfo.Save();

                onSuccess?.Invoke("Assinatura validada com sucesso!");
            }
            else
            {
                onError?.Invoke($"Erro: Usuário não elegível para este produto");
            }
        }

        private IEnumerator LoginKlientoDefaultCoroutine(
            string phoneNumber,
            string password,
            Action<string> onSuccess,
            Action<string> onError
        )
        {
            var numberWithCountryCode = $"%2b{phoneNumber}";
            var url = new StringBuilder();
            var userInfo = new UserInfo();
            userInfo.phone_number = phoneNumber;

            if (string.IsNullOrEmpty(password) || string.IsNullOrWhiteSpace(password))
            {
                url.Append(CarrierSubscriptionConstants.KLIENTO_DEFAULT_URL);
            }
            else
            {
                url.Append(CarrierSubscriptionConstants.KLIENTO_VALIDATOR_URL);
                url.Replace("{VALIDATOR}", EncryptionHelper.Encrypt(password));
                userInfo.password = password;
            }

            url.Replace("{PHONE_NUMBER}", numberWithCountryCode);
            url.Replace("{ATOM_SERVICE_ID}", _atomService_Id.ToString());

            var request = UnityWebRequest.Get(url.ToString());
            request.method = "GET";
            request.SetRequestHeader("Authorization", $"Basic XXXXX");
            request.SetRequestHeader("Content-Type", "application/json");
            request.timeout = 10;

            yield return request.SendWebRequest();

            HandleKlientoLoginRequest(request, userInfo, onSuccess, onError);
        }

        private void HandleKlientoLoginRequest(
            UnityWebRequest request,
            UserInfo userInfo,
            Action<string> onSuccess,
            Action<string> onError
        )
        {
            switch (request.result)
            {
                case UnityWebRequest.Result.Success:
                    try
                    {
                        var response = JsonConvert.DeserializeObject<KlientoResponse>(
                            request.downloadHandler.text
                        );
                        PrintLog(request.downloadHandler.text);

                        if (response.success)
                            HandleKlientoLoggedData(response.data, userInfo, onSuccess, onError);
                        else
                            onError?.Invoke($"Erro: {response.errors[0].ToString()}");
                    }
                    catch (Exception)
                    {
                        onError?.Invoke("Erro: Não foi possível realizar o login no momento.");
                    }
                    break;
                case UnityWebRequest.Result.ConnectionError:
                    onError?.Invoke($"Erro de conexão: {request.responseCode}");
                    break;
                case UnityWebRequest.Result.DataProcessingError:
                    onError?.Invoke("Erro ao obter os dados do servidor");
                    break;
                case UnityWebRequest.Result.ProtocolError:
                    try
                    {
                        var response = JsonConvert.DeserializeObject<KlientoResponse>(
                            request.downloadHandler.text
                        );
                        PrintLog(request.downloadHandler.text);

                        if (response.success == false)
                            onError?.Invoke("Erro: Usuário ou senha inválido");
                        else
                            onError?.Invoke("Erro de conexão");
                    }
                    catch (Exception)
                    {
                        onError?.Invoke("Erro de conexão");
                    }
                    break;
                default:
                    break;
            }
        }

        private void HandleKlientoLoggedData(
            List<KlientoData> dataList,
            UserInfo userInfo,
            Action<string> onSuccess,
            Action<string> onError
        )
        {
            bool isSubscribed = false;

            foreach (var data in dataList)
            {
                foreach (var account in data.accounts)
                {
                    if (account.atom_service_id == _atomService_Id)
                    {
                        var capacities = account.capacities.SingleOrDefault(
                            obj => account.id == obj.account_id
                        );

                        if (capacities != default(KlientoCapacities))
                        {
                            isSubscribed = string.Equals(
                                capacities.status_label,
                                CarrierSubscriptionConstants.KLIENTO_SUBSCRIBED_STATUS_LABEL
                            );
                        }
                    }
                }
            }

            if (isSubscribed)
            {
                var now = DateTime.Now;
                now = now.AddDays(7);
                userInfo.termination_date = now.ToString("yyyy-MM-ddTHH:mm:ss");
                userInfo.Save();
                onSuccess?.Invoke("Assinatura validada com sucesso!");
            }
            else
            {
                onError?.Invoke("Erro: Usuário não elegível para este produto");
            }
        }

        private void PrintLog(object message)
        {
            if (CarrierSubscription.EnableLogs)
            {
                Debug.Log(message);
            }
        }

        #endregion

        #region Singleton
        private static readonly object padlock = new object();
        private static CarrierSubscription _instance;
        public static CarrierSubscription Instance
        {
            get
            {
                lock (padlock)
                {
                    if (_instance == null)
                    {
                        var go = new GameObject();
                        go.name = "@CarrierSubscription";
                        _instance = go.AddComponent<CarrierSubscription>();
                        DontDestroyOnLoad(go);
                    }

                    return _instance;
                }
            }
        }

        private CarrierSubscription() { }
        #endregion
    }
}
