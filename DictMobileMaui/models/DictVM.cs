using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DictMobile.models
{
    public class DictVM
    {
        public uint? DBID { get; set; }
        public string Word { get; set; } = string.Empty;
        public string Translation { get; set; } = string.Empty;
        public string TopicName { get; set; } = string.Empty;        // из Topic
        public string DateRec { get; set; }
        public byte Score { get; set; }
        public bool Usersel { get; set; }
        public bool Phrase { get; set; }
        public byte Relevation { get; set; }
        public bool IsDeleted { get; set; }
        public string Modification_Time { get; set; }
    }
}
