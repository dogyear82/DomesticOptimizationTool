using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dot.Services.Tools
{
    public interface IToolFactory
    {
        ITool GetToolByName(string toolName);
    }
    public class ToolFactory : IToolFactory
    {
        public ITool GetToolByName(string toolName)
        {
            throw new NotImplementedException();
        }
    }
}
