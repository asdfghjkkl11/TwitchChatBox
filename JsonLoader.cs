using System.Net.Json;

namespace TwitchChatBox{
    class JsonLoader{
        private string strLocalPath = System.Environment.CurrentDirectory;
        public string Address { get; set; }
        public int Ports { get; set; }
        public string Password { get; set; }
        public string Nickname { get; set; }
        public string Channel { get; set; }
        public string Color { get; set; }
        public JsonLoader(){
            if (!System.IO.File.Exists(strLocalPath + "\\info.json")){              
                return; //파일없음
            }
            string strReturnValue = System.IO.File.ReadAllText("info.json");
            if (strReturnValue == ""){               
                return; //불러오기 실패
            }
            JsonTextParser jtr = new JsonTextParser();
            JsonObject jo = jtr.Parse(strReturnValue);
            JsonObjectCollection jac = (JsonObjectCollection)jo;
            Address = (jac["Address"].GetValue() != null) ? jac["Address"].GetValue().ToString() : "irc.twitch.tv";
            Ports = (jac["Ports"].GetValue() != null) ? int.Parse(jac["Ports"].GetValue().ToString()) : 6667;
            Password = (jac["Password"].GetValue() != null) ? jac["Password"].GetValue().ToString() : "";
            Nickname = (jac["Nickname"].GetValue() != null) ? jac["Nickname"].GetValue().ToString() : "";
            Channel = (jac["Channel"].GetValue() != null) ? jac["Channel"].GetValue().ToString() : "";
            Color = (jac["Color"].GetValue()!=null) ? jac["Color"].GetValue().ToString() : "#FFFFFF";
        }
    }
}
