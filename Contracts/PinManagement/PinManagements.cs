using Microsoft.AspNetCore.Http;
using SMP.DAL.Models.PinManagement;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SMP.Contracts.PinManagement
{
    public class PrintResultRequest
    {
        public string Pin { get; set; } 
        public string RegistractionNumber { get; set; }
        public Guid SessionClassid;
        public string TermId { get; set; }
    }

    public class BatchPrintResultRequest1
    {
        public Guid SessionClassid { get; set; }
        public Guid TermId { get; set; }
    }

    public class BatchPrintResultRequest2
    {
        public Guid SessionClassId { get; set; }
        public Guid TermId { get; set; }
        public int Students { get; set; }
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

    public class FwsPinValResponseResult
{
        public string pin { get; set; }
        public string studentRegNo { get; set; }
        public object apiKey { get; set; }
        public string clientId { get; set; }
    }

    public class FwsPinValResponse
    {
        public FwsPinValResponseResult result { get; set; }
        public string status { get; set; }
        public Message message { get; set; }
    }

    public class FwsMultiPinValResponse
    {
        public List<FwsPinValResponseResult> result { get; set; }
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
        public int NumberOfTimesRemaining { get; set; } = 0;
        public string RegistrationNumber { get; set; }
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
        public GetPins(IGrouping<Guid, UsedPin> db, string regNoFormat)
        {
            Pin = db.FirstOrDefault().UploadedPin.Pin;
            SerialNumber = db.FirstOrDefault().UploadedPin.Serial;
            NumberOfTimesUsed = db.Count();
            StudentName = db.FirstOrDefault().Student.User.FirstName + " " + db.FirstOrDefault().Student.User.LastName;
            Session = db.FirstOrDefault().SessionClass.Session.StartDate + " / " + db.FirstOrDefault().SessionClass.Session.EndDate;
            Term = db.FirstOrDefault().Sessionterm.TermName + " Term";
            PinStatus = "used";
            SerialNumber = db.FirstOrDefault().UploadedPin.Serial;
            NumberOfTimesRemaining = 3 - db.Count();
            RegistrationNumber = regNoFormat.Replace("%VALUE%", db.FirstOrDefault().Student.RegistrationNumber);
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
        public int NumberOfTimesRemaining { get; set; } = 0;
        public string RegistrationNumber { get; set; }
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
        public PinDetail(IGrouping<Guid, UsedPin> db, string regNoFormat)
        {
            Pin = db.FirstOrDefault().UploadedPin.Pin;
            SerialNumber = db.FirstOrDefault().UploadedPin.Serial;
            NumberOfTimesUsed = db.Count();
            StudentName = db.FirstOrDefault().Student.User.FirstName + " " + db.FirstOrDefault().Student.User.LastName;
            Session = db.FirstOrDefault().SessionClass.Session.StartDate + " / " + db.FirstOrDefault().SessionClass.Session.EndDate;
            Term = db.FirstOrDefault().Sessionterm.TermName + " Term";
            PinStatus = "used";
            SerialNumber = db.FirstOrDefault().UploadedPin.Serial;
            NumberOfTimesRemaining = 3 - db.Count();
            RegistrationNumber = regNoFormat.Replace("%VALUE%", db.FirstOrDefault().Student.RegistrationNumber);
        }
    }

    public class FwsMultiPinOnUploadValResponse
    {
        public PinsValOnUplaodRequest Pins { get; set; }
        public string status { get; set; }
        public Message message { get; set; }
    }

    public class PinsValOnUplaodRequest
    {
        public string ClientId { get; set; }
        public List<PinObject> Pins { get; set; }
    }
    public class PinObject
    {
        public string Pin { get; set; }
        public int ExcelLine { get; set; }
    }

   
}
