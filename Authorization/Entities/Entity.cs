namespace Authorization.Entities
{
    class Entity
    {
        protected uint id;
        protected string name;
        protected string displayname;
        protected int health;
        protected int maxHealth;

        public Entity(uint id, string name) {
            this.id = id;
            this.name = name;
        }
    }
}
