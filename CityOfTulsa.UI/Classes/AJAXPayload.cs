using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;

namespace CityOfTulsaUI.Classes {

   [Serializable]
   [DataContract]
   [JsonObject(MemberSerialization.OptOut)]
   public class AJAXPayload {

      public AJAXPayload(AJAXMessage ajaxMessage) {
         this.cmd = ajaxMessage.cmd;
         this.data = ajaxMessage.data;
         this.context = ajaxMessage.context;
         this.ids = ajaxMessage.ids;
         this.values = ajaxMessage.values;
         this.dict = ajaxMessage.dict;
      }

      private string _cmd = null;
      private string _origcmd = null;
      private string _data = null;
      private string _id = null;
      private string _index = null;
      private string _key = null;
      private string _value = null;
      private string _returncode = null;
      private string _context = null;
      private string _action = null;
      private string _msg = null;
      private string _msgmode = null;
      private string _url = null;
      private string _markup = null;
      private string _label = null;
      private string _text = null;
      private string _type = null;
      private string _timetext = null;
      private string _timespan = null;
      private string _time1 = null;
      private string _time2 = null;
      private string[] _ids = null;
      private string[] _keys = null;
      private string[] _values = null;
      private Dictionary<string, string> _dict = null;

      public string cmd {
         get {
            return (_cmd ?? string.Empty);
         }
         set {
            _cmd = value;
         }
      }

      public string origcmd {
         get {
            return (_origcmd ?? string.Empty);
         }
         set {
            _origcmd = value;
         }
      }

      public string data {
         get {
            return (_data ?? string.Empty);
         }
         set {
            _data = value;
         }
      }

      public string id {
         get {
            return (_id ?? string.Empty);
         }
         set {
            _id = value;
         }
      }

      public string index {
         get {
            return (_index ?? string.Empty);
         }
         set {
            _index = value;
         }
      }

      public string key {
         get {
            return (_key ?? string.Empty);
         }
         set {
            _key = value;
         }
      }

      public string value {
         get {
            return (_value ?? string.Empty);
         }
         set {
            _value = value;
         }
      }

      public string returncode {
         get {
            return (_returncode ?? string.Empty);
         }
         set {
            _returncode = value;
         }
      }

      public string context {
         get {
            return (_context ?? string.Empty);
         }
         set {
            _context = value;
         }
      }

      public string action {
         get {
            return (_action ?? string.Empty);
         }
         set {
            _action = value;
         }
      }

      public string msg {
         get {
            return (_msg ?? string.Empty);
         }
         set {
            _msg = value;
         }
      }

      public string msgmode {
         get {
            return (_msgmode ?? string.Empty);
         }
         set {
            _msgmode = value;
         }
      }

      public string label {
         get {
            return (_label ?? string.Empty);
         }
         set {
            _label = value;
         }
      }

      public string text {
         get {
            return (_text ?? string.Empty);
         }
         set {
            _text = value;
         }
      }

      public string type {
         get {
            return (_type ?? string.Empty);
         }
         set {
            _type = value;
         }
      }

      public string url {
         get {
            return (_url ?? string.Empty);
         }
         set {
            _url = value;
         }
      }

      public string markup {
         get {
            return (_markup ?? string.Empty);
         }
         set {
            _markup = value;
         }
      }

      public string time1 {
         get {
            return (_time1 ?? string.Empty);
         }
         set {
            _time1 = value;
         }
      }

      public string time2 {
         get {
            return (_time2 ?? string.Empty);
         }
         set {
            _time2 = value;
         }
      }

      public string timetext {
         get {
            return (_timetext ?? string.Empty);
         }
         set {
            _timetext = value;
         }
      }

      public string timespan {
         get {
            return (_timespan ?? string.Empty);
         }
         set {
            _timespan = value;
         }
      }

      public string[] ids {
         get {
            return (_ids ?? new string[] { });
         }
         set {
            _ids = value;
         }
      }

      public string[] keys {
         get {
            return (_keys ?? new string[] { });
         }
         set {
            _keys = value;
         }
      }

      public string[] values {
         get {
            return (_values ?? new string[] { });
         }
         set {
            _values = value;
         }
      }

      public Dictionary<string, string> dict {
         get {
            return (_dict ?? new Dictionary<string, string>());
         }
         set {
            _dict = value;
         }
      }
   }
}
