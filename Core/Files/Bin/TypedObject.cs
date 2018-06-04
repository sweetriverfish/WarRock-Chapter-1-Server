using System;
using System.Collections;
using System.Linq;
using System.Text;

namespace Core.Files.Bin {
    public class TypedObject : Hashtable {

        public string Type { get; private set; }

        public TypedObject() {
            this.Type = "";
        }

        public TypedObject(string type) {
            this.Type = type;
        }

        public TypedObject GetTO(string key) {
            return (TypedObject)this[key];
        }

        public TypedObject GetTO(int key) {
            return (TypedObject)this[key];
        }

        public string GetString(string key) {
            return (string)this[key];
        }

        public int GetInt(string key) {
            object val = this[key];
            if (val == null)
                return 0;
            else
                return int.Parse(val.ToString());
        }

        public object[] GetArray(string key) {
            if (this[key] is TypedObject && GetTO(key).Type.Equals("array"))
                return (Object[])GetTO(key)["array"];
            else
                return (Object[])this[key];
        }

        public static TypedObject MakeArrayCollection(object[] data) {
            TypedObject ret = new TypedObject("array");
            ret.Add("array", data);
            return ret;
        }
    }
}

