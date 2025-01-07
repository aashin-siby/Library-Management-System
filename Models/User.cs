namespace LibraryManagementSystem.Models     
{
     public class User 
     {
          public string? Username {get; set;}
          public string? Password {get; set;}
          public int RoleId {get; set;}
          public DateTime CreatedAt { get; set; }

     }
}