using System.Runtime.Serialization;

namespace Plan_It.Enum{

  public enum Category{
            
        [EnumMember(Value = "Personal")]
        Personal,

        [EnumMember(Value = "Group")]
        Group
    }

     public enum Priority{
        
        [EnumMember(Value = "Important")]
        Important,
        [EnumMember(Value = "UnImportant")]
        UnImportant
    }

      public enum TaskStatus{

        [EnumMember(Value = "Pending")]
        Pending,

        [EnumMember(Value = "Ongoing")]
        Ongoing,

        [EnumMember(Value = "Completed")]
        Completed
    }

     public enum Status
    {
        [EnumMember(Value = "GroupCreator")]
        GroupCreator,

        [EnumMember(Value = "GroupMember")]
        GroupMember
    }
}