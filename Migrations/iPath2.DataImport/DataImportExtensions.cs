using iPath.Data;
using iPath.Data.Entities;
using iPath.Data.Helper;
using System.Xml;

namespace iPath2.DataImport;

public static class DataImportExtensions
{
    public static Dictionary<int, Community> communityCache;
    public static Dictionary<int, string> usernameCache;
    public static Dictionary<int, string> groupNameCache;


    private static UserDTO getOwner(int? id)
    {
        if( id.HasValue && usernameCache.ContainsKey(id.Value) )
        {
            return new UserDTO { UserId = id.Value, Username = usernameCache[id.Value] };
        }
        return null;
    }


    public static Node ToNewEntity(this i2object o)
    {
        var n = new Node();
        n.Id = o.id;
        n.CreatedOn = o.entered.ToUniversalTime();
        n.NodeType = o.objclass ?? "default";

        n.OwnerId = o.sender_id.Value;
        n.GroupId = o.group_id;

        n.RootNodeId = o.topparent_id;
        n.ParentNodeId = o.parent_id;
        n.SortNr = o.sort_nr;             

        // old data and info fields
        n.ImportedData ??= new();
        n.ImportedData.Data = o.data;
        n.ImportedData.Info = o.info;
        n.LastModifiedOn = o.modified.HasValue ? o.modified.Value.ToUniversalTime() : null;

        try
        {
            var xml = LoadDataDocument(o.data);

            if (xml.SelectSingleNode("/data/title") != null)
            {
                n.Description.Title = xml.SelectSingleNode("/data/title").InnerText;
                n.Description.Subtitle = xml.SelectSingleNode("/data/subtitle")?.InnerText;
                n.Description.CaseType = xml.SelectSingleNode("/data/type")?.InnerText;
                n.Description.AccessionNo = xml.SelectSingleNode("/data/speciment_code")?.InnerText;
                n.Description.Status = xml.SelectSingleNode("/data/casestat")?.InnerText;
                n.Description.Text = xml.SelectSingleNode("/data/description")?.InnerText;
            }


            if (xml.SelectSingleNode("/data/filename") != null)
            {
                n.File ??= new();
                n.File.Filename = xml.SelectSingleNode("/data/filename").InnerText;
                n.File.MimeType = xml.SelectSingleNode("/data/mimetype")?.InnerText;
            }

        }
        catch (Exception ex)
        {
            throw ex;
        }

        return n;
    }




    public static Annotation ToNewEntity(this i2annotation a)
    {
        var n = new Annotation();
        n.Id = a.id;
        n.CreatedOn = a.entered.ToUniversalTime();
        n.OwnerId = a.sender_id;
        n.NodeId = a.object_id;             

        try
        {
            var xml = LoadDataDocument(a.data);

            if (xml.SelectSingleNode("/data/text") != null)
            {
                n.Text = xml.SelectSingleNode("/data/text").InnerText;
            }
        }
        catch (Exception ex)
        {
            throw ex;
        }

        return n;
    }

    public static UserDTO ToDto(this User p)
    {
        return new UserDTO() { UserId = p.Id, Username = p.Username };
    }


    public static User ToNewEntity(this i2person p)
    {
        var u = new User();
        u.Id = p.id;

        u.Username = p.username;
        u.UsernameInvariant = p.username.ToLowerInvariant().Normalize().Trim();

        u.Email = p.email;
        u.EmailInvariant = p.email.ToLowerInvariant().Normalize().Trim();

        u.PasswordHash = "-"; 
        u.iPath2Password = p.password;

        u.CreatedOn = p.entered.Value.ToUniversalTime();

        try
        {
            var xml = LoadDataDocument(p.data);

            u.Profile ??= new();
            // userid and username are duplicated to profile for easy access in serialized json files
            u.Profile.UserId = p.id;
            u.Profile.Username = p.username;

            u.Profile.FirstName = xml.SelectSingleNode("/data/firstname")?.InnerText;
            u.Profile.FamilyName = xml.SelectSingleNode("/data/name")?.InnerText;
            u.Profile.Specialisation = xml.SelectSingleNode("/data/specialisation")?.InnerText;


            if(!string.IsNullOrEmpty(u.Profile.FirstName))
            {
                u.Profile.Initials = u.Profile.FirstName.Substring(0, 1);
                u.Profile.Initials += string.IsNullOrEmpty(u.Profile.FamilyName) ? "" : u.Profile.FamilyName.Substring(0, 1);
            }
            else
            {
                u.Profile.Initials = u.Username.Substring(0, 1);
            }

            u.Profile.ContactDetails ??= new List<ContactDetails>();
            var cd = new ContactDetails();
            cd.IsMainContact = true;
            u.Profile.ContactDetails.Add(cd);

            cd.Organisation = xml.SelectSingleNode("/data/institute")?.InnerText;
            cd.PhoneNr = xml.SelectSingleNode("/data/phone")?.InnerText;
            cd.Email = p.email; // email is duplicated to contact details. maybe be different from email on user account

            cd.Address ??= new();
            cd.Address.Street = xml.SelectSingleNode("/data/street")?.InnerText;
            cd.Address.PostalCode = xml.SelectSingleNode("/data/zip")?.InnerText;
            cd.Address.City = xml.SelectSingleNode("/data/city")?.InnerText;
            cd.Address.Country = xml.SelectSingleNode("/data/country")?.InnerText;
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error in XML: " + p.data);
        }

        return u;
    }



    public static Community ToNewEntity(this i2community o)
    {
        var n = new Community();
        n.Id = o.id;
        n.Name = o.name;
        n.OwnerId = o.created_by.HasValue ? o.created_by.Value : 1;
        n.CreatedOn = o.created_on.ToUniversalTime();
        return n;
    }



    public static Group ToNewEntity(this i2group g)
    {
        var n = new Group();
        n.Id = g.id;
        n.Name = g.name;
        n.CreatedOn = g.entered.ToUniversalTime();

        // communities
        foreach (var cg in g.communities)
        {
            n.Communities.Add(new CommunityGroup { Group = n, CommunityId = cg.community_id });
        }

        try
        {
            if (!string.IsNullOrEmpty(g.info) && g.info.Contains("&"))
            {
               g.info = System.Text.RegularExpressions.Regex.Replace(g.info, "&(?!amp;)", "&amp;");
            }

            var xml = LoadDataDocument(g.info);

            n.Settings ??= new();
            n.Settings.Purpose = xml.SelectSingleNode("/info/purpose")?.InnerText;
            n.Settings.DefaultText = xml.SelectSingleNode("/info/default_text")?.InnerText;
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error in XML: " + g.info);
        }

        return n;
    }


    public static XmlDocument LoadDataDocument(string data)
    {
        try
        {
            if (!string.IsNullOrEmpty(data))
            {
                if (data.Contains("&"))
                {
                    data = System.Text.RegularExpressions.Regex.Replace(data, "&(?!amp;)", "&amp;");
                }

                var xml = new XmlDocument();
                xml.LoadXml(data);
                return xml;
            }
        }
        catch(Exception ex) 
        { 
            Console.WriteLine(ex.ToString()); 
        }

        return new XmlDocument();
    }
}
