using Newtonsoft.Json;

namespace SuperInnovaLib
{
    [System.Serializable]
    public class iWResponse<T>
    {
        public bool success;
        public int responseType;
        public T data;
        public iWResponseError error;
        public object attachment;

        public string ToJson(bool format = false)
        {
            return JsonConvert.SerializeObject(this, (format ? Formatting.Indented : Formatting.None));
        }

        public string ToDataJson(bool format = false)
        {
            return JsonConvert.SerializeObject(data, (format ? Formatting.Indented : Formatting.None));
        }
    }


    [System.Serializable]
    public class MNull { }

    
}