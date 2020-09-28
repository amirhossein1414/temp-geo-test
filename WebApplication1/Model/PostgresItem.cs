using NpgsqlTypes;

namespace PerfoTest.Model
{
    public class PostgresItem
    {
        public string id;
        public string title;
        public NpgsqlPoint /*DbGeography*/ geodata;
        public string content;
    }
}
