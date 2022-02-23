using System;
using System.Collections.Generic;

namespace AltinnCLI.Commands.Core
{
    public class NumberOption<T> : Option<T>
    {
        public override bool Validate()
        {
            try
            {
                if (EqualityComparer<T>.Default.Equals(TryParse(Value)))
                {
                    IsValid = false;
                    ErrorMessage = $"The value for Option :{Name} is not on correct format.\n";
                    return false;
                }
                else if (CheckRange() == false)
                {
                    IsValid = false;
                    ErrorMessage = $"The value for Option :{Name} is not out of range. Valid range is {Range} \n";
                }

                isValid = true;
            }
            catch (Exception ex)
            {
                IsValid = false;
                ErrorMessage = $"Invalid Parameter value for paramter: <{Name}>,  {ex.Message}";
                return false;
            }
            return true;
        }

        protected override bool CheckRange()
        {
            return true;
        }
    }
}
