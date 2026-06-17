namespace BackendAPI.DTOs
{
    public class UploadFolderRequestDTO
    {
            public string FolderName { get; set; }
            public string Description { get; set; }
            public IFormFile File { get; set; }
    }
}
