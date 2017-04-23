namespace QuickCommander.API
{
    public interface IPlugin
    {
        string Name { get; }
        string Description { get; }
        string Author { get; }
        string Version { get; }

        void OnEnable();
        void OnDisable();
    }
}