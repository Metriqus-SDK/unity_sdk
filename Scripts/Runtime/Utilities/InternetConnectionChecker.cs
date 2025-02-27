using MetriqusSdk.Web;
using System;
using System.Threading.Tasks;
using UnityEngine;

namespace MetriqusSdk
{
    internal class InternetConnectionChecker
    {
        private const string TestUrl = "https://www.google.com";

        public Action OnConnectedToInternet;

        private bool isConnected = false;
        public  bool IsConnected => isConnected;

        public async Task<bool> CheckInternetConnection()
        {
            if (Application.internetReachability == NetworkReachability.NotReachable)
            {
                Debug.Log("No Internet connection (device unreachable).");

                return false;
            }
            else
            {
                isConnected = await CheckInternetConnectionViaRequest();

                if (isConnected)
                   OnConnectedToInternet.Invoke();

                return isConnected;
            }
        }

        private async Task<bool> CheckInternetConnectionViaRequest()
        {
            var response = await RequestSender.GetAsync(TestUrl, null);

            if (response.IsSuccess == false)
            {
                if (response.ErrorType == ErrorType.ConnectionError)
                {
                    return false;
                }
            }

            return true;
        }
    }
}