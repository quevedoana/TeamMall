
using Google.Apis.Auth.OAuth2;
using Google.Apis.Drive.v3;
using Google.Apis.Services;

namespace TeaMall.Services
{
    public class GoogleDriveService
    {
        private readonly DriveService _driveService;
        private readonly string _spreadsheetId;

        public GoogleDriveService(IConfiguration configuration)
        {
            string credentialPath = configuration["GoogleDrive:CredentialPath"]
                ?? throw new InvalidOperationException(
                    "Falta configurar GoogleDrive:CredentialPath.");

            _spreadsheetId = configuration["GoogleDrive:SpreadsheetId"]
                ?? throw new InvalidOperationException(
                    "Falta configurar GoogleDrive:SpreadsheetId.");

            string absolutePath = Path.GetFullPath(credentialPath);

            if (!File.Exists(absolutePath))
            {
                throw new FileNotFoundException(
                    "No se encontró el archivo de credenciales.",
                    absolutePath);
            }

            GoogleCredential credential = GoogleCredential
                .FromFile(absolutePath)
                .CreateScoped(DriveService.Scope.DriveReadonly);

            _driveService = new DriveService(
                new BaseClientService.Initializer
                {
                    HttpClientInitializer = credential,
                    ApplicationName = "TeamMall"
                });
        }

        public async Task<DateTimeOffset?> ObtenerUltimaModificacionAsync()
        {
            var request = _driveService.Files.Get(_spreadsheetId);
            request.Fields = "modifiedTime";

            var archivo = await request.ExecuteAsync();

            return archivo.ModifiedTimeDateTimeOffset;
        }
    }
}
