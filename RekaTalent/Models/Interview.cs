using System;

namespace RekaTalent.Models
{
    public enum InterviewResult
    {
        Pending = 0,
        Pass = 1,
        Fail = 2
    }

    public class Interview
    {
        public int Id { get; set; }
        public string InterviewName { get; set; }
        public int CandidateId { get; set; }
        public string CandidateName { get; set; }
        public string CandidatePosition { get; set; }
        public string BackGroundPendidikan { get; set; }
        public string PengalamanPosisi { get; set; }
        public string ProjectChallenging { get; set; }
        public string StrukturOrganisasi { get; set; }
        public string Pencapaian { get; set; }
        public string FeedBackAtasan { get; set; }
        public string KendalaDeveloper { get; set; }
        public string KelebihanKekurangan { get; set; }
        public double CurrantSalary { get; set; }
        public double ExpectedSalary { get; set; }
        public string Domisili { get; set; }
        public string BackGroundDiriKeluarga { get; set; }
        public string StartDate { get; set; }
        public string PengetahuanPenilaianDiri { get; set; }
        public string PenilaianPnC { get; set; }

        // Properti enum InterviewResult akan otomatis disimpan sebagai integer
        public InterviewResult Result { get; set; }
    }
}