namespace AltinnCLI.Commands.Core
{
    public interface IValidate
    {
        public bool Validate();

        public bool IsValid { get; set; }

        public string ErrorMessage { get; set; }

    }
}
