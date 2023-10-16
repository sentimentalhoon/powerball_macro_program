using System;
using System.Web;

namespace WaterMelonBettingProgram
{
    class LoginHandler
    {
        public String LoginCheck(string Url, string Id, string Pass)
        {
            string url = Url + ":8085/auto/api/user_auth?u=" + Id + "&p=" + HttpUtility.UrlEncode(Pass) + "&v=1.2.3";
            var rm = UtilModel.MakeAsyncRequest(url, "application/x-www-form-urlencoded; charset=UTF-8");
            return rm.Result;
        }
        public string GetUserName(string id)
        {
            string name = id;
            return name;
        }
    }
}
