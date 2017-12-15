using System;


namespace Game.Objects
{
   public abstract class Command
    {
        
        public byte ID { get; private set; }
        public byte AccessLevel { get; private set;}
        public string[] Arguments { get; private set; }

        public Command(byte _id, byte _accesslevel)
        {
            ID = _id;
            AccessLevel = _accesslevel;
        }

        public abstract void Process(Entities.User Caster);

        public virtual void SetArguments(string[] _args)
        {
            Arguments = _args;
        }

        public virtual string GetString(int _index)
        {
            if (Arguments.Length > _index)
                return Arguments[_index];

            return String.Empty;
        }

        public virtual int GetInt(int _index)
        {
            int _value = -1;
            if (Arguments.Length > _index)
            {
                
                try
                {
                    Convert.ToInt32(Arguments[_index], _value);
                }
                catch
                {

                }
            }
            return _value;
        }

        public virtual sbyte GetByte(int _index)
        {
            sbyte _value = -1;
            if (Arguments.Length > _index)
            {

                try
                {
                    Convert.ToSByte(Arguments[_index], _value);
                }
                catch
                {

                }
            }
            return _value;
        }
    }
}
