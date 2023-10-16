using System;
using System.Net;

namespace RedMango_PowerBall_Support_Program
{
    class TimeoutWebClient : WebClient
    {
        //////////////////////////////////////////////////////////////////////////////////////////////////// Field

        ////////////////////////////////////////////////////////////////////////////////////////// Private



        #region Field



        /// <summary>

        /// 타임 아웃

        /// </summary>

        private int timeOut = 10000;



        #endregion



        //////////////////////////////////////////////////////////////////////////////////////////////////// Method

        ////////////////////////////////////////////////////////////////////////////////////////// Protected



        #region 웹 요청 구하기 - GetWebRequest(uri)



        /// <summary>

        /// 웹 요청 구하기

        /// </summary>

        /// <param name="uri">URI</param>

        /// <returns>웹 요청</returns>

        protected override WebRequest GetWebRequest(Uri uri)
        {
            WebRequest webRequest = base.GetWebRequest(uri);
            webRequest.Method = "GET";
            webRequest.ContentType = "application/x-www-form-urlencoded; charset=UTF-8";
            webRequest.Timeout = this.timeOut;
            return webRequest;
        }
        #endregion
    }
}
