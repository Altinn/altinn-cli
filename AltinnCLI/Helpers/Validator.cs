namespace AltinnCLI.Helpers
{
    public static class Validator
    {
        public static bool IsValidOrganizationNumber(string orgNo)
        {
            int[] weight = { 3, 2, 7, 6, 5, 4, 3, 2 };

            // Validation only done for 9 digit numbers
            if (orgNo.Length == 9)
            {
                try
                {
                    int currentDigit = 0;
                    int sum = 0;
                    for (int i = 0; i < orgNo.Length - 1; i++)
                    {
                        currentDigit = int.Parse(orgNo.Substring(i, 1));
                        sum += currentDigit * weight[i];
                    }

                    int ctrlDigit = 11 - (sum % 11);
                    if (ctrlDigit == 11)
                    {
                        ctrlDigit = 0;
                    }

                    return int.Parse(orgNo.Substring(orgNo.Length - 1)) == ctrlDigit;
                }
                catch
                {
                    return false;
                }
            }

            return false;
        }

        /// <summary>
        /// Validates that a given social security number is valid.
        /// </summary>
        /// <param name="ssnNo">
        /// Social security number to validate
        /// </param>
        /// <returns>
        /// true if valid, false otherwise.
        /// </returns>
        /// <remarks>
        /// Validates length, numeric and modulus 11.
        /// </remarks>
        public static bool IsValidPersonNumber(string ssnNo)
        {
            int[] weightDigit10 = { 3, 7, 6, 1, 8, 9, 4, 5, 2 };
            int[] weightDigit11 = { 5, 4, 3, 2, 7, 6, 5, 4, 3, 2 };

            // Validation only done for 11 digit numbers
            if (ssnNo.Length == 11)
            {
                try
                {
                    int currentDigit = 0;
                    int sumCtrlDigit10 = 0;
                    int sumCtrlDigit11 = 0;
                    int ctrlDigit10 = -1;
                    int ctrlDigit11 = -1;

                    // Calculate control digits
                    for (int i = 0; i < 9; i++)
                    {
                        currentDigit = int.Parse(ssnNo.Substring(i, 1));
                        sumCtrlDigit10 += currentDigit * weightDigit10[i];
                        sumCtrlDigit11 += currentDigit * weightDigit11[i];
                    }

                    ctrlDigit10 = 11 - (sumCtrlDigit10 % 11);
                    if (ctrlDigit10 == 11)
                    {
                        ctrlDigit10 = 0;
                    }

                    sumCtrlDigit11 += ctrlDigit10 * weightDigit11[9];
                    ctrlDigit11 = 11 - (sumCtrlDigit11 % 11);
                    if (ctrlDigit11 == 11)
                    {
                        ctrlDigit11 = 0;
                    }

                    // Validate control digits in ssn
                    bool digit10Valid = ctrlDigit10 == int.Parse(ssnNo.Substring(9, 1));
                    bool digit11Valid = ctrlDigit11 == int.Parse(ssnNo.Substring(10, 1));
                    return digit10Valid && digit11Valid;
                }
                catch
                {
                    return false;
                }
            }

            return false;
        }
    }
}
