namespace DrTrottoirApi.Exceptions
{
    [Serializable]
    public class TaskNotFoundException : Exception
    {
        public TaskNotFoundException() : base("Task not found") { }
    }
}
