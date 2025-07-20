namespace Dot.Tools
{
    public interface IAgentProvider
    {
        IAgent GetAgent<T>() where T : IAgent;
    }

    public class AgentProvider : IAgentProvider
    {
        private readonly IEnumerable<IAgent> _agents;
        public AgentProvider(IEnumerable<IAgent> agents)
        {
            _agents = agents;
        }
        public IAgent GetAgent<T>() where T : IAgent
        {
            return _agents.FirstOrDefault(agent => agent.GetType() == typeof(T)) ?? throw new InvalidOperationException($"Agent of type {typeof(T).Name} not found.");
        }
    }
}
