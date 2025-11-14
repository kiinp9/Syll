using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace syll.be.lib.Form.Dtos
{
    public class ParagraphGroupCloneInfo
    {
        public int BlockTruongNhanBan { get; set; }
        public string StartPattern { get; set; } = String.Empty;
        public string EndPattern { get; set; } = String.Empty;
        public List<int> TruongDataIds { get; set; } = new List<int>();
        public int MaxIndexRowTable { get; set; }
    }
}
