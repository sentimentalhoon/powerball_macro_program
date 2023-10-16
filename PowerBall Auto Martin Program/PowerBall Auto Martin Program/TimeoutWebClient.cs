using System;
using System.Net;

namespace PowerBallAutoMartinProgram
{
    class TimeoutWebClient : WebClient
    {
        //////////////////////////////////////////////////////////////////////////////////////////////////// Field

        ////////////////////////////////////////////////////////////////////////////////////////// Private



        #region Field



        /// <summary>

        /// 타임 아웃

        /// </summary>

        private int timeOut = 2000;



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



            webRequest.Timeout = this.timeOut;



            return webRequest;

        }



        #endregion
    }
}
