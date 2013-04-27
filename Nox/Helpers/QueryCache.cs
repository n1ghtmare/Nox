namespace Nox.Helpers
{
    public class QueryCache
    {
        public string Select       { get; set; }
        public string Delete       { get; set; }
        public string Update       { get; set; }
        public string Insert       { get; set; }
        public string InsertWithPk { get; set; }
    }
}