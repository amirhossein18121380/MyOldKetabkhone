namespace DataModel.ViewModel.Common
{
    public class PagedResponse<T> : Response<T>
    {
        public long TotalRecords { get; set; }

        public PagedResponse(T data, long totalRecords)
        {
            Data = data;
            TotalRecords = totalRecords;
        }
    }

    public class Response<T>
    {
        public Response()
        {
        }

        public Response(T data)
        {
            Data = data;
        }
        public T Data { get; set; }
    }
}
