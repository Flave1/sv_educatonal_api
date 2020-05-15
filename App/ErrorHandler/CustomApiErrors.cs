using System.Net;

namespace App.CustomError
{
    public class InternalServerErrors : BaseApiError
    {
        public InternalServerErrors() : base(500, HttpStatusCode.InternalServerError.ToString()) { }
        public InternalServerErrors(string T_message) : base(500, HttpStatusCode.InternalServerError.ToString(), T_message) { }
        public InternalServerErrors(string F_message, string T_message) : base(500, HttpStatusCode.InternalServerError.ToString(), F_message, T_message) { }
    }
    public class NotImplementedErrors : BaseApiError
    {
        public NotImplementedErrors() : base(501, HttpStatusCode.NotImplemented.ToString()) { }
        public NotImplementedErrors(string T_message) : base(501, HttpStatusCode.NotImplemented.ToString(), T_message) { }
        public NotImplementedErrors(string F_message, string T_message) : base(501, HttpStatusCode.NotImplemented.ToString(), F_message, T_message) { }
    }
    public class NotFoundError : BaseApiError
    {
        public NotFoundError() : base(404, HttpStatusCode.NotFound.ToString()) { }
        public NotFoundError(string T_message) : base(404, HttpStatusCode.NotFound.ToString(), T_message) { }
        public NotFoundError(string F_message, string T_message) : base(404, HttpStatusCode.NotFound.ToString(), F_message, T_message) { }
    }

    public class NoContentError : BaseApiError
    {
        public NoContentError() : base(204, HttpStatusCode.NoContent.ToString()) { }
        public NoContentError(string message) : base(204, HttpStatusCode.NoContent.ToString(), message) { }
    }


    public class OKMessage : BaseApiError
    {
        public OKMessage() : base(200, HttpStatusCode.OK.ToString()) { }
        public OKMessage(string message) : base(200, HttpStatusCode.OK.ToString(), message) { }
    }

    public class BadRequestError : BaseApiError
    {
        public BadRequestError() : base(400, HttpStatusCode.BadRequest.ToString()) { }
        public BadRequestError(string F_message) : base(400, HttpStatusCode.BadRequest.ToString(), F_message) { }
        public BadRequestError(string F_message, string T_message) : base(400, HttpStatusCode.BadRequest.ToString(), F_message, T_message) { }
    }

    public class ForBiddenError : BaseApiError
    {
        public ForBiddenError() : base(403, HttpStatusCode.Forbidden.ToString()) { }
        public ForBiddenError(string F_message) : base(403, HttpStatusCode.Forbidden.ToString(), F_message) { }
        public ForBiddenError(string F_message, string T_message) : base(403, HttpStatusCode.Forbidden.ToString(), F_message, T_message) { }
    }


    public class UnauthorizedError : BaseApiError
    {
        public UnauthorizedError() : base(401, HttpStatusCode.Unauthorized.ToString()) { }
        public UnauthorizedError(string message) : base(401, HttpStatusCode.Unauthorized.ToString(), message) { }
    }

    public class HttpRequestExceptionError : BaseApiError
    {
        public HttpRequestExceptionError() : base(408, HttpStatusCode.RequestTimeout.ToString()) { }
        public HttpRequestExceptionError(string F_message) : base(408, HttpStatusCode.RequestTimeout.ToString(), F_message) { }
        public HttpRequestExceptionError(string F_message, string T_message) : base(408, HttpStatusCode.RequestTimeout.ToString(), F_message, T_message) { }
    }
}
