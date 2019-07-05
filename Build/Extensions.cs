using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cake.Core.IO;

namespace Build
{
    public static class Extensions
    {
        public static DirectoryPath Combine(this DirectoryPath path1, string path2)
        {
            return path1.Combine(DirectoryPath.FromString(path2));
        }
    }
}