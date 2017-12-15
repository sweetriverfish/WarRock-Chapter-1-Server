namespace Core.Database {
    public abstract class Database {

        protected readonly string name;
        protected string connectionString;

        public Database(string name) {
            this.name = name;
        }

        public abstract bool Open();
    }
}
