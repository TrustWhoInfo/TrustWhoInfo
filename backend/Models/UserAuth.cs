namespace backend
{
    public class UserAuth
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string Method { get; set; }
        public string External1 { get; set; }
        public string External2 { get; set; }
        public string External3 { get; set; }
        public DateTime LastLogin { get; set; }
        public DateTime Created { get; set; }
    }
}