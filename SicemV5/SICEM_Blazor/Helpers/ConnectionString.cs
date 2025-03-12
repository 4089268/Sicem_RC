using System;
using System.Text.RegularExpressions;

namespace SICEM_Blazor.Helpers
{
    public class ConnectionString {

        public static string MakeMediaConnectionString(string connectionString){
            // Define the regular expression to match "Initial Catalog" value
            string pattern = @"(Initial Catalog=)([^;]+)";
            Match match = Regex.Match(connectionString, pattern);

            if (match.Success)
            {
                // Extract the value of "Initial Catalog" and append "Media"
                string initialCatalog = match.Groups[2].Value;
                string modifiedCatalog = initialCatalog + "Media";

                // Replace the old "Initial Catalog" value with the modified one
                string modifiedConnectionString = Regex.Replace(connectionString, pattern, $"$1{modifiedCatalog}");
                return modifiedConnectionString;
            }
            else
            {
                throw new ArgumentException("Initial Catalog not found in connection string.");
            }
        }
    }
}