using Microsoft.AspNetCore.Http;
using SMP.DAL.Models.PinManagement;
using SMP.DAL.Models.PortalSettings;
using SMP.DAL.Models.SessionEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMP.Contracts.PinManagement
{
    public class PrintResultRequest
    {
        public string Pin { get; set; } 
        public string RegistractionNumber { get; set; }
        public Guid SessionClassid;
        public string TermId { get; set; }
    }

    public class UploadPinRequest
    {
        public string Pin;
        public int ExcelLineNumber;
        public string Serial;
        public IFormFile File { get; set; }
    }

    public class FwsPinValidationRequest
    {
        public string Pin { get; set; }
        public string StudentRegNo { get; set; }
        public string ClientId { get; set; }
    }

    public class Message
    {
        public string friendlyMessage { get; set; }
        public object technicalMessage { get; set; }
    }

    public class FwsResponseResult
{
        public string pin { get; set; }
        public string studentRegNo { get; set; }
        public object apiKey { get; set; }
        public string clientId { get; set; }
    }

    public class FwsResponse
    {
        public FwsResponseResult result { get; set; }
        public string status { get; set; }
        public Message message { get; set; }
    }


    public class GetPins
    {
        public string Pin { get; set; }
        public string SerialNumber { get; set; }
        public int NumberOfTimesUsed { get; set; }
        public string StudentName { get; set; }
        public string Session { get; set; }
        public string Term { get; set; }
        public string PinStatus { get; set; }
        public GetPins(UploadedPin db)
        {
            Pin = db.Pin;
            SerialNumber = db.Serial;
            NumberOfTimesUsed = 3;
            StudentName = "unused";
            Session = "unused";
            Term = "unused";
            PinStatus = "unused";
            SerialNumber = db.Serial;
        }
        public GetPins(IGrouping<Guid, UsedPin> db)
        {
            Pin = db.FirstOrDefault().UploadedPin.Pin;
            SerialNumber = db.FirstOrDefault().UploadedPin.Serial;
            NumberOfTimesUsed = 3 - db.Count();
            StudentName = db.FirstOrDefault().Student.User.FirstName + " " + db.FirstOrDefault().Student.User.LastName;
            Session = db.FirstOrDefault().SessionClass.Session.StartDate + " / " + db.FirstOrDefault().SessionClass.Session.EndDate;
            Term = db.FirstOrDefault().Sessionterm.TermName + " Term";
            PinStatus = "used";
            SerialNumber = db.FirstOrDefault().UploadedPin.Serial;
        }
    }

    public class PinDetail
    {
        public string Pin { get; set; }
        public string SerialNumber { get; set; }
        public int NumberOfTimesUsed { get; set; }
        public string StudentName { get; set; }
        public string Session { get; set; }
        public string Term { get; set; }
        public string PinStatus { get; set; }
        public PinDetail(UploadedPin db)
        {
            Pin = db.Pin;
            SerialNumber = db.Serial;
            NumberOfTimesUsed = 0;
            StudentName = "unused";
            Session = "unused";
            Term = "unused";
            SerialNumber = db.Serial;
        }
        public PinDetail(IGrouping<Guid, UsedPin> db)
        {
            Pin = db.FirstOrDefault().UploadedPin.Pin;
            SerialNumber = db.FirstOrDefault().UploadedPin.Serial;
            NumberOfTimesUsed = db.Count();
            StudentName = db.FirstOrDefault().Student.User.FirstName + " " + db.FirstOrDefault().Student.User.LastName;
            Session = db.FirstOrDefault().SessionClass.Session.StartDate + " / " + db.FirstOrDefault().SessionClass.Session.EndDate;
            Term = db.FirstOrDefault().Sessionterm.TermName + " Term";
            PinStatus = "used";
            SerialNumber = db.FirstOrDefault().UploadedPin.Serial;
        }
    }


}
