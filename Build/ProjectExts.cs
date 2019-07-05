using Build.Projects;
using Cake.Core;

namespace Build
{
    public static class ProjectExts
    {
        public static CakeTaskBuilder Task(this IProject project, string name)
        {
            return CakeBridge.Task($"{project.Name}.{name}");
        }
    }
}