namespace Core.Entities {
    public class Entity {

        public uint ID { get; protected set; }
        public string Name { get; protected set; }
        public string Displayname { get; protected set; }
        public byte AccessLevel { get; protected set; }

        public Entity(uint id, string name, string displayname) {
            this.ID = id;
            this.Name = name;
            this.Displayname = displayname;
            this.AccessLevel = 1;
        }

        public Entity(uint id, string name, string displayname, byte _accessLevel)
        {
            this.ID = id;
            this.Name = name;
            this.Displayname = displayname;
            this.AccessLevel = _accessLevel;
        }
    }
}
