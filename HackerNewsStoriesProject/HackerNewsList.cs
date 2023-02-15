namespace HackerNewsStoriesProject
{
    public class HackerNewsList
    {
        public int Id;
        public string? Type { get; set; }
        public string? Text { get; set; }
        public string? By { get; set; }
    //Stores the Userid of the Author
        public string? Time { get; set; }
        public string? Title { get; set;}
        public string? URL { get; set; }
        public bool Deleted { get; set; }
     }
}
