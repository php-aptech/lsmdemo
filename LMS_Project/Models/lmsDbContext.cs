namespace LMS_Project.Models
{
    using System;
    using System.Data.Entity;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;

    public partial class lmsDbContext : DbContext
    {
        public lmsDbContext()
            : base("name=DbContext")
        {
        }
        public virtual DbSet<tbl_TutoringFee> tbl_TutoringFee { get; set; }
        public virtual DbSet<tbl_FileCurriculumInClass> tbl_FileCurriculumInClass { get; set; }
        public virtual DbSet<tbl_CurriculumDetailInClass> tbl_CurriculumDetailInClass { get; set; }
        public virtual DbSet<tbl_CurriculumInClass> tbl_CurriculumInClass { get; set; }
        public virtual DbSet<tbl_DocumentLibraryDirectory> tbl_DocumentLibraryDirectory { get; set; }
        public virtual DbSet<tbl_DocumentLibrary> tbl_DocumentLibrary { get; set; }
        public virtual DbSet<tbl_Contract> tbl_Contract { get; set; }
        public virtual DbSet<tbl_FeedbackReply> tbl_FeedbackReply { get; set; }
        public virtual DbSet<tbl_Feedback> tbl_Feedback { get; set; }
        public virtual DbSet<tbl_NewsFeedReply> tbl_NewsFeedReply { get; set; }
        public virtual DbSet<tbl_NewsFeedComment> tbl_NewsFeedComment { get; set; }
        public virtual DbSet<tbl_NewsFeedLike> tbl_NewsFeedLike { get; set; }
        public virtual DbSet<tbl_StudentAssessment> tbl_StudentAssessment { get; set; }
        public virtual DbSet<tbl_ScheduleAvailable> tbl_ScheduleAvailable { get; set; }
        public virtual DbSet<tbl_HistoryDonate> tbl_HistoryDonate { get; set; }
        public virtual DbSet<tbl_MarkSalary> tbl_MarkSalary { get; set; }
        public virtual DbSet<tbl_MarkQuantity> tbl_MarkQuantity { get; set; }
        public virtual DbSet<tbl_PaymentApprove> tbl_PaymentApprove { get; set; }
        public virtual DbSet<tbl_PaymentAllow> tbl_PaymentAllow { get; set; }
        public virtual DbSet<tbl_PackageStudent> tbl_PackageStudent { get; set; }
        public virtual DbSet<tbl_PackageSkill> tbl_PackageSkill { get; set; }
        public virtual DbSet<tbl_PackageSection> tbl_PackageSection { get; set; }
        public virtual DbSet<tbl_Like> tbl_Like { get; set; }
        public virtual DbSet<tbl_FileInNewsFeed> tbl_FileInNewsFeed { get; set; }
        public virtual DbSet<tbl_Comment> tbl_Comment { get; set; }
        public virtual DbSet<tbl_UserInNFGroup> tbl_UserInNFGroup { get; set; }
        public virtual DbSet<tbl_NewsFeed> tbl_NewsFeed { get; set; }
        public virtual DbSet<tbl_Background> tbl_Background { get; set; }
        public virtual DbSet<tbl_NewsFeedGroup> tbl_NewsFeedGroup { get; set; }
        public virtual DbSet<tbl_VideoActiveCode> tbl_VideoActiveCode { get; set; }
        public virtual DbSet<tbl_Cart> tbl_Cart { get; set; }
        public virtual DbSet<tbl_Tag> tbl_Tag { get; set; }
        public virtual DbSet<tbl_TagCategory> tbl_TagCategory { get; set; }
        public virtual DbSet<tbl_Salary> tbl_Salary { get; set; }
        public virtual DbSet<tbl_SalaryConfig> tbl_SalaryConfig { get; set; }
        public virtual DbSet<tbl_Refund> tbl_Refund { get; set; }
        public virtual DbSet<tbl_ClassReserve> tbl_ClassReserve { get; set; }
        public virtual DbSet<tbl_ClassRegistration> tbl_ClassRegistration { get; set; }
        public virtual DbSet<tbl_ClassChange> tbl_ClassChange { get; set; }
        public virtual DbSet<tbl_NotificationInClass> tbl_NotificationInClass { get; set; }
        public virtual DbSet<tbl_Point> tbl_Point { get; set; }
        public virtual DbSet<tbl_Transcript> tbl_Transcript { get; set; }
        public virtual DbSet<tbl_TimeLine> tbl_TimeLine { get; set; }
        public virtual DbSet<tbl_RollUp> tbl_RollUp { get; set; }
        public virtual DbSet<tbl_Bill> tbl_Bill { get; set; }
        public virtual DbSet<tbl_PaymentSession> tbl_PaymentSession { get; set; }
        public virtual DbSet<tbl_BillDetail> tbl_BillDetail { get; set; }
        public virtual DbSet<tbl_PaymentMethod> tbl_PaymentMethod { get; set; }
        public virtual DbSet<tbl_TeacherOff> tbl_TeacherOff { get; set; }
        public virtual DbSet<tbl_TeacherInProgram> tbl_TeacherInProgram { get; set; }
        public virtual DbSet<tbl_Schedule> tbl_Schedule { get; set; }
        public virtual DbSet<tbl_StudentInClass> tbl_StudentInClass { get; set; }
        public virtual DbSet<tbl_Class> tbl_Class { get; set; }
        public virtual DbSet<tbl_FileInCurriculumDetail> tbl_FileInCurriculumDetail { get; set; }
        public virtual DbSet<tbl_CurriculumDetail> tbl_CurriculumDetail { get; set; }
        public virtual DbSet<tbl_Curriculum> tbl_Curriculum { get; set; }
        public virtual DbSet<tbl_StudyTime> tbl_StudyTime { get; set; }
        public virtual DbSet<tbl_Program> tbl_Program { get; set; }
        public virtual DbSet<tbl_Grade> tbl_Grade { get; set; }
        public virtual DbSet<tbl_TestAppointmentNote> tbl_TestAppointmentNote { get; set; }
        public virtual DbSet<tbl_TestAppointment> tbl_TestAppointment { get; set; }
        public virtual DbSet<tbl_CustomerNote> tbl_CustomerNote { get; set; }
        public virtual DbSet<tbl_CustomerStatus> tbl_CustomerStatus { get; set; }
        public virtual DbSet<tbl_Customer> tbl_Customer { get; set; }
        public virtual DbSet<tbl_Permission> tbl_Permission { get; set; }
        public virtual DbSet<tbl_FrequentlyQuestion> tbl_FrequentlyQuestion { get; set; }
        /// <summary>
        /// Các mẫu
        /// </summary>
        public virtual DbSet<tbl_Template> tbl_Template { get; set; }
        public virtual DbSet<tbl_Idiom> tbl_Idiom { get; set; }
        /// <summary>
        /// Thông báo chung
        /// </summary>
        public virtual DbSet<tbl_GeneralNotification> tbl_GeneralNotification { get; set; }
        public virtual DbSet<tbl_Purpose> tbl_Purpose { get; set; }
        public virtual DbSet<tbl_Job> tbl_Job { get; set; }
        public virtual DbSet<tbl_DayOff> tbl_DayOff { get; set; }
        public virtual DbSet<tbl_Source> tbl_Source { get; set; }
        public virtual DbSet<tbl_LearningNeed> tbl_LearningNeed { get; set; }
        public virtual DbSet<tbl_Discount> tbl_Discount { get; set; }
        public virtual DbSet<tbl_Room> tbl_Room { get; set; }
        public virtual DbSet<tbl_Branch> tbl_Branch { get; set; }
        public virtual DbSet<tbl_SeminarRecord> tbl_SeminarRecord { get; set; }
        public virtual DbSet<tbl_AnswerResult> tbl_AnswerResult { get; set; }
        public virtual DbSet<tbl_ExerciseResult> tbl_ExerciseResult { get; set; }
        public virtual DbSet<tbl_ExerciseGroupResult> tbl_ExerciseGroupResult { get; set; }
        public virtual DbSet<tbl_ExamSectionResult> tbl_ExamSectionResult { get; set; }
        public virtual DbSet<tbl_ExamResult> tbl_ExamResult { get; set; }
        public virtual DbSet<tbl_ZoomRoom> tbl_ZoomRoom { get; set; }
        public virtual DbSet<tbl_ZoomConfig> tbl_ZoomConfig { get; set; }
        public virtual DbSet<tbl_Seminar> tbl_Seminar { get; set; }
        public virtual DbSet<tbl_Certificate> tbl_Certificate { get; set; }
        public virtual DbSet<tbl_CertificateConfig> tbl_CertificateConfig { get; set; }
        public virtual DbSet<tbl_SectionCompleted> tbl_SectionCompleted { get; set; }
        public virtual DbSet<tbl_LessonCompleted> tbl_LessonCompleted { get; set; }
        public virtual DbSet<tbl_AnswerInVideo> tbl_AnswerInVideo { get; set; }
        public virtual DbSet<tbl_QuestionInVideo> tbl_QuestionInVideo { get; set; }
        public virtual DbSet<tbl_VideoCourseStudent> tbl_VideoCourseStudent { get; set; }
        public virtual DbSet<tbl_FileInVideo> tbl_FileInVideo { get; set; }
        public virtual DbSet<tbl_LessonVideo> tbl_LessonVideo { get; set; }
        public virtual DbSet<tbl_Section> tbl_Section { get; set; }
        public virtual DbSet<tbl_Product> tbl_Product { get; set; }
        public virtual DbSet<tbl_ChangeInfo> tbl_ChangeInfo { get; set; }
        public virtual DbSet<tbl_Config> tbl_Config { get; set; }
        public virtual DbSet<tbl_Area> tbl_Area { get; set; }
        public virtual DbSet<tbl_District> tbl_District { get; set; }
        public virtual DbSet<tbl_Notification> tbl_Notification { get; set; }
        public virtual DbSet<tbl_NotificationInVideo> tbl_NotificationInVideo { get; set; }
        public virtual DbSet<tbl_UserInformation> tbl_UserInformation { get; set; }
        public virtual DbSet<tbl_Ward> tbl_Ward { get; set; }
        public virtual DbSet<tbl_ExerciseGroup> tbl_ExerciseGroup { get; set; }
        public virtual DbSet<tbl_Exercise> tbl_Exercise { get; set; }
        public virtual DbSet<tbl_Answer> tbl_Answer { get; set; }
        public virtual DbSet<tbl_Exam> tbl_Exam { get; set; }
        public virtual DbSet<tbl_ExamSection> tbl_ExamSection { get; set; }
        public virtual DbSet<tbl_StudentRollUpQrCode> tbl_StudentRollUpQrCode { get; set; }
        public virtual DbSet<tbl_StudyRoute> tbl_StudyRoute { get; set; }
        public virtual DbSet<tbl_CertificateTemplate> tbl_CertificateTemplate { get; set; }
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
        }
    }
}
