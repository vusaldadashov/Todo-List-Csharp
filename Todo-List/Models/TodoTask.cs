namespace Todo_List.Models
{
    public class TodoTask
    {
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public DateTime CreatedDate { get; set; }

        public string Status { get; set; } = "Incompleted";
        public int Id { get; set; }

        public DateTime Deadline { get; set; }
        
    }
    
}
