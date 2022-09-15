using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMP.BLL.Constants
{
    public static class Messages
    {
        public const string Created = "Successfully created";
        public const string Saved = "Successfully Saved";
        public const string Updated = "Successfully updated";
        public const string GetSuccess = "Fetched successfully";
        public const string DeletedSuccess = "Deleted successfully";
        public const string FriendlyException = "(Error: 500 Occurred!!): Please contact administrator";
        public const string ClassTransitionException = "(Error: 5111 Occurred!!): Error thrown tryiing to transition student to a new class";
        public const string FriendlyNOTFOUND = "Item not found!! Unable to locate selected item";
        public const string FriendlyForbidden = "You do not have enough permission to perform operation";
        public const string Unreachable = "Some technical errors has occured between this service and fws";
    }
}
