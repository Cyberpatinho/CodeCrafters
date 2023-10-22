namespace codecrafters_bittorrent.src.Attributes
{
    public class DisplayAttribute : Attribute
    {
        public string DisplayName { get; }
        public DisplayAttribute(string displayName) 
        {
            DisplayName = displayName;
        }
    }
}
