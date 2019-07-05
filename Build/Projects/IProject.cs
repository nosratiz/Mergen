namespace Build.Projects
{
    public interface IProject
    {
        string Name { get; }
        void AddTasks();
    }
}