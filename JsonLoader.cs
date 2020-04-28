using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Net.Json;

namespace TwitchChatBox
{
    class JsonLoader
    {
        private string strLocalPath = System.Environment.CurrentDirectory;
        public string Address { get; set; }
        public int Ports { get; set; }
        public string Password { get; set; }
        public string Nickname { get; set; }
        public string Channel { get; set; }
        public JsonLoader()
        {
            if (!System.IO.File.Exists(strLocalPath + "\\info.json"))
            {
                //파일없음
                return;
            }
            string strReturnValue = System.IO.File.ReadAllText("info.json");
            if (strReturnValue == "")
            {
                //불러오기 실패
                return;
            }
            JsonTextParser jtr = new JsonTextParser();
            JsonObject jo = jtr.Parse(strReturnValue);
            JsonObjectCollection jac = (JsonObjectCollection)jo;
            Address = jac["Address"].GetValue().ToString();
            Ports = int.Parse(jac["Ports"].GetValue().ToString());
            Password = jac["Password"].GetValue().ToString();
            Nickname = jac["Nickname"].GetValue().ToString();
            Channel = jac["Channel"].GetValue().ToString();
            
        }
    }
}
