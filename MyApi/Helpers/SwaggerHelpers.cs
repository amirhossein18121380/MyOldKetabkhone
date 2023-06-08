using NJsonSchema;
using NSwag;
using NSwag.Annotations;
using NSwag.Generation.Processors;
using NSwag.Generation.Processors.Contexts;

namespace Helpers
{
    public class RegisterUploadAttribute : OpenApiOperationProcessorAttribute
    {
        public RegisterUploadAttribute() : base(typeof(FileUploadOperation))
        {
        }
    }
    public class FileUploadOperation : IOperationProcessor
    {
        public bool Process(OperationProcessorContext context)
        {
            var operation = context.OperationDescription.Operation;
            operation.Parameters.Clear();

            operation.Parameters.Add(new OpenApiParameter()
            {
                Name = "userImage",
                Kind = OpenApiParameterKind.FormData,
                IsRequired = true,
                Type = JsonObjectType.File,
                Description = "User Image"
            });

            operation.Consumes.Add("multipart/form-data");

            return true;
            //throw new System.NotImplementedException();
        }
    }
}