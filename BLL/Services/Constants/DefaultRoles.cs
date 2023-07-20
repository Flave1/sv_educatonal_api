namespace BLL.Constants
{
    public static class DefaultRoles
    {
        public const string FLAVETECH = "FLAVETECH";
        public const string SCHOOLADMIN = "SCHOOL_ADMIN"; 
        public const string STUDENT = "STUDENT";
        public const string TEACHER = "TEACHER";
        public const string PARENTS = "PARENTS";

        public static string AdminRole(string client)
        {
            return SCHOOLADMIN + client;
        }
        public static string TeacherRole(string client)
        {
            return TEACHER + client;
        }
        public static string StudentRole(string client)
        {
            return STUDENT + client;
        }
        public static string ParentRole(string client)
        {
            return PARENTS + client;
        }
    }
 
}
