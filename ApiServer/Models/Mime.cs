using System.Collections.Generic;
namespace ApiServer.Models{
   public class Mime
    {
        public Dictionary<string, string> types {get;}

        public Mime() {
            types = new Dictionary<string, string>();
            types.Add("text", "text");
            types.Add("image", "image");
            types.Add("audio", "audio");
            types.Add("application", "application");
        }
    }
}