
class Program
{
    static async Task Main(string[] args)
    {
        const string nasUrl = "http://10.78.43.8:5000";
        const string username = "Dmitry22_s";
        const string password = ",2H[oOQS"; //QTiB0LJt
        const string targetFolder = "/质量部/16-无损检测资料 NDT （заявка и заключение НК и РК）";
        const string downloadsFolder = @"C:\Users\User\Downloads\1";
        HashSet<string> documentNumbers = new()  //file names contain these numbers
        {
            "NGS-VT-PI-ECU-6469",
            "NGS-VT-PI-ECU-64691",
            "AKS-VT-PI-ECU-0741-0571",
            "AKS-VT-PI-ECU-0741-0573",
            "NGS-RT-PI-UIO-PAUT-0006",
            "NGS-VT-PI-UIO-1166",
            "NGS-VT-EL-UG-0123",
            "NGS-VT-EL-UG-1123",
            "NGS-RT-PI-UIO-7123",
            "VT-T40B-0121 0123",
            "VT-CS-UIO-0126_0127 PT-CS-UIO-0122_0123",
            "TT 064 М-КРАН-10123",
            "AKS-VT-PI-WS-ECU-0001-45276"
        };

        SynologyNas.NasClient nasClient = new(nasUrl, username, password);

        //await nasClient.IndexFolderAsync(targetFolder, @"C:\Users\User\Downloads");

        // string? apiInfo = await nasClient.GetApiInfoAsync();
        //bool targetFolderExist = await nasClient.CheckFolderExistsAsync(targetFolder);
        // string? sharedFolders = await nasClient.GetListSharedFoldersAsync();

        Dictionary<string, string>? pdfFilesToDownload = await nasClient.SearchPdfFilesAsync(targetFolder, documentNumbers);
        Dictionary<string, List<string>>? sigFilesToDownload = await nasClient.SearchSigFilesAsync(targetFolder, documentNumbers);

        HashSet<string> allFilesToDownload = new();

        if (pdfFilesToDownload != null)
        {
            foreach (var file in pdfFilesToDownload)
            {
                allFilesToDownload.Add(file.Value);
            }
        }

        if (sigFilesToDownload != null)
        {
            foreach (var file in sigFilesToDownload)
            {
                foreach (string filePath in file.Value)
                {
                    allFilesToDownload.Add(filePath);
                }
            }
        }
        await nasClient.DownloadFilesAsync(allFilesToDownload, downloadsFolder);
    }
}   