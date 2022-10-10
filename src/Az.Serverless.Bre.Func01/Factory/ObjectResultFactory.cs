

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.Net.Http.Headers;

namespace Az.Serverless.Bre.Func01.Factory
{
    public static class ObjectResultFactory
    {
        public static ObjectResult Create(int statusCode, string contentType, string message)
        {

            return new ObjectResult(message)
            {
                StatusCode = statusCode,
                ContentTypes = new MediaTypeCollection
                {
                    new MediaTypeHeaderValue(contentType)

                }
            };
        }
    }
}
