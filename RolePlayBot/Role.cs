using Microsoft.Extensions.Logging;
class Role
{
    public int ID {get; set;} = 0;
    public string Name {get; set;} = string.Empty;
    public string? Description {get; set;}
    private readonly ILogger<Role> RoleLogger;
    public Role()
    {
        RoleLogger = LoggerFactory.Create(builder =>
        {
            builder.AddSimpleConsole(options =>
            {
                options.IncludeScopes = true;
                options.SingleLine = true;
                options.TimestampFormat = "HH:mm:ss ";
            });
            builder.AddFilter("System", LogLevel.Debug).SetMinimumLevel(LogLevel.Information);
        }).CreateLogger<Role>();
    }
    public int AddRole(string ConnectionString)
    {
        RoleLogger.LogInformation("Got new request to register Role {rolename}", this.Name);

        Role? ExistingRole = new Role();
        ExistingRole.Name = this.Name;
        ExistingRole = ExistingRole.GetRole(ConnectionString);

        if(this.Name != string.Empty && ExistingRole is null)
        {
            try
            {
                using (RoleContext db = new RoleContext(ConnectionString))
                {
                    db.Roles.AddRange(this);
                    db.SaveChanges();
                    RoleLogger.LogInformation("Registered new Role: {Rolename}", this.Name);
                    return 1;
                }
            }
            catch(Exception ex)
            {
                RoleLogger.LogError("Couldn't Add New Role: {exception}", ex.Message);
                return -1;
            }
        }
        else
        {
            RoleLogger.LogInformation("Bad request for registering Role - Name is empty or Role already exists");
            return 0;
        }
    }
    public Role? GetRole(string ConnectionString)
    {
        if(this.Name != string.Empty)
        {
            return GetRoleByName(ConnectionString, this.Name);
        }
        if(this.ID != 0 && this.ID > 0)
        {
            return GetRoleByID(ConnectionString, this.ID);
        }
        else
        {
            return null;
        }
    }
    protected Role? GetRoleByID(string ConnectionString, int ID)
    {
        using (RoleContext db = new RoleContext(ConnectionString))
        {
            RoleLogger.LogInformation("Got request to det Role by ID({ID})", ID);
            try
            {
                Role? NeededRole = db.Roles.FirstOrDefault(r => r.ID == ID);
                if(NeededRole is not null)
                {
                    RoleLogger.LogInformation("Found Role Name by ID: {ID} - {Name}", ID, NeededRole.Name);
                    return NeededRole;
                }
                else
                {
                    RoleLogger.LogInformation("Not found Role Name by ID: {ID}", ID);
                    return null;
                }
            }
            catch(Exception ex)
            {
                RoleLogger.LogError(ex.Message);
                return null;
            }
        }
    }
    protected Role? GetRoleByName(string ConnectionString, string Name)
    {
        using (RoleContext db = new RoleContext(ConnectionString))
        {
            RoleLogger.LogInformation("Got request to det Role by Name({Name})", Name);
            try
            {
                Role? NeededRole = db.Roles.FirstOrDefault(r => r.Name == Name);
                if(NeededRole is not null)
                {
                    RoleLogger.LogInformation("Found Role by Name: {ID} - {Name}", NeededRole.ID, NeededRole.Name);
                    return NeededRole;
                }
                else
                {
                    RoleLogger.LogInformation("Not found Role by Name: {Name}", Name);
                    return null;
                }
            }
            catch(Exception ex)
            {
                RoleLogger.LogError(ex.Message);
                return null;
            }
        }
    }
}