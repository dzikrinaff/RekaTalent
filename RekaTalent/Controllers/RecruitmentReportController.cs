using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PdfSharpCore.Drawing;
using PdfSharpCore.Pdf;
using RekaTalent.Data;
using RekaTalent.Models;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace RekaTalent.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RecruitmentReportController : ControllerBase
    {
        private readonly RekaTalentDbContext _context;

        public RecruitmentReportController(RekaTalentDbContext context)
        {
            _context = context;
        }

        [HttpGet("export/all")]
        public async Task<IActionResult> ExportAllToPdf()
        {
            // Ambil data rekrutmen dengan melakukan left join untuk memastikan setiap kandidat ditampilkan
            var reports = await (from c in _context.Candidates
                                 join p in _context.Psychotests on c.Id equals p.CandidateId into ps
                                 from p in ps.DefaultIfEmpty()  // Left join untuk tabel Psychotests
                                 join i in _context.Interviews on c.Id equals i.CandidateId into isv
                                 from i in isv.DefaultIfEmpty()  // Left join untuk tabel Interviews
                                 select new RecruitmentReport
                                 {
                                     CandidateId = c.Id,
                                     CandidateName = c.Name,
                                     Position = c.Position,
                                     PsychotestScore = p != null ? p.Score : 0, // Nilai psikotes, jika tidak ada gunakan nilai default 0
                                     PsychotestResult = p != null && p.Score >= 75 ? "Passed" : "Failed", // Hasil psikotes
                                     Interviewer = i != null ? i.InterviewName : "N/A" // Nama wawancara, jika tidak ada gunakan "N/A"
                                 }).ToListAsync();

            if (reports == null || !reports.Any())
            {
                return NotFound("Tidak ada laporan rekrutmen yang tersedia.");
            }

            // Generate PDF untuk semua laporan
            var pdfBytes = GeneratePdfForAllReports(reports);

            // Mengembalikan PDF sebagai response
            return File(pdfBytes, "application/pdf", "Semua_LaporanRekrutmen.pdf");
        }

        // Fungsi untuk membuat PDF dari daftar laporan
        private byte[] GeneratePdfForAllReports(List<RecruitmentReport> reports)
        {
            using (var memoryStream = new MemoryStream())
            {
                var document = new PdfDocument();
                var fontTitle = new XFont("Arial", 12, XFontStyle.Bold);
                var fontContent = new XFont("Arial", 10, XFontStyle.Regular);

                // Pengaturan layout PDF
                int reportsPerRow = 2; // Jumlah laporan per baris
                int reportWidth = 250;  // Lebar tiap kotak laporan
                int reportHeight = 180; // Tinggi tiap kotak laporan
                int marginLeft = 40;    // Margin kiri
                int marginTop = 40;     // Margin atas
                int spacing = 20;       // Spasi antar laporan

                int currentX = marginLeft;
                int currentY = marginTop;
                int reportsInCurrentRow = 0;

                var page = document.AddPage();
                var graphics = XGraphics.FromPdfPage(page);

                // Menggambar setiap laporan ke dalam PDF
                foreach (var report in reports)
                {
                    // Cek apakah perlu berpindah ke baris berikutnya
                    if (reportsInCurrentRow == reportsPerRow)
                    {
                        currentX = marginLeft;
                        currentY += reportHeight + spacing;
                        reportsInCurrentRow = 0;

                        // Jika halaman penuh, buat halaman baru
                        if (currentY + reportHeight > page.Height - marginTop)
                        {
                            page = document.AddPage();
                            graphics = XGraphics.FromPdfPage(page);
                            currentY = marginTop;
                        }
                    }

                    // Gambar kotak laporan
                    var rect = new XRect(currentX, currentY, reportWidth, reportHeight);
                    graphics.DrawRectangle(XPens.Black, rect);

                    // Menambahkan konten laporan ke dalam kotak
                    graphics.DrawString($"ID Kandidat: {report.CandidateId}", fontTitle, XBrushes.Black, new XPoint(currentX + 10, currentY + 20));
                    graphics.DrawString($"Nama: {report.CandidateName}", fontContent, XBrushes.Black, new XPoint(currentX + 10, currentY + 40));
                    graphics.DrawString($"Posisi: {report.Position}", fontContent, XBrushes.Black, new XPoint(currentX + 10, currentY + 60));
                    graphics.DrawString($"Nilai Psikotes: {report.PsychotestScore}", fontContent, XBrushes.Black, new XPoint(currentX + 10, currentY + 80));
                    graphics.DrawString($"Hasil Psikotes: {report.PsychotestResult}", fontContent, XBrushes.Black, new XPoint(currentX + 10, currentY + 100));
                    graphics.DrawString($"Wawancara oleh: {report.Interviewer}", fontContent, XBrushes.Black, new XPoint(currentX + 10, currentY + 120));

                    // Update posisi untuk laporan berikutnya
                    currentX += reportWidth + spacing;
                    reportsInCurrentRow++;
                }

                // Simpan dan kembalikan file PDF dalam bentuk byte array
                document.Save(memoryStream);
                return memoryStream.ToArray();
            }
        }
    }
}
