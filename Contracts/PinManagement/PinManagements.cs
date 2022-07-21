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
        public IFormFile File { get; set; }
    }
 
    public class GetUploadPinRequest
    { 
        public string Pin;
        public long SerialNumber;
        public int PinCount;
        public string Session;
        public string StudentName;
        public string TermPrinted;

        public GetUploadPinRequest(UploadedPin db)
        {
            List<UploadedPin> pins = new() ;
            foreach(var pin in pins)
            {
                //SerialNumber = db.SerialNumber;
                Pin = pin.Pin;
                PinCount = pins.Where(x => x.Pin == pin.Pin).Count();
                
            }
        }
        public GetUploadPinRequest(UploadedPin db, SessionTerm session)
        { 
            List<UploadedPin> pins  = new() {db};
            
             Pin =  db.Pin;
            StudentName = $"{db.UsedPin.FirstOrDefault(x=>x.UploadedPin.Pin == db.Pin).Student.User.LastName}" + $"{db.UsedPin.FirstOrDefault(x => x.UploadedPin.Pin == db.Pin).Student.User.FirstName}";
            Session = $"{session.Session.StartDate}" +"/" +  $"{session.Session.EndDate}";
            PinCount = pins.Select(x => new UploadedPin().Pin).ToList().Count;
            TermPrinted = session.TermName;
            
        }
    } 
    public class GetUsedPinRequest
    { 
        public string Pin;
        public long SerialNumber;
        public int PinCount;
        public string StudentName;
        public string Session;
        public string TermPrinted;

        public GetUsedPinRequest(UsedPin db)
        {
            List<UsedPin> pins = new();
            //SerialNumber = db.SerialNumber;
            Pin = db.UploadedPin.Pin; 
            PinCount = pins.Where(x => x.UsedPinId == db.UsedPinId).Count(); 
        }
        public GetUsedPinRequest(UsedPin db, SessionTerm session)
        { 
            List<UsedPin> pins = new() {db};
            Pin = db.UploadedPin.Pin;
            //SerialNumber = db.UploadedPin.SerialNumber;
            StudentName = $"{db.UploadedPin.UsedPin.FirstOrDefault().Student.User.LastName}" + $"{db.UploadedPin.UsedPin.FirstOrDefault().Student.User.FirstName}";
            Session = $"{session.Session.StartDate}" + "/" + $"{session.Session.EndDate}";
            PinCount = pins.Select(x=>new UsedPin().UploadedPin.UsedPin.Any()).Count();
            TermPrinted = session.TermName;
        }
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

  

}
