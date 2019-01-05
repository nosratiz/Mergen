using System.Collections.Generic;
using System.IO;

namespace Mergen.Core.Services
{
    public interface IImageProcessingService
    {
        Stream Combine(IEnumerable<Stream> files);
    }
}