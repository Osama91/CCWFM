using System;
using System.Collections.Generic;
using System.DirectoryServices;
using System.DirectoryServices.AccountManagement;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Activation;

namespace CCWFM.Web.Service
{
    [ServiceContract(Namespace = "")]
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]

    public class UserDomains
    {
        [OperationContract]
        public List<Users> GetADUsers()
        {
            try
            {
                List<Users> lstADUsers = new List<Users>();
                //  string DomainPath = "LDAP://DC=ccasual.loc,DC=com";
                string DomainPath = "LDAP://DC=ccasual,DC=loc";
                PrincipalContext ctx = new PrincipalContext(ContextType.Domain);
                DirectoryEntry searchRoot = new DirectoryEntry(DomainPath);
                DirectorySearcher search = new DirectorySearcher(searchRoot);
                search.Filter = "(&(objectClass=user)(objectCategory=person))";
                search.PropertiesToLoad.Add("samaccountname");
                search.PropertiesToLoad.Add("mail");
                search.PropertiesToLoad.Add("usergroup");
                search.PropertiesToLoad.Add("displayname");//first name
                SearchResult result;
                SearchResultCollection resultCol = search.FindAll();
                if (resultCol != null)
                {
                    for (int counter = 0; counter < resultCol.Count; counter++)
                    {
                        string UserNameEmailString = string.Empty;
                        result = resultCol[counter];
                        if (result.Properties.Contains("samaccountname") &&
                                 result.Properties.Contains("mail") &&
                            result.Properties.Contains("displayname"))
                        {
                            Users objSurveyUsers = new Users();
                            objSurveyUsers.Email = (String)result.Properties["mail"][0] +
                              "^" + (String)result.Properties["displayname"][0];
                            objSurveyUsers.UserName = (String)result.Properties["samaccountname"][0];
                            objSurveyUsers.DisplayName = (String)result.Properties["displayname"][0];

                            //SID
                            try
                            {
                                UserPrincipal user = UserPrincipal.FindByIdentity(ctx, objSurveyUsers.UserName);
                                objSurveyUsers.SID = user.Sid.ToString();
                            }
                            catch { }
                            lstADUsers.Add(objSurveyUsers);
                        }
                    }
                }
                return lstADUsers;
            }
            catch (Exception ex)
            {
                return null;
            }
        }


        // Add more operations here and mark them with [OperationContract]
    }
}
