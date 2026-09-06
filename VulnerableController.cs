using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;

namespace MasterThesisDotNetApp
{
    [ApiController]
    [Route("api/[controller]")]
    public class VulnerableController : ControllerBase
    {
        // PODATNOŚĆ 1: Hardcoded Secret (Twardo zakodowane klucze/hasła)
        // SAST natychmiast powinien to wykryć jako Critical/High Issue.
        private const string AdminApiKey = "AIzaSyD-TEST-KEY-99283112-Xyz7788A";
        private const string ConnectionString = "Server=myServerAddress;Database=myDataBase;User Id=admin;Password=SuperSecretPassword123!;";

        /// <summary>
        /// 1. SQL INJECTION (Podatność na wstrzykiwanie kodu SQL)
        /// </summary>
        [HttpGet("search-users")]
        public IActionResult GetUsers([FromQuery] string searchTerm)
        {
            // BŁĄD SAST: Bezpośrednie łączenie stringów (String Concatenation) zamiast użycia parametrów.
            string query = "SELECT * FROM Users WHERE UserName = '" + searchTerm + "'";

            using (SqlConnection connection = new SqlConnection(ConnectionString))
            {
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    // Kod dla celów SAST (narzędzie analizuje sam przepływ danych, 
                    // wykonanie zapytania nie musi fizycznie działać w bazie).
                    return Ok(new { Message = $"Wykonano podatne zapytanie: {query}" });
                }
            }
        }

        /// <summary>
        /// 2. PATH TRAVERSAL / ARBITRARY FILE READ (Nieautoryzowany dostęp do plików systemowych)
        /// </summary>
        [HttpGet("read-log")]
        public IActionResult ReadLogFile([FromQuery] string fileName)
        {
            // BŁĄD SAST: Brak walidacji i oczyszczania (sanitization) ścieżki.
            // Użytkownik może przekazać: fileName = "../../../etc/passwd" lub "../../../windows/win.ini"
            string baseDir = AppDomain.CurrentDomain.BaseDirectory;
            string fullPath = Path.Combine(baseDir, "Logs", fileName);

            // Narzędzie SAST wykryje, że nie sprawdzono, czy fullPath nie wychodzi poza katalog "Logs"
            if (System.IO.File.Exists(fullPath))
            {
                // Symulacja odczytu
                return Ok(new { FilePath = fullPath, Content = "Zawartość pliku logów..." });
            }

            return NotFound();
        }

        /// <summary>
        /// 3. WEAK CRYPTOGRAPHY (Użycie słabych algorytmów haszujących)
        /// </summary>
        [HttpPost("hash-password")]
        public IActionResult HashPassword([FromQuery] string password)
        {
            // BŁĄD SAST: MD5 jest uznawany za złamany i podatny na kolizje.
            // SAST powinien zgłosić regułę typu "Do not use weak cryptographic algorithms".
            using (System.Security.Cryptography.MD5 md5 = System.Security.Cryptography.MD5.Create())
            {
                byte[] inputBytes = System.Text.Encoding.ASCII.GetBytes(password);
                byte[] hashBytes = md5.ComputeHash(inputBytes);

                string hash = Convert.ToHexString(hashBytes);
                return Ok(new { PasswordHash = hash });
            }
        }
    }
}
