abstract class Character
{
    public string Name {get; set;} = string.Empty;
    public int? Age {get; set;}
    public long TelegramID {get; set;} = 0;
    public int RoleID {get; set;} = 0;
}