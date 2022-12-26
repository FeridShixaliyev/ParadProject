using Parad.Models;
using System.Collections.Generic;

namespace Parad.ViewModels
{
    public class CommentReportVM
    {
        public AppUser AppUser { get; set; }
        public Comment Comment { get; set; }
        public CommentReport CommentReport { get; set; }
        public List<ReasonReport> ReasonReports { get; set; }
    }
}
