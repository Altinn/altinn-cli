namespace AltinnCLI.Commands.Core
{
/// <summary>
/// Interface that defines the propertioes and methods that
/// shall be implemented by an Option class
/// </summary>
public interface IOption : IValidate
{
    /// <summary>
    /// The Name og the oprtion that must match the Name 
    /// of the otion in the CommandDefinition file
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// The name of the opsjon in ther API. 
    /// </summary>
    public string ApiName { get; set; }

    /// <summary>
    /// The value of the option as a string
    /// </summary>
    string Value { get; set; }

    /// <summary>
    ///  Defines if the option is defined with a vlue in
    ///  the command line 
    /// </summary>
    bool IsAssigned { get; set; }

    /// <summary>
    /// The description of the option that will be used by help
    /// </summary>
    string Description { get; set; }

    /// <summary>
    /// Valid range for the paramtere. 
    /// </summary>
    string Range { get; set; }

    /// <summary>
    /// Gets the typed value of the option as defined 
    /// in the option definition
    /// </summary>
    /// <returns></returns>
    object GetValue();

}
}