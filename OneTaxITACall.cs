using System;
using Microsoft.IT.Security.CorporatePermissions.Client;
using Microsoft.IT.Security.CorporatePermissions.DataEntities;
using System.Collections.Generic;
using System.Configuration;
using System.Collections;


namespace OneTaxITAAPI
{
    class OneTaxITAAPICall
    {
        private static string accountName;
        private static string companyCode;
        private static string countryName;
        private static string roleName = null;
        private static bool isUserExists = false;

        static void Main(string[] args)
        {
            string applicationName = "OneTax";
            int n = args.Length;
            if (n == 0)
            {
                System.Console.WriteLine("Please enter a valid alias, country/company code,role.");
                System.Console.WriteLine("Usage: OneTax <alias>,<CompanyCode>,<CountryName>,<RoleName>");
            }
            else
            {
                accountName = args[0];
                roleName = args[3];
                isUserExists = bool.Parse(args[4]);
                var client = new CorporatePermissionsServiceClient();

                Microsoft.IT.Security.CorporatePermissions.DataEntities.RoleKey myRoleKey = new Microsoft.IT.Security.CorporatePermissions.DataEntities.RoleKey { ApplicationName = applicationName, RoleName = roleName };
                UserKey userKey = new UserKey { RealmName = "corp.microsoft.com", AccountName = accountName };

                ScopeValuesCollection scopeValues = new ScopeValuesCollection();
                ScopeValue value = new ScopeValue();

                var companyCodeString = args[1];
                if (companyCodeString != "")
                {
                    ScopeValue companyCodeScope = new ScopeValue
                    {
                        ScopeTypeName = "OneTaxCC",
                        Values = new List<string>()
                    };
                    companyCodeScope.Values.AddRange(companyCodeString.Split(','));
                    scopeValues.Add(companyCodeScope);
                }

                var countryNameString = args[2];
                if (countryNameString != "")
                {
                    ScopeValue countryNameScope = new ScopeValue
                    {
                        ScopeTypeName = "OneTaxCountries",
                        Values = new List<string>()
                    };

                    countryNameScope.Values.AddRange(countryNameString.Split(','));
                    scopeValues.Add(countryNameScope);
                }

                RoleAssignmentCollection roleAssignments = new RoleAssignmentCollection();
                if (scopeValues != null)
                {
                    roleAssignments.Add(new RoleAssignment
                    {
                        UtcStartDateTime = DateTime.UtcNow,
                        RoleKey = myRoleKey,
                        Scopes = scopeValues
                    });
                }
                else
                {
                    roleAssignments.Add(new RoleAssignment
                    {
                        UtcStartDateTime = DateTime.UtcNow,
                        RoleKey = myRoleKey
                    });
                }
                //API call.                
                if (isUserExists)
                {
                    client.UnassignRolesFromUser(roleAssignments, userKey);
                    client.AssignRolesToUser(roleAssignments, userKey);
                }
                else
                {
                    client.AssignRolesToUser(roleAssignments, userKey);
                }

            } //end of else
        }
    }
}
