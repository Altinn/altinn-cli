namespace AltinnCLI.Commands.Core
{
    public interface IHelp
    {
        string Name { get; }

        string Description { get; }

        string Usage { get; }

        string GetHelp();
    }
}
