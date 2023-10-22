namespace codecrafters_bittorrent.src.Attributes
{
    public class NameAttribute : Attribute
    {
        public string Name { get; }
        public NameAttribute(string name)
        {
            Name = name;
        }
    }
}
