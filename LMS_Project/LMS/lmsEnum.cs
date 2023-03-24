using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LMS_Project.Models
{
    public static class lmsEnum
    {
        public enum FeedbackStatus
        {
            MoiGui = 1,
            DangXuLy,
            DaXong
        }
        public enum AnswerType
        {
            Text,
            Image,
            Audio
        }
        public enum TagCategoryType
        {
            Video = 1,
        }
        public enum LessonFileType
        {
            LinkYoutube = 1,
            FileUpload
        }
        public enum AnswerEssay
        {
            Exist = 1,
            NotExist
        }
        public enum ExerciseLevel
        {
            Easy = 1,
            Normal,
            Difficult
        }
        public enum ExerciseType
        {
            MultipleChoice = 1,
            DragDrop,
            FillInTheBlank,
            Mindmap,
            TrueOrFalse,
            Sort,
            Write,
            Speak,
        }
        public enum SeminarStatus
        {
            ChuaDienRa = 1,
            DangDienRa,
            KetThuc
        }
        public enum RoleEnum
        {
            admin = 1,
            teacher,
            student,
            manager,
            sale,
            accountant,
            academic,
            parents
        }
        public enum AccountStatus
        {
            active,
            inActive
        }
        public enum GenderEnum
        {
            nu = 0,
            nam = 1,
            khac = 2
        }
        public enum AllowRegister
        {
            Allow,
            UnAllow
        }
        public enum ChangeInfoStatus
        {
            Approve,
            Cancel
        }
        public enum LessonType
        {
            Video = 1,
            BaiTap
        }

        public enum StudyRouteStatus
        {
            ChuaHoc = 1,
            DangHoc = 2,
            HoanThanh = 3
        }

        public enum StatisticalOverviewGroups
        {
            KhachHang = 1,//liên quan tới khách hàng
            HocTap = 2,// liên quan tới học viên, buổi học
            DoanhThu = 3,// liên quan tới thu chi
            GiangDay = 4// liên quan tới lớp học, buổi dạy
        }

        public static List<EnumObject> ListStudyRouteStatus()
        {
            return new List<EnumObject> {
                new EnumObject
                {
                    Key = (int)StudyRouteStatus.ChuaHoc,
                    Value = "Chưa học"
                },
                new EnumObject
                {
                    Key = (int)StudyRouteStatus.DangHoc,
                    Value = "Đang học"
                },
                 new EnumObject
                {
                    Key = (int)StudyRouteStatus.HoanThanh,
                    Value = "Hoàn Thành"
                },
            };
        }
        public static List<EnumObject> ListStatisticalOverviewGroups()
        {
            return new List<EnumObject> {
                new EnumObject
                {
                    Key = (int)StatisticalOverviewGroups.KhachHang,
                    Value = "Khách hàng"
                },
                new EnumObject
                {
                    Key = (int)StatisticalOverviewGroups.HocTap,
                    Value = "Học tập"
                },
                 new EnumObject
                {
                    Key = (int)StatisticalOverviewGroups.DoanhThu,
                    Value = "Lợi nhuận"
                },
                 new EnumObject
                {
                    Key = (int)StatisticalOverviewGroups.GiangDay,
                    Value = "Giảng dạy"
                },
            };
        }
    }
    public class EnumObject
    {
        public int Key { get; set; }
        public string Value { get; set; }
    }
}