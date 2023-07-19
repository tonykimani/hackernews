namespace libs.Converters
{
    [AttributeUsage(AttributeTargets.Property)]
    public class MappedNameAttribute : Attribute
    {
        public MappedNameAttribute(string oldName, string newName)
        {
            OldName = oldName;
            NewName = newName;
        }

        public string OldName { get; set; }
        public string NewName { get; set; }

    }
}
